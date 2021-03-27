using DeepSweeper.Data;
using System;
using TMPro;
using UnityEngine;

namespace DeepSweeper.Menu.Contract
{
    public class TotalRewardDisplay : MonoBehaviour
    {
        #region Class Members
        private TextMeshProUGUI text;
        private int[] amounts;
        #endregion

        private void Awake() {
            this.text = GetComponent<TextMeshProUGUI>();
            this.amounts = new int[3];
            ContractScreen contract = GetComponentInParent<ContractScreen>();
            contract.ContextRegionChangeEvent += OnRegionChange;
            contract.ContextDifficultyChangeEvent += OnDifficultyChange;
        }

        /// <summary>
        /// Load all difficulties data from the database.
        /// </summary>
        /// <param name="region">The current context region</param>
        private void OnRegionChange(Region region) {
            var proc = SQLProcGetTotalRegionReward.Instance;
            var difficultyLevels = Enum.GetValues(typeof(DifficultyLevel));
            int difficultyCounter = 0;

            //iterate each difficulty
            foreach (DifficultyLevel diff in difficultyLevels) {
                string regionStr = region.ToString().Replace('_', ' ');
                string difficultyStr = diff.ToString().ToLower();
                var procReq = new GetTotalRegionRewardRequest(regionStr, difficultyStr);
                var procRes = proc.Run(procReq);
                amounts[difficultyCounter++] = procRes.Total;
            }

            OnDifficultyChange(DifficultyLevel.Easy); //default
        }

        /// <summary>
        /// Change the displayed total reward amount,
        /// to the one that corresponds the selected difficulty.
        /// </summary>
        /// <param name="diff">The current context difficulty</param>
        private void OnDifficultyChange(DifficultyLevel diff) {
            int index = -1;

            switch (diff) {
                case DifficultyLevel.Easy: index = 0; break;
                case DifficultyLevel.Medium: index = 1; break;
                case DifficultyLevel.Hard: index = 2; break;
            }

            text.text = amounts[index].ToString();
        }
    }
}