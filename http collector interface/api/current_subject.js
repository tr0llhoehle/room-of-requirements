var express = require('express');
var router = express.Router();

let state = null;
let default_subject = {
    "id": "105",
    "age": 25,                    // age in years
    "gender": 23,                 // min 0 indicates male, max 100 indicates female
    "height":185,                 // height in meters
    "weight":80,                  // weight in kg
    "color_traits": {
    },
    "additional_traits": {
    },
    "color_history" : []
  };

function getValue(req, res) {
  console.log(JSON.stringify(state.subject));
  res.writeHead(200, {'Content-Type': 'json'});
  res.end(JSON.stringify(state.subject)); // Send the file data to the browser.
}

function route(external_state) {
  state = external_state;
  external_state.subject = default_subject;
  router.get('/', getValue);
  return router;
}

module.exports = route;
