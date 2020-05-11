using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    [ScheduledPlugIn(DisplayName = "Remove Expired Tokens Job", GUID = "ee619008-3e76-4886-b3c7-aa025a0c2603")]
    public class RemoveExpiredTokensJob : ScheduledJobBase
    {
        public override string Execute()
        {
            OnStatusChanged($"Starting execution of {GetType()}");

            var repository = ServiceLocator.Current.GetInstance<IExternalReviewLinksRepository>();
            var removedLinksCount = repository.RemoveExpiredLinks();

            return $"Removed {removedLinksCount} links";
        }
    }
}
