// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// Derived from Microsoft.AspNetCore.Mvc.TestCommon.SolutionPathUtility

using System;
using System.IO;

namespace Advanced.CMS.IntegrationTests
{
    public static class SolutionPathUtility
    {
        public static string GetSolutionPath(string solutionRelativePath, string solutionName)
        {
            var applicationBasePath = Environment.CurrentDirectory; // Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, solutionName));
                if (solutionFileInfo.Exists)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath));
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using current application directory {applicationBasePath}.");
        }
    }
}
