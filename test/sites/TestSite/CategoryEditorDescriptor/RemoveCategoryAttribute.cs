using System;

namespace TestSite.CategoryEditorDescriptor
{
    /// <summary>
    /// Remove category property attribute, UI descriptor attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RemoveCategoryAttribute : Attribute
    {
    }
}
