using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GamedevUtil
{
    public class Puppeteer
    {
        #region Constants
        private static readonly int TRIGGER_RESET_TIME = 200;
        #endregion

        #region Class Members
        private Animator animator;
        private IDictionary<string, bool> boolParams;
        private IDictionary<string, float> floatParams;
        private IDictionary<string, int> intParams;
        #endregion

        #region Properties
        public List<string> Parameters {
            get {
                return (from state in animator.parameters
                        select state.name).ToList();
            }
        }
        #endregion

        public Puppeteer(Animator animator) {
            this.animator = animator;
            this.boolParams = new Dictionary<string, bool>();
            this.floatParams = new Dictionary<string, float>();
            this.intParams = new Dictionary<string, int>();
        }

        /// <summary>
        /// Set a boolean parameter's value.
        /// </summary>
        /// <param name="param">The parameter's name</param>
        /// <param name="flag">The new parameter value</param>
        public void Manipulate(string param, bool flag) {
            animator.SetBool(param, flag);
            boolParams[param] = flag;
        }

        /// <summary>
        /// Set a float parameter's value.
        /// </summary>
        /// <param name="param">The parameter's name</param>
        /// <param name="value">The new parameter value</param>
        public void Manipulate(string param, float value) {
            animator.SetFloat(param, value);
            floatParams[param] = value;
        }

        /// <summary>
        /// Set an integer parameter's value.
        /// </summary>
        /// <param name="param">The parameter's name</param>
        /// <param name="value">The new parameter value</param>
        public void Manipulate(string param, int value) {
            animator.SetInteger(param, value);
            intParams[param] = value;
        }

        /// <summary>
        /// Activate a trigger parameter.
        /// </summary>
        /// <param name="param">The new parameter value</param>
        public void Manipulate(string param) {
            animator.SetTrigger(param);

            void ResetTrigger() { animator.ResetTrigger(param); }
            Task.Delay(TRIGGER_RESET_TIME).ContinueWith(x => ResetTrigger());
        }
    }
}