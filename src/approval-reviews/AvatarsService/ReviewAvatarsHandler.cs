using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Http;

namespace AdvancedApprovalReviews.AvatarsService
{
    // /// <summary>
    // /// Handler used to get user avatar based on username
    // /// </summary>
    // [AuthorizeContent]
    // public class ReviewAvatarsHandler: IHttpHandler
    // {
    //     private readonly ICustomAvatarResolver _customAvatarResolver;
    //     private readonly IdenticonGenerator _identiconGenerator = new IdenticonGenerator();
    //
    //     public ReviewAvatarsHandler(ICustomAvatarResolver customAvatarResolver)
    //     {
    //         _customAvatarResolver = customAvatarResolver;
    //     }
    //
    //     public ReviewAvatarsHandler(): this(ServiceLocator.Current.GetInstance<ICustomAvatarResolver>())
    //     {
    //     }
    //
    //     //TODO: try to use EPiServer.Web.MediaHandlerBase to turn on caching
    //     public void ProcessRequest(HttpContext context)
    //     {
    //
    //     }
    //
    //     public bool IsReusable => false;
    // }
}
