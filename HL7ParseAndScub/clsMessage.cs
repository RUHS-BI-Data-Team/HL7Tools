using System;
using System.Collections.Generic;
using System.Text;

namespace HL7ParseAndScub
{
    class clsMessage
    {
        string messageValue;
        List<Char> delimiters;
        Char[] segmentDelimiter;
        List<clsSegment> segments;
        
        public clsMessage(String Message, List<Char> Delimiters)
        {
            delimiters = Delimiters;
            segmentDelimiter = new Char[] { delimiters[0], delimiters[1] };
            messageValue = Message;
            segments = new List<clsSegment>();
            LoadSegments();
        }
        
        public char[] SegmentDelimiter
        {
            get { return segmentDelimiter; }
        }
        public clsSegment GetSegment(Int32 Index)
        {
            return segments[Index];
        }
        public String GetSegmentValue(Int32 Index)
        {
            return segments[Index].Value;
        }
        public List<clsSegment> Segments
        {
            get { return segments; }
        }
        public Int32 Count()
        {
            return segments.Count;
        }
        public String Value
        {
            get { return messageValue; }
        }
        
        private void LoadSegments()
        {
            String[] Segments = messageValue.Split(segmentDelimiter);
            //try
            //{
                for (Int16 s = 0; s < Segments.Length; s++)
                {
                    clsSegment newSegment = new clsSegment(Segments[s], delimiters);
                    segments.Add(newSegment);
                }
                foreach (clsSegment s in segments)
                {
                    s.OnSegmentValueChanged += new EventHandler(SegmentValueChanged);
                }
            //}
            //catch(Exception e)
            //{
            //    throw new Exception(e.Message);
            //}
        }
        private void RebuildMessage()
        {
            String tmpMessage = "";
            foreach (clsSegment s in segments)
            {
                if (tmpMessage.Length > 0)
                {
                    //tmpMessage = tmpMessage + segmentDelimiter.GetValue(1).ToString() + s.Value;
                    tmpMessage = tmpMessage + "\r" + s.Value;
                }
                else
                {
                    tmpMessage = s.Value;
                }
            }
            messageValue = tmpMessage;
        }
        protected virtual void SegmentValueChanged(object sender, EventArgs e)
        {
            RebuildMessage();
        }
    }
}
