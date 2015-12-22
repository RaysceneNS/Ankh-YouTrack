using System;
using System.Xml.Linq;

namespace Ankh.YouTrack.Services
{
	static class XElementExtensions
	{
		internal static string GetStringValue(this XElement element, string fieldName)
		{
			foreach (var subElement in element.Elements("field"))
			{
				if (string.Equals(subElement.Attribute("name").Value, fieldName, StringComparison.OrdinalIgnoreCase))
				{
					var valueElement = subElement.Element("value");
					if(valueElement != null)
						return valueElement.Value;
				}
			}
			return null;
		}
	}
}