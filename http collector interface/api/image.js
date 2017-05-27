var express = require('express');
var router = express.Router();
var fs = require('fs');

//curried function to send data
function send(res) {
  return function (data) {
    res.send(data);
  }
}

function getImage(req, res) {
  console.log(req.query);
  fs.readFile('./image.png', function(err, data) {
    if (err) throw err; // Fail if the file can't be read.
    res.writeHead(200, {'Content-Type': 'image/png'});
    res.end(data); // Send the file data to the browser.
    console.log('wrote image');
  });
  

function route(sequelize) {
  router.get('/', getImage);
  return router;
}
module.exports = route;
