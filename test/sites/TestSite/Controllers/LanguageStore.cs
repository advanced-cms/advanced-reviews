using EPiServer.Shell.Services.Rest;
using System.Linq;

namespace EPiServer.Cms.Shell.UI.Rest.Internal
{
    [RestStore("language")]
    [ValidateAntiForgeryReleaseToken]
    public class LanguageStore : RestControllerBase
    {
        //Purpose of this is to have test that verifies that conflicting controller names are handled properly
        public RestResult Get()
        {
            return Rest(new { cultureName = "CustomLanguage" });
        }
    }
}
