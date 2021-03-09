using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace GamedevUtil.Data
{
    public abstract class SQLProcedure : MonoBehaviour
    {
        #region Class Members
        private Queue<SQLInput> inputQueue;
        private Queue<SQLOutput> outputQueue;
        private int outputCounter;
        #endregion

        #region Properties
        protected abstract string ProcName { get; set; }
        #endregion

        private void Start() {
            this.inputQueue = new Queue<SQLInput>();
            this.outputQueue = new Queue<SQLOutput>();
            ResetProcedure();
        }

        /// <summary>
        /// Reset the object's procedure tracking memebers after an execution.
        /// </summary>
        private void ResetProcedure() {
            inputQueue.Clear();
            outputQueue.Clear();
            outputCounter = 0;
        }

        /// <summary>
        /// Execute the procedure.
        /// </summary>
        /// <returns>A list of the response values as generic objects.</returns>
        protected List<object> Execute() {
            List<object> res = SQLController.ExecProcedure(ProcName, inputQueue, outputQueue);
            ResetProcedure();
            return res;
        }

        /// <summary>
        /// Insert a new input parameter.
        /// </summary>
        /// <param name="paramName">The name of the parameter</param>
        /// <param name="type">The type of the parameter</param>
        /// <param name="value">The parameter's value</param>
        protected void In(string paramName, SqlDbType type, object value) {
            inputQueue.Enqueue(new SQLInput(paramName, type, value));
        }

        /// <summary>
        /// Insert a new expected return value.
        /// </summary>
        /// <param name="type">The type of the return value</param>
        protected void Out(SqlDbType type) {
            outputQueue.Enqueue(new SQLOutput(outputCounter++, type));
        }

        public object CalculatePhaseGrade(string map, string difficulty, int passedTime) {
            In("map", SqlDbType.VarChar, map);
            In("difficulty", SqlDbType.VarChar, difficulty);
            In("passedTime", SqlDbType.Int, passedTime);
            Out(SqlDbType.Decimal);

            var res = Execute();
            return new {
                Value = (float) res[0]
            };
        }
    }
}