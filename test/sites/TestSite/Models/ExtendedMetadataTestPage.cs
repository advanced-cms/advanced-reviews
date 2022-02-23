using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using TestSite.CategoryEditorDescriptor;
using EPiServer.SpecializedProperties;
using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;
using EPiServer.PlugIn;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;

namespace TestSite.Models
{
    /// <summary>
    /// Used for the site's start page and also acts as a container for site settings
    /// </summary>
    [ContentType(
        GUID = "645e4526-e78f-449c-b74f-8bf87ce382af",
        GroupName = Global.GroupNames.Specialized)]
    // [SiteImageUrl]
    [AvailableContentTypes(
        Availability.Specific,
        Include = new[] { typeof(StandardPage), typeof(ContentFolder) }, // Pages we can create under the start page...
        ExcludeOn = new[] { typeof(StandardPage) })] // ...and underneath those we can't create additional start pages
    public class ExtendedMetadataTestPage : SitePageData
    {
        [RemoveCategory]
        public override CategoryList Category { get; set; }

        [Required(ErrorMessage = "This message should override default message")]
        public override string CreatedBy { get => base.CreatedBy; set => base.CreatedBy = value; }

        [StringLength(100)]
        public override string ChangedBy { get => base.ChangedBy; set => base.ChangedBy = value; }

        [Required]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "I am limited long")]
        [RegularExpression("^[a-zA-Z0-9 ]{0,100}$", ErrorMessage = "I am regex validated")]
        public virtual string Heading { get; set; }

        [SelectOne(SelectionFactoryType = typeof(LanguageSelectionFactory))]
        public virtual string SingleLanguage { get; set; }

        [SelectMany(SelectionFactoryType = typeof(LanguageSelectionFactory))]
        public virtual string MultipleLanguage { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        [CultureSpecific]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 300)]
        public virtual LinkItemCollection ProductPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 350)]
        public virtual LinkItemCollection CompanyInformationPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 400)]
        public virtual LinkItemCollection NewsPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 450)]
        public virtual LinkItemCollection CustomerZonePageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference GlobalNewsPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference ContactsPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference SearchPageLink { get; set; }

        //[Display(GroupName = Global.GroupNames.SiteSettings)]
        //public virtual SiteLogotypeBlock SiteLogotype { get; set; }

        [Display(Name = "Customers")]
        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<Customer>))]
        public virtual IList<Customer> Customers { get; set; }

        [Range(minimum: 1, maximum: 99)]
        public virtual int Count { get; set; }
    }

    [PropertyDefinitionTypePlugIn]
    public class CustomersProperty : PropertyList<Customer> { }

    public class Customer
    {
        [Display(Name = "First name")]
        public virtual string Name { get; set; }

        [Display(Name = "Customer notes")]
        public virtual XhtmlString CustomerNotes { get; set; }

        [SelectOne(SelectionFactoryType = typeof(LanguageSelectionFactory))]
        public virtual string SingleLanguage { get; set; }

        [SelectMany(SelectionFactoryType = typeof(LanguageSelectionFactory))]
        public virtual string MultipleLanguage { get; set; }
    }

    public class LanguageSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return new ISelectItem[] { new SelectItem() { Text = "English", Value = "EN" }, new SelectItem() { Text = "Swahili", Value = "SW" }, new SelectItem() { Text = "French Polonesia", Value = "PF" } };
        }
    }
}
