using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class GuidManager
{
	public static bool AutomaticallyLoadAndStoreGuids = true;
	
	public static string GuidFilePath { get; set; }
	
	public static string GuidFileSeparator = "|";
	
	public static Dictionary<string, Guid> _guidCache = new Dictionary<string, Guid>();
	
	private static bool HasCacheBeenLoaded;

	public static Guid GetId(Varigence.Languages.Biml.AstNamedNode astNamedNode)
	{
		if (astNamedNode == null)
		{
			throw new Exception("Null objects are not supported by the GuidManager");
		}
		
		return GetId(astNamedNode.ScopedName);
	}
	
	public static Guid GetId(string scopedName)
	{
		if (AutomaticallyLoadAndStoreGuids && !HasCacheBeenLoaded)
		{
			LoadGuids();
		}
		
		if (string.IsNullOrEmpty(scopedName))
		{
			throw new Exception("Null and empty scoped names are not supported by the GuidManager");
		}
		
		Guid guid;
		if (!_guidCache.TryGetValue(scopedName, out guid))
		{
			guid = Guid.NewGuid();
			_guidCache[scopedName] = guid;
			if (AutomaticallyLoadAndStoreGuids)
			{
				StoreGuids();
			}
		}		
		
		return guid;
	}
	
	public static void LoadGuids()
	{
		HasCacheBeenLoaded = true;

		if (string.IsNullOrEmpty(GuidFilePath))
		{
			throw new Exception("GuidManager.GuidFilePath was not specified");
		}
		
		if (File.Exists(GuidFilePath))
		{
			var guidLines = File.ReadAllLines(GuidFilePath);
			foreach (var guidLine in guidLines)
			{
				var columnArray = guidLine.Split(new[] { GuidFileSeparator }, StringSplitOptions.RemoveEmptyEntries);
				// Try/Catch saves some checking for array length, nulls, guid formats, and other exceptional conditions
				try
				{
					_guidCache[columnArray[0]] = new Guid(columnArray[1]);
				}
				catch (Exception) {}
			}
		}
	}
	
	public static void StoreGuids()
	{
		if (string.IsNullOrEmpty(GuidFilePath))
		{
			throw new Exception("GuidManager.GuidFilePath was not specified");
		}
		
		var builder = new StringBuilder();
		foreach (var kvp in _guidCache)
		{
			builder.AppendLine(kvp.Key + GuidFileSeparator + kvp.Value);
		}
		
		Directory.CreateDirectory(Path.GetDirectoryName(GuidFilePath));
		File.WriteAllText(GuidFilePath, builder.ToString());
	}
}
