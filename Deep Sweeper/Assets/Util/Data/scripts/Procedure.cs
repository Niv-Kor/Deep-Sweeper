using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedevUtil.Data
{
    public class Procedure : Singleton<Procedure>
    {
        #region Class Members
        protected Queue<SQLInput> inputQueue;
        protected Queue<SQLOutput> outputQueue;
        #endregion

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
    }
}