using EPiServer.Framework.DataAnnotations;

namespace TestSite.Models;

[ContentType]
[MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
public class ImageFile : ImageData
{
}
