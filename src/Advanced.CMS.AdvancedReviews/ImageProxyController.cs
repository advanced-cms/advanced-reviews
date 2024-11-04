using System.Collections.Generic;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Framework.Blobs;
using EPiServer.ImageLibrary;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.AdvancedReviews;

internal class ImageProxyController: Controller
{
    private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
    private readonly IContentLoader _contentLoader;
    private readonly ThumbnailManager _thumbnailManager;
    private readonly IMimeTypeResolver _mimeTypeResolver;

    private string ThumbnailMimeType => _mimeTypeResolver.GetMimeMapping(ThumbnailHelper.ThumbnailExtension);

    public ImageProxyController(IExternalReviewLinksRepository externalReviewLinksRepository,
        IContentLoader contentLoader, ThumbnailManager thumbnailManager, IMimeTypeResolver mimeTypeResolver)
    {
        _externalReviewLinksRepository = externalReviewLinksRepository;
        _contentLoader = contentLoader;
        _thumbnailManager = thumbnailManager;
        _mimeTypeResolver = mimeTypeResolver;
    }

    public IActionResult Index([FromRoute] string token, [FromRoute] string contentLink, [FromQuery] int? width, [FromQuery] int? height)
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
        if (content is not ImageData imageData)
        {
            return new NotFoundResult();
        }

        var returnThumbnail = width.HasValue && height.HasValue;

        var originalBlobBytes = imageData.BinaryData.ReadAllBytes();
        var blobToReturn = returnThumbnail ? Generate(originalBlobBytes, width.Value, height.Value) : originalBlobBytes;

        return File(blobToReturn, imageData.MimeType);
    }

    private byte[] Generate(byte[] blobBytes, int width, int height)
    {
        var imgOperation = new ImageOperation(ImageEditorCommand.ResizeKeepScale, width, height)
        {
            // use transparency color
            BackgroundColor = "#00000000"
        };

        try
        {
            return _thumbnailManager.ImageService.RenderImage(blobBytes,
                new List<ImageOperation> { imgOperation }, ThumbnailMimeType, 1, 50);
        }
        catch
        {
            return null;
        }
    }
}
