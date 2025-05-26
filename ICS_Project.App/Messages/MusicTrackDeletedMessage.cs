using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace ICS_Project.App.Messages
{
    public class MusicTrackDeletedMessage : ValueChangedMessage<Guid>
    {
        public MusicTrackDeletedMessage(Guid deletedMusicTrackId) : base(deletedMusicTrackId)
        {
        }
    }
}