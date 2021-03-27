using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace GamedevUtil.Data
{
    public abstract class SQLProcedure<REQ, RES> : Procedure where REQ : ISQLio where RES : ISQLio
    {
        #region Properties
        protected abstract string ProcName { get; }
        protected abstract List<SQLOutput> ReturnTypes { get; }
        #endregion

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
        public async Task<RES> Run(REQ req) {
            FieldInfo[] reqFields = typeof(REQ).GetFields();

            //configure inputs
            inputQueue = new Queue<SQLInput>();
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
            outputQueue = new Queue<SQLOutput>();
            foreach (var output in ReturnTypes) Out(output);

            var res = await SQLController.ExecProcedure(ProcName, inputQueue, outputQueue);
            return MapResult(res);
        }
    }
}