using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nothke.Interaction
{
    public static class InteractableUtils
    {

        public static Vector3 GetMousePointOnPlane(Plane plane)
        {
            Vector3 screenPoint = Input.mousePosition;

            Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float e = 0;
            if (plane.Raycast(screenRay, out e))
                screenPoint.z = e;

            return screenRay.GetPoint(e);
        }

        public static Vector3 GetJointAnchorInWorldSpace(Joint joint)
        {
            if (!joint.connectedBody) return joint.connectedAnchor;

            return joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
        }

        public static bool IsNonUniform(this Transform transform)
        {
            Vector3 ls = transform.lossyScale;
            return !(Mathf.Approximately(ls.x, ls.y) && Mathf.Approximately(ls.y, ls.z));
        }
    }
}