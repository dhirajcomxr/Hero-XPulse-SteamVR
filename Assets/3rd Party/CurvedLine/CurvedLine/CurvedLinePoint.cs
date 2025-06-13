using UnityEngine;

[RequireComponent(typeof(ConnectedObjects))]
public class CurvedLinePoint : MonoBehaviour {

    [HideInInspector]
    public float gizmoSize = 0.05f;
    [HideInInspector]
    public Color gizmoColor = new Color(1, 0, 0, 0.5f);

    void OnDrawGizmos() {
        //Gizmos.color = gizmoColor;
        //Gizmos.DrawSphere(this.transform.position, gizmoSize);
    }
}
