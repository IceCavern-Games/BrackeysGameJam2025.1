using UnityEngine;

[CreateAssetMenu(fileName = "FetchTask", menuName = "Tasks/FetchTask")]
public class FetchTask : GameTask
{
    [HideInInspector] public FetchObject FetchObject;
    [HideInInspector] public FetchTarget FetchTarget;
    
    public override TaskStatus CheckTask(float time)
    {
        if (time > _deadline)
            FinishTask(false);
        else if (FetchTarget.IsPositionInside(FetchObject.transform.position))
            FinishTask(true);

        return _status;
    }
    
    public override void StartTask()
    {
        base.StartTask();
        FetchTarget.SetEnabled(true);
    }

    public override void FinishTask(bool isSuccess)
    {
        base.FinishTask(isSuccess);
        FetchTarget.SetEnabled(false);
    }
}
