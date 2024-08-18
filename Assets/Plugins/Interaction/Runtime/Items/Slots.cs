using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nothke.Interaction.Items;

namespace Nothke.Interaction
{
    public interface ISlottable
    {
        string SlotTag { get; }

        bool IsSlottable { get; }
        void OnStartedPlacing(IHands hands);
        void OnSlotted();

        IItemReceivable SlottedIn { get; set; }

        /// <summary>
        /// Called before IsSlottable is nulled
        /// </summary>
        void OnRemovedFromSlot();
    }

    public interface IItemReceivable
    {
        string SlotTag { get; }

        bool CanReceive(ISlottable slottable);
        bool SetItemInSlot(ISlottable slottable);
    }
}