using System.Collections.Generic;
using UnityEngine;

public class FetchObject : MonoBehaviour
{
    [SerializeField] private List<GameTask> _gameTasks;

    private void OnEnable()
    {
        foreach (GameTask gameTask in _gameTasks)
            gameTask.FetchObject = this;
    }
}
