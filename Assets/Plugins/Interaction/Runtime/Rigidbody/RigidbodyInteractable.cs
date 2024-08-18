using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nothke.Interaction.Items
{
    public class RigidbodyInteractable : Interactable
    {
        public Rigidbody rb;
        public bool rotateAroundPoint;

        [System.NonSerialized]
        public bool held;

        private void Awake()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();
        }

        public override void Use(InteractionController im)
        {
            base.Use(im);

            DragRigidbody.e.Attach(im.hit, !rotateAroundPoint);
            DragRigidbody.e.OnSlipped += Unhold;
        }

        public override void StartHold()
        {
            base.StartHold();
            manager.FreezeDetection();

            held = true;

            OnStartedHold();
        }

        public override void EndHold()
        {
            base.EndHold();

            if (!held)
                return;

            DragRigidbody.e.End();

            Unhold();
        }

        void Unhold()
        {
            manager.UnfreezeDetection();
            manager.LockFreeLook(false);
            DragRigidbody.e.OnSlipped -= Unhold;
            held = false;
            OnEndedHold();
        }

        protected virtual void OnStartedHold() { }
        protected virtual void OnEndedHold() { }
    }
}