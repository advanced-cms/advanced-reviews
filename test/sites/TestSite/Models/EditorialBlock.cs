﻿using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace TestSite.Models;

[ContentType(GUID = "541C1C77-0040-4D05-AF42-14C165D882C7")]
public class EditorialBlock : BlockData
{
    public virtual string MainBody { get; set; }
}
