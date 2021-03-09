using System.Collections.Generic;
using UnityEngine;

namespace GamedevUtil.Data
{
    [System.Serializable]
    public class ConnectionString
    {
        [System.Serializable]
        public struct Entry
        {
            public string Key;
            public string Value;
        }

        #region Exposed Editor parameters
        [Header("Meta Data")]
        [Tooltip("The name of the connection string."
               + "This property is essential for fetching the string.")]
        [SerializeField] private string stringName = "ConnectionString<" + instanceCounter++ + ">";

        [Header("Content")]
        [Tooltip("A list of the connections string's parameters.")]
        [SerializeField] private List<Entry> parameters;

        [Header("Format")]
        [Tooltip("The character that comes after each key name in the string.")]
        [SerializeField] private char equalSign = '=';

        [Tooltip("The character that comes after each entry in the string.")]
        [SerializeField] private char delimiter = ';';
        #endregion

        #region Class Members
        private static int instanceCounter = 1;
        #endregion

        #region Properties
        public string Name { get => stringName; }
        public string AsString {
            get {
                string cs = "";
                foreach (Entry param in parameters)
                    cs += param.Key + equalSign + param.Value + delimiter;

                return cs;
            }
        }

        public List<Entry> AsList { get => parameters; }
        #endregion
    }
}