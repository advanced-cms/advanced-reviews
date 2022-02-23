namespace Advanced.CMS.IntegrationTests
{
    public sealed class CmsDatabaseFixture : DatabaseFixture
    {
        public CmsDatabaseFixture(string databaseMdfTemplateFile, string destinationDatabaseFile)
        {
            CMS_MDF_FILE_PATH = databaseMdfTemplateFile;
            DESTINATION_MDF_PATH = destinationDatabaseFile;
            DropExistingDatabases(DESTINATION_MDF_PATH);
            CopyDatabaseFiles();
            SetFolderAccess();
            CreateDatabase(DESTINATION_MDF_PATH);
        }
        protected override string CMS_MDF_FILE_PATH { get; }
        protected override string DESTINATION_MDF_PATH { get; }
        protected override string CONNECTION_STRING_TEMPLATE { get; }

    }
}
