using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nothke.Interaction;
using Nothke.Interaction.Items;

public class SetItemsOnStart : MonoBehaviour
{
    public Hands hands;

    public GenericItem[] items;

    void Start()
    {
        foreach (var item in items)
        {
            hands.Take(item);
        }
    }
}
