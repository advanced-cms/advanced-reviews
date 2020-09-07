using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;

namespace AdvancedApprovalReviews.AvatarsService
{
    /// <summary>
    /// Handler used to get user avatar based on username
    /// </summary>
    [AuthorizeContent]
    public class ReviewAvatarsHandler: IHttpHandler
    {
        private readonly ICustomAvatarResolver _customAvatarResolver;
        private readonly IdenticonGenerator _identiconGenerator = new IdenticonGenerator();

        public ReviewAvatarsHandler(ICustomAvatarResolver customAvatarResolver)
        {
            _customAvatarResolver = customAvatarResolver;
        }

        public ReviewAvatarsHandler(): this(ServiceLocator.Current.GetInstance<ICustomAvatarResolver>())
        {
        }

        //TODO: try to use EPiServer.Web.MediaHandlerBase to turn on caching
        public void ProcessRequest(HttpContext context)
        {
            var userName = context.Request.QueryString.Get("userName");
            if (string.IsNullOrWhiteSpace(userName))
            {
                context.Response.StatusCode = 404;
                return;
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

                context.Response.Clear();
                context.Response.ContentType = "Image/jpeg";

                context.Response.BinaryWrite(bytesInStream);

                // context.Response.End() affects the cache
                //context.Response.End();
            }
        }

        public bool IsReusable => false;
    }
}
