using System;
using System.Linq;
using UnityEngine;

public class UniqueObject : MonoBehaviour
{
    [SerializeField] private string _id;

    public string Id => _id;

    private void ResetId()
    {
        _id = Guid.NewGuid().ToString();
    }

    public static bool IsUnique(string id)
    {
        return Resources.FindObjectsOfTypeAll<UniqueObject>().Count(x => x.Id == id) == 1;
    }

    private void OnValidate()
    {
        // if scene isn't valid, it's probably prefab context
        if (!gameObject.scene.IsValid())
        {
            _id = string.Empty;
            return;
        }

        if (string.IsNullOrEmpty(_id) || !IsUnique(_id))
        {
            ResetId();
        }
    }
}
