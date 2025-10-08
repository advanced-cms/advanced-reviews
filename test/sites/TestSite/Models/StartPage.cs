namespace TestSite.Models;

[ContentType(GUID = "f1a11787-541a-4297-991f-5d277c953fae", GroupName = Global.GroupNames.Specialized)]
[AvailableContentTypes(Availability.Specific, Include = [typeof(StandardPage)], ExcludeOn = [typeof(StandardPage)])]
public class StartPage : PageData;
