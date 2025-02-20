using UnityEngine;

[CreateAssetMenu(fileName = "LocationTask", menuName = "Tasks/LocationTask")]
public class LocationTask : GameTask
{
    public LocationTarget LocationTarget { get; set; }

    public override void Check(float time)
    {
        if (LocationTarget == null)
            return;

        base.Check(time);

        if (Status == TaskStatus.InProgress && LocationTarget.PlayerIsIn)
            Complete();
    }
}
