namespace DataStoreHL7Listener
{
    partial class HL7ListenerService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HL7ListenerEvents = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.HL7ListenerEvents)).BeginInit();
            // 
            // HL7ListenerEvents
            // 
            this.HL7ListenerEvents.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.HL7ListenerEvents_EntryWritten);
            // 
            // HL7ListenerService
            // 
            this.ServiceName = "Data Store HL7 Listener";
            ((System.ComponentModel.ISupportInitialize)(this.HL7ListenerEvents)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog HL7ListenerEvents;
    }
}
