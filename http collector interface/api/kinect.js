var express = require('express');
var bodyParser = require('body-parser');
var router = express.Router();

var state = {time: 0};

function getState(req, res) {
  console.log(req.query);
  res.writeHead(200, {'Content-Type': 'application/json'});
  res.end(JSON.stringify(state)); // Send the file data to the browser.
}

function setState(req, res) {
  if (state.time < req.body.time)
  {
      console.log(JSON.stringify(req.body));
      state = req.body;
  }
  else
  {
      console.log("Reject");
  }
  res.sendStatus(200);
}

function route(sequelize) {
  var jsonParser = bodyParser.json();
  router.get('/', getState);
  router.post('/', jsonParser, setState);
  return router;
}

module.exports = route;
