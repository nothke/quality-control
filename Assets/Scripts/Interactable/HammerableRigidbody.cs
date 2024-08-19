using Nothke.Interaction;
using Nothke.Interaction.Items;
using UnityEngine;

public class HammerableRigidbody : RigidbodyInteractable
{
    public void Hammer()
    {
        var product = GetComponentInChildren<Product>();

        if (!product)
            return;

        var clip = product.Type.SelectClip(product.Defect != DefectType.None);

        clip.Play(
            transform.position,
            Random.Range(0.8f, 1.2f));
    }
}