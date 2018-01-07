using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Varigence.Languages.Biml.Table;

public static class DependencyAnalysis
{
	public static IEnumerable<AstTableNode> TopoSort(this IEnumerable<AstTableNode> tables)
	{
		var visitedList = new List<AstTableNode>();
		var outputList = new List<AstTableNode>();
		foreach (var rootTable in tables/*.Where(t => !t.Columns.OfType<AstTableColumnTableReferenceNode>().Any()*/)
		{
			TopoVisit(rootTable, outputList, visitedList);
		}
		
		return outputList;
	}

	private static void TopoVisit(AstTableNode node, List<AstTableNode> outputList, List<AstTableNode> visitedList)
	{
		if (!visitedList.Contains(node))
		{
			visitedList.Add(node);
			foreach (var dependentTable in node.Columns.OfType<AstTableColumnTableReferenceNode>().Select(c => c.ForeignTable))
			{
				TopoVisit(dependentTable, outputList, visitedList);
	        }
			
			outputList.Add(node);
	    }
	}
	
	public static IEnumerable<AstTableNode> TopoSort2(this IEnumerable<AstTableNode> tables)
	{
		return tables.OrderBy(t => t, new TopologicalComparer());
	}
	
	
	
	public class TopologicalComparer : IComparer<AstTableNode>
	{
		public int Compare(AstTableNode t1, AstTableNode t2)
		{
			if (t2.Columns.OfType<AstTableColumnTableReferenceNode>().Select(c => c.ForeignTable).Contains(t1))
			{
				return 1;
	        }
			
			if (t1.Columns.OfType<AstTableColumnTableReferenceNode>().Select(c => c.ForeignTable).Contains(t2))
			{
				return -1;
	        }

			return 0;
	    }
	}
	
	
	
	/******************************/
	
	public static IEnumerable<AstTableNode> TopoSortA(this IEnumerable<AstTableNode> nodes)
	{
		return TopoSortA(nodes, t=>t.Columns.OfType<AstTableColumnTableReferenceNode>().Select(c => c.ForeignTable)).Reverse();
	}
	
	public static IEnumerable<T> TopoSortA<T>(this IEnumerable<T> nodes, Expression<Func<T,IEnumerable<T>>> getDependencies)
	{
		var func = getDependencies.Compile();
		var visitedList = new List<T>();
		var outputList = new List<T>();
		foreach (var rootNode in nodes)
		{
			TopoVisitA(rootNode, outputList, visitedList, func);
		}
		
		return outputList;
	}

	private static void TopoVisitA<T>(T node, List<T> outputList, List<T> visitedList, Func<T,IEnumerable<T>> getDependencies)
	{
		if (!visitedList.Contains(node))
		{
			visitedList.Add(node);
			foreach (var dependentTable in getDependencies.Invoke(node))
			{
				TopoVisitA(dependentTable, outputList, visitedList, getDependencies);
	        }
			
			outputList.Add(node);
	    }
	}
	
	
	
	
	
	/*
    public static IEnumerable<T> TopologicalSort<T>(IEnumerable<T> source, Expression<Func<T, T>> relationProperty)
    {
        var relationPropertyFunc = relationProperty.Compile();

        var nodeList = new LinkedList<T>();
        var visitedList = new HashSet<T>();

        foreach (var node in source)
        {
            TopologicalSortVisit(node, nodeList, visitedList, relationPropertyFunc);
        }

        return nodeList.Reverse();
    }

    private static void TopologicalSortVisit<T>(T node, LinkedList<T> nodeList, HashSet<T> visitedList, Func<T, T> relationPropertyFunc)
    {
        if (!visitedList.Contains(node))
        {
            visitedList.Add(node);
            TopologicalSortVisit(relationPropertyFunc.Invoke(node), nodeList, visitedList, relationPropertyFunc);
            nodeList.AddLast(node);
        }
    }
	
	public static IEnumerable<AstTableNode> TopologicalSort(this IEnumerable<AstTableNode> source)
	{
		return TopologicalSort(source, t => t.Columns.OfType<AstTableColumnTableReferenceNode>().Select(c => c.ForeignTable));
	}
	
    public static IEnumerable<T> TopologicalSort<T>(IEnumerable<T> source, Expression<Func<T, IEnumerable<T>>> relationProperty)
    {
        var relationPropertyFunc = relationProperty.Compile();

        var nodeList = new LinkedList<T>();
        var visitedList = new HashSet<T>();

        foreach (var node in source)
        {
            TopologicalSortVisit(node, nodeList, visitedList, relationPropertyFunc);
        }

        return nodeList.Reverse();
    }

    private static void TopologicalSortVisit<T>(T node, LinkedList<T> nodeList, HashSet<T> visitedList, Func<T, IEnumerable<T>> relationPropertyFunc)
    {
        if (!visitedList.Contains(node))
        {
            visitedList.Add(node);
            foreach (var sink in relationPropertyFunc.Invoke(node))
            {
                TopologicalSortVisit(sink, nodeList, visitedList, relationPropertyFunc);
            }
            nodeList.AddLast(node);
        }
    }
	*/
	
	/*
	public List<int> TopologicalSort()
	{
	    var startingNodes = Lessons.Where(x => x.Prerequisites.Count == 0).ToList();
	    var sortedNodes = new List<LessonStructureItem>();

	    while (startingNodes.Count > 0)
	    {
	        LessonStructureItem currentNode = startingNodes.First();
	        sortedNodes.Add(currentNode);
	        startingNodes.Remove(currentNode);

	        if (currentNode != null && currentNode.NextLessons != null && currentNode.NextLessons.Count > 0)
	        {
	            foreach (int i in currentNode.NextLessons)
	            {
	                LessonStructureItem currentLesson = Lessons.Find(x => x.LessonId == i);
	                currentLesson.Prerequisites.Remove(currentNode.LessonId);
	                if (currentLesson.Prerequisites.Count == 0)
	                {
	                    startingNodes.Add(Lessons.Find(x => x.LessonId == i));
	                }
	            }
	        }
	    }

	    return sortedNodes.Select(x => x.LessonId).ToList();
	}
	*/
}

