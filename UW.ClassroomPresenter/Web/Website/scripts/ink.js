//#########################################################
// The Ink Library
//#########################################################
// Construct the Ink namespace
var Ink = {};

// Helper function to compress a stroke collection before sending
Ink.compressStrokeCollection = function ( collection ) {
	var data = new Array();
	// Iterate through the collection and compress all strokes
	for( var i = 0; i<collection.strokeData.length; i++ ) {
		data.push( compressStroke( collection.strokeData[i] ) );
	}
	return data;
}

// Compress a single stroke by storing only the deltas not the absolute
// position of each point
Ink.compressStroke = function ( stroke ) {
	var data = new Array();
	var lastPoint = stroke.pointData[0];
	var index = 0;
	
	// Add the first point
	data.push( lastPoint.xCoord );
	data.push( lastPoint.yCoord );
	
	// Iterate through the rest of the points
	for( var i = 1; i<stroke.pointData.length; i++ ) {
		// Add the deltas
		var currentPoint = stroke.pointData[i];
		data.push( currentPoint.xCoord - lastPoint.xCoord );
		data.push( currentPoint.yCoord - lastPoint.yCoord );
		lastPoint = currentPoint;
	}
	return data;
}

//---------------------------------------------------------
// 	class StrokeCollection
//		Class representing a collection of stroke objects
//---------------------------------------------------------
Ink.StrokeCollection = function () {
	// The Data Array
	this.strokeData = new Array();
	
	// Append a stroke to the data Array
	this.addStroke = function( stroke ) {
		// Add a stroke to the collection
		this.strokeData.push( stroke );
	}
	
	// Delete all the strokes from the array
	this.deleteAllStrokes = function() {
		this.strokeData = new Array();
	}
}

//---------------------------------------------------------
// 	class Stroke
// 		Class representing a point in a collection strokes
//---------------------------------------------------------
Ink.Stroke = function () {
	// The point data
	this.pts = [];
	this.color = 'black';
	
	// Add a point to the stroke
	this.addPoint = function( xCoord, yCoord, pressure ) {
		this.pts.push( { x:xCoord, y:yCoord, p:pressure } );
	}
}
