using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using HL7ParseAndScub;
using System.Collections.Generic;
using System.Xml;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString ufnScubMessage(String HL7Message,String XMLReplacementValues)
    {
        ParseAndScrub myParserScubber = new ParseAndScrub(Convert.ToChar("."), Convert.ToChar("#"));
        myParserScubber.LoadMessage(HL7Message);
        return myParserScubber.ScrubMessage(XMLReplacementValues);
    }
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString ufnSelectHL7Value(string HL7Message, String HL7Location)
    {
        ParseAndScrub myParserScubber = new ParseAndScrub(Convert.ToChar("."), Convert.ToChar("#"));
        myParserScubber.LoadMessage(HL7Message);
        String returnValue;
        returnValue = "";
        System.Collections.Generic.List<String> Values = myParserScubber.FindParseValue(HL7Location);
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
        return returnValue;
    }
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString ufnParseHL7Value(string HL7Message, String HL7Location, Int16 RepeatLocation = 0)
    {
        string returnValue = "";
        LightWeightParser parser = new LightWeightParser(HL7Message);
        if (parser.FindValue(HL7Location))
        { 
            List<string> Values = parser.ParsedValue;
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
