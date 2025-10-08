using Microsoft.AspNetCore.Mvc.Razor;

namespace TestSite;

public class SiteViewEngineLocationExpander : IViewLocationExpander
{
    public const string BlockFolder = "~/Views/Shared/Blocks/";
    public const string PagePartialsFolder = "~/Views/Shared/PagePartials/";

    private static readonly string[] AdditionalPartialViewFormats = new[]
    {
        BlockFolder + "{0}.cshtml",
        PagePartialsFolder + "{0}.cshtml"
    };

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        foreach (var location in viewLocations)
        {
            yield return location;
        }

        for (int i = 0; i < AdditionalPartialViewFormats.Length; i++)
        {
            yield return AdditionalPartialViewFormats[i];
        }
    }
    public void PopulateValues(ViewLocationExpanderContext context) { }
}
