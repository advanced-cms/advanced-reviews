using Advanced.CMS.ApprovalReviews.AvatarsService;
using Advanced.CMS.ApprovalReviews.Notifications;
using EPiServer.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.ApprovalReviews;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApprovalReviews(this IServiceCollection services)
    {
        services.AddTransient<ISiteUriResolver, SiteUriResolver>();
        services.AddTransient<IStartPageUrlResolver, StartPageUrlResolver>();
        services.AddTransient<IUserNotificationFormatter, ReviewContentNotificationFormatter>();
        services.AddTransient<ReviewsNotifier>();
        services.AddTransient<ReviewLocationParser>();
        services.AddTransient<ReviewUrlGenerator>();
        services.AddSingleton<IApprovalReviewsRepository, DdsApprovalReviewsRepository>();
        services.AddSingleton<ICustomAvatarResolver, NullCustomAvatarResolver>();

        return services;
    }
}
