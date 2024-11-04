namespace Advanced.CMS.ApprovalReviews.AvatarsService;

/// <summary>
/// Service responsible for returning unique avatar based on username
/// </summary>
public interface ICustomAvatarResolver
{
    /// <summary>
    /// Returns user photo based on username
    /// Image should be a square. Recommended  image size is 100x100 pixels
    /// </summary>
    /// <param name="userName">Requested user name</param>
    /// <returns>User avatar</returns>
    byte[] GetImage(string userName);
}

/// <summary>
/// Empty implementation for <see cref="ICustomAvatarResolver"/>
/// Always returns "null" when resolving user
/// </summary>
internal class NullCustomAvatarResolver : ICustomAvatarResolver
{
    public byte[] GetImage(string userName)
    {
        return null;
    }
}
