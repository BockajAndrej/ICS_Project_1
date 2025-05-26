namespace ICS_Project.App.Services.Interfaces
{
    public interface IPopupService
    {
        void ShowPopup(Type popupViewType, object? bindingContext = null, VisualElement? anchor = null);
    }

}
