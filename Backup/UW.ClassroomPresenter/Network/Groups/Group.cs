using System;
using System.Collections.Generic;
using System.Text;

namespace UW.ClassroomPresenter.Network.Groups {
    /// <summary>
    /// A token representing a group of participants (usually with a specific set of 
    /// capabilities) that you want to communicate with. This allows us to specifically send
    /// network messages to a group of participants. Listeners should filter out messages 
    /// not meant for themselves and not re-request these messages.
    /// </summary>
    [Serializable]
    public class Group {
        #region Static Members

        /// <summary>
        /// Represents the group of all participants, everyone should always be a part of this
        /// group
        /// Value: {9AD19089-6A58-41c7-ACD8-5096A7C2ACDF}
        /// </summary>
        public static readonly Guid AllParticipantGuid = new Guid( "{9AD19089-6A58-41c7-ACD8-5096A7C2ACDF}" );
        public static readonly Group AllParticipant = new Group( Group.AllParticipantGuid, "All Participants" );

        /// <summary>
        /// Represents the group of all instructor participants
        /// Value: {EC16EB3F-320C-4f25-A263-8029B9905358}
        /// </summary>
        public static readonly Guid AllInstructorGuid = new Guid( "{EC16EB3F-320C-4f25-A263-8029B9905358}" );
        public static readonly Group AllInstructor = new Group( Group.AllInstructorGuid, "All Instructors" );

        /// <summary>
        /// Represents the group of all public display participants
        /// Value: {CA68CCA8-1854-4ba5-8D7B-1F815910E620}
        /// </summary>
        public static readonly Guid AllPublicGuid = new Guid( "{CA68CCA8-1854-4ba5-8D7B-1F815910E620}" );
        public static readonly Group AllPublic = new Group( Group.AllPublicGuid, "All Public Displays" );

        /// <summary>
        /// Represents the group of all student participants
        /// Value: {4725E70C-7BB2-481f-B9B6-AA1C1A723302}
        /// </summary>
        public static readonly Guid AllStudentGuid = new Guid( "{4725E70C-7BB2-481f-B9B6-AA1C1A723302}" );
        public static readonly Group AllStudent = new Group( Group.AllStudentGuid, "All Students" );

        /// <summary>
        /// Represents the group of all participants the receive student submissions.
        /// The instructors and public displays should both be part of this group.
        /// Value: {740B2A17-F1FE-49ee-8E46-BB0B3A4D5D02}
        /// </summary>
        public static readonly Guid SubmissionsGuid = new Guid( "{740B2A17-F1FE-49ee-8E46-BB0B3A4D5D02}" );
        public static readonly Group Submissions = new Group( Group.SubmissionsGuid, "All Receivers of Student Submissions" );

        #endregion

        #region Public Members

        /// <summary>
        /// The guid presenting this group
        /// </summary>
        private Guid m_id;
        public Guid ID {
            get { return m_id; }
        }

        /// <summary>
        /// A friendly name for this 
        /// </summary>
        private string m_friendlyName;
        public String FriendlyName {
            get { return m_friendlyName; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The globally unique id of the group</param>
        /// <param name="name">The friendly name to give this group</param>
        public Group( Guid id, string name ) {
            this.m_id = id;
            this.m_friendlyName = name;
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Determines if two different groups are equal
        /// </summary>
        /// <param name="obj">The group object to compare to</param>
        /// <returns>True if the objects are the same group, false otherwise</returns>
        public override bool Equals( object obj ) {
            if( (obj is Group) && ((Group)obj).ID == this.ID )
                return true;
            return false;
        }

        /// <summary>
        /// Get a hashcode for this group
        /// </summary>
        /// <returns>Returns the hashcode of the group ID</returns>
        public override int GetHashCode() {
            return this.ID.GetHashCode();
        }

        #endregion
    }
}