using AdvancedExternalReviews;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AlloyTemplates.Business.AlloyReviews
{
    /// <summary>
    /// Module for customizing external reviews
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ExternalReviewInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // Editable external review links are turned off by default
            var options = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            options.EditableLinksEnabled = true;
            options.PinCodeSecurity.Enabled = true;
            options.PinCodeSecurity.CodeLength = 5;
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
