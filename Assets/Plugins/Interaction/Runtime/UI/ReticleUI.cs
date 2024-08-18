using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Nothke.Interaction.UI
{
    public class ReticleUI : MonoBehaviour
    {
        static ReticleUI e;
        private void Awake() { e = this; }

        public bool showIconForNone;

        public Texture icon_none;
        public Texture icon_generic;
        public Texture icon_grab;
        public Texture icon_hold;
        public Texture icon_take;
        public Texture icon_slot;

        public Texture icon_ride;
        public Texture icon_scroll;
        public Texture icon_grab_knob;
        public Texture icon_hold_knob;

        public RawImage image;

        public enum State { None, Idle, Grab, Hold, Take, Slot, Ride, Scroll, GrabKnob, HoldKnob }
        public static State state;
        static State lastState;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private void Init()
        {
            state = State.None;
            lastState = State.None;
        }

        public static void Hide()
        {
            state = State.None;
            e.SetState();
        }

        public static void ResetReticlePosition()
        {
            if (!e) return;
            RectTransform t = e.image.GetComponent<RectTransform>();
            t.anchoredPosition = Vector2.zero;
        }

        public static void SetReticlePosition(Vector2 screenPos)
        {
            if (!e) return;
            RectTransform t = e.image.GetComponent<RectTransform>();
            t.position = screenPos;
        }

        private void Start()
        {
            SetState();
        }

        void Update()
        {
            if (state != lastState)
                SetState();

            lastState = state;
        }

        void SetState()
        {
            if (!showIconForNone)
                image.enabled = state != State.None;

            switch (state)
            {
                case State.None: image.texture = icon_none; break;
                case State.Idle: image.texture = icon_generic; break;
                case State.Grab: image.texture = icon_grab; break;
                case State.Hold: image.texture = icon_hold; break;
                case State.Take: image.texture = icon_take; break;
                case State.Slot: image.texture = icon_slot; break;

                case State.Ride: image.texture = icon_ride; break;
                case State.Scroll: image.texture = icon_scroll; break;
                case State.GrabKnob: image.texture = icon_grab_knob; break;
                case State.HoldKnob: image.texture = icon_hold_knob; break;
            }
        }
    }
}