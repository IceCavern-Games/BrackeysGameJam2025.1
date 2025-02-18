using System.Collections.Generic;
using UnityEngine;

public class FetchObject : MonoBehaviour
{
    [SerializeField] private List<FetchTask> _gameTasks;

    private void OnEnable()
    {
        foreach (FetchTask gameTask in _gameTasks)
            gameTask.FetchObject = this;
    }
}
