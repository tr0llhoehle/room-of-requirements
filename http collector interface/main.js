console.log("### http collector interface ###")
console.log("## starting")

var express = require('express');
var app = express();
var fs = require('fs');
var bodyParser = require('body-parser');
var routePath = './api';

/**
 * Routes all definitions made in a file under a certain URI.
 * @param file file with further definitions for routing
 * @param route the root address under which the file will be routed
 */
function routeFile(route, file) {
  app.use(route, require('' + file)());
  // output to where it's mapped
  console.log(route + " (" + file + ")");
}

/*
 * Initialize all routes including subfolders. Bind according to name of file.
 * For example:
 * index.js will be mapped to /
 * contacts/index.js will be mapped to /contacts
 * contacts/groups.js will be mapped to /contacts/groups
 * @param currentFolder the folder currently in
 */
function routeRecursive(currentFolder) {
  fs.readdirSync(routePath + currentFolder).forEach(function (file) {
    var currentFile = currentFolder + '/' + file;
    
    if (fs.lstatSync(routePath + currentFile).isDirectory()) {
      routeRecursive(currentFile);
    } else if (file.toLowerCase() == "index.js") {
      routeFile(currentFolder, routePath + currentFile);
    } else if (file.toLowerCase().indexOf('.js')) {
      // cut off the '.js'
      var route = currentFile.substring(0, currentFile.indexOf('.js'));
      var file = routePath + currentFile;
      routeFile(route, file);
    }
  });
}

function root(req, res) {
  res.send('You\'re at /\nGood job!');
}

routeRecursive('');

// routing for /
app.get('/', root);

var server = app.listen(3000, function () {
  var host = server.address().address
  var port = server.address().port
  console.log('Listening at http://%s:%s', host, port)
})

console.log("routing done")


