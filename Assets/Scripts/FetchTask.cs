using UnityEngine;

[CreateAssetMenu(fileName = "FetchTask", menuName = "Tasks/FetchTask")]
public class FetchTask : GameTask
{
    public FetchObject FetchObject { get; set; }
    public FetchTarget FetchTarget { get; set; }

    public override void Check(float time)
    {
        if (FetchObject == null || FetchTarget == null)
            return;

        base.Check(time);

        if (Status == TaskStatus.InProgress && FetchTarget.IsPositionInside(FetchObject.transform.position))
            Complete();
    }
}
