
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RayCastDebug : MonoBehaviour 
{
    public string rayCastDebugName              = "";
    public Color lineColor                      = Color.yellow;
    public bool initialized                     = false;
    public string initialName                   = "RayCast Debug";
    public Vector3 startPoint                   = Vector3.zero;
    public Vector3 endPoint                     = new Vector3(0, 1, 0);
    public float distance                       = 0.0f;
    public float gizmoRadius                    = 0.1f;
    public bool scaleToPixels                   = false;
    public int pixelPerUnit                     = 128;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = this.lineColor;
        Gizmos.DrawWireSphere(startPoint, gizmoRadius);
        Gizmos.DrawWireSphere(endPoint, gizmoRadius);
        Gizmos.DrawLine(startPoint, endPoint);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = this.lineColor;
        Gizmos.DrawWireSphere(startPoint, gizmoRadius);
        Gizmos.DrawWireSphere(endPoint, gizmoRadius);
        Gizmos.DrawLine(startPoint, endPoint);
    }
}


