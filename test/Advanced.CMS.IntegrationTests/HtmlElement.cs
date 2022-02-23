using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EPiServer.HtmlParsing;

namespace Advanced.CMS.IntegrationTests
{
    internal class HtmlElement : IHtmlElement
    {
        public static readonly IDictionary<string, string> AnyAttributes = new Dictionary<string, string> { { Wildcard, "" } };

        static readonly IHtmlElement emptyElement = new HtmlElement("");

        IDictionary<string, string> _attributes = new Dictionary<string, string>();
        readonly List<IHtmlElement> _children = new List<IHtmlElement>();
        string _value;
        string _name;

        public const string Wildcard = "*";

        public HtmlElement() { }

        public HtmlElement(object value)
            : this()
        {
            var text = value != null ? value.ToString() : null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                _children.Add(new TextElement(text));
            }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string Value
        {
            get { return _value ?? InnerHtml(); }
            set { _value = value; }
        }

        public IDictionary<string, string> Attributes
        {
            get { return _attributes; }
            set { _attributes = value ?? new Dictionary<string, string>(); }
        }

        public virtual IList<IHtmlElement> Children
        {
            get { return _children; }
        }

        public virtual bool IsEmpty
        {
            get { return Value == null && Children.Count == 0; }
        }

        public virtual string InnerHtml()
        {
            using (var sw = new StringWriter())
            {
                ToWriter(sw, Children);
                return sw.ToString();
            }
        }

        public virtual string OuterHtml()
        {
            using (var sw = new StringWriter())
            {
                ToWriter(sw, new[] { this });
                return sw.ToString();
            }
        }

        static void ToWriter(TextWriter writer, IEnumerable<IHtmlElement> elements)
        {
            foreach (var element in elements)
            {
                var name = element.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    writer.Write("<{0}", name);

                    foreach (var attribute in element.Attributes)
                    {
                        writer.Write(" {0}=\"{1}\"", attribute.Key, attribute.Value);
                    }

                    writer.Write(">");
                }

                writer.Write(element.Value ?? "");

                if (!string.IsNullOrEmpty(name))
                {
                    writer.Write("</{0}>", name);
                }

            }
        }

        public static IHtmlElement ParseControlContent(string html, string wrapperID)
        {
            return ParseControlContent(html, wrapperID != null ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "id", wrapperID } } : null);
        }

        public static IHtmlElement ParseControlContent(string html, IDictionary<string, string> elementAttributes = null, string elementName = Wildcard, bool outerElement = false)
        {
            var reader = new HtmlStreamReader(html, ParserOptions.DefaultOptions | ParserOptions.EnableCallbackOnEndElement);
            bool elementFound = elementAttributes == null, closingElementFound = false;
            ParsedElement root = null;
            var parentStack = new Stack<ParsedElement>();

            foreach (var fragment in reader)
            {
                if (!elementFound)
                {
                    var elementFragment = fragment as ElementFragment;
                    if (elementFragment != null && ElementMatches(elementFragment, elementAttributes, elementName))
                    {
                        if (elementFragment.IsEmpty)
                        {
                            return outerElement ? new ParsedElement(fragment) : emptyElement;
                        }
                        elementFound = true;
                        elementFragment.CallbackOnEndElement = () => closingElementFound = true;
                    }
                    if (!elementFound || !outerElement)
                        continue;
                }

                if (closingElementFound)
                {
                    break;
                }

                // Skip empty text elements
                if (fragment.FragmentType == HtmlFragmentType.Text && string.IsNullOrWhiteSpace(fragment.Value))
                {
                    continue;
                }

                var element = new ParsedElement(fragment);
                if (root == null)
                {
                    root = element;
                }
                else
                {
                    if (fragment.FragmentType == HtmlFragmentType.EndElement && parentStack.Count > 0)
                    {
                        parentStack.Pop();
                    }
                    if (parentStack.Count > 0)
                    {
                        var parent = parentStack.Peek();
                        parent.Children.Add(element);
                    }
                }

                if (fragment.FragmentType == HtmlFragmentType.Element && !fragment.IsEmpty)
                {
                    parentStack.Push(element);
                }
            }

            return elementFound ? root ?? emptyElement : null;
        }

        private static bool ElementMatches(ElementFragment elementFragment, IDictionary<string, string> elementAttributes, string elementName)
        {
            if (elementName != Wildcard && !string.Equals(elementFragment.Name, elementName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            foreach (KeyValuePair<string, string> a in elementAttributes)
            {
                if (elementFragment.Attributes[a.Key] == null || elementFragment.Attributes[a.Key].UnquotedValue != a.Value) return false;
            }
            return true;
        }

        private class ParsedElement : IHtmlElement
        {
            readonly HtmlFragment _fragment;
            readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();
            readonly List<ParsedElement> _children = new List<ParsedElement>();

            public ParsedElement(HtmlFragment fragment)
            {
                _fragment = fragment;

                var elementFragment = fragment as ElementFragment;
                if (elementFragment != null)
                {
                    foreach (var attribute in elementFragment.Attributes)
                    {
                        _attributes.Add(attribute.Name, attribute.UnquotedValue);
                    }
                }
            }

            public string Name
            {
                get { return _fragment.Name; }
            }

            public string Value
            {
                get { return _fragment.Value ?? InnerHtml(); }
            }

            public bool IsEmpty
            {
                get { return _fragment.IsEmpty; }
            }

            public IDictionary<string, string> Attributes
            {
                get { return _attributes; }
            }

            public IList<ParsedElement> Children
            {
                get { return _children; }
            }

            IList<IHtmlElement> IHtmlElement.Children
            {
                get { return Children.Where(x => !(x._fragment is EndElementFragment)).Cast<IHtmlElement>().ToArray(); }
            }

            public override string ToString()
            {
                using (var sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    _fragment.ToWriter(sw);
                    return sw.ToString();
                }
            }

            public string InnerHtml()
            {
                using (var sw = new StringWriter())
                {
                    ToWriter(this, sw);
                    return sw.ToString();
                }
            }

            public string OuterHtml()
            {
                using (var sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    _fragment.ToWriter(sw);
                    ToWriter(this, sw);
                    if (!string.IsNullOrEmpty(Name) && !IsEmpty)
                    {
                        sw.Write("</{0}>", Name);
                    }
                    return sw.ToString();
                }
            }

            static void ToWriter(ParsedElement element, TextWriter writer)
            {
                foreach (var child in element._children)
                {
                    child._fragment.ToWriter(writer);
                    if (!child._fragment.IsEmpty)
                    {
                        ToWriter(child, writer);
                    }
                }
            }

        }

        private class TextElement : IHtmlElement
        {
            public TextElement(string value)
            {
                Value = value;
            }

            public string Name { get { return null; } }
            public string Value { get; private set; }
            public IDictionary<string, string> Attributes { get { return new Dictionary<string, string>(0); } }
            public IList<IHtmlElement> Children { get { return new IHtmlElement[0]; } }
            public bool IsEmpty { get { return true; } }
            public string InnerHtml()
            {
                return Value;
            }

            public string OuterHtml()
            {
                return Value;
            }
        }

    }
}
