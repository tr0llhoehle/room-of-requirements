var express = require('express');
var router = express.Router();
var fs = require('fs');
var csv = require('csv');
var bodyParser = require('body-parser');
var analysis = require('../lib/analysis');


var cold_wall;
var warm_wall;
var analizer = analysis();

let state = null;

// debug
function getSeconds(list) {
  for (var i = 1; i < list.length; i++) {
    time_str = list[i][0];
    start_end = time_str.split('-');
    start = start_end[0].split(':');
    end = start_end[1].split(':');
    start_seconds = parseInt(start[0])*60 + parseInt(start[1]);
    end_seconds = parseInt(end[0])*60 + parseInt(end[1])-1;
  }
}

function isInRange(range_str, time) {
  range = range_str.split('-');
  start = parseInt(range[0]);
  end = parseInt(range[1]);
  time = parseInt(time);
  if (time >= start && time <= end) {
    return true;
  } else {
    return false;
  }
}

function getColorNames(time, colorcsv) {
  var colors = [];
  for (var i = 1; i < colorcsv.length; i++) {
    if(isInRange(colorcsv[i][1], time)) {
      for (var j = 2; j < colorcsv[i].length; j++) {
        if(colorcsv[i][j] != "") {
          colors.push(colorcsv[i][j]);
        }
      }
      return colors;
    }
  }
  return colors;
}

function computeHistogram(history) {
  let histogram = {};
  let count = 0;
  history.forEach(colors => {
    colors.forEach(color => {
      count++;
      if (histogram[color] === undefined) histogram[color] = 0;
      histogram[color]++;
    });
  });

  let tuples = [];
  for (let color in histogram) {
      tuples.push([color, histogram[color] / count]);
  }

  return tuples;
}

function setColors(req, res) {
  var colors = [];

  if (req.body.wall == "cold") {
    colors = getColorNames(req.body.time, cold_wall);
  } else if (req.body.wall == "warm") {
    colors = getColorNames(req.body.time, warm_wall);
  } else {
    res.sendStatus(400);
    return;
  }
  state.subject.color_history.push(colors);

  let histogram = computeHistogram(state.subject.color_history);
  let data = analizer(histogram);

  state.subject.color_traits = data.color_traits;
  state.subject.additional_traits = data.additional_traits;

  res.sendStatus(200);
}

function route(external_state) {
  state = external_state;

  fs.readFile('./cold_wall.csv', function(err, data) {
    if (err) {
      console.log("Could not read cold_wall.csv file, make sure it exists");
      throw err; // Fail if the file can't be read.
    }
    csv.parse(data, function(err, data){
      cold_wall = data;
      if (err) {
        console.log("Could not parse cold_wall.csv file");
      }
    });

    console.log('read cold_wall.csv file');
  });
  fs.readFile('./warm_wall.csv', function(err, data) {
    if (err) {
      console.log("Could not warm_wall.csv file, make sure it exists");
      throw err; // Fail if the file can't be read.
    }
    csv.parse(data, function(err, data){
      warm_wall = data;
      if (err) {
        console.log("Could not parse warm_wall.csv file");
      }
    });
    console.log('read warm_wall.csv file');
  });
  var jsonParser = bodyParser.json();

  router.post('/', jsonParser, setColors);
  return router;
}

module.exports = route;
