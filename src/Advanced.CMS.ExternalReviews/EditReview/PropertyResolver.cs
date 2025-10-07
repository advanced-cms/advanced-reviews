using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.UI.Rest;

namespace Advanced.CMS.ExternalReviews.EditReview;

internal class PropertyResolver(ExtensibleMetadataProvider metadataProvider, IMetadataStoreModelCreator modelCreator)
{
    public IDictionary<string, string> Resolve(ContentData content)
    {
        var metadata = metadataProvider.GetExtendedMetadataForType(typeof(ContentData), () => content);
        var storeModel = modelCreator.Create(metadata);
        var properties = new Dictionary<string, string>();
        foreach (var metadataStoreModel in storeModel.Properties)
        {
            var props = storeModel.MappedProperties.Where(x => x.To == metadataStoreModel.Name).ToList();
            foreach (var propertyMapping in props)
            {
                properties.Add(propertyMapping.From, metadataStoreModel.DisplayName);
            }
            properties.Add(metadataStoreModel.Name, metadataStoreModel.DisplayName);
        }

        return properties;
    }
}
