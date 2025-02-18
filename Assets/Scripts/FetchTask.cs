using UnityEngine;

[CreateAssetMenu(fileName = "FetchTask", menuName = "Tasks/FetchTask")]
public class FetchTask : GameTask
{
    [HideInInspector] public FetchObject FetchObject;
    [HideInInspector] public FetchTarget FetchTarget;
    
    public override bool CheckTask()
    {
        return FetchTarget.IsPositionInside(FetchObject.transform.position);
    }
    
    public override void StartTask()
    {
        base.StartTask();
        FetchTarget.SetEnabled(true);
    }

    public override void FinishTask()
    {
        base.FinishTask();
        FetchTarget.SetEnabled(false);
    }
}
