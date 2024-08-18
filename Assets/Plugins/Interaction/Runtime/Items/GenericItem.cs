using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nothke.Interaction.Items
{
    public class GenericItem : Interactable,
        ITakeable, IDroppable, IThrowable, INicePlaceable,
        IExaminable, ICustomHoldPivot
    {
        [System.Serializable]
        public class ItemSettings
        {
            //public Vector3 customHoldPosition = new Vector3(0.15f, -0.15f, 0.1f);
            public Transform holdPivot;

            public Collider[] collidersToDisable;
            public bool autoPopulateColliderListFromChildren;

            public bool useCustomExaminePosition;
            public Vector3 customExaminePosition = new Vector3(0, 0, 0.15f);
            public float examineHorizontalAngle = 90;
            public float examineVerticalAngle = 90;

            public bool canBeThrown = true;
            public float minThrowVelocity = 5;
            public float maxThrowVelocity = 10;

            public Transform nicePlacePivot;
        }

        public ItemSettings itemSettings;

        public Transform Transform => transform;
        public Rigidbody Rigidbody => rb;

        public bool DelayDrop => false;

        public float HorizontalAngle => itemSettings.examineHorizontalAngle;
        public float VerticalAngle => itemSettings.examineVerticalAngle;

        protected Rigidbody rb;
        Collider col;

        public bool canThrow => itemSettings.canBeThrown;
        public float minThrowAcceleration => itemSettings.minThrowVelocity;
        public float maxThrowAcceleration => itemSettings.maxThrowVelocity;

        protected virtual void Awake()
        {
            rb = GetComponentInParent<Rigidbody>();
            col = GetComponent<Collider>();

            Debug.Assert(rb, "GenericItem has no Rigidbody", this);

            if ((!itemSettings.holdPivot && transform.IsNonUniform()) ||
                (itemSettings.holdPivot && itemSettings.holdPivot.IsNonUniform()))
                Debug.LogError("GenericItem's scale (or its hold pivot's scale) is non-uniform, this is not allowed. Placement will be skewed.", this);
        }

        private void OnValidate()
        {
            if (itemSettings == null)
                itemSettings = new ItemSettings();

            if (itemSettings.autoPopulateColliderListFromChildren)
            {
                itemSettings.collidersToDisable = GetComponentsInChildren<Collider>(true);
            }
        }

        public virtual void OnDropped(IHands hands)
        {
            SetCollisions(true);
            FixRigidbody(false);
        }

        public virtual void OnTaken(IHands hands)
        {
            SetCollisions(false);
            FixRigidbody(true);
        }

        public virtual void OnStartedPlacing(IHands hands)
        {

        }

        public virtual void OnNicePlaced(IHands hands)
        {
            SetCollisions(true);
            rb.isKinematic = false;
        }

        protected void SetCollisions(bool enable)
        {
            if (col)
                col.enabled = enable;

            foreach (Collider col in itemSettings.collidersToDisable)
                col.enabled = enable;
        }

        public void FixRigidbody(bool fix)
        {
            rb.interpolation = fix ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            rb.isKinematic = fix;
        }

        public void GetHoldPivot(out Vector3 holdPos, out Quaternion holdRot)
        {
            if (!itemSettings.holdPivot)
            {
                holdPos = Vector3.zero;
                holdRot = Quaternion.identity;
            }
            else
            {
                holdPos = itemSettings.holdPivot.localPosition;
                holdRot = itemSettings.holdPivot.localRotation;
            }
        }

        public void GetPlacePivot(out Vector3 placePos, out Quaternion placeRot)
        {
            if (!itemSettings.nicePlacePivot)
            {
                Bounds bounds = Utils.BoundsUtils.GetObjectSpaceColliderBounds(gameObject, true);
                placePos = bounds.center;
                placePos.y = bounds.min.y;
                placeRot = Quaternion.identity;
                //Debug.Log("Got bounds offset: " + placePos.y);
            }
            else
            {
                placePos = itemSettings.nicePlacePivot.localPosition;
                placeRot = itemSettings.nicePlacePivot.localRotation;
            }
        }
    }
}