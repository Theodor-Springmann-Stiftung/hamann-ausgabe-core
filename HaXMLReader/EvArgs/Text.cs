using System;
using System.Collections.Generic;
using System.Text;
using HaXMLReader.Interfaces;

namespace HaXMLReader.EvArgs
{
    public class Text : EventArgs, IReaderEvArg
    {
        public string Value { get; set; } = "";
    }
}
