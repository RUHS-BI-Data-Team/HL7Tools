using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HL7ParseAndScub;

namespace HL7ParserTest
{
    public partial class Form1 : Form
    {
        //private HL7MessageParser.Parser myParser = new HL7MessageParser.Parser();
        private ParseAndScrub myParserScubber = new ParseAndScrub(Convert.ToChar("."),Convert.ToChar("#"));
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtParsedValue.Text = "";
            List<String> Values = new List<string>();
            //LightWeightParser parser = new LightWeightParser(txtHL7Message.Text);
            
            //   if (parser.FindValue(txtLocation.Text))
            //{
            //    Values = parser.ParsedValue;
            //}
            Values = myParserScubber.FindParseValue(txtLocation.Text);
            foreach (string v in Values)
            {
                if (txtParsedValue.Text == "")
                {
                    txtParsedValue.Text = v;
                }
                else
                {
                    txtParsedValue.Text = txtParsedValue.Text + ", " + v;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnScrub_Click(object sender, EventArgs e)
        {
            txtScubbedMessage.Text = myParserScubber.ReplaceParseValue(txtLocation.Text, txtScrubValue.Text);
            Clipboard.SetText(myParserScubber.HL7Message);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            myParserScubber.LoadMessage(txtHL7Message.Text);
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            String ReplaceValues = "<Root><ReplaceValues><HL7ValueLocation>PID5.2</HL7ValueLocation><ReplacementValue>MICKEY</ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID5.1</HL7ValueLocation><ReplacementValue>MOUSE</ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID5.3</HL7ValueLocation><ReplacementValue></ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID3.1</HL7ValueLocation><ReplacementValue>123456789</ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID11.1</HL7ValueLocation><ReplacementValue>1 DISNEY WAY</ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID11.3</HL7ValueLocation><ReplacementValue>TOMORROW LAND</ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID11.4</HL7ValueLocation><ReplacementValue>CA</ReplacementValue></ReplaceValues><ReplaceValues><HL7ValueLocation>PID11.5</HL7ValueLocation><ReplacementValue>99999</ReplacementValue></ReplaceValues></Root>";

            
            txtScubbedMessage.Text = myParserScubber.ScrubMessage(ReplaceValues);
            Clipboard.SetText(myParserScubber.HL7Message);
        }
    }
}
