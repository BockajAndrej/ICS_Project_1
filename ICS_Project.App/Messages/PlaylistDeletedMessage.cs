using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace ICS_Project.App.Messages
{
    public class PlaylistDeletedMessage : ValueChangedMessage<Guid>
    {
        public PlaylistDeletedMessage(Guid deletedPlaylistId) : base(deletedPlaylistId)
        {
        }
    }
}