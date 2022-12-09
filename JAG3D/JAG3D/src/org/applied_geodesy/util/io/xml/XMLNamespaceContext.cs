using System.Collections.Generic;

/// <summary>
///*********************************************************************
/// Copyright by Michael Loesler, https://software.applied-geodesy.org   *
///                                                                      *
/// This program is free software; you can redistribute it and/or modify *
/// it under the terms of the GNU General Public License as published by *
/// the Free Software Foundation; either version 3 of the License, or    *
/// at your option any later version.                                    *
///                                                                      *
/// This program is distributed in the hope that it will be useful,      *
/// but WITHOUT ANY WARRANTY; without even the implied warranty of       *
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
/// GNU General Public License for more details.                         *
///                                                                      *
/// You should have received a copy of the GNU General Public License    *
/// along with this program; if not, see <http://www.gnu.org/licenses/>  *
/// or write to the                                                      *
/// Free Software Foundation, Inc.,                                      *
/// 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.            *
///                                                                      *
/// **********************************************************************
/// </summary>

namespace org.applied_geodesy.util.io.xml
{


	using Attr = org.w3c.dom.Attr;
	using Document = org.w3c.dom.Document;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	public abstract class XMLNamespaceContext : NamespaceContext
	{
		// https://www.ibm.com/developerworks/library/x-nmspccontext/
		private readonly string defaultPrefix;
		private IDictionary<string, string> prefix2Uri = new Dictionary<string, string>();
		private IDictionary<string, LinkedHashSet<string>> uri2Prefix = new Dictionary<string, LinkedHashSet<string>>();

		public XMLNamespaceContext(Document document) : this(document, false)
		{
		}

		public XMLNamespaceContext(Document document, bool topLevelOnly) : this(document, XMLConstants.DEFAULT_NS_PREFIX, XMLConstants.NULL_NS_URI, topLevelOnly)
		{
		}

		public XMLNamespaceContext(Document document, string defaultPrefix, string defaultURI) : this(document, defaultPrefix, defaultURI, false)
		{
		}

		public XMLNamespaceContext(Document document, string[] defaultPrefixes, string[] defaultURIs) : this(document, defaultPrefixes, defaultURIs, false)
		{
		}

		public XMLNamespaceContext(Document document, string defaultPrefix, string defaultURI, bool topLevelOnly) : this(document, new string[] {defaultPrefix}, new string[] {defaultURI}, false)
		{
		}

		public XMLNamespaceContext(Document document, string[] defaultPrefixes, string[] defaultURIs, bool topLevelOnly)
		{
			this.defaultPrefix = defaultPrefixes == null || defaultPrefixes.Length == 0 ? XMLConstants.DEFAULT_NS_PREFIX : defaultPrefixes[0];
			for (int i = 0; i < defaultPrefixes.Length; i++)
			{
				this.putInCache(string.ReferenceEquals(defaultPrefixes[i], null) ? XMLConstants.DEFAULT_NS_PREFIX : defaultPrefixes[i], string.ReferenceEquals(defaultURIs[i], null) ? XMLConstants.NULL_NS_URI : defaultURIs[i]);
			}

			this.examineNode(document.getFirstChild(), topLevelOnly);
	//		System.out.println("\nThe list of the cached namespaces:");
	//		for (String key : this.prefix2Uri.keySet()) {
	//			System.out.println("prefix " + key + ": uri " + this.prefix2Uri.get(key));
	//		}
	//		System.out.println();
		}

		private void examineNode(Node node, bool attributesOnly)
		{
			NamedNodeMap attributes = node.getAttributes();
			for (int i = 0; i < attributes.getLength(); i++)
			{
				Node attribute = attributes.item(i);
				this.storeAttribute((Attr) attribute);
			}

			if (!attributesOnly)
			{
				NodeList chields = node.getChildNodes();
				for (int i = 0; i < chields.getLength(); i++)
				{
					Node chield = chields.item(i);
					if (chield.getNodeType() == Node.ELEMENT_NODE)
					{
						this.examineNode(chield, false);
					}
				}
			}
		}

		private void storeAttribute(Attr attribute)
		{
			// examine the attributes in namespace xmlns
			if (attribute.getNamespaceURI() != null && attribute.getNamespaceURI().Equals(XMLConstants.XMLNS_ATTRIBUTE_NS_URI))
			{
				// Default namespace xmlns="uri goes here"
				if (attribute.getNodeName().Equals(XMLConstants.XMLNS_ATTRIBUTE))
				{
					string prefix = this.getPrefix(attribute);
					this.putInCache(prefix, attribute.getNodeValue());
				}
				else
				{
					// Here are the defined prefixes stored
					this.putInCache(attribute.getLocalName(), attribute.getNodeValue());
				}
			}
		}

		public abstract string getPrefix(Attr attribute);

		public virtual string DefaultPrefix
		{
			get
			{
				return this.defaultPrefix;
			}
		}

		private void putInCache(string prefix, string uri)
		{
			this.prefix2Uri[prefix] = uri;
			if (!this.uri2Prefix.ContainsKey(uri))
			{
				this.uri2Prefix[uri] = new LinkedHashSet<string>(5);
			}
			LinkedHashSet<string> ps = this.uri2Prefix[uri];
			ps.add(prefix);
		}

		public override string getNamespaceURI(string prefix)
		{
			if ((string.ReferenceEquals(prefix, null) || prefix.Equals(XMLConstants.DEFAULT_NS_PREFIX)) && this.prefix2Uri.ContainsKey(this.defaultPrefix))
			{
				return this.prefix2Uri[this.defaultPrefix];
			}
			if (this.prefix2Uri.ContainsKey(prefix))
			{
				return this.prefix2Uri[prefix];
			}
			return XMLConstants.NULL_NS_URI;
		}

		public override string getPrefix(string namespaceURI)
		{
			foreach (string prefix in this.uri2Prefix[namespaceURI])
			{
				return prefix;
			}
			return null;
		}

		public override IEnumerator<string> getPrefixes(string namespaceURI)
		{
			return this.uri2Prefix.ContainsKey(namespaceURI) ? this.uri2Prefix[namespaceURI].GetEnumerator() : Collections.emptyIterator<string>();
		}
	}

}