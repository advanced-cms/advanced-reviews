using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.UI.Rest;

namespace Advanced.CMS.ExternalReviews.EditReview
{
    [ServiceConfiguration]
    public class PropertyResolver
    {
        private readonly ExtensibleMetadataProvider _metadataProvider;
        private readonly IMetadataStoreModelCreator _modelCreator;

        public PropertyResolver(ExtensibleMetadataProvider metadataProvider, IMetadataStoreModelCreator modelCreator)
        {
            _metadataProvider = metadataProvider;
            _modelCreator = modelCreator;
        }

        public IDictionary<string, string> Resolve(ContentData content)
        {
            var metadata = _metadataProvider.GetExtendedMetadataForType(typeof(ContentData), () => content);
            var storeModel = _modelCreator.Create(metadata);
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
}
