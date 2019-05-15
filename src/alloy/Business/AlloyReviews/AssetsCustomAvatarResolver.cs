using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AdvancedApprovalReviews.AvatarsService;
using AlloyTemplates.Business.Initialization;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using SiteDefinition = EPiServer.Web.SiteDefinition;

namespace AlloyTemplates.Business.AlloyReviews
{
    /// <summary>
    /// Sample implementation of <see cref="ICustomAvatarResolver"/>.
    /// Use images from "User Avatar" folder (under Global Assets) as user avatar images
    /// </summary>
    [ServiceConfiguration(typeof(ICustomAvatarResolver), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AssetsCustomAvatarResolver : ICustomAvatarResolver
    {
        private readonly IContentLoader _contentLoader;

        public AssetsCustomAvatarResolver(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public byte[] GetImage(string userName)
        {
            var avatarsRoot = GetAvatarsRootFolder();
            if (avatarsRoot == null)
            {
                return null;
            }
            var avatarContent = _contentLoader.GetChildren<IContent>(avatarsRoot).OfType<IContentMedia>()
                .FirstOrDefault(x => x.Name.StartsWith(userName + "."));
            if (avatarContent == null)
            {
                return null;
            }

            using (var stream = avatarContent.BinaryData.OpenRead())
            {
                var image = Image.FromStream(stream);
                var resized = ResizeImage(image, 100, 100);
                using (var resultStream = new MemoryStream())
                {
                    resized.Save(resultStream, ImageFormat.Bmp);
                    resultStream.Position = 0;
                    return ToArray(resultStream);
                }
            }
        }

        public static byte[] ToArray(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private ContentReference GetAvatarsRootFolder()
        {
            var globalAssetsRoot = SiteDefinition.Current.GlobalAssetsRoot;
            var parentPage = _contentLoader.GetChildren<IContent>(globalAssetsRoot)
                .FirstOrDefault(x => x.Name == "User Avatar");
            return parentPage?.ContentLink;
        }
    }

    [InitializableModule]
    [ModuleDependency(typeof(DependencyResolverInitialization))]
    public class CustomAvatarsInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            //Implementations for custom interfaces can be registered here.

            context.ConfigurationComplete += (o, e) =>
            {
                //Register custom implementations that should be used in favour of the default implementations
                context.Services.AddSingleton<ICustomAvatarResolver, AssetsCustomAvatarResolver>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
