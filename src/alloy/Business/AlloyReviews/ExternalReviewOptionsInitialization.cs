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
                options.PinCodeSecurity.Enabled = true;
                options.PinCodeSecurity.Required = true;
                options.PinCodeSecurity.CodeLength = 5;
            });
        }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}
