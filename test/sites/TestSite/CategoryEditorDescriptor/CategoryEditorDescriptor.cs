using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace TestSite.CategoryEditorDescriptor
{
    /// <summary>
    /// Category handler editor descriptor
    /// </summary>
    [EditorDescriptorRegistration(TargetType = typeof(CategoryList))]
    public class CategoryEditorDescriptor : EditorDescriptor
    {
        private const string IcategorizableCategory = "icategorizable_category";

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            if (metadata.DefaultMetadata.PropertyName != IcategorizableCategory)
            {
                return;
            }

            var categoryAttribute = metadata.Attributes.FirstOrDefault(a => typeof(RemoveCategoryAttribute) == a.GetType());

            if (categoryAttribute is RemoveCategoryAttribute)
            {
                metadata.ShowForEdit = false;
                return;
            }

            var contentMetadata = (ContentDataMetadata)metadata;
            var ownerContent = contentMetadata.OwnerContent;
            if (!(ownerContent is BlockData))
            {
                return;
            }

            metadata.ShowForEdit = false;
        }
    }
}
