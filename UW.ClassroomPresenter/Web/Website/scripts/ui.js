//#########################################################
// The P_UI Library
//#########################################################
// Construct the UI namespace
var P_UI = {};

//---------------------------------------------------------
//	class P_UI
//		Class that builds the UI for the application
//---------------------------------------------------------
P_UI = function () {
	this.Toolbar = new P_UI.Toolbar( 'toolbar' );
	this.MainView = new P_UI.MainSlideView();
	this.FilmStrip = new P_UI.FilmStrip( 'filmstrip' );
	this.FilmStrip.hide();

	// Add the UI Elements to the body
	document.body.appendChild( this.Toolbar.UIObject );
	document.body.appendChild( this.FilmStrip.UIObject );
	document.body.appendChild( this.MainView.UIObject );
	
	// Finish creating the UI Elements
	//this.MainView.CreateInkLayer();
}

//---------------------------------------------------------
//	class Toolbar
// 		Class representing the toolbar
//---------------------------------------------------------
P_UI.Toolbar = function ( name ) {
	var toolbar = document.createElement('div');
	toolbar.id = name;
	this.buttons = {};

    // Create the Quick Poll control	
	this.QPControl = new P_UI.QuickPollControl( 'quickpoll', ['A','B','C','D'], 
	                    function() { Presenter.OnQuickPollChanged( 'quickpoll' ); } );
	
	// PEN COLOR TOOLBAR
	toolbar.appendChild( this.CreateButton( 'orange', 
											0,0,0,
											[[['./images/toolbar/color_1_up.png','./images/toolbar/color_1_up_grey.png'],
											  ['./images/toolbar/color_1_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/color_1_down.png','./images/toolbar/color_1_down_grey.png']]],
											  
											[[['Select the custom color pen.','Select the custom color pen.'],
											  ['Select the custom color pen.','Select the custom color pen.']],
											 [['Select the custom color pen.','Select the custom color pen.'],
											  ['Select the custom color pen.','Select the custom color pen.']]],
											function() { Presenter.OnColorClick( 'orange' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'blue', 
											0,0,0,
											[[['./images/toolbar/color_2_up.png','./images/toolbar/color_2_up_grey.png'],
											  ['./images/toolbar/color_2_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/color_2_down.png','./images/toolbar/color_2_down_grey.png']]],
											  
											[[['Select the blue pen.','Select the blue pen.'],
											  ['Select the blue pen.','Select the blue pen.']],
											 [['Select the blue pen.','Select the blue pen.'],
											  ['Select the blue pen.','Select the blue pen.']]],
											function() { Presenter.OnColorClick( 'blue' ); }											  
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'green', 
											0,0,0,
											[[['./images/toolbar/color_3_up.png','./images/toolbar/color_3_up_grey.png'],
											  ['./images/toolbar/color_3_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/color_3_down.png','./images/toolbar/color_3_down_grey.png']]],
											  
											[[['Select the green pen.','Select the green pen.'],
											  ['Select the green pen.','Select the green pen.']],
											 [['Select the green pen.','Select the green pen.'],
											  ['Select the green pen.','Select the green pen.']]],
											function() { Presenter.OnColorClick( 'green' ); }											  
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'red', 
											0,0,0,
											[[['./images/toolbar/color_4_up.png','./images/toolbar/color_4_up_grey.png'],
											  ['./images/toolbar/color_4_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/color_4_down.png','./images/toolbar/color_4_down_grey.png']]],
											  
											[[['Select the red pen.','Select the red pen.'],
											  ['Select the red pen.','Select the red pen.']],
											 [['Select the red pen.','Select the red pen.'],
											  ['Select the red pen.','Select the red pen.']]],
											function() { Presenter.OnColorClick( 'red' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'yellow', 
											0,0,0,
											[[['./images/toolbar/color_5_up.png','./images/toolbar/color_5_up_grey.png'],
											  ['./images/toolbar/color_5_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/color_5_down.png','./images/toolbar/color_5_down_grey.png']]],
											  
											[[['Select the yellow pen.','Select the yellow pen.'],
											  ['Select the yellow pen.','Select the yellow pen.']],
											 [['Select the yellow pen.','Select the yellow pen.'],
											  ['Select the yellow pen.','Select the yellow pen.']]],
											function() { Presenter.OnColorClick( 'yellow' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'black', 
											1,1,0,
											[[['./images/toolbar/color_6_up.png','./images/toolbar/color_6_up_grey.png'],
											  ['./images/toolbar/color_6_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/color_6_down.png','./images/toolbar/color_6_down_grey.png']]],
											  
											[[['Select the black pen.','Select the black pen.'],
											  ['Select the black pen.','Select the black pen.']],
											 [['Select the black pen.','Select the black pen.'],
											  ['Select the black pen.','Select the black pen.']]],
											function() { Presenter.OnColorClick( 'black' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateSeparator() );
	// PEN TOOLBAR
	toolbar.appendChild( this.CreateButton( 'pen_tool', 
											1,1,0,
											[[['./images/toolbar/pen_up.png','active up grey'],
											  ['./images/toolbar/pen_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/pen_down.png','active down grey']]],
											  
											[[['Select the pen tool.','Select the pen tool.'],
											  ['Select the pen tool.','Select the pen tool.']],
											 [['Select the pen tool.','Select the pen tool.'],
											  ['Select the pen tool.','Select the pen tool.']]],
											function() { Presenter.OnToolClick( 'pen_tool' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'eraser_tool', 
											0,0,0,
											[[['./images/toolbar/eraser_up.png','inactive up grey'],
											  ['./images/toolbar/eraser_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/eraser_down.png','active down grey']]],
											  
											[[['Select the eraser tool.','Select the eraser tool.'],
											  ['Select the eraser tool.','Select the eraser tool.']],
											 [['Select the eraser tool.','Select the eraser tool.'],
											  ['Select the eraser tool.','Select the eraser tool.']]],
											function() { Presenter.OnToolClick( 'eraser_tool' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'text_tool', 
											0,0,0,
											[[['./images/toolbar/text_up.png','inactive up grey.png'],
											  ['./images/toolbar/text_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/text_down.png','active down grey']]],
											  
											[[['Select the text tool.','Select the text tool.'],
											  ['Select the text tool.','Select the text tool.']],
											 [['Select the text tool.','Select the text tool.'],
											  ['Select the text tool.','Select the text tool.']]],
											function() { Presenter.OnToolClick( 'text_tool' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateSeparator() );
	// ERASE TOOLBAR
	toolbar.appendChild( this.CreateButton( 'erase_all', 
											0,0,0,
											[[['./images/toolbar/eraseall_up.png','./images/toolbar/eraseall_up.png'],
											  ['./images/toolbar/eraseall_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['active down non-grey','active down grey']]],
											  
											[[['Erase all ink on the slide.','Erase all ink on the slide.'],
											  ['Erase all ink on the slide.','Erase all ink on the slide.']],
											 [['Erase all ink on the slide.','Erase all ink on the slide.'],
											  ['Erase all ink on the slide.','Erase all ink on the slide.']]],
											function() { Presenter.OnEraseAllClick( 'erase_all' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateSeparator() );
	// UNLINK TOOLBAR
	toolbar.appendChild( this.CreateButton( 'link_unlink', 
											1,1,0,
											[[['./images/toolbar/linked-student_up.png','./images/toolbar/linked-student_up_grey.png'],
											  ['./images/toolbar/linked-student_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['./images/toolbar/linked-student_down.png','active down grey']]],
											  
											[[['Erase all ink on the slide.','Erase all ink on the slide.'],
											  ['Erase all ink on the slide.','Erase all ink on the slide.']],
											 [['Erase all ink on the slide.','Erase all ink on the slide.'],
											  ['Erase all ink on the slide.','Erase all ink on the slide.']]],
											function() { Presenter.OnLinkUnlinkClick( 'link_unlink' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateSeparator() );
	// STUDENT SUBMISSION TOOLBAR
	toolbar.appendChild( this.QPControl.UIObject );
	toolbar.appendChild( this.CreateButton( 'submit', 
											0,0,0,
											[[['./images/toolbar/submit_up.png','./images/toolbar/submit_up.png'],
											  ['./images/toolbar/submit_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['active down non-grey','active down grey']]],
											  
											[[['Advance to the previous slide.','Advance to the previous slide.'],
											  ['Advance to the previous slide.','Advance to the previous slide.']],
											 [['Advance to the previous slide.','Advance to the previous slide.'],
											  ['Advance to the previous slide.','Advance to the previous slide.']]],
											function() { Presenter.OnSubmitClick( 'submit' ); }
										   ).UIObject );
	toolbar.appendChild( this.CreateSeparator() );
	// NAVIGATION TOOLBAR
	toolbar.appendChild( this.CreateButton( 'backward', 
											0,0,1,
											[[['./images/toolbar/back_up.png','./images/toolbar/back_up_grey.png'],
											  ['./images/toolbar/back_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['active down non-grey','active down grey']]],
											  
											[[['Advance to the previous slide.','Advance to the previous slide.'],
											  ['Advance to the previous slide.','Advance to the previous slide.']],
											 [['Advance to the previous slide.','Advance to the previous slide.'],
											  ['Advance to the previous slide.','Advance to the previous slide.']]],
											function() { /*Presenter.OnBackwardClick( 'backward' );*/ }
										   ).UIObject );
	toolbar.appendChild( this.CreateButton( 'forward', 
											0,0,1,
											[[['./images/toolbar/forward_up.png','./images/toolbar/forward_up_grey.png'],
											  ['./images/toolbar/forward_pressed.png','inactive down grey']],
											 [['active up non-grey','active up grey'],
											  ['active down non-grey','active down grey']]],
											  
											[[['Advance to the next slide.','Advance to the next slide.'],
											  ['Advance to the next slide.','Advance to the next slide.']],
											 [['Advance to the next slide.','Advance to the next slide.'],
											  ['Advance to the next slide.','Advance to the next slide.']]],
											function() { /*Presenter.OnForwardClick( 'forward' );*/ }
										   ).UIObject );
    this.UIObject = toolbar;	
}
// Create a button for the toolbar
P_UI.Toolbar.prototype.CreateButton = function( name, active, down, greyed, srcs, alts, click ) {
	var button = document.createElement('img');
	button.id = name;
	button.name = name;
	var toolbar_button = new P_UI.ToolbarButton( button, active, down, greyed, srcs, alts, click);
	this.buttons[name] = toolbar_button;
	
	return toolbar_button;
}
// Create a separator for the toolbar
P_UI.Toolbar.prototype.CreateSeparator = function() {
    return document.createTextNode('\u00a0\u00a0');
}

//---------------------------------------------------------
//	class QuickPollControl
// 	    A quick poll control, basically a drop down list
//      that allows 
//---------------------------------------------------------
P_UI.QuickPollControl = function( name, options, change_handler ) {
    // The Value Currently Selected
    this.CurrentIndex = 0;
    this.ChangeHandler = change_handler;

    // Create the UI elements
    var select = document.createElement( 'select' );
    select.id = name;
    select.style.verticalAlign = 'top';
    
    this.UIObject = select;
    
    // Add the options to the quick poll control
    this.SetOptions( options );
    
    // Listen to changes and update the value
    select.onchange = this.CreateChangeHandler( this );
}
P_UI.QuickPollControl.prototype.SetValue = function( value ) {
}
// Set the options for everything 
P_UI.QuickPollControl.prototype.SetOptions = function( options ) {
    // Remove all child nodes if we want to
    while( this.UIObject.hasChildNodes() ) {
        this.UIObject.removeChild( this.UIObject.lastChild );
    }
    // Append all the options
    this.AppendOption( 'Select a Choice:' );
    for( var i=0; i<options.length; i++ ) {
        this.AppendOption( options[i] );
    }
}
// Append a new option to the quick poll
P_UI.QuickPollControl.prototype.AppendOption = function( text ) {
    var o = document.createElement( 'option' );
    o.text = text;
    try {
        this.UIObject.add( o, null );
    } catch( ex ) {
        this.UIObject.add( o );
    }
}
// Disable and hide the quick poll
P_UI.QuickPollControl.prototype.Hide = function() {
    this.UIObject.style.visibility = 'hidden';
    this.UIObject.style.display = 'none';
    this.CurrentIndex = 0;
    this.UIObject.selectedIndex = 0;
}
// Enable and show the quick poll
P_UI.QuickPollControl.prototype.Show = function() {
    this.UIObject.style.visibility = 'visible';
    this.UIObject.style.display = 'inline';
}
// Functor to create a event handler that keeps the quick poll object around
// so that we can keep track of the quickpoll object in the event handler 
P_UI.QuickPollControl.prototype.CreateChangeHandler = function ( obj ) {
	return function ( e ) {
		// Boiler Plate Header
		e = e || event;
		var target = e.target || e.srcElement;

		// Main Code
		// Update the values of the current index, based on the selected index
		if( obj.CurrentIndex == 0 ) {
		    // Only update if the value actually changes
		    if( obj.CurrentIndex != this.selectedIndex ) {
    		    obj.CurrentIndex = this.selectedIndex;
		        if( obj.ChangeHandler != null )
    		        obj.ChangeHandler();
    		}
		} else {
		    // Don't allow the value to be rechanged to 0, after it has left.
		    if( this.selectedIndex == 0 ) {
		        this.selectedIndex = obj.CurrentIndex;
		        alert( 'You cannot unvote after you have already voted.' );
		    } else {
		        // Only update if the value actually changes
		        if( obj.CurrentIndex != this.selectedIndex ) {
		            obj.CurrentIndex = this.selectedIndex;
		            if( obj.ChangeHandler != null )
            		    obj.ChangeHandler();
		        }
		    }
		}
		
		// Boiler Plate Footer
		e.cancelBubble = true;
		if( e.stopPropagation ) {
			e.stopPropagation();
		}
		e.returnValue = false;
		if( e.preventDefault ) {
			e.preventDefault();
		}
		return false;
	}
}



//---------------------------------------------------------
//	class ToolbarButton
// 		Class representing a button on the toolbar
//---------------------------------------------------------
P_UI.ToolbarButton = function ( obj, active, down, greyed, paths, alts, click ) {
	this.UIObject = obj;
	this.ImagePaths = paths;
	this.AltText = alts;
	this.Active = active;
	this.Down = down;
	this.Greyed = greyed;
	this.UIObject.src = this.ImagePaths[this.Active][this.Down][this.Greyed];
	this.UIObject.alt = this.AltText[this.Active][this.Down][this.Greyed];
	obj.onclick = click;
}
// Sets the button state of the button
P_UI.ToolbarButton.prototype.setState = function( active, down, greyed ) {
	this.Active = active;
	this.Down = down;
	this.Greyed = greyed;
	this.UIObject.src = this.ImagePaths[this.Active][this.Down][this.Greyed];
	this.UIObject.alt = this.AltText[this.Active][this.Down][this.Greyed];
}

//---------------------------------------------------------
//	class MainSlideView
// 		Construct the main slide viewer
//---------------------------------------------------------
P_UI.MainSlideView = function () {
	// Setup the UI for the main slide view
	var view = document.createElement( 'div' );
	view.id = 'main_view';
	view.style.backgroundColor = 'white';
	view.style.position = 'relative';
	view.style.width = '720px';
	view.style.height = '540px';
	view.style.overflow = 'visible';
	
	// Setup the background image
	this.Img = document.createElement( 'img' );
	this.Img.style.position = 'absolute';
	this.Img.style.zIndex = 1;
	this.Img.alt = 'No Slide';
	view.appendChild( this.Img );
	
	// Keep a reference to this UI element
	this.UIObject = view;
	
	// Add the two ink layers. We want the layer the user interacts with always on top
	this.InstructorInkLayer = new P_UI.RemoteInkLayer( this.UIObject, 'instructorInkLayer' );
	this.InkLayer = new P_UI.InkLayer( this.UIObject, 'inkLayer' );
}
// Sets the slide image that is currently being displayed
P_UI.MainSlideView.prototype.SetImage = function( filename ) {
	this.Img.src = /*Presenter.ClientState.Path +*/ filename;
}

//---------------------------------------------------------
//	class SlideView
// 		Construct a div with ink layers
//---------------------------------------------------------
P_UI.SlideView = function ( name ) {
    // Default Width and Height
    this.Width = 720;
    this.Height = 540;

	// Setup the UI for the main slide view
	var view = document.createElement( 'div' );
	view.id = name;
	view.style.backgroundColor = 'white';
	view.style.position = 'relative';
	view.style.width = this.Width + 'px';
	view.style.height = this.Height + 'px';
	view.style.overflow = 'visible';
	
	// Setup the background image
	this.Img = document.createElement( 'img' );
	this.Img.style.position = 'absolute';
	this.Img.style.zIndex = 1;
	this.Img.alt = 'No Slide';
	view.appendChild( this.Img );
	
	// Keep a reference to this UI element
	this.UIObject = view;
	
	// Add the two ink layers. We want the layer the user interacts with always on top
	this.InstructorInkLayer = new P_UI.RemoteInkLayer( this.UIObject, 'instructorInkLayer' );
//	this.InkLayer = new P_UI.RemoteLayer( this.UIObject, 'inkLayer' );
}
// Sets the slide image that is currently being displayed
P_UI.SlideView.prototype.SetImage = function( filename ) {
	this.Img.src = /*Presenter.ClientState.Path +*/ filename; 
}
// Only use this function to resize the slide view because we need to resize multiple controls
P_UI.SlideView.prototype.Resize = function( w, h ) {
    this.Width = w;
    this.Height = h;
    // Resize the slide view div
    this.UIObject.style.width = this.Width + 'px';
    this.UIObject.style.height = this.Height + 'px';
    // Resize the background layers of the slide
    this.Img.stype.width = this.Width + 'px';
    this.Img.style.height = this.Height + 'px';
    // Resize all the annotation layers of the slide
}
// Get the width of the slide
P_UI.SlideView.prototype.GetWidth = function() {
    return this.Width;
}
// Get the height of the slie
P_UI.SlideView.prototype.GetHeight = function() {
    return this.Height;
}


// -------------------- HELPER FUNCTIONS ----------------------
// There functions aren't mine, but they are designed to get
// the location of a div relative to the screen corner. They
// are used so that we can get the coordinate of the cursor
// within the div when writing ink...
function zxcWWHS() {
	if (document.all){
		zxcWH=document.documentElement.clientHeight;
		zxcWW=document.documentElement.clientWidth;
		zxcWS=document.documentElement.scrollTop; //ie trans & strict
		if (zxcWH==0){
			zxcWS=document.body.scrollTop; //ie trans & strict
			zxcWH=document.body.clientHeight;
			zxcWW=document.body.clientWidth;
		}
	}
	else if (document.getElementById){
		zxcWH=window.innerHeight-15;
		zxcWW=window.innerWidth-15;
		zxcWS=0;//window.pageYOffset;
	}
	zxcWC=Math.round(zxcWW/2);
	return zxcWS;
}

function zxcPos(zxc){
	zxcObjLeft = zxc.offsetLeft;
	zxcObjTop = zxc.offsetTop;
	while ( zxc.offsetParent!=null ) {
		zxcObjParent=zxc.offsetParent;
		zxcObjLeft+=zxcObjParent.offsetLeft;
		zxcObjTop+=zxcObjParent.offsetTop;
		zxc=zxcObjParent;
	}
	return [zxcObjLeft,zxcObjTop];
}
// -------------------- END HELPER FUNCTIONS ----------------------

//---------------------------------------------------------
//	class InkLayer
// 		Construct a new InkLayer object.
//		This layer is designed to get user input and convert
//		it into ink strokes.
//---------------------------------------------------------
P_UI.InkLayer = function ( node, name ) {
	// Local Variables
	// 1 when the mouse is down on the canvas, 0 otherwise
	this.inkLayerMouseStatus = 0;
	// The layer that we render the strokes into
	this.inkDiv = null;
	// The collections of ink strokes
	this.inkStrokes = new Ink.StrokeCollection();
	// The current potentially incomplete stroke
	this.currentStroke = null;
	// The last location of the cursor in the current stroke
	this.lastX = null;
	this.lastY = null;
	
	// Create the Ink Div
	this.inkDiv = document.createElement( 'div' );
	this.inkDiv.style.position = 'absolute';
	this.inkDiv.style.width = '720px';
	this.inkDiv.style.height = '540px';
	this.inkDiv.style.zIndex = 2;
	this.inkDiv.style.id = name;
	node.appendChild( this.inkDiv );

	// The graphics
	this.graphics = new jsGraphics( this.inkDiv );	

	// Draws a line segment
	this.drawLine = function ( color, xs, ys, xe, ye ) {
		this.graphics.setColor( color );
		this.graphics.setStroke( 2 );
		this.graphics.drawLine( xs, ys, xe, ye );
		this.graphics.paint();
	}
	
	// Clear the canvas of any ink
	this.clear = function() {
		this.graphics.clear();
		this.inkStrokes = new Ink.StrokeCollection();
	}
	
	// Redraw all the strokes in this collection to the canvas
	this.redraw = function() {
		this.graphics.clear();
		if( this.inkStrokes ) {
			for( var i=0; i<this.inkStrokes.strokeData.length; i++ ) {
				// Draw each stroke
				this.drawStroke( this.inkStrokes.strokeData[i] );
			}
		}
	}
	
	// Draw a stroke from a stroke object
	this.drawStroke = function( stroke ) {
		if( stroke ) {
			// Set stroke attributes
			this.graphics.setColor(stroke.color);
			this.graphics.setStroke( 2 );
		
			// Get the stroke coordinates
			var xCoords = new Array();
			var yCoords = new Array();
			for( var i=0; i<stroke.pts.length; i++ ) {
				xCoords.push( stroke.pts[i].x );
				yCoords.push( stroke.pts[i].y );
			}
		
			// Draw the stroke
			this.graphics.drawPolyline( xCoords, yCoords );
			this.graphics.paint();
		}
	}
		
	// ==== IPOD Event Handlers ====
	// TODO: Need to fix this to use the latest graphics library
	// Handle the ipod touches touch down event
	this.handleTouchDown = ( function ( obj ) {
		return function ( e ) {
			// Main Code
			if( e.touches.length == 1 ) { 	// Only deal with one finger
				obj.inkLayerMouseStatus = 1;
				obj.inkDiv.addEventListener( 'touchmove', obj.handleTouchMove, false);		
			}
	
			// Boiler Plate Footer
			e.cancelBubble = true;
			if( e.stopPropagation ) {
				e.stopPropagation();
			}
			e.returnValue = false;
			if( e.preventDefault ) {
				e.preventDefault();
			}
			return false;	
		}
	} )(this);

	// Handle the finger being raised on the ipod touch
	this.handleTouchUp = ( function ( obj ) {
		return function ( e ) {
			// Main Code
			if( e.touches.length == 1 ) { 	// Only deal with one finger
				obj.inkLayerMouseStatus = 0;
				obj.inkDiv.removeEventListener( 'touchmove', obj.handleTouchMove , false);		
			}
	
			// Boiler Plate Footer
			e.cancelBubble = true;
			if( e.stopPropagation ) {
				e.stopPropagation();
			}
			e.returnValue = false;
			if( e.preventDefault ) {
				e.preventDefault();
			}
			return false;	
		}
	} )(this);

	// Handle the finger moving in the browser
	this.handleTouchMove = ( function ( obj ) {
		return function ( e ) {
			if( e.touches.length == 1 ) { 	// Only deal with one finger
				var touch = e.touches[0]; 	// Get the information for finger #1
				var x = touch.pageX;
				var y = touch.pageY;
				obj.createInkPixel( 'black', x, y, 2, 2 );
			}
		
			// Boiler Plate Footer
			e.cancelBubble = true;
			if( e.stopPropagation ) {
				e.stopPropagation();
			}
			e.returnValue = false;
			if( e.preventDefault ) {
				e.preventDefault();
			}
			return false;	
		}
	} )(this);

	// ==== Browser Event Handlers ====
	// Handle the mouse being pressed down in the ink area
	// NOTE: We need to use closure to access the object from an event handler
	this.handleMouseDown = ( function( obj ) { 
		return function ( e ) { 
			// Boiler Plate Header
			e = e || event;
			var target = e.target || e.srcElement;
	
			// Main Code
			// Denote that we are ready to add strokes...
			obj.inkLayerMouseStatus = 1;
			obj.currentStroke = new Ink.Stroke();
//			obj.inkDiv.onmousemove = obj.handleMouseMove;
			document.onmousemove = obj.handleMouseMove;
	
			// Boiler Plate Footer
			e.cancelBubble = true;
			if( e.stopPropagation ) {
				e.stopPropagation();
			}
			e.returnValue = false;
			if( e.preventDefault ) {
				e.preventDefault();
			}
			return false;	
		}	 
	} )(this);

	// End the stroke being drawn
	this.endStroke = function() {
		// Main Code
		// Zero out all stroke values and add the current stroke to the collection of strokes
		this.inkLayerMouseStatus = 0;
		if( this.currentStroke != null ) {
			if( this.currentStroke.pts.length != 0 ) {
				this.currentStroke.color = Presenter.ClientState.penColor;
				this.inkStrokes.addStroke( this.currentStroke );
			}
		}
		this.currentStroke = null;
		this.lastX = null;
		this.lastY = null;

//		if( this.inkDiv )
//			this.inkDiv.onmousemove = null;	
		document.onmousemove = null;
	}
	
	// Handle the mouse being released anywhere
	// NOTE: We need to use closure to access the object from an event handler
	this.handleMouseUp = ( function ( obj ) {
		return function ( e ) {
			// Boiler Plate Header
			e = e || event;
			var target = e.target || e.srcElement;

			// Main Code
			// Zero out all stroke values and add the current stroke to the collection of strokes
			obj.endStroke();
	
			// Boiler Plate Footer
			e.cancelBubble = true;
			if( e.stopPropagation ) {
				e.stopPropagation();
			}
			e.returnValue = false;
			if( e.preventDefault ) {
				e.preventDefault();
			}
			return false;
		}
	} )(this);

	// Handle the mouse moving in the ink area
	// NOTE: We need to use closure to access the object from an event handler
	this.handleMouseMove = ( function ( obj ) {
		return function ( e ) {
			// Boiler Plate Header
			e = e || event;
			var target = e.target || e.srcElement;
	
			// Get the X and Y coordinates in pixels
			var x = e.clientX;
			var y = e.clientY;
			x = e.clientX - zxcPos( obj.inkDiv )[0];
			y = e.clientY - zxcPos( obj.inkDiv )[1] + zxcWWHS();
			if( x >= 0 && y >= 0 && x < 720 && y < 540 ) {

				// Draw a line segment if this is the second point
				if( obj.lastX != null && obj.lastY != null ) {
					obj.drawLine( Presenter.ClientState.penColor, obj.lastX, obj.lastY, x, y );
				}
				obj.lastX = x;
				obj.lastY = y;

				// Add the point to the stroke object
				if( obj.currentStroke != null ) {
					obj.currentStroke.addPoint( x, y, 0 );
				}
			}
			// Boiler Plate Footer
			e.cancelBubble = true;
			if( e.stopPropagation ) {
				e.stopPropagation();
			}
			e.returnValue = false;
			if( e.preventDefault ) {
				e.preventDefault();
			}
			return false;	
		}
	} )(this);
	
	// Setup the message handlers
	if( /iPhone/.test(navigator.userAgent) ) {
		document.body.addEventListener( 'touchend', this.handleTouchUp, false);
		this.inkDiv.addEventListener( 'touchstart', this.handleTouchDown, false);
	} else {
		document.onmouseup = this.handleMouseUp;
		document.onmousedown = this.handleMouseDown;
//		this.inkDiv.onmousedown = this.handleMouseDown;
	}	
}

//---------------------------------------------------------
//	class RemoteInkLayer
// 		Construct a new RemoteInkLayer object
//		The remote ink layer is used to draw ink from
//		remote sources, such as from the instructor
//---------------------------------------------------------
P_UI.RemoteInkLayer = function ( node, name ) {
	// Local Variables
	// The UI object that is used to render the ink
	this.inkDiv = null;
	// The ink strokes that are in the remote layer
	// TODO: Eventually keep track of drawn strokes here to smartly update when we are asked to draw new stuff
	this.inkStrokes = new Ink.StrokeCollection();
	
	// Create the Ink Div
	this.inkDiv = document.createElement( 'div' );
	this.inkDiv.style.position = 'absolute';
	this.inkDiv.style.width = '720px';
	this.inkDiv.style.height = '540px';
	this.inkDiv.style.zIndex = 1;
	this.inkDiv.style.id = name;
	node.appendChild( this.inkDiv );
	
	// The graphics
	this.graphics = new jsGraphics( this.inkDiv );	
				
	// Draw all strokes
	this.drawInk = function( inks ) {
		// Draw each stroke
		for( var i=0; i<inks.length; i++ ) {
			this.drawStroke( inks[i] );
		}
	}
	
	// Draw a stroke from a collection of points
	this.drawStroke = function( pts ) {
		// Set stroke attributes
		this.graphics.setColor('#000000');
		this.graphics.setStroke( 2 );
		
		// Get the stroke coordinates
		var xCoords = new Array();
		var yCoords = new Array();
		for( var i=0; i<pts.length; i+=2 ) {
			xCoords.push( pts[i] );
			yCoords.push( pts[i+1] );
		}
		
		// Draw the stroke
		this.graphics.drawPolyline( xCoords, yCoords );
		this.graphics.paint();
	}
	
	// Clear all the ink from this layer
	this.clear = function() {
		this.graphics.clear();
		this.inkStrokes = new Ink.StrokeCollection();
	}	
}

//---------------------------------------------------------
//	class FilmStrip
// 		Construct a new Filmstrip object
//		The filmstrip is used to show all the decks and all
//      the slides in a deck.
//---------------------------------------------------------
P_UI.FilmStrip = function ( name ) {
	// Local Variables
    // Settings
    this.removeChildNodesWhenHidden = false;    
    this.onCreateElements = function () {};
	
	// Construct the div
	this.UIObject = null;
	this.UIObject = document.createElement( 'div' );
	this.UIObject.id = name;
	this.UIObject.style.position = 'absolute';
	this.UIObject.style.left = '730px';
	this.UIObject.style.width = '200px';
	this.UIObject.style.height = '540px';
	this.UIObject.style.backgroundColor = '#FF0000';
    this.UIObject.style.overflow = 'scroll';
		
	// Show and Hide the Filmstrip
	this.show = function() {
	    if( this.removeChildNodesWhenHidden ) {
	        this.onCreateElements();
	    }
	    this.UIObject.style.visibility = 'visible';
	}
	this.hide = function() {
	    if( this.removeChildNodesWhenHidden ) {
	        // Remove all child nodes if we want to
            while( this.UIObject.hasChildNodes() ) {
                this.UIObject.removeChild( this.UIObject.lastChild );
            }
	    }
	    this.UIObject.style.visibility = 'hidden';
	}
}

//---------------------------------------------------------
//	class TabbedFilmStrip
// 		Construct a new TabbedFilmstrip object
//		The filmstrip is used to show all the decks and all
//      the slides in a deck.
//---------------------------------------------------------
P_UI.TabbedFilmStrip = function( node, name ) {
    // Array of filmstrips
    this.filmStrips = [];
}
