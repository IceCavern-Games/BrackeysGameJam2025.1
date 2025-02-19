using UnityEngine;

public static class ScriptableObjectUtils
{
    public static T Clone<T>(this T original) where T : ScriptableObject
    {
        return Object.Instantiate(original);
    }
}
