using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Web;

namespace Advanced.CMS.IntegrationTests
{
    public static class DefinitionSetup
    {
        public static readonly Guid TestSiteID = new("EF3727EA-D14F-4FB3-834F-F673BC870B7F");

        public static void SetupTestEnvironment()
        {
            InitializeStaticReferences();
        }

        public static SiteDefinition CreateDefaultSiteDefinition()
        {
            var siteDef = new SiteDefinition
            {
                Name = "TestSite",
                Id = TestSiteID
            };


            var siteAssetRoot = new ContentReference(3);
            siteAssetRoot.MakeReadOnly();
            siteDef.SiteAssetsRoot = siteAssetRoot;


            var startPage = new ContentReference(7);
            startPage.MakeReadOnly();
            siteDef.StartPage = startPage;

            siteDef.SiteUrl = new Uri("http://localhost:6666/");
            siteDef.Hosts = new List<HostDefinition>
            {
                new HostDefinition() { Name = "*" }
            };
            return siteDef;
        }

        public static SystemDefinition CreateDefaultSystemDefinition()
        {
            return new SystemDefinition(new ContentReference(1), new ContentReference(2), new ContentReference(3),
                new ContentReference(4));
        }

        public static void InitializeStaticReferences()
        {
            SiteDefinition.Current = CreateDefaultSiteDefinition();
            SystemDefinition.Current = CreateDefaultSystemDefinition();
        }
    }
}
