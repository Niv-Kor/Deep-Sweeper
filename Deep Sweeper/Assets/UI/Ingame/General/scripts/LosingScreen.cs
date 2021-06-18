using DeepSweeper.CameraSet;
using DeepSweeper.CameraSet.PostProcessing;
using DeepSweeper.Characters;
using DeepSweeper.UI.Ingame.Spatials.Commander;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Promt
{
    public class LosingScreen : MonoBehaviour
    {
        #region Class Members
        private PostProcessingManager postProcess;
        private Coroutine toneCoroutine;

        [SerializeField] public List<EffectInstructions> VisualFX { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        #endregion

        private void Awake() {
            this.postProcess = CameraManager.Instance.GetPostProcessManager(CameraRole.Main);
            var commander = SpatialsManager.Instance.Get(typeof(CommanderSpatial)) as CommanderSpatial;
            commander.CommanderDeadEvent += Lose;
        }

        private IEnumerator ChangeScreenTone() {
            yield return null;
        }

        public void Lose(Persona deadCommander, List<Persona> remainingCommanders) {
            
        }
    }
}