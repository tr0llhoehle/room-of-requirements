var express = require('express');
var router = express.Router();

var value;
var example_value = {
    "id":"105",
    "age": 25,                    // age in years
    "gender": 23,                 // min 0 indicates male, max 100 indicates female
    "height":185,                 // height in meters
    "weight":80,                  // weight in kg
    "personality_traits":{        // traits with a scale of 0 to 100 ("percentage")
      "neuroticism":18,
      "openness": 63,
      "conscientiousness": 24,
      "agreeableness": 44,
      "extroversion": 56
    }
  };

function getValue(req, res) {
  console.log(req.query);
  res.writeHead(200, {'Content-Type': 'json'});
  res.end(JSON.stringify(value)); // Send the file data to the browser.
  console.log('wrote value ',value);
}

function setValue(req, res) {
  console.log(req.query);
  res.sendStatus(0);
}

function route(sequelize) {
  value = example_value;
  router.get('/', getValue);
  router.put('/', setValue);
  return router;
}

module.exports = route;
