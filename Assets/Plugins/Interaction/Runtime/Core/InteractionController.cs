//#define NEVER_MOUSE_RAY
#define USE_FIXED_UPDATE

using UnityEngine;
using Nothke.Interaction.Items;
using Nothke.Interaction.Integration;

namespace Nothke.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        Transform originTransform;

        public float interactDistance = 2;

        public bool rayModeToggling = false;
        public bool mouseRay = false;

        public MonoBehaviour lockableFreeLookComponent;
        ILockableFreeLook lockableFreeLook;

        Interactable interactable;
        Interactable lastHovered;

        public Interactable hovered { get; private set; }

        public MonoBehaviour handsComponent;
        public IHands hands;
        public LayerMask raycastLayerMask = -1;

        [HideInInspector]
        public Vector3 startScreenPosition;

        [System.NonSerialized]
        public RaycastHit hit;
        [System.NonSerialized]
        public bool hasHit;

        bool LMB;
        bool LMBup;
        bool rayModeToggleDown;

        bool freezeDetection;
        public bool detectionFrozen => freezeDetection;

        bool isDetached;
        bool wasMouseRayBeforeTemp;

        public delegate void HoverAction(Interactable interactable);
        public event HoverAction OnHover;
        public delegate void DehoverAction();
        public event DehoverAction OnDehover;
        public delegate void RayModeChangeAction(bool enable);
        public event RayModeChangeAction OnRayModeChange;

        public bool debug;

        Camera cam;

        protected Ray interactionRay;

        private void Awake()
        {
            if (handsComponent)
            {
                hands = handsComponent.GetComponent<IHands>();
                hands.controller = this;
                Debug.Assert(hands != null, "Assigned component doesn't implement IHands");
            }
            else
            {
                hands = GetComponent<IHands>();
                if (hands != null)
                    hands.controller = this;
            }
        }

        void Start()
        {
            cam = Camera.main;

            if (lockableFreeLookComponent)
            {
                lockableFreeLook = lockableFreeLookComponent.GetComponent<ILockableFreeLook>();

                if (lockableFreeLook == null)
                    Debug.Log("ILockableFreeLook not found on assigned component", lockableFreeLookComponent);
            }

            originTransform = transform;

            Dehover();

            SetRayMode(mouseRay);
        }

        public void SetTempRayOrigin(Transform t)
        {
            originTransform = t;
        }

        public void ResetTempRayOrigin()
        {
            originTransform = transform;
        }

        public void SetInput(bool interactDown, bool interactUp, bool rayModeChangeDown)
        {
            LMB = interactDown;
            LMBup = interactUp;
            this.rayModeToggleDown = rayModeChangeDown;
        }

        public void UpdateRaycast()
        {
            if (mouseRay)
                interactionRay = cam.ScreenPointToRay(Input.mousePosition);
            else
                interactionRay = new Ray(originTransform.position, originTransform.forward);

            hovered = null;

            // DETECTION - Raycast
            if (!freezeDetection)
            {
                GameObject hito = GetInteractingObject();

                if (hito)
                {
                    hasHit = true;

                    hovered = hito.GetComponentInParent<Interactable>();

                    // Prevent interaction with items that don't want to be interacted with
                    // unless holding a certain item
                    if (hovered is IItemInHandsInteractionDependable)
                    {
                        if (hands == null)
                            hovered = null;
                        else
                        if (!(hovered as IItemInHandsInteractionDependable).IsInteractableIfHolding(hands.item))
                            hovered = null;
                    }

                    // Prevent interaction if item in hands prevents interaction
                    if (hands != null && hands.item && hands.item is IEnvironmentInteractionPreventable eip)
                    {
                        if (!eip.EnvironmentInteractionIsAllowed)
                            hovered = null;
                    }

#pragma warning disable CS0618 // Type or member is obsolete
                    if (hovered is ISelectiveInteractable)
                    {
                        if (!(hovered as ISelectiveInteractable).CanInteract)
                            hovered = null;
                    }
#pragma warning restore CS0618 // Type or member is obsolete

                    if (hovered is ICanInteract)
                    {
                        if (!(hovered as ICanInteract).CanInteract(this))
                            hovered = null;
                    }

                    if (hovered != lastHovered && hovered != null)
                        Hover(hovered);
                }
                else hasHit = false;

                if (hovered != lastHovered && hovered == null)
                    Dehover();
            }
        }

        /// <summary>
        /// Override this to provide a custom raycaster for example. You can get the default provided ray from interactionRay.
        /// </summary>
        protected virtual GameObject GetInteractingObject()
        {
            if (Physics.Raycast(interactionRay, out hit, interactDistance, raycastLayerMask))
                return hit.collider.gameObject;
            else return null;
        }

        /// <summary>
        /// Call this after you set input. Should be called in Update()
        /// </summary>
        public void UpdateInput()
        {
            // On click
            bool interactedThisFrame = false;
            if (hovered && LMB)
            {
                interactable = hovered;

                if (hands != null && interactable is ITakeable)
                {
                    if (interactable is ISelectiveTakeable)
                    {
                        if ((interactable as ISelectiveTakeable).CanBeTaken())
                            hands.Take(interactable as ITakeable);
                    }
                    else
                        hands.Take(interactable as ITakeable);
                }

                interactable.Use(this);
                interactable.StartHold();
                interactedThisFrame = true;

                startScreenPosition = Input.mousePosition;
            }

            // On scroll
            if (hovered && hovered is IScrollInteractable scrollInteractable)
            {
                float scroll = Input.mouseScrollDelta.y;

                if (scroll != 0)
                    scrollInteractable.Scroll(this, scroll);
            }

            // On release
            if (interactable && LMBup)
            {
                interactable.EndHold();
                interactable = null;
            }

            lastHovered = hovered;

            LMB = false;
            LMBup = false;

            if (!interactedThisFrame && hands != null)
                hands.UpdateInteraction();
        }

        public void Recast()
        {
            hovered = null;
        }

        void ToggleRayMode()
        {
            mouseRay = !mouseRay;

            SetRayMode(mouseRay);
        }

        public void FreezeDetection()
        {
            freezeDetection = true;
            hovered = null;
        }

        public void UnfreezeDetection()
        {
            freezeDetection = false;
            Dehover();
            Recast();
        }

        public void SetTempMouseRay(bool tempMouse)
        {
            if (tempMouse)
            {
                wasMouseRayBeforeTemp = mouseRay;
                SetRayMode(true);
            }
            else
            {
                if (!wasMouseRayBeforeTemp)
                {
                    SetRayMode(false);
                }
            }
        }

        public void SetRayMode(bool isMouse)
        {
            mouseRay = isMouse;

#if !NEVER_MOUSE_RAY
            if (mouseRay)
            {
                Cursor.lockState = CursorLockMode.None;

                // TODO: if windows
                //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(10, 10);
            }
            else
            {
                //Cursor.lockState = CursorLockMode.Locked;
                //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Screen.width / 2, Screen.height / 2);
            }
#endif

            if (lockableFreeLook != null)
                lockableFreeLook.LockFreeLook(mouseRay);
            else
                Debug.LogWarning("No freelook lockable");

            OnRayModeChange?.Invoke(mouseRay);

            if (debug)
                Debug.Log("Ray mode changed to " + (mouseRay ? " Mouse" : " Center"));

            //Cursor.visible = mouseRay;

            if (hands != null)
                hands.isFixed = mouseRay;
        }

        void Hover(Interactable interactable)
        {
            interactable.OnHover();
            OnHover?.Invoke(interactable);

            if (debug)
                Debug.Log("Hovered " + interactable.name);
        }

        void Dehover()
        {
            if (lastHovered)
                lastHovered.OnDehover();

            OnDehover?.Invoke();

            if (debug)
                Debug.Log("Dehovered");
        }

        public void LockFreeLook(bool b)
        {
            if (lockableFreeLook != null)
                lockableFreeLook.LockFreeLook(b);
        }
    }
}