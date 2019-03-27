using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using EPiServer.Web.Mvc;

namespace AdvancedApprovalReviews.AvatarsService
{
    [AuthorizeContent]
    public class ReviewAvatarsHandler: IHttpHandler
    {
        private readonly IdenticonGenerator _identiconGenerator = new IdenticonGenerator();

        //[AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(CacheProfile = "CustomerImages")]
        public void ProcessRequest(HttpContext context)
        {
            var userName = context.Request.RequestContext.RouteData.Values["userName"] as string;
            
            var identicon = _identiconGenerator.CreateIdenticon(userName, 100);
            using (var memoryStream = new MemoryStream())
            {
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, 90L) } };
                identicon.Save(memoryStream, encoder, encParams);
                var bytesInStream = memoryStream.ToArray();

                context.Response.Clear();
                context.Response.ContentType = "Image/jpeg";
                
                context.Response.BinaryWrite(bytesInStream);
                context.Response.End();
            }
        }

        public bool IsReusable => false;
    }
}
