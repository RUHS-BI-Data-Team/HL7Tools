using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;


public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString HL7Parser(string HL7Message, string HL7Element, Int16 RepeatLocation = 0)
    {
        Parser myParser = new Parser();
        string returnValue = "";
        myParser.Message = HL7Message;
        //returnValue = myParser.Message;
        if (myParser.FindValue(HL7Element) == true)
        {
            List<string> Values = myParser.ParsedValue;
            if (RepeatLocation == 0)
            {
                foreach (string v in Values)
                {
                    if (returnValue == "")
                    {
                        returnValue = v;
                    }
                    else
                    {
                        returnValue = returnValue + ", " + v;
                    }
                }
            }
            else
            {
                if (RepeatLocation <= Values.Count)
                {
                    returnValue = Values[RepeatLocation - 1];
                }
                else
                {
                    returnValue = "";
                }
            }
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }
}
 
class Parser
{
    String message;
    List<String> parsedValue = new List<string>();
    List<Char> delimiters = new List<char>();
    private struct sctParseValueLocation
    {
        public String Segment;
        public Int16 Field;
        public Int16 Component;
        public Int16 Subcomponent;

    }
    public Parser()
    {
        SetDelimiterDefault();
    }
    public Parser(String Message, List<Char> Delimiters)
    {
        message = Message;
        delimiters = Delimiters;
    }
    public Parser(String Message)
    {
        message = Message;
        SetDelimiterDefault();
    }
    public Parser(List<Char> Delimiters)
    {
        delimiters = Delimiters;
    }
    public String Message
    {
        get { return message; }
        set { message = value; }
    }
    public List<Char> Delimiters
    {
        get { return delimiters; }
        set { delimiters = value; }
    }
    public List<String> ParsedValue
    {
        get { return parsedValue; }
    }
    private void SetDelimiterDefault()
    {
        delimiters.Add(Convert.ToChar("\n"));
        delimiters.Add(Convert.ToChar("\r"));
        delimiters.Add(Convert.ToChar("|"));
        delimiters.Add(Convert.ToChar("^"));
        delimiters.Add(Convert.ToChar("~"));
        delimiters.Add(Convert.ToChar("\\"));
        delimiters.Add(Convert.ToChar("&"));
    }
    private sctParseValueLocation CreateLocation(String Location)
    {
        sctParseValueLocation rtnLocation = new sctParseValueLocation();
        rtnLocation.Segment = Location.Substring(0, 3);
        if (Location.Length > 3)
        {
            Char[] LocationDelimiter = new Char[] { Convert.ToChar(".") };
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
    public Boolean FindValue(String ValueLocation)
    {
        try
        {
            parsedValue.Clear();
            sctParseValueLocation newLocation = CreateLocation(ValueLocation);

            Char[] SegmentDelimiter = new Char[] { delimiters[0], delimiters[1] };
            Char[] FieldDelimiter = new Char[] { delimiters[2] };
            Char[] ComponentDelimiter = new Char[] { delimiters[3] };
            Char[] SubcomponentDelimiter = new Char[] { delimiters[4] };
            Char[] RepeatDelimiter = new Char[] { delimiters[5] };


            String[] Segments = message.Split(SegmentDelimiter);
            int v = 0;
            for (int s = 0; s < Segments.Length; s++) //Move through Segments
            {
               try
                    {
                    if (Segments[s].Length > 4 && Segments[s].Substring(0, 3) == newLocation.Segment) //Check to see if Sebement matches Search
                        {
                            parsedValue.Add("");
                            parsedValue[v] = Segments[s];
                            string tmpSeg;
                            if (Segments[s].Substring(0, 3) != "MSH")
                            {
                                tmpSeg = Segments[s].Substring(4);
                            }
                            else
                            {
                                tmpSeg = Segments[s];
                            }
                            if (newLocation.Field > 0)
                            {
                                String[] Fields = tmpSeg.Split(FieldDelimiter);
                                if (Fields.Length >= newLocation.Field)
                                {
                                    parsedValue[v] = Fields[newLocation.Field - 1];
                                    if (newLocation.Component > 0)
                                    {
                                    String[] ComponenetGroups = parsedValue[v].Split(RepeatDelimiter);
                                    foreach(String g in ComponenetGroups)
                                    {
                                        String[] Componenets = g.Split(ComponentDelimiter);
                                        if (Componenets.Length >= newLocation.Component)
                                        {
                                            parsedValue[v] = Componenets[newLocation.Component - 1];
                                            if (newLocation.Subcomponent > 0)
                                            {
                                                String[] Subcomponenets = parsedValue[v].Split(SubcomponentDelimiter);
                                                if (Subcomponenets.Length >= newLocation.Subcomponent)
                                                {
                                                    parsedValue[v] = Subcomponenets[newLocation.Subcomponent - 1];

                                                }
                                                else
                                                {
                                                    return false;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                   }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            v++;
                        }
                    }catch(Exception ex)
                    {
                        parsedValue.Add("1: " + ex.Message);
                        return true;
                    }
            }
            return true;
        }
        catch (Exception ex)
        {
            parsedValue.Add("2: " + ex.Message);
            return true;
        }
    }

}
