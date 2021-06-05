//TODO: NETCORE:
// using System.Web;
// using AdvancedExternalReviews.ReviewLinksRepository;
// using EPiServer;
// using EPiServer.Core;
// using EPiServer.Framework.Blobs;
// using EPiServer.ServiceLocation;
// using EPiServer.Web.Mvc;
//
// namespace AdvancedExternalReviews.ImageProxy
// {
//     [AuthorizeContent]
//     public class ImageProxyHandler: IHttpHandler
//     {
//         private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
//         private readonly IContentLoader _contentLoader;
//
//         public ImageProxyHandler(IExternalReviewLinksRepository externalReviewLinksRepository, IContentLoader contentLoader)
//         {
//             _externalReviewLinksRepository = externalReviewLinksRepository;
//             _contentLoader = contentLoader;
//         }
//
//         public ImageProxyHandler() : this(ServiceLocator.Current.GetInstance<IExternalReviewLinksRepository>(),
//             ServiceLocator.Current.GetInstance<IContentLoader>())
//         {
//         }
//
//         public void ProcessRequest(HttpContext context)
//         {
//             var token = context.Request.QueryString["token"];
//
//             var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
//             if (externalReviewLink.IsExpired())
//             {
//                 Show404(context);
//                 return;
//             }
//
//             var contentLinkQuery = context.Request.QueryString["contentLink"];
//             if(!ContentReference.TryParse(contentLinkQuery.Trim('/'), out var contentLink))
//             {
//                 Show404(context);
//                 return;
//             }
//
//             var content = _contentLoader.Get<IContent>(contentLink);
//             if (!(content is ImageData imageData))
//             {
//                 Show404(context);
//                 return;
//             }
//
//             context.Response.ContentType = imageData.MimeType;
//             context.Response.BinaryWrite(imageData.BinaryData.ReadAllBytes());
//         }
//
//         public void Show404(HttpContext context)
//         {
//             context.Response.StatusCode = 404;
//             context.Response.End();
//         }
//
//         public bool IsReusable => false;
//     }
// }
