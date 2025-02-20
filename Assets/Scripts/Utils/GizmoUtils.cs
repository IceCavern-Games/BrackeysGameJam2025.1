using UnityEngine;

public static class GizmoUtils
{
    /// <summary>
    /// Helper method to draw a capsule in Gizmos
    /// </summary>
    public static void DrawWireCapsule(Vector3 start, Vector3 end, float radius)
    {
        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(end, radius);
        Gizmos.DrawLine(start + Vector3.right * radius, end + Vector3.right * radius);
        Gizmos.DrawLine(start - Vector3.right * radius, end - Vector3.right * radius);
        Gizmos.DrawLine(start + Vector3.forward * radius, end + Vector3.forward * radius);
        Gizmos.DrawLine(start - Vector3.forward * radius, end - Vector3.forward * radius);
    }
}
