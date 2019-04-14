using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAccess.Internal;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using EPiServer.Shell.WebForms;
using EPiServer.UI.Admin;

namespace AdvancedApprovalReviews
{
    [GuiPlugIn(DisplayName = "Advanced approval review", Description = "",
        LanguagePath = null,
        Area = PlugInArea.AdminConfigMenu,
        UrlFromModuleFolder = "Views/admin/ReviewLocationPreview.aspx",
        SortIndex = 400)]
    public partial class ReviewLocationsPreview : WebFormsBase
    {
        protected Injected<IApprovalReviewsRepository> _repository { get; set; }
        protected Injected<IObjectSerializerFactory> _serializerFactory { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var targetCtrl = Page.Request.Params.Get("__EVENTTARGET");
            if (targetCtrl == "delete_review")
            {
                var contentLink = Page.Request.Params.Get("__EVENTARGUMENT");
                DeleteReviewLocation(
                    string.IsNullOrWhiteSpace(contentLink) ? null: ContentReference.Parse(contentLink));
            }

            SystemMessageContainer.Heading = "Review locations";
            SystemMessageContainer.Description = "List of all review locations";
            DataBind();
        }

        private void DeleteReviewLocation(ContentReference contentLink)
        {
            _repository.Service.Delete(contentLink);
        }

        protected string AllReviewLocations
        {
            get
            {
                var result = _repository.Service.LoadAll().GroupBy(x => x.ContentLink.ToReferenceWithoutVersion()).Select(x => new
                {
                    Id = x.Key,
                    ContentLinks = x.Select(c => new { c.ContentLink, c.SerializedReview })
                });

                return _serializerFactory.Service.GetSerializer(KnownContentTypes.Json).Serialize(result);
            }
        }
    }
}
