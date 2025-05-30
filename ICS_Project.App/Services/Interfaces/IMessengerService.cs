using CommunityToolkit.Mvvm.Messaging;

namespace ICS_Project.App.Services.Interfaces
{
    public interface IMessengerService
    {
        IMessenger Messenger { get; }

        void Send<TMessage>(TMessage message)
            where TMessage : class;
    }
}