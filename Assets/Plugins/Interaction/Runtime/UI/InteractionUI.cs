#define CENTER_TEXT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CENTER_TEXT
using TMPro;
#endif

using Nothke.Interaction.Items;

namespace Nothke.Interaction.UI
{
    public interface ICustomReticle
    {
        ReticleUI.State GetCustomReticle();
    }

    public interface IReticleUIOverridable
    {
        bool OverrideUI { get; }
        bool ReleaseInteractionBlocking { get; }
    }

    public class InteractionUI : MonoBehaviour
    {
        public static InteractionUI e;
        private void Awake()
        {
            e = this;

            controller = GetComponent<InteractionController>();
            if (!controller) controller = FindObjectOfType<InteractionController>();
            if (!controller)
            {
                Debug.LogError("No controller found");
                enabled = false;
            }
        }

#if CENTER_TEXT
        public TMP_Text centerText;
#endif

        InteractionController controller;

        bool mouseRay;

        RigidbodyInteractable riHovered;
        RigidbodyInteractable ri;

        IReticleUIOverridable overridable;

        bool ovrr;

        private void OnEnable()
        {
            controller.OnHover += OnHover;
            controller.OnDehover += OnDehover;
            controller.OnRayModeChange += OnRayModeChange;
        }

        private void OnDisable()
        {
            controller.OnHover -= OnHover;
            controller.OnDehover -= OnDehover;
            controller.OnRayModeChange += OnRayModeChange;
        }

#if CENTER_TEXT
        public void SetText(string description)
        {
            centerText.enabled = true;
            centerText.text = description;
        }

        public void HideText()
        {
            centerText.enabled = false;
        }
#endif

        protected virtual void OnHover(Interactable interactable)
        {
            if (ovrr) return;

#if CENTER_TEXT
            if (centerText)
            {
                SetText(interactable.Label);
            }
#endif

            // Interactable icon choice

            if (interactable is ICustomReticle)
            {
                ReticleUI.state = (interactable as ICustomReticle).GetCustomReticle();
            }
            else if (interactable is ITakeable)
            {
                ReticleUI.state = ReticleUI.State.Take;
            }
            else if (interactable is RigidbodyInteractable)
            {
                ReticleUI.state = ReticleUI.State.Grab;

                riHovered = interactable as RigidbodyInteractable;
            }
            else
            {
                ReticleUI.state = ReticleUI.State.Idle;
            }

            if (interactable is IReticleUIOverridable)
            {
                overridable = interactable as IReticleUIOverridable;
            }
        }

        private void Update()
        {
            if (overridable != null)
            {
                ovrr = overridable.OverrideUI;
                if (ovrr == false)
                {
                    OnDehover();
                    overridable = null;
                }
            }

            if (riHovered && riHovered.held && ri == null)
            {
                ri = riHovered;
                ovrr = true;
                ReticleUI.state = ReticleUI.State.Hold;
            }

            if (ri && !ri.held)
            {
                ovrr = false;
                OnDehover();

                if (controller.hovered)
                    OnHover(controller.hovered);

                ri = null;
            }

            if (controller.mouseRay)
            {
                ReticleUI.SetReticlePosition(Input.mousePosition);
            }
        }

        protected virtual void OnDehover()
        {
            if (ovrr) return;

#if CENTER_TEXT
            if (centerText)
                centerText.enabled = false;
#endif

            ReticleUI.state = ReticleUI.State.None;
        }

        protected virtual void OnRayModeChange(bool enable)
        {
            if (!enable)
                ReticleUI.ResetReticlePosition();
        }
    }
}