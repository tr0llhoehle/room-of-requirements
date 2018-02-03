var express = require('express');
var router = express.Router();
var fs = require('fs');

var image;

function getImage(req, res) {
  console.log(req.query);
  res.writeHead(200, {'Content-Type': 'image/png'});
  res.end(image); // Send the file data to the browser.
  console.log('wrote image');
}

function setImage(req, res) {
  console.log(req.query);
  res.sendStatus(0);
/*  fs.readFile('./image.png', function(err, data) {
    if (err) throw err; // Fail if the file can't be read.
    res.writeHead(200, {'Content-Type': 'image/png'});
    res.end(data); // Send the file data to the browser.
    console.log('wrote image');
  });*/
}

function route(sequelize) {
  fs.readFile('./image.png', function(err, data) {
    if (err) {
      console.log("Could not read image file, make sure image.png exists");
      throw err; // Fail if the file can't be read.
    }
    image = data;
    console.log('read image');
  });
  router.get('/', getImage);
  router.post('/', setImage);
  return router;
}

module.exports = route;
