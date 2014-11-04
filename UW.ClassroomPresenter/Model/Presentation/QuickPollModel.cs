using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using UW.ClassroomPresenter.Model.Network;

namespace UW.ClassroomPresenter.Model.Presentation {
    /// <summary>
    /// Class representing a result for a quickpoll
    /// </summary>
    [Serializable]
    public class QuickPollResultModel : PropertyPublisher {
        #region Private Members

        /// <summary>
        /// The owner of this result
        /// </summary>
        private readonly Guid m_OwnerId;

        /// <summary>
        /// The value of this result
        /// </summary>
        private string m_ResultString;

        #endregion

        #region Public Members

        /// <summary>
        /// Get or Set the result for this model
        /// </summary>
        [Published]
        public string ResultString {
            get { return this.GetPublishedProperty( "ResultString", ref this.m_ResultString ); }
            set { this.SetPublishedProperty( "ResultString", ref this.m_ResultString, value ); }
        }

        /// <summary>
        /// Get the participant who owns this result model
        /// </summary>
        public Guid OwnerId {
            get { return this.m_OwnerId; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">The owner of this result</param>
        public QuickPollResultModel( Guid owner ) {
            this.m_OwnerId = owner;
            this.m_ResultString = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">The owner of this result</param>
        /// <param name="result">The result string</param>
        public QuickPollResultModel( Guid owner, string result ) {
            this.m_OwnerId = owner;
            this.m_ResultString = result;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Equal iff the owner are the same
        /// </summary>
        /// <param name="obj">The result model to compare to</param>
        /// <returns>True if the owners are equal, false otherwise</returns>
        public override bool Equals( object obj ) {
            if( obj is QuickPollResultModel ) {
                return ((QuickPollResultModel)obj).m_OwnerId.Equals( this.m_OwnerId );
            }
            return false;
        }

        /// <summary>
        /// Return a hashcode based on the participant only
        /// </summary>
        /// <returns>The hashcode for this result</returns>
        public override int GetHashCode() {
            return this.m_OwnerId.GetHashCode();
        }

        #endregion
    }

    [Serializable]
    public class QuickPollModel : PropertyPublisher {

        #region Enumerations

        // Fixed Set of Quick Poll Styles
        public enum QuickPollStyle {
            Custom = 0,
            YesNo = 1,
            YesNoBoth = 2,
            YesNoNeither = 3,
            ABC = 4,
            ABCD = 5,
            ABCDE = 6,
            ABCDEF = 7
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The unique ID representing this QuickPoll
        /// </summary>
        private readonly Guid m_Id;
        /// <summary>
        /// The original slide that is associated with this quickpoll
        /// </summary>
        private readonly Guid m_OriginalSlideId;
        /// <summary>
        /// The style of this quickpoll
        /// </summary>
        private readonly QuickPollStyle m_QuickPollStyle;
        /// <summary>
        /// Hashtable of ParticipantId to vote string
        /// </summary>
        private QuickPollResultCollection m_QuickPollResults;


        private List<String> m_instructorQA;
        private bool m_queryStudent;
        private bool m_Changed;

        /// <summary>
        /// Holds a list of choices for the QuickPoll
        /// </summary>
        private string[] m_Choices;

        #endregion

        #region Public Members

        /// <summary>
        /// Public accessor to the Id
        /// </summary>
        public Guid Id {
            get { return this.m_Id; }
        }

        /// <summary>
        /// Accessor for the original slide id that the quickpoll was created from
        /// </summary>
        public Guid OriginalSlideId {
            get { return this.m_OriginalSlideId; }
        }

        public List<String> instructorQA{
            get{return this.m_instructorQA;}
            set{ this.instructorQA=value;}
        }

        public bool queryStudent
        {
            get { return this.m_queryStudent;}
            set { this.m_queryStudent = value; }
        }

        /// <summary>
        /// Accessor to get the style of the quickpoll
        /// </summary>
        public QuickPollStyle PollStyle {
            get { return this.m_QuickPollStyle; }
        }

        /// <summary>
        /// Accessor to the collection of results that are part of this quickpoll
        /// </summary>
        [Published]
        public QuickPollResultCollection QuickPollResults {
            get { return this.GetPublishedProperty( "QuickPollResults", ref this.m_QuickPollResults ); }
        }

        [Published]
        public bool Updated {
            get { return this.GetPublishedProperty( "Updated", ref this.m_Changed ); }
            set { this.SetPublishedProperty( "Updated", ref this.m_Changed, value ); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public QuickPollModel( Guid id, QuickPollModel m) {
            using( Synchronizer.Lock( m.SyncRoot ) ) {
                this.m_Id = id;
                this.m_OriginalSlideId = m.OriginalSlideId;
                this.m_QuickPollStyle = m.PollStyle;
                this.m_QuickPollResults = new QuickPollResultCollection( this, "QuickPollResults" );
                this.m_Changed = false;
                this.m_Choices = (string[])m.m_Choices.Clone();
                this.m_instructorQA=m.instructorQA;
                this.m_queryStudent = m.queryStudent;
                // Update the results
                foreach( QuickPollResultModel res in m.QuickPollResults ) {
                    this.AddResult( res );
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="slideId"></param>
        /// <param name="style"></param>
        public QuickPollModel( Guid id, Guid slideId, QuickPollStyle style, List<String> instructorQA ) {
            this.m_Id = id;
            this.m_OriginalSlideId = slideId;
            this.m_QuickPollStyle = style;
            this.m_QuickPollResults = new QuickPollResultCollection( this, "QuickPollResults");
            this.m_Changed = false;
            this.m_Choices = new string[0];
            this.m_instructorQA=instructorQA;
      
        }

        #endregion

        public void AddResult( QuickPollResultModel result ) {
            using( Synchronizer.Lock( this.SyncRoot ) ) {
                if( !this.QuickPollResults.Contains( result ) ) {
                    // Add the value
                    this.QuickPollResults.Add( result );
                } else {
                    // Update the value only
                    QuickPollResultModel res = this.QuickPollResults[this.QuickPollResults.IndexOf( result )];
                    using( Synchronizer.Lock( res.SyncRoot ) ) {
                        using( Synchronizer.Lock( result.SyncRoot ) ) {
                            res.ResultString = result.ResultString;
                        }
                    }
                }
                this.Updated = !this.Updated;
            } 
        }

        #region QuickPollResultCollection

        [Serializable]
        public class QuickPollResultCollection : PropertyCollectionBase {
            internal QuickPollResultCollection( PropertyPublisher owner, string property )
                : base( owner, property ) {
            }

            public QuickPollResultModel this[int index] {
                get { return ((QuickPollResultModel)List[index]); }
                set { List[index] = value; }
            }

            public int Add( QuickPollResultModel value ) {
                return List.Add( value );
            }

            public int IndexOf( QuickPollResultModel value ) {
                return List.IndexOf( value );
            }

            public void Insert( int index, QuickPollResultModel value ) {
                List.Insert( index, value );
            }

            public void Remove( QuickPollResultModel value ) {
                List.Remove( value );
            }

            public bool Contains( QuickPollResultModel value ) {
                return List.Contains( value );
            }

            protected override void OnValidate( Object value ) {
                if( !typeof( QuickPollResultModel ).IsInstanceOfType( value ) )
                    throw new ArgumentException( "Value must be of type DeckTraversalModel.", "value" );
            }
        }

        #endregion

        #region Results

        /// <summary>
        /// Gets the vote count
        /// </summary>
        /// <returns></returns>
        public Hashtable GetVoteCount() {
            // Get the possible strings
            ArrayList strings = QuickPollModel.GetVoteStringsFromStyle( this.m_QuickPollStyle );
            Hashtable counts = new Hashtable();
            foreach( string s in strings ) {
                counts.Add( s, 0 );
            }

            // Count up the votes
            foreach( QuickPollResultModel m in this.m_QuickPollResults ) {
                using( Synchronizer.Lock( m.SyncRoot ) ) {
                    System.Diagnostics.Debug.Assert( counts.ContainsKey( m.ResultString ) );
                    counts[m.ResultString] = ((int)counts[m.ResultString]) + 1;
                }
            }

            return counts;
        }

        public static string GetLocalizedQuickPollString( string value ) {
            switch( value ) {
                case "Yes":
                    return Strings.QuickPollYes;
                case "No":
                    return Strings.QuickPollNo;
                case "Both":
                    return Strings.QuickPollBoth;
                case "Neither":
                    return Strings.QuickPollNeither;
                default:
                    return value;
            }
        }

        public static ArrayList GetVoteStringsFromStyle( QuickPollStyle style ) {
            ArrayList strings = new ArrayList();
            switch( style )
            {
                case QuickPollStyle.YesNo:
                    strings.Add( "Yes" );
                    strings.Add( "No" );
                    break;
                case QuickPollStyle.YesNoBoth:
                    strings.Add( "Yes" );
                    strings.Add( "No" );
                    strings.Add( "Both" );
                    break;
                case QuickPollStyle.YesNoNeither:
                    strings.Add( "Yes" );
                    strings.Add( "No" );
                    strings.Add( "Neither" );
                    break;
                case QuickPollStyle.ABC:
                    strings.Add( "A" );
                    strings.Add( "B" );
                    strings.Add( "C" );
                    break;
                case QuickPollStyle.ABCD:
                    strings.Add( "A" );
                    strings.Add( "B" );
                    strings.Add( "C" );
                    strings.Add( "D" );
                    break;
                case QuickPollStyle.ABCDE:
                    strings.Add( "A" );
                    strings.Add( "B" );
                    strings.Add( "C" );
                    strings.Add( "D" );
                    strings.Add( "E" );
                    break;
                case QuickPollStyle.ABCDEF:
                    strings.Add( "A" );
                    strings.Add( "B" );
                    strings.Add( "C" );
                    strings.Add( "D" );
                    strings.Add( "E" );
                    strings.Add( "F" );
                    break;
                case QuickPollStyle.Custom:
                    // Do Nothing for now
                    break;
            }

            return strings;
        }

        #endregion
    }
}
