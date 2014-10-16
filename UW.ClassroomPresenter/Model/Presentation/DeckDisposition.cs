// $Id: DeckDisposition.cs 1554 2008-02-29 18:55:05Z cmprince $

using System;

namespace UW.ClassroomPresenter.Model.Presentation {
    [Flags]
    public enum DeckDisposition : long {
        Empty = 0,
        Remote = 1,
        Whiteboard = 2,
        StudentSubmission = 4,
        QuickPoll = 8,
    }
}
