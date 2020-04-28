using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7DataAnalyser
{
   class HL7ParsedDataTemplete : ITemplate
    {
        private Label lbl = new Label();
        private Label lblId = new Label();
        ListItemType TypeOfTemplete;
        string DatabaseFieldName;
        String parsingField;

        public HL7ParsedDataTemplete(ListItemType Templete, string Field, string ParsingField = "")
        {
            TypeOfTemplete = Templete;
            DatabaseFieldName = Field;
            parsingField = ParsingField;
            //IdFieldName = IdField;
        }
        public void InstantiateIn(Control container)
        {
            lbl = new Label();
            lbl.DataBinding += new EventHandler(HL7DataBinding);
            container.Controls.Add(lbl);
        }
        private void HL7DataBinding(object sender, EventArgs e)
        {
            Label lblData = sender as Label;
            GridViewRow container = lblData.NamingContainer as GridViewRow;
            object dataValue = DataBinder.Eval(container.DataItem, DatabaseFieldName);
            if (dataValue != DBNull.Value)
            {
                lblData.Text = dataValue.ToString();
                string dbField = DatabaseFieldName;
                string IsParsingField = "false";
                if (parsingField != "")
                {
                    dbField = parsingField;
                    IsParsingField = "true";
                }
                if (lblData.Text != "HL7MessageDate")
                {
                    lbl.Attributes.Add("oncontextmenu", "ShowPopupFilterWindow('" + dbField + "','" + dataValue.ToString() + "','" + lbl.ClientID + "'," + IsParsingField + "); return false;"); //ShowFilterWindow('MRN', '1245689');
                }
            }
        }
    }
    class HL7DataTemplete : ITemplate
    {
        private Label lbl = new Label();
        private Label lblId = new Label();
        private Button btn = new Button();
        ListItemType TypeOfTemplete;
        string DatabaseFieldName;
        Int16 textLength;
        String messageType;


        public HL7DataTemplete(ListItemType Templete, String MessageType)
        {
            TypeOfTemplete = Templete;
            //DatabaseFieldName = Field;
            //textLength = TextLength;
            messageType = MessageType;
        }
        public void InstantiateIn(Control container)
        {
            //lbl = new Label();
            btn = new Button();
            btn.DataBinding += new EventHandler(btnHL7DataBinding);
            //lbl.DataBinding += new EventHandler(HL7DataBinding);
            container.Controls.Add(btn);
            //container.Controls.Add(lbl);
        }
        private void btnHL7DataBinding(object sender, EventArgs e)
        {
            Button btnData = (Button)sender;
            GridViewRow container = btnData.NamingContainer as GridViewRow;
            object IdValue = DataBinder.Eval(container.DataItem, "Id");
            btnData.Text = "HL7";
            btnData.OnClientClick= "OpenNewHL7DataMessageinNewWindow('" + IdValue.ToString() + "','" + messageType + "'); return false;";
        }
        //private void HL7DataBinding(object sender, EventArgs e)
        //{
        //    Label lblData = sender as Label;
        //    GridViewRow container = lblData.NamingContainer as GridViewRow;
        //    object dataValue = DataBinder.Eval(container.DataItem, DatabaseFieldName);
        //    object IdValue = DataBinder.Eval(container.DataItem, "Id");
        //    if (dataValue != DBNull.Value)
        //    {
        //        lblData.Text = dataValue.ToString().Substring(0,textLength);
        //        string dbField = DatabaseFieldName;
                
        //        if (lblData.Text != "HL7MessageDate")
        //        {
        //            lbl.Attributes.Add("oncontextmenu", "ShowPopupHL7ParsingWindow('" + IdValue.ToString() + "','ADT'); return false;"); //ShowFilterWindow('MRN', '1245689');
        //        }
        //    }
        //}
    }
    class GridSpacerTemplete : ITemplate
    {
        private Label lbl;
        String Value;
        ListItemType TypeOfTemplete;
        public GridSpacerTemplete(ListItemType Templete, string SpaceValue)
        {
            TypeOfTemplete = Templete;
            Value = SpaceValue;
        }
        public void InstantiateIn(Control container)
        {
            lbl = new Label();
            lbl.Text = Value;
            container.Controls.Add(lbl);
        }
    }
}
