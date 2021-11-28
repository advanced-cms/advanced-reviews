using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer.Framework.Modules.Internal;

namespace AdvancedApprovalReviews.AvatarsService
{
    /// <summary>
    /// Handler used to get user avatar based on username
    /// </summary>
    public class ReviewAvatarsController: Controller
    {
        private readonly ICustomAvatarResolver _customAvatarResolver;
        private readonly IdenticonGenerator _identiconGenerator = new IdenticonGenerator();

        public ReviewAvatarsController(ICustomAvatarResolver customAvatarResolver)
        {
            _customAvatarResolver = customAvatarResolver;
        }

        public static string GetUrl()
        {
            ModuleResourceResolver.Instance.TryResolvePath(typeof(ReviewAvatarsController).Assembly,
                "ReviewAvatars/Index",
                out var avatarUrl);

            return avatarUrl;
        }

        //TODO: try to use EPiServer.Web.MediaHandlerBase to turn on caching
        [HttpGet]
        public ActionResult Index(string id)
        {
            var userName = id;
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new HttpNotFoundResult();
            }

            userName = HttpUtility.UrlDecode(userName);
            using (var memoryStream = new MemoryStream())
            {
                var customAvatar = _customAvatarResolver.GetImage(userName);
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
    }
}
