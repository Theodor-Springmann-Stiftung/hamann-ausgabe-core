using System;
using System.Collections.Generic;
using System.Text;
using HaXMLReader.Interfaces;
namespace HaXMLReader.EvArgs
{
    public class Tag : EventArgs, IReaderEvArg
    {
        public string Name { get; set; } = "";
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
        public bool IsEmpty { get; set; } = false;
        public bool EndTag { get; set; } = false;

        // Privides safe access to the values Dict
        public string this[string key]
        {
            get
            {
                if (Values != null && Values.ContainsKey(key))
                    return Values[key];
                else
                    return "";
            }
        }
    }
}
