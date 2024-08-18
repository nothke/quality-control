using UnityEngine;
using System.Collections;

namespace Nothke.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [HideInInspector]
        public InteractionController manager;

        [System.Serializable]
        public class Info
        {
            public string name;

            //public string descriptionShort;
            //[Multiline()]
            //public string description;
        }

        public Info info;
        public virtual string Label => info.name;

        public virtual void Use(InteractionController im)
        {
            manager = im;
            //Debug.Log("No use");
        }

        public virtual void OnHover() { }
        public virtual void OnDehover() { }

        public virtual void StartHold() { }
        public virtual void EndHold() { }
        public virtual void UseHold() { }
    }
}