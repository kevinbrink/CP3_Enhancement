// $Id: InstructorModel.cs 1554 2008-02-29 18:55:05Z cmprince $

using System;

using UW.ClassroomPresenter.Model.Presentation;

namespace UW.ClassroomPresenter.Model.Network {
    [Serializable]
    public class InstructorModel : RoleModel {

        // Published properties:
        private PresentationModel m_CurrentPresentation; // PresentationModel
        private DeckTraversalModel m_CurrentDeckTraversal; // DeckTraversalModel
        private bool m_AcceptingStudentSubmissions; // bool
        private bool m_ForcingStudentNavigationLock; // bool

        // Quick Polling
        private bool m_AcceptingQuickPollSubmissions;
        private QuickPollModel m_CurrentQuickPoll; // TODO: Can get this from the PresentationModel

        public InstructorModel(Guid id) : base(id) {
            this.m_CurrentPresentation = null;
            this.m_CurrentDeckTraversal = null;
            this.m_CurrentQuickPoll = null;
            this.m_AcceptingStudentSubmissions = true;
            this.m_AcceptingQuickPollSubmissions = false;
            this.m_ForcingStudentNavigationLock = false;
        }

        [Published] public PresentationModel CurrentPresentation {
            get { return this.GetPublishedProperty("CurrentPresentation", ref this.m_CurrentPresentation);; }
            set { this.SetPublishedProperty("CurrentPresentation", ref this.m_CurrentPresentation, value); }
        }

        [Published] public DeckTraversalModel CurrentDeckTraversal {
            get { return this.GetPublishedProperty("CurrentDeckTraversal", ref this.m_CurrentDeckTraversal); }
            set { this.SetPublishedProperty("CurrentDeckTraversal", ref this.m_CurrentDeckTraversal, value); }
        }

        [Published] public QuickPollModel CurrentQuickPoll {
            get { return this.GetPublishedProperty( "CurrentQuickPoll", ref this.m_CurrentQuickPoll ); }
            set { this.SetPublishedProperty( "CurrentQuickPoll", ref this.m_CurrentQuickPoll, value ); }
        }

        [Published] public bool AcceptingStudentSubmissions {
            get { return this.GetPublishedProperty("AcceptingStudentSubmissions", ref this.m_AcceptingStudentSubmissions); }
            set { this.SetPublishedProperty("AcceptingStudentSubmissions", ref this.m_AcceptingStudentSubmissions, value); }
        }

        [Published] public bool AcceptingQuickPollSubmissions {
            get { return this.GetPublishedProperty( "AcceptingQuickPollSubmissions", ref this.m_AcceptingQuickPollSubmissions ); }
            set { this.SetPublishedProperty( "AcceptingQuickPollSubmissions", ref this.m_AcceptingQuickPollSubmissions, value ); }
        }

        [Published] public bool ForcingStudentNavigationLock {
            get { return this.GetPublishedProperty("ForcingStudentNavigationLock", ref this.m_ForcingStudentNavigationLock); }
            set { this.SetPublishedProperty("ForcingStudentNavigationLock", ref this.m_ForcingStudentNavigationLock, value); }
        }
    }
}
