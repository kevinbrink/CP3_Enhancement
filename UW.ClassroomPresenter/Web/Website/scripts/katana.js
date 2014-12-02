// Ensure that Katana has been defined
if( !this.Katana ) {
	Katana = {};
}

// Helper function that generates a random string that should be unique for all clients
function randomString( iLen ) {
	var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
	var string_length = iLen;
	var randomstring = '';
	for (var i=0; i<string_length; i++) {
		var rnum = Math.floor(Math.random() * chars.length);
		randomstring += chars.substring(rnum,rnum+1);
	}
	return randomstring;
}

// Singleton network class
Katana.network = {
	// False when we haven't yet called initialized
	_initialized: false,
	
	// The number of simultaneous outstanding polling requests that we have
	_pushThreads: 1,
	
	_recvFunction: null,
	
	// Used internally in order to keep track of what we are sending
	_sendFrame: null,
	_sendForm: null,
	_sendText: null,
	
	// Generate a unique id for me
	_uid: randomString( 8 ),
	
	// Initialize the networking by building the appropriate hidden iframes
	initialize: function( send_address, recv_address, recv_function, push_threads ) {
		// Handle default values
//		var send_address  = (send_address  == null) ? 'http://localhost:9090/post' : send_address;
//		var recv_address  = (recv_address  == null) ? 'http://localhost:9090/recv' : recv_address;
//		var send_address  = (send_address  == null) ? 'http://wye.dyn.cs.washington.edu:9090/post' : send_address;
//		var recv_address  = (recv_address  == null) ? 'http://wye.dyn.cs.washington.edu:9090/recv' : recv_address;
		var send_address  = (send_address  == null) ? 'post' : send_address;
		var recv_address  = (recv_address  == null) ? 'recv' : recv_address;
		var recv_function = (recv_function == null) ? function(obj){} : recv_function;
		var push_threads  = (push_threads  == null) ? 1 : push_threads;
	
		// Set the number of push threads
		this._pushThreads = push_threads;
		
		// Set the receive function
		this._recvFunction = recv_function;
		
		// Create the sending iframe
		var iframe;
		try {
			iframe = document.createElement('<iframe name="katana_send">');
		} catch (ex) {
			iframe = document.createElement('iframe');
		}
		iframe.id = 'katana_send';
		iframe.name = 'katana_send';
		iframe.style.display = 'none';
		document.body.appendChild( iframe );
/*		
		var element = document.createElement( 'iframe' );
		element.id = 'katana_send2';
		element.name = 'katana_send';
		element.style.display = 'none';
		document.body.appendChild( element );
*/		
		// Create the sending form
		var form = document.createElement( 'form' );
		form.action = send_address;
		form.method = 'POST';
		form.target = 'katana_send';
		form.style.display = 'none';
		
		// Create the text area
		var textarea = document.createElement( 'textarea' );
		textarea.name = 'data';
		textarea.value = '';
		textarea.style.display = 'none';
		form.appendChild( textarea );
		
		// Create the submit button
		var input = document.createElement( 'input' );
		input.type = 'submit';
		input.value = 'test';
		input.style.display = 'none';
		form.appendChild( input );
		
		document.body.appendChild( form );

		// Cache these values for later
		this._sendFrame = iframe;
		this._sendForm = form;		
		this._sendText = textarea;
		
		// Create the receiving iframes
		for( var i = 0; i< this._pushThreads; i++ ) {
			iframe = document.createElement( 'iframe' );
			iframe.id = 'katana_' + i;
			iframe.style.display = 'none';
			document.body.appendChild( iframe );
			iframe.src = recv_address + '?u=' + this._uid;
		}
		
		// Identify that we are initialized
		this._initialized = true;
	},
	
	// Send the given object as JSON to the server
	post: function ( object ) {
		if( !this._initialized ) return;
		
		var wrapper = {};
		wrapper.uid = this._uid;
		wrapper.d = object;
		
		var JSONString = JSON.stringify( wrapper );
		alert( JSONString );
		this._sendText.value = JSONString;
		this._sendForm.submit();
	},

	// Send the given object as JSON to the server
	post2: function ( str ) {
		if( !this._initialized ) return;
		
		this._sendText.value = str;
		this._sendForm.submit();
	},

	
	// Build a query and submit it in the sending iframe
	send: function ( address, query_string ) {
		if( !this._initialized ) return;	
		var address = (address == null) ? 'post' : address;
		var query_string = (query_string == null) ? '' : query_string;

		this._sendFrame.src = address + query_string;
	},
	
	// This function gets invokes when the server pushes something
	deliver: function ( json_string ) {
		if( !this._initialized ) return;
		
		if( json_string != '' ) {
			var myJSONObject = JSON.parse( json_string );
			this._recvFunction( myJSONObject );
		}
	}
};
