using System.ComponentModel.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace TestSite.Models;

[ContentType(GUID = "85FC1C77-0040-4D05-AF42-14C165D882C7")]
public class StandardPage : PageData
{
    public virtual bool ShowBreadcrumbs { get; set; }
    public virtual ContentArea ContentArea { get; set; }
    public virtual XhtmlString Html { get; set; }

    [UIHint(UIHint.Image)]
    public virtual ContentReference Image { get; set; }

    public const string PageNameFromGetChildrenCallNodeName = "page-name-from-get-children-call";
    public string PageNameFromGetChildrenCall =>
        ServiceLocator.Current.GetInstance<IContentRepository>()
            .GetChildren<PageData>(ContentReference.StartPage).FirstOrDefault()?.Name;
}
