using EPiServer.Data;

namespace Advanced.CMS.IntegrationTests
{
    public class SwitchableDatabaseMode : IDatabaseMode
    {
        private readonly IDatabaseMode _inner;

        public SwitchableDatabaseMode(IDatabaseMode inner)
        {
            _inner = inner;
        }

        public DatabaseMode? ManualMode { get; set; }

        public DatabaseMode DatabaseMode => ManualMode ?? _inner.DatabaseMode;
    }
}
