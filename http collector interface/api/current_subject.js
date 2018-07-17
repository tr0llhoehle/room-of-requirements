var express = require('express');
var router = express.Router();

function getRandomInt(min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min)) + min; //The maximum is exclusive and the minimum is inclusive
}

let state = null;
let default_subject = {
    "id": getRandomInt(200, 5000),
    "age": 25,                    // age in years
    "gender": 'f',                 // min 0 indicates male, max 100 indicates female
    "height":1.85,                 // height in meters
    "weight":80,                  // weight in kg
    "color_traits": [
    ],
    "additional_traits": [
    ],
    "color_history" : []
  };

function getValue(req, res) {
  res.writeHead(200, {'Content-Type': 'json'});
  res.end(JSON.stringify(state.subject)); // Send the file data to the browser.
}

function reset(req, res) {
  console.log("Reset!");
  state.subject = JSON.parse(JSON.stringify(default_subject));
  state.subject.id = getRandomInt(200, 5000); 
  res.end();
}

function route(external_state) {
  state = external_state;
  state.subject = JSON.parse(JSON.stringify(default_subject));
  router.get('/', getValue);
  router.get('/reset', reset);
  return router;
}

module.exports = route;
