using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableContentRepository : DefaultContentRepository
{
    public MockableContentRepository(IContentProviderManager providerManager, DefaultContentEvents contentEventsHandler,
        IPermanentLinkMapper permanentLinkMapper, IContentTypeRepository contentTypeRepository,
        IContentVersionRepository versionRepository, ContentTypeAvailabilityService contentTypeAvailablilityService,
        IContentLoader contentLoader, ISynchronizedObjectInstanceCache cacheManager,
        IContentLanguageAccessor languageAccessor, IContentVersionResolver versionResolver,
        IStatusTransitionEvaluator statusTransitionEvaluator, RequiredAccessResolver requiredAccessResolver,
        IContentCacheHandler cacheHandler, IContentTypeBaseResolver contentTypeBaseResolver) : base(providerManager,
        contentEventsHandler, permanentLinkMapper, contentTypeRepository, versionRepository,
        contentTypeAvailablilityService, contentLoader,
        ServiceLocator.Current.GetInstance<MockableContentAccessChecker>(), cacheManager, languageAccessor,
        versionResolver,
        statusTransitionEvaluator, requiredAccessResolver, cacheHandler, contentTypeBaseResolver)
    {
    }
}
