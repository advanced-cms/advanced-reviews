using EPiServer.Data;

namespace Advanced.CMS.IntegrationTests;

public class SwitchableDatabaseMode(IDatabaseMode inner) : IDatabaseMode
{
    public DatabaseMode? ManualMode { get; set; }

    public DatabaseMode DatabaseMode => ManualMode ?? inner.DatabaseMode;
}
