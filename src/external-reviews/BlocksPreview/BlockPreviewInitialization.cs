using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace AdvancedExternalReviews.BlocksPreview
{
    /// <summary>
    /// Module for support external blocks rendering.
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class BlockPreviewInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // context.Locate.TemplateResolver()
            //     .TemplateResolved += BlocksTemplateCoordinator.OnTemplateResolved;
        }

        public void Uninitialize(InitializationEngine context)
        {
            // ServiceLocator.Current.GetInstance<TemplateResolver>()
            //     .TemplateResolved -= BlocksTemplateCoordinator.OnTemplateResolved;
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
