using System.Collections.Generic;
using System.Linq;

namespace Advanced.CMS.IntegrationTests
{
    internal interface IHtmlElement
    {
        string Name { get; }
        string Value { get; }
        IDictionary<string, string> Attributes { get; }
        IList<IHtmlElement> Children { get; }
        bool IsEmpty { get; }
        string InnerHtml();
        string OuterHtml();
    }

    internal static class HtmlElementExtensions
    {
        public static IHtmlElement FirstChild(this IHtmlElement element) => element.Children.First();
        public static IHtmlElement LastChild(this IHtmlElement element) => element.Children.Last();
    }
}
