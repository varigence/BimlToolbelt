using System.Collections;
using System.Linq;
using Varigence.DynamicObjects;
using Varigence.Biml.CoreLowerer.SchemaManagement;
using Varigence.Biml.Extensions;
using Varigence.Biml.Extensions.SchemaManagement;
using Varigence.Flow.FlowFramework;
using Varigence.Flow.FlowFramework.Utility;
using Varigence.Languages.Biml;
using Varigence.Languages.Biml.Connection;
using Varigence.Languages.Biml.Cube;
using Varigence.Languages.Biml.Cube.Action;
using Varigence.Languages.Biml.Cube.Aggregation;
using Varigence.Languages.Biml.Cube.Calculation;
using Varigence.Languages.Biml.Cube.Partition;
using Varigence.Languages.Biml.Dimension;
using Varigence.Languages.Biml.Fact;
using Varigence.Languages.Biml.FileFormat;
using Varigence.Languages.Biml.LogProvider;
using Varigence.Languages.Biml.Measure;
using Varigence.Languages.Biml.MeasureGroup;
using Varigence.Languages.Biml.Metadata;
using Varigence.Languages.Biml.Project;
using Varigence.Languages.Biml.Platform;
using Varigence.Languages.Biml.Principal;
using Varigence.Languages.Biml.Script;
using Varigence.Languages.Biml.Table;
using Varigence.Languages.Biml.Task;
using Varigence.Languages.Biml.Transformation;
using Varigence.Languages.Biml.Transformation.Destination;

public class UtilityHelper
{
	public string Label { get; set; }
}

public static class DebuggerUtilities
{
	public static string GetAllPropertyValues(this AstNode astNode)
	{
		if (astNode == null)
		{
			return "Object is NULL";
		}
		
		var builder = new System.Text.StringBuilder();
		var type = astNode.GetType();
		builder.AppendLine(string.Format("'{0}' is of type {1}", astNode.ItemLabel, type.FullName));
		foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
		{
			var propertyValue = astNode.GetPropertyValue(property.Name);
			var propertyType = property.PropertyType;
			var enumerablePropertyValue = propertyValue as IEnumerable;
			if (enumerablePropertyValue != null && !(enumerablePropertyValue is string))
			{
				builder.AppendLine(string.Format("  {0}: ", property.Name));
				int i = 0;
				foreach (var collectionItem in enumerablePropertyValue)
				{
					builder.AppendLine(string.Format("    {0}", collectionItem == null ? "NULL" : collectionItem.ToString()));
					if (++i > 10)
					{
						break;
					}
				}
			}
			else
			{
				builder.AppendLine(string.Format("  {0}: {1}", property.Name, propertyValue == null ? "NULL" : propertyValue));
			}
	    }
		
		return builder.ToString();
	}
}
