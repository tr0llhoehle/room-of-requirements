var fs = require('fs');
var csv = require('csv');

var traits_cold_colors;
var traits_warm_colors;
var traits_cold;
var traits_warm;

function getColdWarmBoth(color) {
  var cold_count = 0;
  var warm_count = 0;
  var pro = [];
  var contra = [];
  for (var i = 1; i < traits_cold_colors.length; i++) {
    if (traits_cold_colors[i][0] == color) {
      cold_count += 1;
      pro = traits_cold_colors[i][1].split(';');
      contra = traits_cold_colors[i][2].split(';');
    }
  }
  for (var i = 1; i < traits_warm_colors.length; i++) {
    if (traits_warm_colors[i][0] == color) {
      warm_count += 1;
      pro = traits_cold_colors[i][1].split(';');
      contra = traits_cold_colors[i][2].split(';');
    }
  }
  if (cold_count > warm_count) {
    return ['cold', pro, contra];
  } else if (cold_count < warm_count) {
    return ['warm', pro, contra];
  } else {
    return ['both', pro, contra];
  }
}

function getRandomStrings(strings, count) {
  if (count >= strings.length) {
    return strings;
  } else {
    var ret = [];
    for (var i = 0; i < count; i++) {
      var random = Math.floor(Math.random() * Math.floor(strings.length - 1));
      ret.push(strings[random]);
      strings.splice(random, 1);
    }
    return ret;
  }
}

function getAdditionalTraits(wall) {
  if (wall == 'cold') {
    wall = traits_cold;
  } else {
    wall = traits_warm;
  }
  //console.log(wall);
  var strength = wall[1][0].split(";");
  var weakness = wall[1][1].split(";");
  var likes = wall[1][2].split(";");
  var dislikes = wall[1][3].split(";");
  var traits = wall[1][4].split(";");

  var additional_traits = {};
  additional_traits.strength = getRandomStrings(strength, 2);
  additional_traits.weakness = getRandomStrings(weakness, 2);
  additional_traits.likes = getRandomStrings(likes, 3);
  additional_traits.dislikes = getRandomStrings(dislikes, 2);
  additional_traits.traits = getRandomStrings(traits, 1);

  return additional_traits;
}

function get(data) {
  var count = 0;
  var color_traits = []
  var additional_traits = [];

  for (var i = 0; i < data.length; i++) {
    var color = data[i][0];
    var percentage = data[i][1];
    var association = getColdWarmBoth(color);
    var procontra = {};
    procontra.pro = association[1];
    procontra.contra = association[2];
    color_traits.push(procontra);
    if (association[0] == 'cold') {
      count += 1*percentage;
    } else if (association[0] == 'warm') {
      count -= 1*percentage;
    }
  }
  if (count > 0) {
    //cold
    additional_traits = getAdditionalTraits('cold');
  } else if (count < 0) {
    //warm
    additional_traits = getAdditionalTraits('warm');
  } else {
    //both, pick one at random
    var random = Math.floor(Math.random());
    if (random > 0) {
      additional_traits = getAdditionalTraits('cold');
    } else {
      additional_traits = getAdditionalTraits('warm');
    }
  }
  var analysis = {};
  analysis.color_traits = color_traits;
  analysis.additional_traits = additional_traits;

  return analysis;
}

function analysis() {
  fs.readFile('./traits_cold_colors.csv', function(err, data) {
    if (err) {
      console.log("Could not read traits_cold_colors.csv file, make sure it exists");
      throw err; // Fail if the file can't be read.
    }
    csv.parse(data, function(err, data){
      traits_cold_colors = data;
      if (err) {
        console.log("Could not parse traits_cold_colors.csv file");
      }
    });

    console.log('read traits_cold_colors.csv file');
  });
  fs.readFile('./traits_warm_colors.csv', function(err, data) {
    if (err) {
      console.log("Could not traits_warm_colors.csv file, make sure it exists");
      throw err; // Fail if the file can't be read.
    }
    csv.parse(data, function(err, data){
      traits_warm_colors = data;
      if (err) {
        console.log("Could not parse traits_warm_colors.csv file");
      }
    });
    console.log('read traits_warm_colors.csv file');
  });
  fs.readFile('./traits_cold.csv', function(err, data) {
    if (err) {
      console.log("Could not read traits_cold.csv file, make sure it exists");
      throw err; // Fail if the file can't be read.
    }
    csv.parse(data, function(err, data){
      traits_cold = data;
      if (err) {
        console.log("Could not parse traits_cold.csv file");
      }
    });

    console.log('read traits_cold.csv file');
  });
  fs.readFile('./traits_warm.csv', function(err, data) {
    if (err) {
      console.log("Could not read traits_warm.csv file, make sure it exists");
      throw err; // Fail if the file can't be read.
    }
    csv.parse(data, function(err, data){
      traits_warm = data;
      if (err) {
        console.log("Could not parse traits_warm.csv file");
      }
    });

    console.log('read traits_warm.csv file');
  });

  return get;
}

module.exports = analysis;
