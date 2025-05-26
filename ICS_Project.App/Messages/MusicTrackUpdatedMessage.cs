using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace ICS_Project.App.Messages
{
    public class MusicTrackUpdatedMessage : ValueChangedMessage<Guid>
    {
        public MusicTrackUpdatedMessage(Guid UpdatedMusicTrackId) : base(UpdatedMusicTrackId)
        {
        }
    }
}