using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace TixFactory.Assembly.Documentation
{
	/// <inheritdoc cref="IAssemblyDocumenter"/>
	public class AssemblyDocumenter : IAssemblyDocumenter
	{
		private readonly IDictionary<string, XElement> _AssemblyMembers;

		/// <inheritdoc cref="IAssemblyDocumenter.Names"/>
		public ISet<string> Names { get; }

		/// <summary>
		/// Initializes a new <see cref="AssemblyDocumenter"/>.
		/// </summary>
		/// <param name="assemblyDocumentations">The root <see cref="XElement"/>s of the assembly documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="assemblyDocumentations"/></exception>
		public AssemblyDocumenter(params XElement[] assemblyDocumentations)
		{
			if (assemblyDocumentations == null)
			{
				throw new ArgumentNullException(nameof(assemblyDocumentations));
			}

			_AssemblyMembers = new Dictionary<string, XElement>();

			var assemblyNames = new HashSet<string>();

			foreach (var assemblyDocumentation in assemblyDocumentations)
			{
				foreach (var documentationElement in assemblyDocumentation.Descendants())
				{
					if (documentationElement.Name.LocalName == "name" && documentationElement.Parent?.Name.LocalName == "assembly")
					{
						assemblyNames.Add(documentationElement.Value);
						continue;
					}

					var nameAttribute = documentationElement.Attributes().FirstOrDefault(a => a.Name.LocalName == "name");
					if (string.IsNullOrWhiteSpace(nameAttribute?.Value) || _AssemblyMembers.ContainsKey(nameAttribute.Value))
					{
						continue;
					}

					if (documentationElement.Name.LocalName == "member")
					{
						_AssemblyMembers.Add(nameAttribute.Value, documentationElement);
					}
				}
			}

			Names = assemblyNames;
		}

		/// <inheritdoc cref="IAssemblyDocumenter.GetTypeDocumentation"/>
		public IAssemblyDocumentation GetTypeDocumentation(Type type)
		{
			var documentationKey = $"T:{type.FullName}";
			var documentationElement = _AssemblyMembers.ContainsKey(documentationKey) ? _AssemblyMembers[documentationKey] : null;
			return GetXmlDocumentation(documentationKey, documentationElement);
		}

		/// <inheritdoc cref="IAssemblyDocumenter.GetFieldDocumentation"/>
		public IAssemblyDocumentation GetFieldDocumentation(FieldInfo fieldInfo)
		{
			var documentationKey = $"F:{fieldInfo.ReflectedType?.FullName}.{fieldInfo.Name}";
			var documentationElement = _AssemblyMembers.ContainsKey(documentationKey) ? _AssemblyMembers[documentationKey] : null;
			return GetXmlDocumentation(documentationKey, documentationElement);
		}

		/// <inheritdoc cref="IAssemblyDocumenter.GetPropertyDocumentation"/>
		public IAssemblyDocumentation GetPropertyDocumentation(PropertyInfo propertyInfo)
		{
			var documentationKey = $"P:{propertyInfo.ReflectedType?.FullName}.{propertyInfo.Name}";
			var documentationElement = _AssemblyMembers.ContainsKey(documentationKey) ? _AssemblyMembers[documentationKey] : null;
			return GetXmlDocumentation(documentationKey, documentationElement);
		}

		private IAssemblyDocumentation GetXmlDocumentation(string name, XElement documentationElement)
		{
			if (documentationElement == null)
			{
				return null;
			}

			var summary = documentationElement.Descendants().FirstOrDefault(e => e.Name.LocalName == "summary");
			var remarks = documentationElement.Descendants().FirstOrDefault(e => e.Name.LocalName == "remarks");

			var documentation = new AssemblyDocumentation
			{
				Name = name,
				Summary = GetInnerXml(summary),
				Remarks = GetInnerXml(remarks)
			};

			foreach (var inheritdocElement in documentationElement.Descendants().Where(e => e.Name.LocalName == "inheritdoc"))
			{
				var cref = GetAttributeValue(inheritdocElement, "cref");
				if (!string.IsNullOrWhiteSpace(cref) && _AssemblyMembers.ContainsKey(cref))
				{
					var inheritedDocumentation = GetXmlDocumentation(cref, _AssemblyMembers[cref]);
					if (inheritedDocumentation != null)
					{
						documentation.InheritedAssemblyDocumentations.Add(inheritedDocumentation);
					}
				}
			}

			return documentation;
		}

		private string GetInnerXml(XElement element)
		{
			return element?.Nodes().Aggregate("", (b, node) =>
			{
				var nodeString = node.ToString();

				if (node is XElement nodeElement && nodeElement.Name.LocalName == "see")
				{
					var cref = GetAttributeValue(nodeElement, "cref");
					if (!string.IsNullOrWhiteSpace(cref) && _AssemblyMembers.ContainsKey(cref))
					{
						var shortName = cref.Split('.').Last();
						if (!string.IsNullOrWhiteSpace(shortName))
						{
							nodeString = $"<see cref=\"{shortName}\" />";
						}
					}
				}

				return b += nodeString;
			}).Trim();
		}

		private string GetAttributeValue(XElement element, string attributeName)
		{
			var attribute = element.Attributes().FirstOrDefault(e => e.Name.LocalName == attributeName);
			return attribute?.Value;
		}
	}
}
