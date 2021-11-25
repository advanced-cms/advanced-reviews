Advanced.CMS.AdvancedReviews


Installation
============


In order to start using AdvancedReviews you need to add it explicitly to your site.
Please add the following statement to your Startup.cs


public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddAdvancedReviews();
        ...
    }
    ...
}

AddAdvancedReviews extension method also accepts optional parameter of Action<ExternalReviewOptions> which
lets you configure the add-on according to your needs.
Full documentation can be found here: https://github.com/advanced-cms/advanced-reviews
