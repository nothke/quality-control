using Nothke.Interaction;
using Nothke.Interaction.Items;
using UnityEngine;

public class HammerRepair: Interactable
{
    public override void Use(InteractionController im)
    {
        if (im.hands.item == null)
        {
            base.Use(im);
            return;
        }

        var product = GetComponentInChildren<Product>();

        if (!product)
        {
            return;
        }

        var clip = product.Type.SelectClip(product.Defect != DefectType.None);
        NAudio.Play(clip, transform.position);
    }
}