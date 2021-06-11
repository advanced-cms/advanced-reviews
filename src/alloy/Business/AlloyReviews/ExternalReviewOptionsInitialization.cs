using AdvancedApprovalReviews;
using AdvancedExternalReviews;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AlloyTemplates.Business.AlloyReviews
{
    /// <summary>
    /// Module for customizing external reviews
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class ExternalReviewInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Configure<ExternalReviewOptions>(options =>
            {
                options.EditableLinksEnabled = true;
                options.PinCodeSecurity.Enabled = false;
                options.PinCodeSecurity.Required = false;
                options.PinCodeSecurity.CodeLength = 5;
                options.ProlongDays = 10;
            });

            context.Services.Configure<ApprovalOptions>(options =>
            {
                options.Notifications.NotificationsEnabled = true;
            });
        }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}
