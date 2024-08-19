using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nothke.Interaction;
using Nothke.Interaction.Items;

public class Hammer : GenericItem
{
    private void Update()
    {
        if (manager && manager.hands.item == this)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (manager.hovered is HammerableRigidbody hammerable)
                {
                    hammerable.Hammer();

                    var hands = (manager.hands as Hands);
                    var hand = hands.hand;

                    hand.transform.position = manager.hit.point;
                    hand.transform.localPosition += new Vector3(0f, -0.4f, 0f); // Offset to match the hammer head
                    hands.ResetOffset();
                }
            }
        }
    }
}