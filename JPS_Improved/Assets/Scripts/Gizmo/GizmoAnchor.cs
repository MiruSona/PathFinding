#if UNITY_EDITOR
using UnityEngine;

namespace FastPathFinder
{
    public enum Type_Gizmo
    {
        Cube, Sphere, WireCube, WireSphere
    }

    public class GizmoAnchor : MonoBehaviour
    {
        [SerializeField] private Type_Gizmo GizmoType = Type_Gizmo.Cube;
        [SerializeField] private Color GizmoColor = Color.red;
        [SerializeField] private Vector3 GizmoSize = Vector3.one;
        [SerializeField] private float GizmoRadius = 1.0f;

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }

        public Vector3 GetLocalPosition()
        {
            return transform.localPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GizmoColor;

            switch (GizmoType)
            {
                case Type_Gizmo.Cube:
                    Gizmos.DrawCube(transform.position, GizmoSize);
                    break;

                case Type_Gizmo.WireCube:
                    Gizmos.DrawWireCube(transform.position, GizmoSize);
                    break;

                case Type_Gizmo.Sphere:
                    Gizmos.DrawSphere(transform.position, GizmoRadius);
                    break;

                case Type_Gizmo.WireSphere:
                    Gizmos.DrawWireSphere(transform.position, GizmoRadius);
                    break;
            }
        }
    }
}
#endif