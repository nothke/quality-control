using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nothke.Interaction.Items;
using Nothke.Interaction.UI;

namespace Nothke.Interaction
{
    public class Slot : Interactable, IItemReceivable, IItemInHandsInteractionDependable
    {
        public Transform slotPivot;

        public string slotTag;
        public virtual string SlotTag => slotTag;

        public ISlottable itemInSlot;
        public bool Occupied => itemInSlot != null;

        public override string Label
        {
            get
            {
                if (itemInSlot != null)
                    return (itemInSlot as Interactable).Label;

                return "";
            }
        }

#if UNITY_EDITOR
        private void Awake()
        {
            if (slotPivot.IsNonUniform())
                Debug.LogError("Slot lossy scale is not uniform, this is not allowed.", slotPivot);
        }
#endif

        public virtual bool IsInteractableIfHolding(Interactable item)
        {
            // if occupied and holding an item, not able to interact
            if (item != null && itemInSlot != null)
                return false;

            // if occupied and not holding anything, able to take
            if (item == null && itemInSlot != null)
                return true;

            var slottable = item as ISlottable;

            if (slottable == null) return false;

            if (!slottable.IsSlottable)
                return false;

            if (CanReceive(slottable))
                return true;

            return false;
        }

        public virtual bool CanReceive(ISlottable slottable)
        {
            if (itemInSlot != null) return false;

            Debug.Assert(slottable != null, "CanReceive slottable is null");

            if (slottable.SlotTag == SlotTag)
                return true;
            else return false;
        }

        public override void Use(InteractionController im)
        {
            manager = im;
            SlotOrTake(im);
        }

        public void SlotOrTake(InteractionController im)
        {
            var hands = im.hands;

            if (!hands.item && itemInSlot != null)
            {
                if (IsTakeable)
                    Take();
            }
            else
            if (hands.item && hands.item is ISlottable)
            {
                SlotItemFromHands(hands);
            }
        }

        public virtual bool IsTakeable => true;

        public void Take()
        {
            if (itemInSlot == null)
            {
                Debug.LogError("Attempting to take an item from slot but there is none");
                return;
            }

            var takeable = itemInSlot as ITakeable;

            Debug.Assert(takeable != null, "Item is slottable but not takeable", this);

            OnBeforeTaken();

            manager.hands.Take(takeable);

            itemInSlot.OnRemovedFromSlot();
            itemInSlot.SlottedIn = null;
            itemInSlot = null;

            manager.Recast();
        }

        public void SlotItemFromHands(IHands hands)
        {
            if (CanReceive(hands.item as ISlottable))
            {
                manager.hands.Place(Vector3.zero, Quaternion.identity, slotPivot, this);

                var item = hands.item;

                // if not smooth placement:
                //itemInSlot = item as ISlottable;
                //itemInSlot.SlottedIn = this;
                //itemInSlot.OnSlotted();
                //OnSlot();

                // if smooth placement, hands will callback the slot:
                (item as ISlottable).OnStartedPlacing(hands);
            }
        }

        public bool SetItemInSlot(ISlottable item)
        {
            Debug.Assert(item != null, "ISlottable passed is null");
            //Debug.Log("Slotted " + (item as Component).gameObject.name + " into " + gameObject.name, this);

            if (CanReceive(item))
            {
                var itemT = (item as Component).transform;
                itemT.parent = slotPivot;
                itemT.localPosition = Vector3.zero;
                itemT.localRotation = Quaternion.identity;

                itemInSlot = item;
                item.SlottedIn = this;

                item.OnSlotted();
                OnSlot();
            }
            else
            {
                Debug.LogError("Slot can't receive this item, tags don't match OR already holding item", item as Component);
                return false;
            }

            if (manager)
                manager.Recast();

            return true;
        }

        public ISlottable RemoveItemFromSlot()
        {
            var takeable = itemInSlot as ITakeable;

            OnBeforeTaken();

            var _item = itemInSlot;
            itemInSlot = null;

            (_item as Component).transform.SetParent(null);
            _item.OnRemovedFromSlot();
            _item.SlottedIn = null;

            return _item;
        }

        public virtual void OnBeforeTaken() { }
        public virtual void OnSlot() { }

        public ReticleUI.State GetCustomReticle()
        {
            if (itemInSlot != null)
                return ReticleUI.State.Take;
            else
                return ReticleUI.State.Slot;
        }
    }
}