using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nothke.Audio
{
    [CreateAssetMenu(menuName = "CollisionSoundsProfile", fileName = "CollisionSoundsProfile")]
    public class CollisionSoundsProfile : ScriptableObject
    {
        public AudioClip[] clips;
    }
}

