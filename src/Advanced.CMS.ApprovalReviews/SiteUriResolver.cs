using System;
using EPiServer.Core;
using EPiServer.Web;

namespace Advanced.CMS.ApprovalReviews;

public interface ISiteUriResolver
{
    Uri GetUri(ContentReference contentReference);
}

internal class SiteUriResolver : ISiteUriResolver
{
    public Uri GetUri(ContentReference contentReference)
    {
        return SiteDefinition.Current.SiteUrl;
    }
}
