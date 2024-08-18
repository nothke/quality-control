using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nothke.Interaction.Items;

namespace Nothke.Interaction.Integration
{
    public interface ILockableFreeLook
    {
        void LockFreeLook(bool _lock);
    }

    public interface IFocusableEffect
    {
        void FocusEffect(bool focus, float distance);
    }

    public interface IZoomable
    {
        void ZoomIn(bool _zoomIn);
    }
}
