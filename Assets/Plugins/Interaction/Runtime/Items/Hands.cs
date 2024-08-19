using System.Collections.Generic;
using Nothke.Interaction.Integration;
using UnityEngine;
using UnityEngine.UI;

namespace Nothke.Interaction.Items
{
    public class Hands : MonoBehaviour, IHands
    {
        #region Inspector variables

        public Transform hand;

        public Vector3 handAimPos = new Vector3(0, 0, 0.8f);

        public float examinationMouseSensitivity = 1;

        [Header("Smooth taking")]
        public bool smoothTake;
        public float smoothMoveFactor = 0.1f;
        public float smoothRotateRate = 10;

        [Header("Throwing")]
        public bool throwFromCenter = true;
        public float throwUpFactor = 0.2f;
        public float throwAngularVelocity = 10;
        public Transform throwTransform;

        public float handNoiseGain = 1;

        [System.NonSerialized] public MonoBehaviour[] mouseLooks; // TODO: Remove
        public MonoBehaviour lockableFreeLookComponent;
        ILockableFreeLook lockableFreeLook;
        public MonoBehaviour focusEffectComponent;
        IFocusableEffect focusableEffect;

        public bool nicePlacement;

        public bool disableShadowsOnTakenObjects = true;

        #endregion

        #region Public properties

        public bool isFixed { get; set; }
        [System.NonSerialized] public Interactable item;

        #endregion

        #region Private variables

        const float smoothPlaceTimeLimit = 1;

        Transform itemInHands;

        Vector3 handStartPos;
        Vector3 handTargetPos;
        Quaternion handStartRot;
        Quaternion handTargetRot;

        RaycastHit hit;

        Vector3 mouseSpeed;
        Vector3 handRefVelo;

#if FOV
        float originalFoV;
        float FoVRefVelo;
#endif

        Interactable IHands.item => item;
        public InteractionController controller { get; set; }

        Vector3 placeTLocalPos;
        Quaternion placeTLocalRot;
        Vector3 placeHandOffset;
        Quaternion placeHandRotation;
        Transform placeT;
        bool placing;
        bool placingIsNice;
        float placeStartTime;

        Transform handParent;

        #endregion

        #region Public methods

        public void Take(ITakeable _item)
        {
            if (itemInHands) return;

            item = _item as Interactable;
            item.manager = controller;

            if (_item.Rigidbody)
                _item.Rigidbody.isKinematic = true;

            var itemT = _item.Transform;

            Vector3 targetHoldPos = Vector3.zero;
            Quaternion targetHoldRot = Quaternion.identity;

            if (item is ICustomHoldPivot)
                (item as ICustomHoldPivot).GetHoldPivot(out targetHoldPos, out targetHoldRot);

            if (smoothTake)
            {
                hand.transform.SetPositionAndRotation(
                    itemT.TransformPoint(targetHoldPos),
                    itemT.rotation * targetHoldRot);
            }

            itemT.parent = hand;

            if (!smoothTake)
            {
                itemT.localPosition = -targetHoldPos;
                itemT.localRotation = Quaternion.Inverse(targetHoldRot);
            }

            if (disableShadowsOnTakenObjects)
                EnableShadowcasting(item, true);

            itemInHands = _item.Transform;

            _item.OnTaken(this);
        }

        public void Drop()
        {
            if (!itemInHands) return;

            var droppable = item as IDroppable;

            if (droppable == null) Debug.LogError("Attempting to drop undroppable item");

            Debug.Assert(droppable != null, "Droppable is null");
            //Debug.Assert(droppable.Rigidbody != null, "Item rigidbody is null");

            if (droppable.Rigidbody)
                droppable.Rigidbody.isKinematic = false;

            itemInHands.parent = null;

            if (disableShadowsOnTakenObjects)
                EnableShadowcasting(item, true);

            Debug.Log($"Dropped {item.name}");

            item.manager = null;

            itemInHands = null;
            item = null;
        }

        public void DropFixed()
        {
            if (!itemInHands) return;

            itemInHands.parent = null;

            if (disableShadowsOnTakenObjects)
                EnableShadowcasting(item, true);

            Debug.Log($"Dropped fixed {item.name}");

            if (item is IDroppable droppable)
                droppable.OnDropped(this);

            itemInHands = null;
            item = null;
        }

        void EnableShadowcasting(Interactable item, bool enable)
        {
            Renderer[] renderers = item.transform.GetComponentsInChildren<Renderer>(); // alloc!
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].shadowCastingMode =
                    enable ?
                    UnityEngine.Rendering.ShadowCastingMode.On :
                    UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        IItemReceivable placingIntoSlot;

        public void Place(Vector3 position, Quaternion rotation, Transform relativeParent, IItemReceivable intoSlot = null)
        {
            placeT = relativeParent;
            placeTLocalPos = position;
            placeTLocalRot = rotation;
            placing = true;

            if (item is ICustomHoldPivot)
                (item as ICustomHoldPivot).GetHoldPivot(out placeHandOffset, out placeHandRotation);
            else
            {
                placeHandOffset = Vector3.zero;
                placeHandRotation = Quaternion.identity;
            }

            if (item is INicePlaceable placeable)
                placeable.OnStartedPlacing(this);
            else if (item is ISlottable slottable)
                slottable.OnStartedPlacing(this);

            placingIsNice = placeT == null;
            placingIntoSlot = intoSlot;

            // TODO: Add movement velocity to smoothing speed

            placeStartTime = Time.time;

            hand.parent = null;
        }

        public void OverrideHandPositionAndRotation(Vector3 pos, Quaternion rot)
        {
            handTargetPos = pos;
            handTargetRot = rot;
        }

        public void SetHandVelocity(Vector3 velo)
        {
            handRefVelo = velo;
        }

        public void ResetOffset()
        {
            handTargetPos = handStartPos;
            handTargetRot = handStartRot;
        }

        #endregion

        void Awake()
        {
            if (hand == null)
                hand = transform;

            handStartPos = hand.transform.localPosition;
            handTargetPos = handStartPos;
            handStartRot = hand.transform.localRotation;
            handTargetRot = handStartRot;

            if (lockableFreeLookComponent)
            {
                lockableFreeLook = lockableFreeLookComponent.GetComponent<ILockableFreeLook>();
                if (lockableFreeLook != null)
                    Debug.Log("Found lockable free look on GameObject");
            }

            if (focusEffectComponent)
            {
                focusableEffect = focusEffectComponent.GetComponent<IFocusableEffect>();
                if (focusableEffect != null)
                    Debug.Log("Found focusable effect on GameObject");
            }

            handParent = hand.parent;
        }

        #region Update

        private void Update()
        {
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            // sanity check
            if (item == null)
                if (placing)
                {
                    this.EndPlacing();
                }

            float dt = Time.deltaTime;
            mouseSpeed = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

            Vector3 targetPos = handTargetPos;

            if (item is IOverrideHandPositon ohp)
            {
                targetPos = ohp.HandPosition;
            }

            if (item is IOffsetHandPosition offhp)
            {
                targetPos += offhp.HandPositionOffset;
            }

            float lrRot = (-0.5f + Mathf.PerlinNoise(Time.time * 3.34f, 0.234f)) * 5 * handNoiseGain;
            float udRot = (-0.5f + Mathf.PerlinNoise(Time.time * 3.34f, 34.783f)) * 5 * handNoiseGain;

            Quaternion targetRot = handTargetRot * Quaternion.Euler(lrRot, udRot, 0);

            if (placing)
            {
                // Get hold pivot offset and rotation
                Vector3 holdPos = Vector3.zero;
                Quaternion holdRot = Quaternion.identity;
                if (item is ICustomHoldPivot chp)
                {
                    chp.GetHoldPivot(out holdPos, out holdRot);
                    float slotScale = placeT ? 1.0f / placeT.lossyScale.x : 1;
                    holdPos = holdPos * item.transform.lossyScale.x * slotScale;
                }

                Vector3 placeWorldPos;
                Quaternion placeWorldRot;

                if (!placeT) // calcaulte in world space
                {
                    Matrix4x4 worldLocationMatrix = Matrix4x4.TRS(placeTLocalPos, placeTLocalRot, Vector3.one);
                    Matrix4x4 handPivotMatrix = Matrix4x4.TRS(holdPos, holdRot, Vector3.one);
                    Vector3 posOff = worldLocationMatrix * holdPos;

                    //var mat = handPivotMatrix * worldLocationMatrix;
                    placeWorldPos = placeTLocalPos + (Vector3)(worldLocationMatrix * holdPos);
                    placeWorldRot = placeTLocalRot * holdRot;
                    //placeWorldRot = mat.rotation;
                }
                else // if placeT, calculate in local space of target object
                {
                    placeWorldPos = placeT.TransformPoint(placeTLocalPos + holdPos);
                    placeWorldRot = placeT.rotation * placeTLocalRot * holdRot;
                }

                // Calculate targets in world space
                targetPos = placeWorldPos;
                targetRot = placeWorldRot;

                bool isCloseToTarget = Vector3.Distance(hand.position, targetPos) < 0.02f;
                bool isAngleCloseToTarget = Quaternion.Angle(hand.rotation, targetRot) < 1;
                bool isSafetyTimeout = Time.time - placeStartTime > smoothPlaceTimeLimit;

                // END PLACING
                if ((isCloseToTarget && isAngleCloseToTarget) || isSafetyTimeout)
                {
                    EndPlacing();
                    return;
                }
            }

            // Assign hand position and rotation:
            if (placing) // while placing is happening, lerping is in world space, otherwise it should be in localspace
            {
                hand.position = Vector3.SmoothDamp(hand.position, targetPos, ref handRefVelo, smoothMoveFactor);
                hand.rotation = Quaternion.Slerp(hand.rotation, targetRot, dt * smoothRotateRate);
            }
            else
            {
                hand.localPosition = Vector3.SmoothDamp(hand.localPosition, targetPos, ref handRefVelo, smoothMoveFactor);
                hand.localRotation = Quaternion.Slerp(hand.localRotation, targetRot, dt * smoothRotateRate);
            }

            if (examining)
            {
                if (itemInHands)
                {
                    float x = mouseSpeed.x * examinationMouseSensitivity;
                    float y = mouseSpeed.y * examinationMouseSensitivity;

                    IExaminable examinable = item as IExaminable;

                    if (examinable != null)
                    {
                        if (examinable.HorizontalAngle > 0)
                            hand.Rotate(hand.transform.parent.up, x, Space.World);

                        if (examinable.VerticalAngle > 0)
                            hand.Rotate(hand.transform.parent.right, y, Space.World);
                    }
                }
            }
        }

        void EndPlacing()
        {
            placing = false;

            // Temp, make it get velocity from target:
            handRefVelo = Vector3.zero;

            var _item = itemInHands;
            if (_item != null)
            {
                DropFixed();
                _item.parent = placeT;
                _item.localPosition = placeTLocalPos;
                _item.localRotation = placeTLocalRot;
            }
            else Debug.LogError("Ending placement but item was null! This shouldn't happen");

            if (placingIsNice && _item.GetComponent<Interactable>() is INicePlaceable placeable)
                placeable.OnNicePlaced(this);

            //Debug.Log("Ending placeming");
            if (placingIntoSlot != null)
                Debug.Log("PLACINGINTOSLOT");

            if (placingIntoSlot != null && _item.GetComponent<ISlottable>() != null)
                placingIntoSlot.SetItemInSlot(_item.GetComponent<ISlottable>());

            placeT = null;
            placingIsNice = false;

            hand.parent = handParent;
        }

        //bool gunning = false;
        //public Transform gunningThing;

        public struct HandsInput
        {
            public bool useDown;
            public bool useUp;
            public bool examineDown;
            public bool examineUp;
            public bool throwDown;
            public bool throwUp;
            public bool dropDown;
            public bool placeDown;
        }

        HandsInput input;

        public void SetInput(HandsInput input)
        {
            this.input = input;
        }

        public void UpdateInteraction()
        {
            #region Unused
            /*
            if (input.examineDown)
            {
                if (!gunning)
                {
                    gunning = true;
                    OverrideHandPositionAndRotation(Vector3.forward * 0.3f, Quaternion.LookRotation(-Vector3.right + Vector3.forward * 0.4f));
                    GetComponent<InteractionController>().SetRayMode(true);
                }
                else
                {
                    gunning = false;
                    ResetOffset();
                    GetComponent<InteractionController>().SetRayMode(false);
                }
            }

            gunningThing.localPosition = Vector3.Lerp(gunningThing.localPosition, !gunning ? Vector3.down : Vector3.zero, Time.deltaTime * 10);
            */

            // RMB
            /*
            if (input.useDown)
            {
                if (item is ISecondaryUsable usable)
                {
                    usable.UseSecondary();

                    if (usable.AllowExamination)
                        StartAim();
                }
                else if (item is IExaminable examinable)
                {
                    StartAim();
                }
            }*/
            #endregion

            if (placing)
                return;

            // RMB up
            if (input.examineUp)
            {
                EndAim();
            }
            if (input.useDown)
            {
                if (item is IUsable usable)
                    usable.Use();
            }

            if (input.useUp && item is IHoldUsable holdUsable)
            {
                holdUsable.UseEnd();
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                if (item is IScrollableInHand scrollable)
                    scrollable.ScrollInHand(Input.mouseScrollDelta.y);
            }

            if (input.dropDown && item is IDroppable)
            {
                IDroppable droppable = item as IDroppable;
                droppable.OnDropped(this);

                if (!droppable.DelayDrop)
                    Drop();
            }

            if (input.throwDown && item is IThrowable)
            {
                IThrowable throwable = item as IThrowable;

                if (throwFromCenter)
                {
                    hand.localPosition = new Vector3(0, 0, hand.localPosition.z);

                    if (throwTransform)
                        hand.position = throwTransform.position;
                }

                Drop();
                throwable.OnDropped(this);

                Vector3 throwFrw = throwTransform ? throwTransform.forward : transform.forward;

                float throwSpeed = throwable.minThrowAcceleration;
                Vector3 force = throwFrw * throwSpeed + Vector3.up * throwUpFactor;

                throwable.Rigidbody.AddForce(force, ForceMode.VelocityChange);
                throwable.Rigidbody.AddTorque(transform.forward * throwAngularVelocity, ForceMode.VelocityChange);
            }
        }

        #endregion

        #region Aiming

        bool examining;

        void StartAim()
        {
            examining = true;

            if (item && item is ICustomExaminePosition cep)
                handTargetPos = cep.ExaminePosition;
            else
                handTargetPos = handAimPos;

            handTargetRot = Quaternion.identity;

            LockFreeLook(true);
            focusableEffect?.FocusEffect(true, handTargetPos.z);
        }

        void EndAim()
        {
            examining = false;

            if (item && item is ICustomHandPosition chp)
                handTargetPos = chp.HandsOffset;
            else
                handTargetPos = handStartPos;

            handTargetRot = handStartRot;

            LockFreeLook(false);
            focusableEffect?.FocusEffect(false, 1000);
        }

        public void LockFreeLook(bool b)
        {
            if (mouseLooks != null)
                for (int i = 0; i < mouseLooks.Length; i++)
                    mouseLooks[i].enabled = !b;

            lockableFreeLook?.LockFreeLook(b);
        }

        #endregion
    }
}