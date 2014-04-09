using System;
using System.Collections.Generic;
using System.Net;

using UW.ClassroomPresenter.Model.Network;

using UW.ClassroomPresenter.Network.Messages;
using UW.ClassroomPresenter.Network.Messages.Network;
using UW.ClassroomPresenter.Network.Messages.Presentation;

namespace UW.ClassroomPresenter.Network.TCP {
    [Serializable]
    public class TCPHandshakeMessage {
        public readonly Guid ParticipantId;
        public readonly IPEndPoint EndPoint;
        public readonly String HumanName;
        /// <summary>
        /// This is used in reconnect scenarios:  The client should set the last received message (and chunk)
        /// sequence numbers.  This will permit the server to resume sending where it left off.
        /// </summary>
        public ulong LastMessageSequence;
        /// <summary>
        /// This is used in reconnect scenarios:  The client should set the last received chunk (and message)
        /// sequence numbers.  This will permit the server to resume sending where it left off.
        /// </summary>
        public ulong LastChunkSequence;

        public TCPHandshakeMessage(ParticipantModel participant, IPEndPoint ep) {
            this.ParticipantId = participant.Guid;
            this.EndPoint = ep;
            using (Synchronizer.Lock(participant.SyncRoot)) {
                this.HumanName = participant.HumanName;
            }
            LastMessageSequence = LastChunkSequence = 0;
        }
    }
}
