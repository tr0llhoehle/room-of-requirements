var express = require('express');
var bodyParser = require('body-parser');
var router = express.Router();

var external_state = null;
var state = {time: 0};

function getState(req, res) {
  res.writeHead(200, {'Content-Type': 'application/json'});
  res.end(JSON.stringify(state)); // Send the file data to the browser.
}

function setState(req, res) {
  if (state.time < req.body.time)
  {
      external_state.subject.id = req.body.id;
      external_state.subject.height = req.body.height;
      external_state.subject.weight = req.body.weight;
      console.log('face: ' + JSON.stringify(req.body));
      state = req.body;
  }
  else
  {
      console.log("face: reject");
  }
  res.sendStatus(200);
}

function route(s) {
  external_state = s;
  var jsonParser = bodyParser.json();
  router.get('/', getState);
  router.post('/', jsonParser, setState);
  return router;
}

module.exports = route;
