using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public static class UIUtils
{
    /// <summary>
    /// Queries a single element based on a selector string.
    /// Supports querying by name (#element-name) and/or classes (.class-name).
    /// </summary>
    public static T QueryElement<T>(VisualElement root, string selector) where T : VisualElement
    {
        if (string.IsNullOrEmpty(selector))
            return root.Q<T>();

        string elementName = null;
        List<string> classNames = new();

        foreach (var part in selector.Split(' '))
        {
            if (part.StartsWith("#"))
                elementName = part[1..]; // Remove '#'
            else if (part.StartsWith("."))
                classNames.Add(part[1..]); // Remove '.'
        }

        var potentialElements = !string.IsNullOrEmpty(elementName)
            ? root.Query<T>(name: elementName).ToList()
            : root.Query<T>().ToList();

        return potentialElements.FirstOrDefault(el => classNames.All(cls => el.ClassListContains(cls)));
    }

    /// <summary>
    /// Queries multiple elements based on a selector string.
    /// Supports querying by name (#element-name) and/or classes (.class-name).
    /// </summary>
    public static List<T> QueryElements<T>(VisualElement root, string selector) where T : VisualElement
    {
        if (string.IsNullOrEmpty(selector))
            return root.Query<T>().ToList();

        string elementName = null;
        List<string> classNames = new();

        foreach (var part in selector.Split(' '))
        {
            if (part.StartsWith("#"))
                elementName = part[1..]; // Remove '#'
            else if (part.StartsWith("."))
                classNames.Add(part[1..]); // Remove '.'
        }

        var potentialElements = !string.IsNullOrEmpty(elementName)
            ? root.Query<T>(name: elementName).ToList()
            : root.Query<T>().ToList();

        return potentialElements.Where(el => classNames.All(cls => el.ClassListContains(cls))).ToList();
    }
}
