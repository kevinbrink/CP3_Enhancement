using System;
using System.Collections.Generic;

using UW.ClassroomPresenter.Network.Messages;
using UW.ClassroomPresenter.Network.Groups;
using UW.ClassroomPresenter.Model.Network;
using System.Diagnostics;

namespace UW.ClassroomPresenter.Network.Messages.Network {
    [Serializable]
    public abstract class GroupInformationMessage : Message {
        public readonly Guid GroupId;
        public readonly string FriendlyName;
        public readonly bool Singleton;

        public GroupInformationMessage(Group group) : base(Guid.Empty) {
            this.GroupId = group.ID;
            this.FriendlyName = group.FriendlyName;
            this.Singleton = (group is SingletonGroup);
        }
    }

    [Serializable]
    public class ParticipantGroupAddedMessage : GroupInformationMessage {
        public ParticipantGroupAddedMessage(Group group) : base(group) { }

        protected override bool UpdateTarget(ReceiveContext context) {
            // Discard bogus SingletonGroups.  A participant cannot
            // belong to a SingletonGroup that does not match his Guid.
            if (this.Singleton && this.GroupId != context.Participant.Guid)
                return false;

            Group group = this.Singleton
                ? new SingletonGroup(context.Participant)
                : new Group(this.GroupId, this.FriendlyName);

            using (Synchronizer.Lock(context.Participant.SyncRoot)) {
                if (!context.Participant.Groups.Contains(group)) {
                    context.Participant.Groups.Add(group);
                }
            }

            return false;
        }
    }

    [Serializable]
    public class ParticipantGroupRemovedMessage : GroupInformationMessage {
        public ParticipantGroupRemovedMessage(Group group) : base(group) { }

        protected override bool UpdateTarget(ReceiveContext context) {
            // For the purposes of removal, whether the group is a Group or a SingletonGroup
            // doesn't matter; all that matters is the Guid.
            Group group = new Group(this.GroupId, this.FriendlyName);

            using (Synchronizer.Lock(context.Participant.SyncRoot)) {
                if (context.Participant.Groups.Contains(group))
                    context.Participant.Groups.Remove(group);
            }

            return false;
        }
    }
}
