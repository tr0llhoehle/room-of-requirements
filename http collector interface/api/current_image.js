var express = require('express');
var router = express.Router();
var fs = require('fs');
var bodyParser = require('body-parser');
var request = require('request');

let state = null;
var image = null;
let last_gender_update = 0;

function getImage(req, res) {
  if (image != null) {
    res.writeHead(200, {'Content-Type': 'image/png'});
    res.end(image, 'binary');
  } else {
    res.sendStatus(404);
    res.end();
  }
}

function get_gender_age(cb) {
  request('http://localhost:7000/', function(error, response, body) {
    if (error != null)
    {
      return cb(error, null);
    }
    return cb(null, JSON.parse(body));  
  })
}

function set_gender_age(image, timestamp) {
  get_gender_age((err, resp) => {
    if (err != null)
    {
      console.log(err);
      return;
    }

    if (last_gender_update < timestamp)
    {
      if (resp.state == "success")
      {
      	state.subject.age = resp.age;
        state.subject.gender = resp.gender;
      }
      last_gender_update = timestamp;
      console.log(JSON.stringify(resp));
    }
  });
}

function setImage(req, res, next) {
  image = new Buffer(req.body, 'binary');

  set_gender_age(image, +Date.now())

  res.sendStatus(200);
  next();
}

function route(external_state) {
  state = external_state;
  var rawParser = bodyParser.raw({type: '*/*', limit: "50mb"});

  router.get('/', getImage);
  router.post('/', rawParser, setImage);
  return router;
}

module.exports = route;
