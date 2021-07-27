﻿using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ApprovalReviews
{
    [RestStore("approvallanguage")]
    //TODO: any better way to do this? REWRITE THIS TO SHELL MODULE VIEW MODEL!
    public class ApprovalLanguageStore : RestControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            var _username = User.Identity.Name;
            // var profile = EPiServerProfile.Get(_username);

            // return Rest(profile.Language);
            return Rest("en");
        }
    }
}