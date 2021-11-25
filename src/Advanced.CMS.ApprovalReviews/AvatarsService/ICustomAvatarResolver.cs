using EPiServer.ServiceLocation;

namespace Advanced.CMS.ApprovalReviews.AvatarsService
{
    /// <summary>
    /// Service responsible for returning unique avatar based on username
    /// </summary>
    public interface ICustomAvatarResolver
    {
        /// <summary>
        /// Returns user photo based on user name
        /// Image should be a square. Recomended  image size is 100x100 pixels
        /// </summary>
        /// <param name="userName">Requested user name</param>
        /// <returns>User avatar</returns>
        byte[] GetImage(string userName);
    }

    /// <summary>
    /// Empty implementation for <see cref="ICustomAvatarResolver"/>
    /// Always returns "null" when resolving user
    /// </summary>
    [ServiceConfiguration(typeof(ICustomAvatarResolver), Lifecycle = ServiceInstanceScope.Singleton)]
    public class NullCustomAvatarResolver : ICustomAvatarResolver
    {
        public byte[] GetImage(string userName)
        {
            return null;
        }
    }
}
