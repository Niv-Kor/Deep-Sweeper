using DeepSweeper.CameraSet;
using DeepSweeper.CameraSet.PostProcessing;
using DeepSweeper.Characters;
using DeepSweeper.Player;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Promt
{
    public class LosingWindow : PromtWindow
    {
        #region Exposed Editor Parameters
        [Header("FX")]
        [Tooltip("A list of effect instructions to apply on game lost.")]
        [SerializeField] private List<EffectInstructions> FXInstruction;
        #endregion

        #region Class Members
        private VisualEffectsSheet FXSheet;
        #endregion

        #region Properties
        public override PromtType Type { get => PromtType.LosingWindow; }
        #endregion

        private void Start() {
            this.FXSheet = VisualEffectsSheet.Create(CameraRole.Main, FXInstruction);
        }

        /// <summary>
        /// Pop the losing window.
        /// </summary>
        /// <param name="deadCommander">The commander that died and caused the lost of the phase</param>
        /// <param name="remainingCommanders">A list of all remaining alive commanders</param>
        public void Pop(Persona deadCommander, List<Persona> remainingCommanders) {
            
        }

        /// <inheritdoc/>
        protected override void OnOpen() {
            VisualEffectsAnimator.Instance.Animate(FXSheet);
        }

        /// <inheritdoc/>
        protected override void OnClose() {}
    }
}