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
        private int outputCounter;
        #endregion

        #region Properties
        protected abstract string ProcName { get; }
        protected abstract List<SqlDbType> ReturnTypes { get; }
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

        /// <summary>
        /// Map the values of a result list to an object.
        /// </summary>
        /// <param name="result">An ordered list of the procedure returned values</param>
        /// <returns>A defined mapped object with named fields.</returns>
        protected abstract RES MapResult(List<object> result);

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
                In(fieldName, entryType, entryValue);
            }

            //configure outputs
            foreach (SqlDbType type in ReturnTypes) Out(type);

            var res = Execute();
            return MapResult(res);
        }
    }
}