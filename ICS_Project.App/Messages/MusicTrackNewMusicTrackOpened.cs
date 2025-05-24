using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.App.Messages
{
    public class MusicTrackNewMusicTrackOpened : ValueChangedMessage<bool>
    {
        public MusicTrackNewMusicTrackOpened(bool value) : base(value) { }
    }
}
