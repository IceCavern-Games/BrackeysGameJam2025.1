using UnityEngine;

[CreateAssetMenu(fileName = "Task", menuName = "Scriptable Objects/Task")]
public class GameTask : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
}
