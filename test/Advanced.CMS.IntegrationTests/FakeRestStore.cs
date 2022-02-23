using EPiServer.Shell.Modules;
using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.IntegrationTests
{
    [RestStore("fake")]
    public class FakeRestStore : RestControllerBase
    {
        public FakeRestStore()
        {
        }

        [HttpGet]
        public RestResultBase Get(string id)
        {
            return new FakeRestResult();
        }

        [HttpGet]
        public RestResultBase Module(ShellModule module)
        {

            return Rest(new { name = module.Name });
        }
    }
    public class FakeRestResult : RestResultBase
    {
        public FakeRestResult() : base()
        {
        }
    }
}
