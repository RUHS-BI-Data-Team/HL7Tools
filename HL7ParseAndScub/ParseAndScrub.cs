using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HL7ParseAndScub
{
    public class ParseAndScrub
    {
        private struct sctParseValueLocation
        {
            public String Segment;
            public Int16 Field;
            public Int16 Component;
            public Int16 Subcomponent;
            public Int32 RepeatLocation;

        }
        clsMessage message;
        List<String> parsedValue = new List<string>();
        List<Char> delimiters = new List<char>();
        List<String> allowedSegments = new List<string>();
        Boolean messageLoaded = false;
        Char repeatDelimiter, locationDelimiter;
        public ParseAndScrub(Char LocationDelimiter,Char RepeatDelimiter)
        {
            repeatDelimiter = RepeatDelimiter;
            locationDelimiter = LocationDelimiter;
        }
        public String ScrubMessage(String XMLReplacementValues)
        {
            if (!messageLoaded)
            {
                return "";
            }
            XmlDocument docValues = new XmlDocument();
            docValues.LoadXml(XMLReplacementValues);
            XmlNodeList ValuesList = docValues.FirstChild.ChildNodes;
            foreach( XmlNode n in docValues.FirstChild.ChildNodes)
            {
                FindValues(n.ChildNodes[0].InnerText, n.ChildNodes[1].InnerText);
            }
            return message.Value;
        }
        public ParseAndScrub(List<Char> Delimiters)
        {
            
        }
        
        public void LoadMessage(String Message)
        {
            delimiters.Add(Convert.ToChar("\n"));
            delimiters.Add(Convert.ToChar("\r"));
            message =  new clsMessage(Message, delimiters);
            messageLoaded = true;
        }
        public String HL7Message
        {
            get { return message.Value; }
        }
        public List<String> FindParseValue(String ParseLocation)
        {
            return FindValues(ParseLocation);
        }
        public String ReplaceParseValue(String ParseLocation, String NewValue)
        {
            FindValues(ParseLocation, NewValue);
            return message.Value;
        }
        private List<String> FindValues(String ValueLocation, String NewValue = null)
        {
            if (!messageLoaded)
            {
                return new List<string>(0);
            }
            Int32 l = 0;
            sctParseValueLocation Locate = CreateLocation(ValueLocation);
            List<String> returnValues = new List<string>();
            
            foreach (clsSegment s in message.Segments)
            {
                
                if (s.Name == Locate.Segment)
                {
                    if (Locate.Field == 0)
                    {
                        l++;
                        if (l == Locate.RepeatLocation || Locate.RepeatLocation == 0)
                        {
                            returnValues.Add("");
                            returnValues[returnValues.Count - 1] = s.Value;
                            
                        }
                    }
                    else
                    {
                        foreach(clsField f in s.Fields)
                        {
                            if(f.Location == Locate.Field)
                            {
                                if(Locate.Component == 0)
                                {
                                    l++;
                                    if (l == Locate.RepeatLocation || Locate.RepeatLocation == 0)
                                    {
                                        returnValues.Add("");
                                        returnValues[returnValues.Count - 1] = f.Value;
                                        if (NewValue != null)
                                        {
                                            f.Value = NewValue;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (clsComponent c in f.Components)
                                    {
                                        if(c.Location == Locate.Component)
                                        {
                                            if(Locate.Subcomponent == 0)
                                            {
                                                l++;
                                                if (l == Locate.RepeatLocation || Locate.RepeatLocation == 0)
                                                {
                                                    returnValues.Add("");
                                                    returnValues[returnValues.Count - 1] = c.Value;
                                                    if (NewValue != null)
                                                    {
                                                        c.Value = NewValue;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach(clsSubcomponent sc in c.Subcomponents)
                                                {
                                                    l++;
                                                    if (l == Locate.RepeatLocation || Locate.RepeatLocation == 0)
                                                    {
                                                        returnValues.Add("");
                                                        returnValues[returnValues.Count - 1] = sc.Value;
                                                        if (NewValue != null)
                                                        {
                                                            sc.Value = NewValue;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return returnValues;
        }
        private sctParseValueLocation CreateLocation(String Location)
        {
            sctParseValueLocation rtnLocation = new sctParseValueLocation();
            Char[] RepeatDelimiter = new Char[] { repeatDelimiter };
            String[] RepeatSearch = Location.Split(RepeatDelimiter);
            if(RepeatSearch.Length > 1)
            {
                rtnLocation.RepeatLocation =Convert.ToInt32(RepeatSearch[1]);
            }
            Location = RepeatSearch[0];

            rtnLocation.Segment = Location.Substring(0, 3);
            if (Location.Length > 3)
            {
                Char[] LocationDelimiter = new Char[] { locationDelimiter };
                String[] Search = Location.Substring(3).Split(LocationDelimiter);
                rtnLocation.Field = Convert.ToInt16(Search[0]);
                if (Search.Length > 1)
                {
                    rtnLocation.Component = Convert.ToInt16(Search[1]);
                    if (Search.Length > 2)
                    {
                        rtnLocation.Subcomponent = Convert.ToInt16(Search[2]);
                    }
                    else
                    {
                        rtnLocation.Subcomponent = 0;
                    }
                }
                else
                {
                    rtnLocation.Component = 0;
                    rtnLocation.Subcomponent = 0;
                }

            }
            else
            {
                rtnLocation.Field = 0;
                rtnLocation.Component = 0;
                rtnLocation.Subcomponent = 0;
            }


            return rtnLocation;
        }
    }
}
