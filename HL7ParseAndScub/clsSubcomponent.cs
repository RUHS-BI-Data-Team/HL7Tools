using System;
using System.Collections.Generic;
using System.Text;

namespace HL7ParseAndScub
{
    class clsSubcomponent
    {
        List<Char> delimiters;
        Char[] repeatDelimiter;
        private String subcomponentValue;
        public event EventHandler OnSubcomponentValueChanged;
        public clsSubcomponent (string Value, List<Char> Delimiters)
        {
            delimiters = Delimiters;
            repeatDelimiter = new Char[] { delimiters[4] };
            subcomponentValue = Value;
        }
        public Char[] RepeatDelimiter
        {
            get { return repeatDelimiter; }
        }
        public String Value
        {
            get { return subcomponentValue; }
            set { subcomponentValue = value; SubcomponentValueChanged(EventArgs.Empty); }
        }
        protected virtual void SubcomponentValueChanged(EventArgs e)
        {
            OnSubcomponentValueChanged(this, e);
        }
    }
}
