using System;
using System.Collections.Generic;
using System.Text;

namespace HL7ParseAndScub
{
    class clsField
    {
        List<clsComponent> components;
        Char[] componentDelimiter, repeatDelimiter;
        List<Char> delimiters;
        string fieldValue;
        Int32 locationId;
        private List<String> fieldValues = new List<string>();
        Boolean splitValue;
        public event EventHandler OnFieldValueChanged;
        public clsField(String Value, List<Char> Delimiters,Int32 LocationId, Boolean SplitValue = true)
        {
            components = new List<clsComponent>();
            delimiters = Delimiters;
            componentDelimiter = new Char[] { delimiters[3] };
            repeatDelimiter = new Char[] { delimiters[4] };
            fieldValue = Value;
            locationId = LocationId;
            splitValue = SplitValue;
            LoadComponents();
        }
        public Int32 Location
        {
            get { return locationId; } 
        }
        public List<clsComponent> Components
        {
            get { return components; }
        }
        public String GetComponentValue(Int16 Index)
        {
            return components[Index].Value;
        }
        public clsComponent GetComponent(Int32 Index)
        {
            return components[Index];
        }
        public String Value
        {
            get { return fieldValue; }
            set { fieldValue = value; FieldValueChanged(EventArgs.Empty); }
        }
        public Int32 Count()
        {
            return components.Count;
        }
        private void RebuildField()
        {
            String tmpField = "";
            Int32 tmpLocationId = 0;
            foreach (clsComponent c in components)
            {
                if (tmpField.Length > 0)
                {
                    if(tmpLocationId+1 == c.Location)
                    {
                        tmpField = tmpField + componentDelimiter.GetValue(0).ToString() + c.Value;
                    }
                    else
                    {
                        tmpField = tmpField + repeatDelimiter.GetValue(0).ToString() + c.Value;
                    }
                }
                else
                {
                    tmpField = c.Value;
                }
                tmpLocationId = c.Location;
            }
            fieldValue = tmpField;
        }
        private void LoadComponents()
        {
            String[] componentGroup = new String[0];
            if (!splitValue)
            {
                componentGroup = new String[] { fieldValue };
            }
            else
            {
                componentGroup = fieldValue.Split(repeatDelimiter);
            }
            for (Int32 g = 0; g < componentGroup.Length; g++)
            {
                String[] myComponents;
                if (!splitValue)
                {
                    myComponents = new String[] { componentGroup[g] };
                }
                else
                {
                    myComponents = componentGroup[g].Split(componentDelimiter);
                }
                for (Int32 c = 0; c < myComponents.Length; c++)
                {
                    clsComponent newComponent = new clsComponent(myComponents[c], delimiters, c + 1);
                    components.Add(newComponent);
                }
            
            }
            foreach (clsComponent c in this.components)
            {
                c.OnComponentValueChanged += new EventHandler(ComponentValueChanged);
            }

        }
        protected virtual void FieldValueChanged(EventArgs e)
        {
            OnFieldValueChanged(this, e);
        }
        protected virtual void ComponentValueChanged(object sender, EventArgs e)
        {
            RebuildField();
            FieldValueChanged(EventArgs.Empty);
        }
        
    }
}
