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
            if (contentAccessControlList.Contains(EveryoneRole.RoleName))
            {
                contentAccessControlList.Remove(EveryoneRole.RoleName);
            }

            contentAccessControlList.AddEntry(new AccessControlEntry(EveryoneRole.RoleName,
                AccessLevel.FullAccess));

            writable.ACL = contentAccessControlList;
            return writable;

        }
    }
}
