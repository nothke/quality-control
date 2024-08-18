using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nothke.Interaction;

public class InteractableTest : Interactable
{
    public override void Use(InteractionController im)
    {
        base.Use(im);
        Debug.Log("works!");
    }
}
