using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace ICS_Project.App.Messages
{
    public class MusicTrackSelectedMessage : ValueChangedMessage<Guid>
    {
        public MusicTrackSelectedMessage(Guid MusicTrackId) : base(MusicTrackId) { }
    }
}