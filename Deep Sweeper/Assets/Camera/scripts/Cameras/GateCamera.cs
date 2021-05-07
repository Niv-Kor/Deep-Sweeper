﻿using UnityEngine.Events;

namespace DeepSweeper.Camera
{
    public class GateCamera : DynamicCamera
    {
        /// <inheritdoc/>
        protected override void OnActivation() {
            DiegeticsManager.Instance.Activate(false, 0);
        }

        /// <inheritdoc/>
        protected override void OnDeactivation(UnityAction callback = null) {
            DiegeticsManager.Instance.Activate(true, 0);
            callback?.Invoke();
        }
    }
}