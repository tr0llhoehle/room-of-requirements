var express = require('express');
var router = express.Router();
var fs = require('fs');
var csv = require('csv');

var cold_wall;
var warm_wall;

// debug
function getSeconds(list) {
  for (var i = 1; i < list.length; i++) {
    //console.log(list[i][0]);
    time_str = list[i][0];
    start_end = time_str.split('-');
    //console.log(start_end);
    start = start_end[0].split(':');
    end = start_end[1].split(':');
    start_seconds = parseInt(start[0])*60 + parseInt(start[1]);
    end_seconds = parseInt(end[0])*60 + parseInt(end[1])-1;
    console.log(start_seconds+'-'+end_seconds);
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

function getColors(req, res) {
  console.log('wall: '+ req.query.wall + 'time: ' + req.query.time);
  var colors = [];
  if (req.query.wall == "cold") {
    colors = getColorNames(req.query.time, cold_wall);
  } else if (req.query.wall == "warm") {
    colors = getColorNames(req.query.time, warm_wall);
  }
  res.writeHead(200, {'Content-Type': 'application/json'});
  res.end(JSON.stringify(colors)); // Send the colors
}

function route(sequelize) {
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
  router.get('/', getColors);
  return router;
}

module.exports = route;
