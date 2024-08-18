#### Simple interaction
- Add `InteractionController` component to your camera
- You must provide an input for the controller, so add `ExampleInteractionControllerInput` to the same object. Implement your own to provide custom input.
- Drop `Example Interaction Canvas.prefab` into the scene to get some UI feedback. If you wish to customize it, make a copy of the prefab and customize as you wish.
- To create an interactable object, create a new script and inherit `Nothke.Interaction.Interactable`. Override `void Use(InteractableController im)` and add `Debug.Log("Hello, interactable!")`. When you click on it, you should get a message in your log.

#### Holding Items
Item handling is a separate system that builds on top of simple interaction. It is managed by the `Hands` component, residing in `Nothke.Interaction.Items` namespace, which manages taking, throwing, interacting with held items etc.
- Add a `Hands` component to where your `InteractionController` is (you can also assign it to the handsComponent property on the InteractionController if you don't want to put it on the same object);
- Now if you create a new script that inherits `Interactable` and also implements `ITakeable`, you will be able to take it;
  - Note that you will have to also implement IDroppable to be able to drop the item

#### Specifying interactable behavior
Specifying interaction on interactables is handled via interfaces. For example `ITakeable` makes the interactable takeable, while `IScrollInteractable` makes it possible to implement value changing when interacting with the mouse scroll wheel. Or you can specify which item can interact with which item by implementing `IItemInHandsInteractionDependable` or `ICanInteract`. See [InteractionInterfaces.cs](Runtime/Core/InteractionInterfaces.cs) for a full list of built-in interaction interfaces.

#### Default implementations
The package also provides a few default implementations of aformentioned interfaces, which can be inherited for a much easier implmentation, such as:
- `GenericItem` which implements holdable item in a single class, behavior of which can be configured in the inspector, or by overriding virtual functions;
- `Slot` provides a default implementation for slotting, that is, an interactable that can accept another item. I.e. an electricity socket which can accept a plug.

So, why is "low level" implmementation provided via interfaces and not via classes like these? Well, because there could be conflicting behaviors, like, what if I want to make a takeable item, which is also a slot? You can totally do that with just implmementing all the necessary interfaces.

#### Interfacing with other game features
InteractionController needs to interact with a few features that depend on your implemention in your project, such as mouse look locking. This functionality is done by providing the InteractionController with interfaces, these are located in [IntegrationInterfaces.cs](Runtime/Integration/IntegrationInterfaces.cs).

#### UI
The package provides an implementation of a simple UI, which shows an interactable's label and also shows a reticle that changes on the item behavior. Best is if you use this as an example of how to implement your own UI.

#### To be changed in the future:
- The project currently uses reticle sprites that are managed by `ReticleUI.cs` and switch according to an enum. It should either be changed to a class that can be extended, or removed completely and user should be encouraged to create their own.