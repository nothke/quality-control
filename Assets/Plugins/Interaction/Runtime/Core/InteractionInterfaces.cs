using UnityEngine;

namespace Nothke.Interaction
{
    public interface IScrollInteractable
    {
        void Scroll(InteractionController im, float scroll);
    }

    [System.Obsolete("Use ICanInteract instead")]
    public interface ISelectiveInteractable
    {
        bool CanInteract { get; }
    }

    public interface ICanInteract
    {
        bool CanInteract(InteractionController controller);
    }
}

namespace Nothke.Interaction.Items
{
    public interface IUsable
    {
        void Use();
    }

    public interface IHoldUsable
    {
        void UseEnd();
    }

    public interface IScrollableInHand
    {
        void ScrollInHand(float scroll);
    }

    public interface ISecondaryUsable
    {
        bool AllowExamination { get; }
        void UseSecondary();
    }

    public interface ITakeable
    {
        Transform Transform { get; }
        Rigidbody Rigidbody { get; }
        void OnTaken(IHands hands);
    }

    public interface ICustomHoldPivot
    {
        void GetHoldPivot(out Vector3 holdPos, out Quaternion holdRot);
    }

    public interface IHands
    {
        Interactable item { get; }
        void Take(ITakeable _item);
        void Drop();
        void DropFixed();
        void UpdateInteraction();
        bool isFixed { get; set; }
        void Place(Vector3 position, Quaternion rotation, Transform relativeParent, IItemReceivable intoSlot = null);
        InteractionController controller { get; set; }
    }

    public interface ISelectiveTakeable
    {
        bool CanBeTaken();
    }

    public interface IDroppable
    {
        Transform Transform { get; }
        Rigidbody Rigidbody { get; }
        void OnDropped(IHands hands);
        bool DelayDrop { get; }
    }

    public interface IThrowable
    {
        Rigidbody Rigidbody { get; }
        bool canThrow { get; }
        float minThrowAcceleration { get; }
        float maxThrowAcceleration { get; }
        void OnDropped(IHands hands);
    }

    public interface INicePlaceable
    {
        void GetPlacePivot(out Vector3 placePos, out Quaternion placeRot);
        void OnStartedPlacing(IHands hands);
        void OnNicePlaced(IHands hands);
    }

    public interface IExaminable
    {
        float HorizontalAngle { get; }
        float VerticalAngle { get; }
    }

    public interface ICustomExaminePosition
    {
        Vector3 ExaminePosition { get; }
    }

    public interface ICustomHandPosition
    {
        Vector3 HandsOffset { get; }
    }

    /// <summary>
    /// Sets position of held hand
    /// </summary>
    public interface IOverrideHandPositon
    {
        Vector3 HandPosition { get; }
    }

    /// <summary>
    /// Adds offset translation to held hand
    /// </summary>
    public interface IOffsetHandPosition
    {
        Vector3 HandPositionOffset { get; }
    }

    /// <summary>
    /// Implement this if you want to prevent held items interacting with other (environment) interactables
    /// </summary>
    public interface IEnvironmentInteractionPreventable
    {
        bool EnvironmentInteractionIsAllowed { get; }
    }

    /// <summary>
    /// Implement this on interactables that you want to be interactable only if the player is holding a certain item
    /// </summary>
    public interface IItemInHandsInteractionDependable
    {
        bool IsInteractableIfHolding(Interactable item);
    }
}