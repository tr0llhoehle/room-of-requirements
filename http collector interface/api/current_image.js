var express = require('express');
var router = express.Router();
var fs = require('fs');
var bodyParser = require('body-parser');

var image = null;

function getImage(req, res) {
  if (image != null) {
    res.writeHead(200, {'Content-Type': 'image/png'});
    res.end(image, 'binary');
  } else {
    res.sendStatus(404);
    res.end();
  }
}

function setImage(req, res) {
  image = new Buffer(req.body, 'binary');
  res.sendStatus(200);
}

function route() {
  var rawParser = bodyParser.raw({type: '*/*'});

  router.get('/', getImage);
  router.post('/', rawParser, setImage);
  return router;
}

module.exports = route;
