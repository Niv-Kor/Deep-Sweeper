using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamedevUtil
{
    [RequireComponent(typeof(Animator))]
    public class Puppeteer : MonoBehaviour
    {
        #region Constants
        private static readonly float TRIGGER_RESET_TIME = .5f;
        #endregion

        #region Class Members
        protected Animator animator;
        protected IDictionary<string, bool> boolParams;
        protected IDictionary<string, float> floatParams;
        protected IDictionary<string, int> intParams;
        #endregion

        #region Properties
        protected List<string> Parameters {
            get {
                return (from state in animator.parameters
                        select state.name).ToList();
            }
        }
        #endregion

        protected virtual void Awake() {
            this.animator = GetComponent<Animator>();
            this.boolParams = new Dictionary<string, bool>();
            this.floatParams = new Dictionary<string, float>();
            this.intParams = new Dictionary<string, int>();
        }

        /// <summary>
        /// Set a boolean parameter's value.
        /// </summary>
        /// <param name="param">The parameter's name</param>
        /// <param name="flag">The new parameter value</param>
        protected virtual void Manipulate(string param, bool flag) {
            animator.SetBool(param, flag);
            boolParams[param] = flag;
        }

        /// <summary>
        /// Set a float parameter's value.
        /// </summary>
        /// <param name="param">The parameter's name</param>
        /// <param name="value">The new parameter value</param>
        protected virtual void Manipulate(string param, float value) {
            animator.SetFloat(param, value);
            floatParams[param] = value;
        }

        /// <summary>
        /// Set an integer parameter's value.
        /// </summary>
        /// <param name="param">The parameter's name</param>
        /// <param name="value">The new parameter value</param>
        protected virtual void Manipulate(string param, int value) {
            animator.SetInteger(param, value);
            intParams[param] = value;
        }

        /// <summary>
        /// Activate a trigger parameter.
        /// </summary>
        /// <param name="param">The new parameter value</param>
        protected virtual void Manipulate(string param) {
            animator.SetTrigger(param);
            StartCoroutine(ResetTrigger(param, TRIGGER_RESET_TIME));
        }

        /// <summary>
        /// Reset a trigger after a fixed amount of time.
        /// </summary>
        /// <param name="param">The name of the trigger parameter</param>
        /// <param name="time">The amount of time (in seconds) after which the trigger will reset</param>
        private IEnumerator ResetTrigger(string param, float time) {
            yield return new WaitForSeconds(time);
            animator.ResetTrigger(param);
        }
    }
}