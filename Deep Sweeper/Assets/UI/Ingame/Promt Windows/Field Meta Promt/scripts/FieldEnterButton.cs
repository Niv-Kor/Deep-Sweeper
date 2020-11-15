using UnityEngine;
using UnityEngine.UI;

namespace FieldMeta
{
    public class FieldEnterButton : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Tooltip("The meta component that consists of the field's amount of mines.")]
        [SerializeField] private FieldMinesMeta minesValueCmp;

        [Tooltip("The meta component that consists of the field's total reward.")]
        [SerializeField] private FieldRewardMeta rewardValueCmp;
        #endregion

        #region Class Members
        private FieldMetaPromt promt;
        #endregion

        private void Start() {
            this.promt = GetComponentInParent<FieldMetaPromt>();
            Button buttonCmp = GetComponent<Button>();
            buttonCmp.onClick.AddListener(InitiateField);
        }

        /// <summary>
        /// Initialize field components and dismiss promt.
        /// </summary>
        private void InitiateField() {
            int minesAmount = int.Parse(minesValueCmp.Value);
            long reward = long.Parse(rewardValueCmp.Value);
            GameFlow.Instance.CurrentPhase.Field.Init(minesAmount, reward);
            promt.StartPhase();
        }
    }
}