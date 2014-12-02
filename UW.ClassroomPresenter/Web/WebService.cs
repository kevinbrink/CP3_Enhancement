#if WEBSERVER
using System;

using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

using UW.ClassroomPresenter.Web.Model;
using Katana;

namespace UW.ClassroomPresenter.Web
{
    /// <summary>
    /// Class the provides the interface between Katana and the rest of Presenter
    /// </summary>
    public class WebService : IDisposable {
        #region Static Members

        /// <summary>
        /// Single global static representation of the model
        /// </summary>
        public static SimpleWebModel GlobalModel = new SimpleWebModel();
        public static WebService t;

        /// <summary>
        /// The directory to use as the root for the website
        /// </summary>
        public static string WebRoot = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UW CSE\\Classroom Presenter 3\\Website\\");

        #endregion

        public event SSEventHandler SubmissionReceived;
        public delegate void SSEventHandler(object source, int deck, int slide, ArrayList strokes);

        public event QPEventHandler QuickPollReceived;
        public delegate void QPEventHandler(object source, Guid ownerId, string val);


        #region Private Members

        /// <summary>
        /// The Katana web server
        /// </summary>
        protected WebServer KatanaServer;
        /// <summary>
        /// We want a thread to listen for handling messages because we don't want to block
        /// the rest of the application as we listen for submissions from the students
        /// We should raise an event when there is a student response...
        /// </summary>
        protected System.Threading.Thread RecvThread;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor for this web service
        /// </summary>
        /// <param name="root">The root where we want the web pages to be served from</param>
        /// <param name="port">The port where we want clients to access Presenter from</param>
        public WebService(string root, string port) {
            // TODO CMPRINCE: Catch exceptions here and handle the ones we can...could be issues with access
            // Get the source web directory
            string webDir = System.IO.Path.Combine( System.Windows.Forms.Application.StartupPath, "Web\\Website\\" );
            DirectoryInfo sourceDir = new DirectoryInfo( webDir );

            // Get the target web directory
            if (root != null) {
                WebService.WebRoot = root;
            }
            string localWebDir = WebService.WebRoot;
            DirectoryInfo targetDir = new DirectoryInfo(localWebDir);

            // Copy over the local files if they are newer
            CopyAllIfNewer(sourceDir, targetDir);

            // Create the Katana server
            this.KatanaServer = new WebServer(WebService.WebRoot, port);

            // Setup and start the receive thread
            this.RecvThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(recv));
            this.RecvThread.Start(this.KatanaServer);
            t = this;
        }

        /// <summary>
        /// Recursively copy all files and directories if and only if the file in the source are newer
        /// </summary>
        /// <param name="source">The source directory to copy</param>
        /// <param name="target">The target to copy them to</param>
        private static void CopyAllIfNewer(DirectoryInfo source, DirectoryInfo target)
        {
            // Create the target if it doesn't exist
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory
            foreach (FileInfo file in source.GetFiles())
            {
                // Get the destination file name
                string destinationFilename = Path.Combine( target.ToString(), file.Name );
                
                // Get the destination file access time
                DateTime targetFiletime = (File.Exists(destinationFilename)) ? File.GetLastWriteTime(destinationFilename) : DateTime.MinValue;
                
                // Overwrite the target file if it is older than the new file
                if( targetFiletime < file.LastWriteTime )
                {
                    file.CopyTo(destinationFilename, true);
                }
            }

            // Recursively copy the subdirectories
            foreach (DirectoryInfo nextSource in source.GetDirectories())
            {
                DirectoryInfo nextTarget = target.CreateSubdirectory(nextSource.Name); 
                CopyAllIfNewer( nextSource, nextTarget );
            }
        }

        /// <summary>
        /// The static receive thread
        /// </summary>
        /// <param name="serv">The parameter to the thread</param>
        public static void recv( object serv ) {
            // Get the server
            WebServer server = (WebServer)serv;
            // Loop ad-infinitum waiting for messages from a client
            while( true ) {
                // Check if there is a packet
                if( server.HasPackets() ) {
                    // Handle the packet
                    string data = server.DequeuePacket();

                    //Build a student submission from the packet and save...
//                    System.Windows.Forms.MessageBox.Show( "received data: " + data );
                    string[] items = data.Split(new char[] { ',' });

                    if (items[0] == "0")
                    {
                        ArrayList strokes = new ArrayList();
                        int numStrokes = Convert.ToInt32(items[0]);
                        int deck = Convert.ToInt32(items[1]);
                        int slide = Convert.ToInt32(items[2]);
                        int index = 3;
                        // Get each stroke
                        for (int i = 0; i < numStrokes; i++)
                        {
                            int numPoints = Convert.ToInt32(items[index++]);
                            System.Drawing.Point[] pts = new System.Drawing.Point[numPoints];

                            // Get each point
                            for (int j = 0; j < numPoints; j++)
                            {
                                // Add the point
                                pts[j].X = (int)(Convert.ToInt32(items[index++]) * 26.37);
                                pts[j].Y = (int)(Convert.ToInt32(items[index++]) * 26.37);
                            }

                            // Add the stroke
                            strokes.Add(pts);
                        }

                        // Raise the event
                        WebService.t.SubmissionReceived(server, deck, slide, strokes);
                    }
                    else if (items[0] == "1")
                    {
                        WebService.t.QuickPollReceived(server, new Guid( items[1] ), items[2]);
                    }
                }
                // Sleep otherwise
                System.Threading.Thread.Sleep( 300 );
            }
        }

        public void SendModel() {
            string msg = WebService.BuildModelString(WebService.GlobalModel);
            this.KatanaServer.BroadcastPacket(msg);
        }

        #endregion

        #region JSON Building

        /// <summary>
        /// Build a JSON string that represents the current model
        /// </summary>
        /// <param name="model">The model to build the HTML string representation of</param>
        /// <returns>The string that is built</returns>
        public static string BuildModelString(SimpleWebModel model) {
            StringBuilder result = new StringBuilder();
            result.Append("{");
                result.Append("\"name\":\"S0\",");
                result.Append("\"dat\":{");
                    result.Append("\"pName\":\"" + model.Name + "\",");
                    result.Append("\"iDeck\":" + model.CurrentDeck + ",");
                    result.Append("\"iSlide\":" + (model.CurrentSlide-1) + ",");
                    result.Append("\"iLinked\":" + ((model.ForceLink) ? "true" : "false") + ",");
                    result.Append("\"iAllowSS\":" + ((model.AcceptingSS) ? "true" : "false") + ",");
                    result.Append("\"iAllowQP\":" + ((model.AcceptingQP) ? "true" : "false") + ",");
                    result.Append("\"iQPStyle\":" + model.PollStyle + ",");
                    result.Append("\"decks\":[");
                    for( int i = 0; i < model.Decks.Count; i++ ) {
                        result.Append( BuildDeckString( i, (SimpleWebDeck)model.Decks[i]) );
                        if (i != model.Decks.Count - 1) {
                            result.Append(",");
                        }
                    }
                    result.Append("]");
                result.Append("}");
            result.Append("}");

            return result.ToString();
        }

        // Build a representation of a deck in JSON
        protected static string BuildDeckString(int index, SimpleWebDeck deck) {
            StringBuilder result = new StringBuilder();
            result.Append("{");
                result.Append("\"i\":" + index + ",");
                result.Append("\"n\":\"" + deck.Name + "\",");
                result.Append("\"s\":[");
                for (int i = 0; i < deck.Slides.Count; i++) {
                    result.Append( BuildSlideString(deck.Name, i, (SimpleWebSlide)deck.Slides[i]));
                    if (i != deck.Slides.Count - 1)
                    {
                        result.Append(",");
                    }
                }
                result.Append("]");
            result.Append("}");
            return result.ToString();
        }

        // Build a representation of the slide in JSON
        protected static string BuildSlideString(string deckName, int index, SimpleWebSlide slide) {
            StringBuilder result = new StringBuilder();
            result.Append("{");
                result.Append("\"i\":" + index + ",");
                result.Append("\"n\":\"" + slide.Name + "\",");
                result.Append("\"u\":\"" + "./images/decks/" + deckName + "/" + deckName + "/" + deckName + "_" + String.Format("{0:000}", index + 1) + ".png\",");
                result.Append("\"ink\":[");
                for( int i=0; i<slide.Inks.Count; i++ ) {
                    result.Append( BuildInkString(i, (System.Drawing.Point[])slide.Inks[i]) + ((i == slide.Inks.Count - 1) ? "" : ",") );
                }
                result.Append("]");
            result.Append("}");
            return result.ToString();
        }

        // Build a representation of the ink in JSON
        protected static string BuildInkString(int index, System.Drawing.Point[] pts)
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            for( int i=0; i<pts.Length; i++ ) {
                result.Append( pts[i].X + "," );
                result.Append( pts[i].Y + ((i==pts.Length-1) ? "" : ",") );
            }
            result.Append("]");
            return result.ToString();
        }

        #endregion

        #region HTML Building
/*
        /// <summary>
        /// Build a string that represents the current model
        /// </summary>
        /// <param name="model">The model to build the HTML string representation of</param>
        /// <returns>The string that is built</returns>
        public static string BuildModelString(SimpleWebModel model)
        {
            string result = "";
            result += "<html>\n";
            result += "<body onload=\"parent.Presenter.Network.RecvFunc()\">\n";

            // Create the main div
            result += "<div id=\"S0\">\n";

            result += "\t<div id=\"S0_PName\">" + model.Name + "</div>\n";
            result += "\t<div id=\"S0_IDeck\">" + model.CurrentDeck + "</div>\n";
            result += "\t<div id=\"S0_ISlide\">" + (model.CurrentSlide - 1) + "</div>\n";
            result += "\t<div id=\"S0_Linked\">" + false + "</div>\n";
            result += "\t<div id=\"S0_AllowSS\">" + true + "</div>\n";
            result += "\t<div id=\"S0_Decks\">\n";
            for (int i = 0; i < model.Decks.Count; i++)
            {
                result += BuildDeckString("S0_Decks", i, (SimpleWebDeck)model.Decks[i]);
            }
            result += "\t</div>\n";

            result += "</div>\n";

            result += "</body>\n";
            result += "</html>\n";

            return result;
        }

        protected static string BuildDeckString(string prefix, int index, SimpleWebDeck deck)
        {
            string newPrefix = prefix + "_" + index;
            string result = "";
            result += "\t\t<div id=\"" + newPrefix + "\">\n";
            result += "\t\t\t<div id=\"" + newPrefix + "_Index\">" + index + "</div>\n";
            result += "\t\t\t<div id=\"" + newPrefix + "_Name\">" + deck.Name + "</div>\n";
            result += "\t\t\t<div id=\"" + newPrefix + "_Slides\">\n";
            for (int i = 0; i < deck.Slides.Count; i++)
            {
                result += BuildSlideString(newPrefix, deck.Name, i, (SimpleWebSlide)deck.Slides[i]);
            }
            result += "\t\t\t</div>\n";
            result += "\t\t</div>\n";

            return result;
        }

        protected static string BuildSlideString(string prefix, string deckName, int index, SimpleWebSlide slide)
        {
            string newPrefix = prefix + "_" + index;
            string result = "";
            result += "\t\t\t\t<div id=\"" + newPrefix + "\">\n";
            result += "\t\t\t\t\t<div id=\"" + newPrefix + "_Index\">" + index + "</div>\n";
            result += "\t\t\t\t\t<div id=\"" + newPrefix + "_Name\">" + slide.Name + "</div>\n";
            result += "\t\t\t\t\t<div id=\"" + newPrefix + "_Image\">" + "./images/" + deckName + "/" + deckName + "/" + deckName + "_" + String.Format("{0:000}", index + 1) + ".png" + "</div>\n";
            result += "\t\t\t\t</div>\n";
            return result;
        }

        public string BuildModelDiffString(SimpleWebModel oldModel, SimpleWebModel newModel)
        {
            // Go recursive, only add if there is a change within
            return "";
        }
*/
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
        ~WebService() {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// The bulk of the clean-up code is implemented here
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            // Stop the thread
            this.RecvThread.Abort();

            if (disposing)
            {
                // Free mananged resources (if any)
                this.KatanaServer.Dispose();
            }
        }

        #endregion
    }
}
#endif