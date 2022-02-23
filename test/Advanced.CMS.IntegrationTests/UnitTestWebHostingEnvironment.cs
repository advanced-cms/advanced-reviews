using System;
using EPiServer.Web;

namespace Advanced.CMS.IntegrationTests
{
    public class UnitTestWebHostingEnvironment : IWebHostingEnvironment, IDisposable
    {
        private string _webRootVirtualPath;
        private IWebHostingEnvironment _originalHostingEnvironment;
        private string _webRootPath;

        public static UnitTestWebHostingEnvironment Auto(string webRootVirtualPath = null)
        {
            var instance = new UnitTestWebHostingEnvironment
            {
                WebRootVirtualPath = webRootVirtualPath,
                _originalHostingEnvironment = WebHostingEnvironment.Instance
            };
            WebHostingEnvironment.Instance = instance;
            return instance;
        }

        public void Dispose()
        {
            if (_originalHostingEnvironment != null)
            {
                WebHostingEnvironment.Instance = _originalHostingEnvironment;
                _originalHostingEnvironment = null;
            }
        }

        public virtual string WebRootPath
        {
            get => _webRootPath ?? Environment.CurrentDirectory;
            set => _webRootPath = value;
        }

        public virtual string WebRootVirtualPath
        {
            get => _webRootVirtualPath ?? "/";
            set => _webRootVirtualPath = value;
        }
    }
}
