var express = require('express');
var router = express.Router();

var value;

function getValue(req, res) {
  console.log(req.query);
  res.writeHead(200, {'Content-Type': 'txt'});
  res.end(value); // Send the file data to the browser.
  console.log('wrote value ',value);
}

function setValue(req, res) {
  console.log(req.query);
  res.sendStatus(0);
}

function route(sequelize) {
  value = "asdfghjkl";
  router.get('/', getValue);
  router.put('/', setValue);
  return router;
}

module.exports = route;
