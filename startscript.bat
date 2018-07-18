start /min C:\Users\GameDemo\Desktop\room-of-requirements\KinectAdapter\KinectHTTPProxy\bin\x64\Release\KinectHTTPProxy.exe
cd "C:\Users\GameDemo\Desktop\room-of-requirements\http collector interface\"
start /min node main.js
start C:\Users\GameDemo\Desktop\room-of-requirements\KinectRoom\bin\x64\Release\kinectroom.exe
start C:\Users\GameDemo\Desktop\room-of-requirements\Runnables\SecurityRoom\SecurityRoom.exe
call C:\Users\GameDemo\Anaconda3\Scripts\activate.bat C:\Users\GameDemo\Anaconda3
cd "C:\Users\GameDemo\Desktop\room-of-requirements\age-gender-estimation\"
set KERAS_BACKEND=theano
start C:\Users\GameDemo\Desktop\room-of-requirements\age-gender-estimation\run.bat