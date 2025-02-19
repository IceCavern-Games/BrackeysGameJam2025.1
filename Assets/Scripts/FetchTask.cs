using UnityEngine;

[CreateAssetMenu(fileName = "FetchTask", menuName = "Tasks/FetchTask")]
public class FetchTask : GameTask
{
    [HideInInspector] public FetchObject FetchObject;
    [HideInInspector] public FetchTarget FetchTarget;

    public override void Check(float time)
    {
        base.Check(time);

        if (FetchTarget == null)
            return;

        if (FetchTarget.IsPositionInside(FetchObject.transform.position))
            Finish();
    }

    public override void Start()
    {
        base.Start();
        FetchTarget.SetEnabled(true);
    }

    public override void Fail()
    {
        base.Fail();
        FetchTarget.SetEnabled(false);
    }

    public override void Finish()
    {
        base.Finish();
        FetchTarget.SetEnabled(false);
    }
}
