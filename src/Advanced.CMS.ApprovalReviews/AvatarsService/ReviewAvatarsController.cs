using System.Drawing.Imaging;
using System.Runtime.Versioning;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ApprovalReviews.AvatarsService;

/// <summary>
/// Handler used to get user avatar based on username
/// </summary>
[SupportedOSPlatform("windows")]
internal class ReviewAvatarsController(ICustomAvatarResolver customAvatarResolver) : Controller
{
    private readonly IdenticonGenerator _identiconGenerator = new();

    //TODO: try to use EPiServer.Web.MediaHandlerBase to turn on caching
    [HttpGet]
    public IActionResult Index(string id)
    {
        var userName = id;
        if (string.IsNullOrWhiteSpace(userName))
        {
            return new NotFoundResult();
        }

        userName = HttpUtility.UrlDecode(userName);
        using (var memoryStream = new MemoryStream())
        {
            var customAvatar = customAvatarResolver.GetImage(userName);
            if (customAvatar != null)
            {
                memoryStream.Write(customAvatar, 0, customAvatar.Length);
            }
            else
            {
                var identicon = _identiconGenerator.CreateIdenticon(userName, 100);
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams = new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, 90L) } };
                identicon.Save(memoryStream, encoder, encParams);
            }

            var bytesInStream = memoryStream.ToArray();

            return File(bytesInStream, "Image/jpeg");
        }
    }

    public bool IsReusable => false;
}
