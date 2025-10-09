using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using TestSite.Models;
using TestSite.ViewModels;

namespace TestSite.Components;

public class ImageFileViewComponent(UrlResolver urlResolver) : PartialContentComponent<ImageFile>
{
    protected override IViewComponentResult InvokeComponent(ImageFile currentContent)
    {
        var model = new ImageViewModel
        {
            Url = urlResolver.GetUrl(currentContent.ContentLink),
            Name = currentContent.Name,
            Copyright = currentContent.Copyright
        };

        return View(model);
    }
}
