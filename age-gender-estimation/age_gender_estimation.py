import os
from flask import Flask, request, redirect, url_for
from werkzeug.utils import secure_filename

import queue
import threading
import os
import cv2
import dlib
import numpy as np
import argparse
from wide_resnet import WideResNet
import io

import requests
import shutil

IMAGE_URL = "http://localhost:3000/current_image"


def get_args():
    parser = argparse.ArgumentParser(description="This script detects faces from web cam input, "
                                                 "and estimates age and gender for the detected faces.",
                                     formatter_class=argparse.ArgumentDefaultsHelpFormatter)
    parser.add_argument("--weight_file", type=str, default=None,
                        help="path to weight file (e.g. weights.18-4.06.hdf5)")
    parser.add_argument("--depth", type=int, default=16,
                        help="depth of network")
    parser.add_argument("--width", type=int, default=8,
                        help="width of network")
    args = parser.parse_args()
    return args


def draw_label(image, point, label, font=cv2.FONT_HERSHEY_SIMPLEX,
               font_scale=1, thickness=2):
    size = cv2.getTextSize(label, font, font_scale, thickness)[0]
    x, y = point
    cv2.rectangle(image, (x, y - size[1]), (x + size[0], y), (255, 0, 0), cv2.FILLED)
    cv2.putText(image, label, point, font, font_scale, (255, 255, 255), thickness)

img_size = 64

input_queue = queue.Queue(1)
output_queue = queue.Queue(1)

def worker():
    depth = 16
    k = 8
    weight_file = os.path.join("pretrained_models", "weights.18-4.06.hdf5")
    # for face detection
    detector = dlib.get_frontal_face_detector()
    # load model and weights
    model = WideResNet(img_size, depth=depth, k=k)()
    model.load_weights(weight_file)

    while True:
        img = input_queue.get()
        print("Got new job")

        input_img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        img_h, img_w, _ = np.shape(input_img)

        # detect faces using dlib detector
        detected = detector(input_img, 1)
        faces = np.empty((len(detected), img_size, img_size, 3))

        for i, d in enumerate(detected):
            x1, y1, x2, y2, w, h = d.left(), d.top(), d.right() + 1, d.bottom() + 1, d.width(), d.height()
            xw1 = max(int(x1 - 0.4 * w), 0)
            yw1 = max(int(y1 - 0.4 * h), 0)
            xw2 = min(int(x2 + 0.4 * w), img_w - 1)
            yw2 = min(int(y2 + 0.4 * h), img_h - 1)
            cv2.rectangle(img, (x1, y1), (x2, y2), (255, 0, 0), 2)
            # cv2.rectangle(img, (xw1, yw1), (xw2, yw2), (255, 0, 0), 2)
            faces[i,:,:,:] = cv2.resize(img[yw1:yw2 + 1, xw1:xw2 + 1, :], (img_size, img_size))

        label = None
        if len(detected) > 0:
            # predict ages and genders of the detected faces
            results = model.predict(faces)
            predicted_genders = results[0]
            ages = np.arange(0, 101).reshape(101, 1)
            predicted_ages = results[1].dot(ages).flatten()
            label = "\"state\":\"success\",\"age\": {}, \"gender\": \"{}\"".format(int(predicted_ages[0]), "f" if predicted_genders[0][0] > 0.5 else "m")
        else:
            label = "\"state\":\"error\""
        print("Finished job:" + label)
        output_queue.put(label)
        input_queue.task_done()

app = Flask(__name__)

@app.route('/', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'GET':
        url = 'https://upload.wikimedia.org/wikipedia/commons/8/8d/President_Barack_Obama.jpg'
        #url = IMAGE_URL
        response = requests.get(url, stream=True)
        in_memory_file = io.BytesIO()
        shutil.copyfileobj(response.raw, in_memory_file)
        data = np.fromstring(in_memory_file.getvalue(), dtype=np.uint8)
        color_image_flag = 1
        img = cv2.imdecode(data, color_image_flag)
        if img is not None:
            input_queue.put(img)
            result = output_queue.get()
            output_queue.task_done()
            return "{"+result+"}"
        else:
            return "\"state\":\"error\""
    if request.method == 'POST':
        # check if the post request has the file part
        if 'file' not in request.files:
            return "{"+"\"state\":\"error\""+"}"
        file = request.files['file']
        # if user does not select file, browser also
        # submit a empty part without filename
        if file.filename == '':
            return "{"+"\"state\":\"error\""+"}"
        if file:
            in_memory_file = io.BytesIO()
            file.save(in_memory_file)
            data = np.fromstring(in_memory_file.getvalue(), dtype=np.uint8)
            color_image_flag = 1
            img = cv2.imdecode(data, color_image_flag)

            input_queue.put(img)
            result = output_queue.get()
            output_queue.task_done()
            return "{"+result+"}"

if __name__ == '__main__':
    thread = threading.Thread(target = worker, args=())
    thread.daemon = True
    thread.start()
    app.run(debug=False, port=7000)
    #thread.join()
    #input_queue.join()
    #output_queue.join()
