using UnityEngine;

namespace Nothke.Interaction.Example
{
    public class ExampleInteractionControllerInput : MonoBehaviour
    {
        public InteractionController controller;
        public Items.Hands hands;

        public int interactMouseButton = 0;
        public int rayModeChangeMouseButton = 1;

        public KeyCode dropKey = KeyCode.Q;
        public KeyCode throwKey = KeyCode.F;
        public KeyCode examineKey = KeyCode.E;
        public KeyCode placeKey = KeyCode.T;

        private void Update()
        {
            controller.SetInput(
                Input.GetMouseButtonDown(interactMouseButton),
                Input.GetMouseButtonUp(interactMouseButton),
                Input.GetMouseButtonDown(rayModeChangeMouseButton));

            if (hands)
            {
                hands.SetInput(new Items.Hands.HandsInput()
                {
                    useDown = Input.GetMouseButtonDown(0),
                    useUp = Input.GetMouseButtonUp(0),
                    dropDown = Input.GetKeyDown(dropKey),
                    throwDown = Input.GetKeyDown(throwKey),
                    throwUp = Input.GetKeyUp(throwKey),
                    examineDown = Input.GetKeyDown(examineKey),
                    examineUp = Input.GetKeyUp(examineKey),
                    placeDown = Input.GetKeyDown(placeKey)
                });
            }

            controller.UpdateInput();
            controller.UpdateRaycast();
        }
    }
}