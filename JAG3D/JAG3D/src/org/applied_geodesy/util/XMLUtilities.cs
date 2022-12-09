using System;

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

  /// <summary>
  ///********************************************************************
  ///                             XMLUtilities                             *
  /// ************************************************************************
  /// Copyright (C) by Michael Loesler, http://derletztekick.com           *
  ///                                                                      *
  /// This program is free software; you can redistribute it and/or modify *
  /// it under the terms of the GNU General Public License as published by *
  /// the Free Software Foundation; either version 3 of the License, or    *
  /// (at your option) any later version.                                  *
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
  /// *********************************************************************
  /// </summary>

namespace org.applied_geodesy.util
{


	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	public class XMLUtilities
	{
		private XMLUtilities()
		{
		}

		public static string transformDocument2String(Document doc)
		{
			try
			{
				StringWriter sw = new StringWriter();
				TransformerFactory tf = TransformerFactory.newInstance();
				Transformer transformer = tf.newTransformer();
				transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
				transformer.setOutputProperty(OutputKeys.METHOD, "xml");
				transformer.setOutputProperty(OutputKeys.INDENT, "yes");
				transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");

				transformer.transform(new DOMSource(doc), new StreamResult(sw));
				return sw.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("Fehler beim Konvertieren in einen String ", ex);
			}
		}

		public static object xpathSearch(Node node, string xpathPattern, NamespaceContext namespaceContext, QName type)
		{
			XPathFactory xPathfactory = XPathFactory.newInstance();
			XPath xpath = xPathfactory.newXPath();
			if (namespaceContext != null)
			{
				xpath.setNamespaceContext(namespaceContext);
			}
			if (type == null)
			{
				type = XPathConstants.NODESET;
			}

			try
			{
				XPathExpression expression = xpath.compile(xpathPattern);
				return expression.evaluate(node, type);
			}
			catch (XPathExpressionException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
		}

		public static object xpathSearch(Document doc, string xpathPattern, NamespaceContext namespaceContext, QName type)
		{
			XPathFactory xPathfactory = XPathFactory.newInstance();
			XPath xpath = xPathfactory.newXPath();
			if (namespaceContext != null)
			{
				xpath.setNamespaceContext(namespaceContext);
			}
			if (type == null)
			{
				type = XPathConstants.NODESET;
			}

			try
			{
				XPathExpression expression = xpath.compile(xpathPattern);
				return expression.evaluate(doc, type);
			}
			catch (XPathExpressionException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
		}

		public static Document copySubTree(Document srcDocument, string tagname)
		{
			return copySubTree(srcDocument, tagname, 0);
		}

		public static Document copySubTree(Document srcDocument, string tagname, int index)
		{
			NodeList nodes = srcDocument.getElementsByTagName(tagname);
			if (nodes.getLength() > index)
			{
				return copySubTree(nodes.item(index));
			}
			return null;
		}

		public static Document copySubTree(Node node)
		{
			try
			{
				Document trgDocument = DocumentBuilderFactory.newInstance().newDocumentBuilder().newDocument();
				//NodeList nodes = document.getDocumentElement().getChildNodes();
				Node newNode = trgDocument.importNode(node, true);
				trgDocument.appendChild(newNode);

				return trgDocument;
			}
			catch (ParserConfigurationException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
		}
	}

}