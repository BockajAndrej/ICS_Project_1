using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace ICS_Project.App.Messages
{
    public class PlaylistSelectedMessage : ValueChangedMessage<Guid>
    {
        public PlaylistSelectedMessage(Guid playlistId) : base(playlistId) { }
    }
}