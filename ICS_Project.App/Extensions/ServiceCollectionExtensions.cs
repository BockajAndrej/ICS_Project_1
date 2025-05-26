using ICS_Project.App.ViewModels;
using ICS_Project.App.Views;
using ServiceScan.SourceGenerator;

namespace ICS_Project.App.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        // --- Register Views ---
        [GenerateServiceRegistrations(AssignableTo = typeof(ContentPageBase), AsSelf = true, Lifetime = ServiceLifetime.Transient)]
        public static partial IServiceCollection AddAppViews(this IServiceCollection services);

        // --- Register ViewModels ---
        [GenerateServiceRegistrations(AssignableTo = typeof(ViewModelBase), AsSelf = true, Lifetime = ServiceLifetime.Transient)]
        public static partial IServiceCollection AddAppViewModels(this IServiceCollection services);
    }
}