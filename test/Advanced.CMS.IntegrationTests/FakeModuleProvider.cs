using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules;

namespace Advanced.CMS.IntegrationTests
{
    public class FakeModuleProvider : IModuleProvider
    {
        public IEnumerable<ShellModule> GetModules()
        {
            var sm = new ShellModule("fakemodule", "fake", "fake");
            var manifest = new ShellModuleManifest();
            manifest.Dojo = new DojoConfiguration()
            {
                Paths = (new[] {new DojoPath() { Name = "Fake", Path = "dojo/path"} }).ToList()
            };
            sm.Manifest = manifest;
            sm.Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            
            yield return sm;
        }
    }
}
