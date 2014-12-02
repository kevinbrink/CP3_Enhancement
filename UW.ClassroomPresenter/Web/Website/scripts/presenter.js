// Classroom Presenter 3 Main Scripts presenter.js v3.1.1, Wed Jan 7 16:57:24 -0800 2008

//--------------------------------- Presenter OBJECT ---------------------------------
var Presenter = {
	Version: '3.1.1',
	ClientState: 0,
	UI: 0,
	Network: 0,
  
	// Loading of the appropriate libraries
	require: function(libraryName) {
		// inserting via DOM fails in Safari 2.0, so brute force approach
		document.write('<script type="text/javascript" src="'+libraryName+'"><\/script>');
	},
	REQUIRED_SCRIPTACULOUS: '1.8.2',
	REQUIRED_JSGRAPHICS: '3.0.3',
	load: function() {
/*
		// Convert a version string into a number
		function convertVersionString(versionString) {
		var v = versionString.replace(/_.*|\./g, '');
		v = parseInt(v + '0'.times(4-v.length));
		return versionString.indexOf('_') > -1 ? v-1 : v;
		}
		// Check for the appropriate version of the required libraries
		if( (typeof Scriptaculous=='undefined') ||
			(convertVersionString(Scriptaculous.Version) < convertVersionString(Presenter.REQUIRED_SCRIPTACULOUS))) {
			throw("Presenter requires Script.aculo.us JavaScript framework >= " + Scriptaculous.REQUIRED_SCRIPTACULOUS);
		}
		if( (typeof JSGraphics=='undefined') ||
			(convertVersionString(JSGraphics.Version) < convertVersionString(Presenter.REQUIRED_JSGRAPHICS))) {
			throw("Presenter requires Script.aculo.us JavaScript framework >= " + Scriptaculous.REQUIRED_JSGRAPHICS);
		}
*/
		// Add child libraries to this document
/*
		var js = /presenter\.js(\?.*)?$/;
		$$('head script[src]').findAll( function(s) {
		return s.src.match(js);
		}).each(function(s) {
		var path = s.src.replace(js, ''),
		includes = s.src.match(/\?.*load=([a-z,]*)/);
		(includes ? includes[1] : 'builder,effects,dragdrop,controls,slider,sound').split(',').each(
		function(include) { Presenter.require(path+include+'.js') });
		});
*/
	
		// Create all the objects
		this.ClientState = new P_ClientState();
		this.UI = new P_UI();
		
		this.Network = new P_Network();
	},
  
	// Handle the different colors being selected
	OnColorClick: function( id ) {
		this.UI.Toolbar.buttons['orange'].setState( 0, 0, 0 );
		this.UI.Toolbar.buttons['blue'].setState( 0, 0, 0 );
		this.UI.Toolbar.buttons['green'].setState( 0, 0, 0 );
		this.UI.Toolbar.buttons['red'].setState( 0, 0, 0 );
		this.UI.Toolbar.buttons['yellow'].setState( 0, 0, 0 );
		this.UI.Toolbar.buttons['black'].setState( 0, 0, 0 );
		this.UI.Toolbar.buttons[id].setState(1, 1, 0);
		this.ClientState.penColor = id;
	},
	// Handle the student submission button being clicked
	OnSubmitClick: function( id ) {
		// Post the ink data to the server
		var result = '0,';
		result += this.UI.MainView.InkLayer.inkStrokes.strokeData.length + ',' +
			      this.ClientState.InstructorDeckIndex + ',' +
				  this.ClientState.Decks[this.ClientState.InstructorDeckIndex].InstructorSlideIndex + ',';
		for( var i =0; i<this.UI.MainView.InkLayer.inkStrokes.strokeData.length; i++ ) {
			result += this.UI.MainView.InkLayer.inkStrokes.strokeData[i].pts.length + ',';
			for( var j=0; j<this.UI.MainView.InkLayer.inkStrokes.strokeData[i].pts.length; j++ ) {
				result += this.UI.MainView.InkLayer.inkStrokes.strokeData[i].pts[j].x + ',';
				result += this.UI.MainView.InkLayer.inkStrokes.strokeData[i].pts[j].y + ',';
			}
		}
		
		this.Network.post2( result );
	},
	// Handle the erase all button being clicked
	OnEraseAllClick: function( id ) {
		// Post the ink data to the server
		this.UI.MainView.InkLayer.clear();
	},
	OnToolClick: function( id ) {
	    this.UI.Toolbar.buttons['pen_tool'].setState( 0, 0, 0 );
	    this.UI.Toolbar.buttons['text_tool'].setState( 0, 0, 0 );
	    this.UI.Toolbar.buttons['eraser_tool'].setState( 0, 0, 0 );
	    this.UI.Toolbar.buttons[id].setState( 1, 1, 0 );
	    this.ClientState.CurrentTool = id;
	},
	// Handle the erase all button being clicked
	OnLinkUnlinkClick: function( id ) {
	    if( this.ClientState.Linked == true ) {
    	    this.ClientState.Linked = false;
    	    this.UI.Toolbar.buttons[id].setState( 0, 0, 0 );
    	    this.UI.Toolbar.buttons['forward'].setState( 0, 0, 0 );
    	    this.UI.Toolbar.buttons['backward'].setState( 0, 0, 0 );
	        this.UI.FilmStrip.show();
	    } else {
    	    this.ClientState.Linked = true;
    	    this.UI.Toolbar.buttons[id].setState( 1, 1, 0 );
    	    this.UI.Toolbar.buttons['forward'].setState( 0, 0, 1 );
    	    this.UI.Toolbar.buttons['backward'].setState( 0, 0, 1 );
	        this.UI.FilmStrip.hide();
	    }
	},
	// Handle the erase all button being clicked
	OnQuickPollChanged: function( id ) {
	    var value = this.UI.Toolbar.QPControl.UIObject.options[this.UI.Toolbar.QPControl.CurrentIndex].text;
		// Post the ink data to the server
		var result = '1,';
		result += Presenter.ClientState.QuickPollGuid + ',';
		result += value;

		this.Network.post2( result );
	},
	
	
	// Currently Unused
	OnButtonMouseDown: function( id ) {
		alert(id);
	},
	OnButtonMouseUp: function( id ) {
		alert(id);
	}, 
	OnForwardClick: function( id ) {
		Presenter.ClientState.InstructorDeckIndex++;
		Presenter.UI.MainView.SetImage( 'Test_00' + Presenter.ClientState.InstructorDeckIndex + '.png' );
	},
	OnBackwardClick: function( id ) {
		if( Presenter.ClientState.InstructorDeckIndex > 1 )
			Presenter.ClientState.InstructorDeckIndex--;
		Presenter.UI.MainView.SetImage( 'Test_00' + Presenter.ClientState.InstructorDeckIndex + '.png' );
	},
  
  
	// This function gets called when the user receives some data from the server
	RecvFunc: function( obj ) {
	    // Handle quick poll style changing
	    if( obj.dat.iQPStyle != Presenter.ClientState.PollStyle ) {
	        switch( obj.dat.iQPStyle ) {
	            case 0:
	                Presenter.UI.Toolbar.QPControl.SetOptions([]);
	                break;
	            case 1:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['Yes','No']);
	                break;
	            case 2:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['Yes','No','Both']);
	                break;
	            case 3:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['Yes','No','Neither']);
	                break;
	            case 4:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['A','B','C']);
	                break;
	            case 5:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['A','B','C','D']);
	                break;
	            case 6:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['A','B','C','D','E']);
	                break;
	            default:
	                Presenter.UI.Toolbar.QPControl.SetOptions(['A','B','C','D','E','F']);
	                break;
	        }
	        Presenter.ClientState.PollStyle = obj.dat.iQPStyle;
	    }
	
	    // Handle the instructor state
	    // TODO CMPRINCE: Handle this in a better way, we can do better than just making the buttons disappear
	    if( obj.dat.iAllowSS ) {
	        // Enable the SS button
	        document.getElementById('submit').style.visibility = 'visible';
	        document.getElementById('submit').style.display = 'inline';
	    } else {
	        // Disable the SS button
	        document.getElementById('submit').style.visibility = 'hidden';
	        document.getElementById('submit').style.display = 'none';
	    }
	    // Show or hide the UI for selecting the Quick Poll response
	    if( obj.dat.iAllowQP ) {
	        Presenter.UI.Toolbar.QPControl.Show();
	    } else {
	        Presenter.UI.Toolbar.QPControl.Hide();
	    }
	    // TODO CMPRINCE: When disabled need to relink the user to the instructor as well 
	    // as hide the UI for free navigation
	    if( obj.dat.iLinked ) {
	    } else {
	    }
	
		// Behave differently if we are just updating the slide versus changing it
		var isNewSlide = false;
		var oldDeckIndex = Presenter.ClientState.InstructorDeckIndex;
		var oldSlideIndex = null;
		if( oldDeckIndex >= 0 && Presenter.ClientState.Decks[oldDeckIndex] != undefined ) {
			oldSlideIndex = Presenter.ClientState.Decks[oldDeckIndex].InstructorSlideIndex;
		}
		// Get the instructor state
		if( Presenter.ClientState.InstructorDeckIndex != obj.dat.iDeck && obj.dat.iDeck < 1 ) {
			Presenter.ClientState.InstructorDeckIndex = obj.dat.iDeck;
			isNewSlide = true;
		}
		
		// Create the new deck if needed
		if( Presenter.ClientState.Decks[obj.dat.iDeck] == undefined ) {
			Presenter.ClientState.Decks[obj.dat.iDeck] = new P_Deck();
		}
		
		// Set the slide in the deck
		if( Presenter.ClientState.Decks[obj.dat.iDeck].InstructorSlideIndex != obj.dat.iSlide ) {
			Presenter.ClientState.Decks[obj.dat.iDeck].InstructorSlideIndex = obj.dat.iSlide;
			isNewSlide = true;
		}
		Presenter.ClientState.ForcedNavigation = obj.dat.iLinked;
		
		// Get the deck state
		for( var i = 0; i< obj.dat.decks.length; i++ ) {
			var d = obj.dat.decks[i];
			
			// Create the deck if it doesn't already exist
			if( Presenter.ClientState.Decks[d.i] == undefined ) {
				Presenter.ClientState.Decks[d.i] = new P_Deck();
			}
			
			// Get the deck name
			Presenter.ClientState.Decks[d.i].Name = d.n;

			// Get the slide information
			for( var j = 0; j<d.s.length; j++ ) {
				var s = d.s[j];
				if( Presenter.ClientState.Decks[d.i].Slides[s.i] == undefined ) {
					Presenter.ClientState.Decks[d.i].Slides[s.i] = new P_Slide('','');
				}
				Presenter.ClientState.Decks[d.i].Slides[s.i].Name = s.n;
				Presenter.ClientState.Decks[d.i].Slides[s.i].ImageURL = s.u;
				Presenter.ClientState.Decks[d.i].Slides[s.i].RemoteStrokes = s.ink;
			}
		}			
	
		// Update Current Slide
		if( Presenter.ClientState.Linked ) {
			var deck = Presenter.ClientState.Decks[Presenter.ClientState.InstructorDeckIndex];
			var slide = deck.Slides[deck.InstructorSlideIndex];
			// New Slide -- Do lots of stuff
			if( slide != undefined ) {
				if( isNewSlide ) {
					// Switch to that slide if it is different...
					Presenter.UI.MainView.SetImage( slide.ImageURL );
					// Clear the instructor ink and draw it
					Presenter.UI.MainView.InstructorInkLayer.clear();
					Presenter.UI.MainView.InstructorInkLayer.drawInk( slide.RemoteStrokes );
					if( oldSlideIndex != null && 
						oldDeckIndex != null &&
						Presenter.ClientState.Decks[oldDeckIndex] != undefined &&
						Presenter.ClientState.Decks[oldDeckIndex].Slides[oldSlideIndex] != undefined ) {
						// Save the current user stroke for the UI and clear
						Presenter.UI.MainView.InkLayer.endStroke();
						Presenter.ClientState.Decks[oldDeckIndex].Slides[oldSlideIndex].LocalStrokes = Presenter.UI.MainView.InkLayer.inkStrokes;
						Presenter.UI.MainView.InkLayer.clear();
						// Set the strokes any data already there for the slide
						Presenter.UI.MainView.InkLayer.inkStrokes = (slide.LocalStrokes) ? slide.LocalStrokes : new Ink.StrokeCollection();
						Presenter.UI.MainView.InkLayer.redraw();
					}
				} else {
					// Updating current slide so just update ink...
					Presenter.UI.MainView.InstructorInkLayer.clear();
					Presenter.UI.MainView.InstructorInkLayer.drawInk( slide.RemoteStrokes );
				}
			}
		}
	}
};
//--------------------------------------------------------------------------------

//---------------------------------- P_Network CLASS ----------------------------------
// The objects for communication between the client and server
function P_Network() {
	Katana.network.initialize( null, null, Presenter.RecvFunc );
}

P_Network.prototype.post = function( obj ) {
	Katana.network.post( obj );
}
P_Network.prototype.post2 = function( obj ) {
	Katana.network.post2( obj );
}

// The objects for communication between the client and server
/*
function P_Network() {
	this.Network = document.createElement( 'iframe' );
	this.Network.id = 'network';
	this.Network.name = 'network';
	this.RecvFunc = function() {};
	this.Network.style.display = 'none';
	
	document.body.appendChild( this.Network );
}
P_Network.prototype.SetRecvFunc = function( recv ) {
	this.RecvFunc = recv;
}
P_Network.prototype.SetLocation = function( url ) {
	this.Network.src = url;
}
P_Network.prototype.Refresh = function() {
	this.SetLocation( this.Network.src );
}
// Test for drawing strokes
function myRefreshInkFunction() {
	if( Presenter.UI.MainView.InkLayer == null ) {
		Presenter.UI.MainView.CreateInkLayer();
	}
	Presenter.UI.MainView.InkLayer.clear();
	var s = new P_Stroke();
	s.AddPoint( 0,0,0 );
	s.AddPoint( 10,50,0 );
	s.AddPoint( 120,70,0 );
	s.DrawStroke( Presenter.UI.MainView.InkLayer );
}
function P_NetworkTimeout() {
	Presenter.Network.Refresh();
	window.setTimeout( "P_NetworkTimeout();", 5000 );
//	myDrawFunction();
}
*/
//--------------------------------------------------------------------------------

//------------------------------ P_ClientState CLASS -----------------------------
// This encapsulates all of the client state needed by the application
function P_ClientState() {
	// Connection Information
	this.Path = './images/Test/Test/';
	this.CurrentTool = 'pen_tool';
	
	this.penColor = 'black'; //Custom, Red, Blue, Black, Yellow, Green
	this.penCustomColor = P_ClientState.DEFAULT_CUSTOM_PEN_COLOR; //Orange
	
	this.TextColor = 'Black'; //Custom, Red, Blue, Black, Yellow, Green
	this.TextCustomColor = P_ClientState.DEFAULT_CUSTOM_TEXT_COLOR;
	
	this.HighlighterColor = 'Yellow';
	this.HighlighterCustomColor = P_ClientState.DEFAULT_CUSTOM_HL_COLOR;
	
	// Keep track of whether the instructor and student are linked or not
	this.Linked = true;
	this.ForcedNavigation = false;
	
	// Keep track of the deck index for the instructor and student
	this.InstructorDeckIndex = 0;
	this.StudentDeckIndex = 0;
	
	this.PollStyle = 5;
	this.QuickPollGuid = P_ClientState.GenerateGuid();
	
	// Slide Decks Array
	this.Decks = [];	
}

// Constants
P_ClientState.PEN_COLOR_BLACK = '#000000';
P_ClientState.PEN_COLOR_BLUE = '#0000FF';
P_ClientState.PEN_COLOR_RED = '#FF0000';
P_ClientState.PEN_COLOR_GREEN = '#00FF00';
P_ClientState.PEN_COLOR_YELLOW = '#FFFF00';
P_ClientState.PEN_COLOR_CYAN = '#00FFFF';
P_ClientState.PEN_COLOR_MAGENTA = '#FF00FF';
P_ClientState.DEFAULT_CUSTOM_PEN_COLOR = '#FFFF00';
P_ClientState.DEFAULT_CUSTOM_TEXT_COLOR = '#FFFF00';
P_ClientState.DEFAULT_CUSTOM_HL_COLOR = '#AAAA00';

P_ClientState.GenerateGuid = function() {
    var str = '';
    var temp = '0123456789abcdef';
    for( var i =0; i<32; i++ ) {
        str += temp.charAt(Math.floor(Math.random()*16));
    }
    return str;
}
	
// Function designed to return whether the client is linked to
// instructor or not
P_ClientState.prototype.IsLinked = function() {
	if( this.ForcedNavigation == true ) {
		return true;
	}
	return this.Linked;
}

// Get the index of the deck that is the current deck in the main slide view
P_ClientState.prototype.GetDeckIndex = function() {
	if( this.IsLinked() == true ) {
		return this.InstructorDeckIndex;
	}
	return this.StudentDeckIndex;
}
	
// Gets the actual slide index from the deck
P_ClientState.prototype.GetSlideIndex = function() {
	var deck = this.Decks[this.GetDeckIndex()];
	if( this.IsLinked() == true ) {
		return deck.InstructorSlideIndex;
	} else {
		return deck.StudentSlideIndex;
	}
}
//--------------------------------------------------------------------------------


//--------------------------------- P_Deck CLASS ---------------------------------
//	This class represents a deck in the presentation. This includes
//	generic information about the deck as well as the slides that
//	are part of the deck.
//	NOTE: In this version of CPLite all slides contain only a flat array
//		  of slides
function P_Deck() {
	this.Name = 'Untitled Deck';
	this.Slides = [];
	this.InstructorSlideIndex = 0;
	this.StudentSlideIndex = 0;
}	
	// Add a slide to the array of slides
P_Deck.prototype.AddSlide = function( slide ) {
	this.Slides.add( slide );
}
//--------------------------------------------------------------------------------

//--------------------------------- P_Slide CLASS --------------------------------
//	This class represents a slide in a deck.
function P_Slide( title, url ) {
	this.Name = title;
	this.ImageURL = url;
	this.LocalStrokes = null;
	this.RemoteStrokes = [];
}
//--------------------------------------------------------------------------------


//--------------------------------- P_Stroke CLASS --------------------------------
//	This class represents a set of stroke data either to be sent to the 
//	server or received from the server. Basically, a set of pixel values
//	with translation and scaling.
function P_Stroke() {
	this.ScaleX = 1.0;
	this.ScaleY = 1.0;
	this.TranslateX = 1.0;
	this.TranslateY = 1.0;
	this.Color = "#000000";
	
	this.XValues = [];
	this.YValues = [];
	this.PressureValues = [];
}
P_Stroke.prototype.AddPoint = function( x, y, p ) {
	this.XValues[ this.XValues.length ] = x;
	this.YValues[ this.YValues.length ] = y;
	this.PressureValues[ this.PressureValues.length ] = p;
}
P_Stroke.prototype.DrawStroke = function( canvas ) {
	var newX = [];
	var newY = [];
	for( i=0; i<this.XValues.length; i++ ) {
		newX[i] = Math.round( (this.XValues[i]*this.ScaleX) + this.TranslateX );
	}
	for( j=0; j<this.YValues.length; j++ ) {
		newY[j] = Math.round( (this.YValues[j]*this.ScaleY) + this.TranslateY );
	}

	canvas.setColor( this.Color );
	canvas.drawPolyLine( newX, newY );
	canvas.paint();
}
//--------------------------------------------------------------------------------
