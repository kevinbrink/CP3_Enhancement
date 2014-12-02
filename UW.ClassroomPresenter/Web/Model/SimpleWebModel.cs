#if WEBSERVER
using System;
using System.Collections;
using System.Text;

namespace UW.ClassroomPresenter.Web.Model {
    /// <summary>
    /// A simple model of a presentation for use with the Web Server
    /// </summary>
    public class SimpleWebModel : ICloneable {
        #region Members

        public ArrayList Decks = new ArrayList();
        public int CurrentDeck = -1;
        public int CurrentSlide = -1;
        public string Name = "Untitled Presentation";
        public bool AcceptingSS = true;
        public bool AcceptingQP = false;
        public bool ForceLink = true;
        public int PollStyle = 5;
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clone this model
        /// </summary>
        /// <returns>The cloned model</returns>
        public object Clone() {
            SimpleWebModel model = new SimpleWebModel();
            model.Decks = (ArrayList)this.Decks.Clone();
            model.CurrentDeck = this.CurrentDeck;
            model.CurrentSlide = this.CurrentSlide;
            model.Name = (string)this.Name.Clone();
            model.AcceptingSS = this.AcceptingSS;
            model.AcceptingQP = this.AcceptingQP;
            model.ForceLink = this.ForceLink;
            model.PollStyle = this.PollStyle;
            return model;
        }

        #endregion
    }

    /// <summary>
    /// A simple model for keeping track of a deck
    /// </summary>
    public class SimpleWebDeck : ICloneable {
        public ArrayList Slides = new ArrayList();
        public string Name = "Untitled Deck";

        #region ICloneable Members

        /// <summary>
        /// Clone this model
        /// </summary>
        /// <returns>The cloned model</returns>
        public object Clone() {
            SimpleWebDeck deck = new SimpleWebDeck();
            deck.Slides = (ArrayList)this.Slides.Clone();
            deck.Name = (string)this.Name.Clone();
            return deck;
        }

        #endregion
    }

    /// <summary>
    /// A simple model for keeping track of a deck
    /// </summary>
    public class SimpleWebSlide : ICloneable {
        public ArrayList Inks = new ArrayList();
        public Guid Id = Guid.Empty;
        public int Index = -1;
        public string Name = "Untitled Slide";

        #region ICloneable Members

        /// <summary>
        /// Clone this model
        /// </summary>
        /// <returns>The cloned model</returns>
        public object Clone() {
            SimpleWebSlide slide = new SimpleWebSlide();
            slide.Inks = (ArrayList)this.Inks.Clone();
            slide.Name = (string)this.Name.Clone();
            return slide;
        }

        #endregion
    }

    /// <summary>
    /// A simple model for keeping track of ink
    /// </summary>
    public class SimpleWebInk : ICloneable {
        public ArrayList Strokes = new ArrayList();

        #region ICloneable Members

        /// <summary>
        /// Clone this model
        /// </summary>
        /// <returns>The cloned model</returns>
        public object Clone() {
            SimpleWebInk ink = new SimpleWebInk();
            ink.Strokes = (ArrayList)this.Strokes.Clone();
            return ink;
        }

        #endregion
    }
}
#endif