using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.App.Messages
{
    public class PlaylistNewPlaylistOpened : ValueChangedMessage<bool>
    {
        public PlaylistNewPlaylistOpened(bool value) : base(value) { }
    }
}
