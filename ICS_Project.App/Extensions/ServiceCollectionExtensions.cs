// In Extensions/ServiceCollectionExtensions.cs
using CommunityToolkit.Mvvm.ComponentModel;
using ICS_Project.App.ViewModels;
using ICS_Project.App.Views;
using ICS_Project.App.Views.Playlist; // For ObservableObject
using Microsoft.Extensions.DependencyInjection;
using ServiceScan.SourceGenerator; // Namespace for the attribute

namespace ICS_Project.App.Extensions // Use your project's namespace
{
    public static partial class ServiceCollectionExtensions
    {
        // --- Register Views ---
        // Adjust AssignableTo if you have a different common base class for Views (e.g., ViewBase)
        // If your views are ContentPages, use typeof(ContentPage) instead of typeof(ContentView)
        [GenerateServiceRegistrations(AssignableTo = typeof(ContentPageBase), AsSelf = true, Lifetime = ServiceLifetime.Transient)]
        public static partial IServiceCollection AddAppViews(this IServiceCollection services);

        // --- Register ViewModels ---
        // Adjust AssignableTo if you have a different common base class (e.g., ViewModelBase)
        [GenerateServiceRegistrations(AssignableTo = typeof(ViewModelBase), AsSelf = true, Lifetime = ServiceLifetime.Transient)]
        public static partial IServiceCollection AddAppViewModels(this IServiceCollection services);
    }
}