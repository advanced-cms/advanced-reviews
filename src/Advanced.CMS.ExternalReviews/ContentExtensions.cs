using EPiServer.Core;
using EPiServer.Security;

namespace Advanced.CMS.ExternalReviews
{
    public static class ContentExtensions
    {
        public static IContent AllowAccessToEveryone(this IContent content)
        {
            if (content is not PageData page)
            {
                return content;
            }

            var writable = page.CreateWritableClone();
            var contentAccessControlList = new ContentAccessControlList();
            contentAccessControlList.AddEntry(new AccessControlEntry("Everyone", AccessLevel.FullAccess));
            writable.ACL = contentAccessControlList;
            return writable;

        }
    }
}
