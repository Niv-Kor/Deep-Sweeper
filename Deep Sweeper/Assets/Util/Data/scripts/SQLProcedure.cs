using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace GamedevUtil.Data
{
    public abstract class SQLProcedure<REQ, RES> : Singleton<SQLProcedure<REQ, RES>> where REQ : ISQLio where RES : ISQLio
    {
        #region Class Members
        private Queue<SQLInput> inputQueue;
        private Queue<SQLOutput> outputQueue;
        #endregion

        #region Properties
        protected abstract string ProcName { get; }
        protected abstract List<SQLOutput> ReturnTypes { get; }
        #endregion

        private void Awake() {
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
        }

        /// <summary>
        /// Execute the procedure.
        /// </summary>
        /// <returns>A list of the response values as generic objects.</returns>
        protected List<List<object>> Execute() {
            var resSet = SQLController.ExecProcedure(ProcName, inputQueue, outputQueue);
            ResetProcedure();
            return resSet;
        }

        /// <summary>
        /// Insert a new input parameter.
        /// </summary>
        /// <param name="input">An input structure</param>
        protected void In(SQLInput input) {
            inputQueue.Enqueue(input);
        }

        /// <summary>
        /// Insert a new expected return value.
        /// </summary>
        /// <param name="output">An output structure</param>
        protected void Out(SQLOutput output) {
            outputQueue.Enqueue(output);
        }

        /// <summary>
        /// Map the values of a result list to an object.
        /// </summary>
        /// <param name="result">An ordered list of the procedure returned values</param>
        /// <returns>A defined mapped object with named fields.</returns>
        protected abstract RES MapResult(List<List<object>> result);

        /// <summary>
        /// Run the procedure.
        /// </summary>
        /// <param name="req">Request struct</param>
        /// <returns>A response struct.</returns>
        public RES Run(REQ req) {
            FieldInfo[] reqFields = typeof(REQ).GetFields();

            //configure inputs
            foreach (FieldInfo field in reqFields) {
                string fieldName = field.Name;
                Type type = field.FieldType;
                var entry = Convert.ChangeType(field.GetValue(req), type);
                var entryTypeField = type.GetField("Type");
                var entryValueField = type.GetField("Value");
                var entryType = (SqlDbType) entryTypeField.GetValue(entry);
                var entryValue = entryValueField.GetValue(entry);
                SQLInput input = new SQLInput(fieldName, entryType, entryValue);
                In(input);
            }

            //configure outputs
            foreach (var output in ReturnTypes) Out(output);

            var res = Execute();
            return MapResult(res);
        }
    }
}