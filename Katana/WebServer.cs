using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Web;

namespace Katana {
    /// <summary>
    /// Class encapsulating the functions required by the web server
    /// Internally uses the mongoose web server
    /// </summary>
    public class WebServer : IDisposable {
        #region Connections

        /// <summary>
        /// Queue that holds the various requests that people have made
        /// </summary>
        protected static Queue RecvQueue = new Queue();

        /// <summary>
        /// Returns whether or not there are packets that have been received
        /// </summary>
        /// <returns>True if there are packets pending, false otherwise</returns>
        public bool HasPackets() {
            // Lock the queue
            lock (WebServer.RecvQueue) {
                // Check if there are packets
                return (WebServer.RecvQueue.Count != 0);
            }
        }

        /// <summary>
        /// Returns the packet that is at the head of the queue
        /// </summary>
        /// <returns>The string of the returned packet</returns>
        public string DequeuePacket() {
            // Lock the queue
            lock (WebServer.RecvQueue) {
                // Return the first item
                return (string)WebServer.RecvQueue.Dequeue();
            }
        }

        /// <summary>
        /// Broadcast a packet to all listeners
        /// </summary>
        /// <param name="packet">The object as a JSON strong</param>
        public void BroadcastPacket( string packet ) {
            // Lock the table of connections
            lock (WebServer.Connections) {
                foreach (Connection conn in WebServer.Connections.Values) {
                    // TODO: For now since we always send the whole model, just allow one packet in the queue
                    conn.SendingQueue.Clear();
                    conn.SendingQueue.Enqueue(packet);
                }
            }
        }

        /// <summary>
        /// Send a packet to a single listener
        /// </summary>
        /// <param name="connection">The name of the person to send to</param>
        /// <param name="packet">The packet to send</param>
        public void SendPacket( string connection, string packet ) {
            lock (WebServer.Connections) {
                if (WebServer.Connections.ContainsKey(connection)) {
                    ((Connection)WebServer.Connections[connection]).SendingQueue.Enqueue(packet);
                }
            }
        }

        /// <summary>
        /// A hashtable of all the connections that we have had so far...
        /// </summary>
        protected static Hashtable /* string to Connection */ Connections = new Hashtable();

        /// <summary>
        /// Represents a connection established to a 
        /// </summary>
        protected class Connection {
            /// <summary>
            /// The name of the connection
            /// </summary>
            public string Name = string.Empty;
            public Queue SendingQueue = new Queue();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The context being used by the server
        /// </summary>
        private IntPtr ServerContext = IntPtr.Zero;
        /// <summary>
        /// An array of bindings to handle special URIs
        /// </summary>
        private ArrayList Bindings = new ArrayList();

        /// <summary>
        /// Special structure that represents a binding
        /// NOTE: We need to keep this information around
        ///       so that we can clean up memory properly
        /// </summary>
        private struct PageBinding
        {
            /// <summary>
            /// The path that is being handled
            /// </summary>
            public string Path;
            /// <summary>
            /// The unmanaged memory that contains the path string
            /// </summary>
            public IntPtr ptrPath;
            /// <summary>
            /// A reference to the handler method so that we don't garbage collect it
            /// </summary>
            public mg_callback_t cbHandler;
            //            public IntPtr cbHandler;
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor for the web-server
        /// </summary>
        /// <param name="root">The full path to the directory to use as the root 
        /// of the web directory</param>
        /// <param name="port">The string representing the port number to listen on</param>
        public WebServer(string root, string port)
        {
            // Create the server
            this.ServerContext = mg_start();
            mg_set_option(this.ServerContext, "ports", port);
            mg_set_option(this.ServerContext, "root", root);

            // Create the bindings
            this.CreateBinding("/post", WebServer.SendHandler );
            this.CreateBinding("/recv", WebServer.RecvHandler );
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Dispose simply called Dispose(true)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// NOTE: Leave out the finalizer altogether if this class doesn't 
        /// own unmanaged resources itself, but leave the other methods
        /// exactly as they are. 
        ~WebServer()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// The bulk of the clean-up code is implemented here
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free mananged resources (if any)
            }

            // Clean up all allocated memory
            foreach (PageBinding b in this.Bindings)
            {
                Marshal.FreeHGlobal(b.ptrPath);
            }
            this.Bindings.Clear();

            // Stop the web server
            mg_stop(this.ServerContext);
            this.ServerContext = IntPtr.Zero;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Helper method that takes an HTTP POST stream that is URL Encoded, finds the form
        /// element that was named "data" and returns the JSON string that that element contains.
        /// </summary>
        /// <param name="data">The POST data</param>
        /// <returns>The JSON string, returns null if there is an error</returns>
        private static string GetJSON(string data) {
            // Get each of the form values
            string[] formElements = data.Split(new char[] { '&' });

            // We want to look for the "data" named element value
            foreach (string s in formElements) {
                // Check if the data name is correct
                if (s.StartsWith("data=")) {
                    // Split apart the element name and the value
                    string[] parts = s.Split(new char[] { '=' });
                    // Check to see if the element has two parts
                    if (parts.Length == 2) {
                        // Decode the second part into the final JSON string
                        return HttpUtility.UrlDecode(parts[1]);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Send/Recv

        /// <summary>
        /// Handle the client posting data via the web server
        /// </summary>
        /// <param name="conn">The connection that we are working with</param>
        /// <param name="request_info">The request information</param>
        /// <param name="user_data">Custom data that we passed in</param>
        public static void SendHandler( IntPtr conn, /*mg_request_info*/IntPtr request_info, IntPtr user_data ) {
            // Marshal the point to the request info data structure
            mg_request_info request = (mg_request_info)Marshal.PtrToStructure(request_info, typeof(mg_request_info));

            // Get the JSON object as a string and add it to the queue
            string jsonData = WebServer.GetJSON( request.post_data.Substring(0, request.post_data_len) );
            lock (WebServer.RecvQueue) {
                WebServer.RecvQueue.Enqueue(jsonData);
            }

            // No need to return anything
            string output = "HTTP/1.0 200 OK\n" +
                            "Date: Sun, Feb 01 02:15:59 PST\n" +
                            "Content-Type: text/html\n\n";
            mg_write(conn, output, output.Length);
        }

        /// <summary>
        /// Handle pushing data from the server to the client
        /// </summary>
        /// <param name="conn">The connection that we are working with</param>
        /// <param name="request_info">The request information</param>
        /// <param name="user_data">Custom data that we passed in</param>
        public static void RecvHandler(IntPtr conn, /*mg_request_info*/IntPtr request_info, IntPtr user_data) {
            // Marshal the point to the request info data structure
            mg_request_info request = (mg_request_info)Marshal.PtrToStructure(request_info, typeof(mg_request_info));

            // The total number of timeouts that have passed tolerate
            int totalTimeouts = 0;
            // The username of the person we are waiting for
            string username = request.query_string.Substring( 2 );
            // The data in the packet waiting to be sent
            string data = null;

            // We need to look at the query string to see which user it is from
            while( data == null && totalTimeouts < 4 ) {
                // Check to see if there is more data
                lock (WebServer.Connections) {
                    if( !WebServer.Connections.ContainsKey(username) ) {
                        // Add it as a connection
                        Connection c = new Connection();
                        c.Name = username;
                        WebServer.Connections.Add(username, c);
                    }

                    // Check to see if the queue has data
                    if (((Connection)WebServer.Connections[username]).SendingQueue.Count != 0) {
                        data = (string)(((Connection)WebServer.Connections[username]).SendingQueue.Dequeue());
                        continue;
                    }
                }

                // Sleep the thread
                System.Threading.Thread.Sleep(100);
                totalTimeouts++;
            }

            // Output the connection response
            string output = "HTTP/1.0 200 OK\n" +
                            "Date: Sun, Feb 01 02:15:59 PST\n" +
                            "Content-Type: text/html\n\n";
            output += "<html>\n";
            output += "<head><script>var temp=\'" + ((data != null) ? data : "") + "\';</script></head>";
            output += "<body onload=\"parent.Katana.network.deliver(temp);window.location.reload()\">\n";
            output += "</body>\n";
            output += "</html>\n";

            // Write out the data
            mg_write(conn, output, output.Length);
        }

        #endregion

        #region Bindings

        /// <summary>
        /// Create a binding between a special web page and the server result
        /// </summary>
        /// <param name="pathRegex">A regular expression that is matched with
        /// the URI path</param>
        /// <param name="handler">The function that is called when a match is 
        /// found</param>
        public void CreateBinding(string pathRegex, mg_callback_t handler)
        {
            // Create the binding
            PageBinding binding;
            binding.Path = pathRegex;
            binding.ptrPath = Marshal.StringToHGlobalAnsi(pathRegex);
            binding.cbHandler = new mg_callback_t(handler);
            //            binding.cbHandler = Marshal.GetFunctionPointerForDelegate( new mg_callback_t(handler) );

            // Add the binding to the array
            this.Bindings.Add(binding);
            // Add the binding to mongoose
            mg_bind_to_uri(this.ServerContext, binding.ptrPath, binding.cbHandler, IntPtr.Zero);
        }

        #endregion

        #region Mongoose Interop

        // Structure representing a part of the HTTP header
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mg_http_header
        {
            public string name;
            public string value;
        }

        // Structure representing an HTTP request
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mg_request_info
        {
            public string request_method;
            public string uri;
            public string query_string;
            public string post_data;
            public string remote_user;
            public int remote_ip;
            public int remote_port;
            public int post_data_len;
            public int http_version_minor;
            public int http_version_major;
            public int status_code;
            public int num_headers;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public mg_http_header[] http_headers;
        }

        // Function delegate for handling the callback when a special path has been entered
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void mg_callback_t(IntPtr ctx, IntPtr request_info, IntPtr user_data);

        // Start the mongoose web server
        [DllImport("mongoose.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr mg_start();
        // Stop the mongoose web server
        [DllImport("mongoose.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void mg_stop(IntPtr ctx);
        // Set a mongoose server option
        [DllImport("mongoose.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int mg_set_option(IntPtr ctx, string opt_name, string opt_value);
        // Bind a given url to a handler
        [DllImport("mongoose.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void mg_bind_to_uri(IntPtr ctx, IntPtr uri_regex, mg_callback_t func, IntPtr user_data);
        // Write to the page output stream
        [DllImport("mongoose.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int mg_write(IntPtr conn, [MarshalAs(UnmanagedType.LPStr)]string fmt, int len);

        #endregion
    }
}
