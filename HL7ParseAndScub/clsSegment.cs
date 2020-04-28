using System;
using System.Collections.Generic;
using System.Text;

namespace HL7ParseAndScub
{
    class clsSegment
    {
        List<Char> delimiters;
        Char[] fieldDelimiter;
        private List<clsField> fields = new List<clsField>();
        private String segmentValue;
        String nameId;
        public event EventHandler OnSegmentValueChanged;
        public clsSegment(String Value, List<Char> Delimiters)
        {
            delimiters = Delimiters;
            segmentValue = Value;
            LoadFields();
        }
        
        private void RebuildSegment()
        {
            String tmpSegment = nameId;
            foreach (clsField f in fields)
            {
                if(tmpSegment.Length > 0)
                {
                    tmpSegment = tmpSegment + fieldDelimiter.GetValue(0).ToString() + f.Value;
                }
                else
                {
                    tmpSegment = f.Value;
                }
            }
            segmentValue = tmpSegment;
        }
        public string Segement
        {
            get { return segmentValue; }
            set { segmentValue = value; }
        }
        private void LoadMSHFields()
        {
            if (segmentValue.Length > 8)
            {
                nameId = segmentValue.Substring(0, 3);
                delimiters.Add(Convert.ToChar(segmentValue.Substring(3, 1)));
                fieldDelimiter = new Char[] { delimiters[2] };
                for (Int32 i = 4; i < 8; i++)
                {
                    delimiters.Add(Convert.ToChar(segmentValue.Substring(i, 1)));
                }
                clsField newField1 = new clsField(segmentValue.Substring(3, 1), delimiters, 1, false);
                fields.Add(newField1);
                String[] myFields = segmentValue.Substring(4).Split(fieldDelimiter);
                for (Int32 f = 0; f < myFields.Length; f++)
                {
                    clsField newField;
                    if (f == 0)
                    {
                        newField = new clsField(myFields[f], delimiters, f+2, false);
                    }
                    else
                    {
                        newField = new clsField(myFields[f], delimiters, f+2);
                    }                    
                    fields.Add(newField);
                }
            }
        }
        private void LoadFields()
        {
            if (segmentValue.Length > 2)
            {
                if (segmentValue.Substring(0, 3) == "MSH")
                {
                    LoadMSHFields();
                }
                else
                {
                    fieldDelimiter = new Char[] { delimiters[2] };
                    String[] myFields = segmentValue.Split(fieldDelimiter);
                    for (Int32 f = 0; f < myFields.Length; f++)
                    {
                        if (f == 0)
                        {
                            nameId = myFields[f];
                        }
                        else
                        {
                            clsField newField = new clsField(myFields[f], delimiters, f);
                            fields.Add(newField);
                        }
                    }
                }
                foreach (clsField f in fields)
                {
                    f.OnFieldValueChanged += new EventHandler(FieldValueChanged);
                }
            }
        }
        
        public String Name
        {
            get { return nameId; }
        }
        public String Value
        {
            get { return this.segmentValue; }
        }
        public List<clsField> Fields
        {
            get { return fields; }
        }
        public String GetFieldValue(Int16 Index)
        {
            return fields[Index].Value;
        }
        public clsField GetField(Int32 Index)
        {
            return fields[Index];
        }
        public Int32 Count()
        {
            return fields.Count;
        }
        
        protected virtual void SegmentValueChanged(EventArgs e)
        {
            OnSegmentValueChanged(this, e);
        }
        protected virtual void FieldValueChanged(object sender, EventArgs e)
        {
            RebuildSegment();
            SegmentValueChanged(EventArgs.Empty);
        }
    }
}
