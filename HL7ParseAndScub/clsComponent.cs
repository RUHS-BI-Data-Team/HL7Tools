using System;
using System.Collections.Generic;
using System.Text;

namespace HL7ParseAndScub
{
    class clsComponent
    {
        List<clsSubcomponent> subcomponents;
        Char[] subcomponentDelimiter, repeatDelimiter;
        List<Char> delimiters;
        String componentValue;
        Int32 locationId;
        public event EventHandler OnComponentValueChanged;
        public clsComponent (string Value, List<Char> Delimiters,Int32 LocationId)
        {
            subcomponents = new List<clsSubcomponent>();
            delimiters = Delimiters;
            subcomponentDelimiter = new Char[] { delimiters[6] };
            repeatDelimiter = new Char[] { delimiters[4] };
            componentValue = Value;
            locationId = LocationId;
            LoadSubcomponents();
        }

        public List<clsSubcomponent> Subcomponents
        {
            get { return subcomponents; }
        }
        public String Value
        {
            get { return componentValue; }
            set { componentValue = value; ComponentValueChanged(EventArgs.Empty); }
        }
        public String GetSubcomponentValue(Int16 Index)
        {
            return subcomponents[Index].Value;
        }
        public clsSubcomponent GetSubcomponent(Int32 Index)
        {
            return subcomponents[Index];
        }
        public Int32 Location
        {
            get { return locationId; }
        }
        public Int32 Count()
        {
            return subcomponents.Count;
        }
        private void LoadSubcomponents()
        {
            String[] myComponents = componentValue.Split(subcomponentDelimiter);
            for (Int32 c = 0; c < myComponents.Length; c++)
            {
                clsSubcomponent newSubcomponent = new clsSubcomponent(myComponents[c], delimiters);
                subcomponents.Add(newSubcomponent);
            }
        }
        private void RebuildComponent()
        {
            String tmpComponent = "";
            foreach (clsSubcomponent sc in subcomponents)
            {
                if (tmpComponent.Length > 0)
                {
                    tmpComponent = tmpComponent + subcomponentDelimiter.GetValue(0).ToString() + sc.Value;
                }
                else
                {
                    tmpComponent = sc.Value;
                }
            }
            componentValue = tmpComponent;
        }
        protected virtual void ComponentValueChanged(EventArgs e)
        {
            OnComponentValueChanged(this, e);
        }
        protected virtual void SubcomponentValueChanged(object sender, EventArgs e)
        {
            RebuildComponent();
            ComponentValueChanged(EventArgs.Empty);
        }
    }
}
