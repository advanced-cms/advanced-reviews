namespace Advanced.CMS.IntegrationTests
{
    public class AuthConstants
    {
        public const string LoginPath = "/auth/login";
        public const string AccessDeniedPath = "/auth/denied";

        /// <summary>
        /// Query params for faking a user using <see cref="FakeUserMiddleware"/>
        /// </summary>
        public const string FakeWebAdmin = "&username=cmsadmin&roles=Administrators,CmsAdmins";

        /// <summary>
        /// Query params for faking a user using <see cref="FakeUserMiddleware"/>
        /// </summary>
        public const string FakeWebEditor = "&username=webeditor&roles=CmsEditors";

        /// <summary>
        /// Represents an autheticated user with some unknown role
        /// </summary>
        public const string FakeAuthenticatedUser = "&username=mrauthenticated&roles=Dude";
    }
}
