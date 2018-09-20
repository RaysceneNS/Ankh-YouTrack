using System;
using System.Xml.Linq;

namespace Ankh.YouTrack.Services
{
    internal static class XElementExtensions
	{
		internal static string GetStringValue(this XElement element, string fieldName)
		{
			foreach (var subElement in element.Elements("field"))
			{
			    var nameAttribute = subElement.Attribute("name");
                if (nameAttribute != null && string.Equals(nameAttribute.Value, fieldName, StringComparison.OrdinalIgnoreCase))
				{
					return subElement.Element("value")?.Value;
				}
			}
			return null;
		}

	    internal static DateTime GetDateTimeValue(this XElement element, string fieldName)
	    {
	        long.TryParse(element.GetStringValue(fieldName), out var millis);
            return new DateTime(1970, 1, 1).AddSeconds(millis / 1000.0);
	    }
	}
}