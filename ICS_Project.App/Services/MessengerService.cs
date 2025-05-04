using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Services.Interfaces; // Use your namespace

namespace ICS_Project.App.Services // Use your namespace
{
    public class MessengerService(IMessenger messenger) : IMessengerService
    {
        public IMessenger Messenger { get; } = messenger;

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            Messenger.Send(message);
        }
    }
}