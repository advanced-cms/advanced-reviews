using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.AdvancedReviews
 {
     public class ImageProxyController: Controller
     {
         private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
         private readonly IContentLoader _contentLoader;

         public ImageProxyController(IExternalReviewLinksRepository externalReviewLinksRepository, IContentLoader contentLoader)
         {
             _externalReviewLinksRepository = externalReviewLinksRepository;
             _contentLoader = contentLoader;
         }

         [HttpGet]
         public IActionResult Index([FromQuery] string token, [FromQuery] string contentLink)
         {
             if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(contentLink))
             {
                 return new NotFoundResult();
             }

             var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
             if (externalReviewLink.IsExpired())
             {
                 return new NotFoundResult();
             }

             if(!ContentReference.TryParse(contentLink.Trim('/'), out var contentReference))
             {
                 return new NotFoundResult();
             }

             var content = _contentLoader.Get<IContent>(contentReference);
             if (!(content is ImageData imageData))
             {
                 return new NotFoundResult();
             }

             return File(imageData.BinaryData.ReadAllBytes(), imageData.MimeType);
         }
     }
 }
