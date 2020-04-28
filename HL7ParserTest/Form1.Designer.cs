namespace HL7ParserTest
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtHL7Message = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtParsedValue = new System.Windows.Forms.TextBox();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnScrub = new System.Windows.Forms.Button();
            this.txtScrubValue = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtScubbedMessage = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnReplace = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtHL7Message
            // 
            this.txtHL7Message.Location = new System.Drawing.Point(12, 36);
            this.txtHL7Message.Name = "txtHL7Message";
            this.txtHL7Message.Size = new System.Drawing.Size(695, 120);
            this.txtHL7Message.TabIndex = 0;
            this.txtHL7Message.Text = resources.GetString("txtHL7Message.Text");
            this.txtHL7Message.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "HL7 Message";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(610, 164);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Parse Message";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtParsedValue
            // 
            this.txtParsedValue.Location = new System.Drawing.Point(16, 203);
            this.txtParsedValue.Name = "txtParsedValue";
            this.txtParsedValue.Size = new System.Drawing.Size(691, 20);
            this.txtParsedValue.TabIndex = 3;
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(495, 167);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(100, 20);
            this.txtLocation.TabIndex = 4;
            this.txtLocation.Text = "PID5.2";
            // 
            // btnScrub
            // 
            this.btnScrub.Location = new System.Drawing.Point(610, 229);
            this.btnScrub.Name = "btnScrub";
            this.btnScrub.Size = new System.Drawing.Size(97, 23);
            this.btnScrub.TabIndex = 5;
            this.btnScrub.Text = "Scrub Message";
            this.btnScrub.UseVisualStyleBackColor = true;
            this.btnScrub.Click += new System.EventHandler(this.btnScrub_Click);
            // 
            // txtScrubValue
            // 
            this.txtScrubValue.Location = new System.Drawing.Point(495, 232);
            this.txtScrubValue.Name = "txtScrubValue";
            this.txtScrubValue.Size = new System.Drawing.Size(100, 20);
            this.txtScrubValue.TabIndex = 6;
            this.txtScrubValue.Text = "Marc";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 167);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 7;
            this.btnLoad.Text = "Load Message";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtScubbedMessage
            // 
            this.txtScubbedMessage.Location = new System.Drawing.Point(27, 285);
            this.txtScubbedMessage.Name = "txtScubbedMessage";
            this.txtScubbedMessage.Size = new System.Drawing.Size(695, 174);
            this.txtScubbedMessage.TabIndex = 8;
            this.txtScubbedMessage.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 269);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Scrubbed Message";
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(16, 230);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(96, 23);
            this.btnReplace.TabIndex = 10;
            this.btnReplace.Text = "Replace Values";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 471);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtScubbedMessage);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtScrubValue);
            this.Controls.Add(this.btnScrub);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.txtParsedValue);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtHL7Message);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtHL7Message;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtParsedValue;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnScrub;
        private System.Windows.Forms.TextBox txtScrubValue;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.RichTextBox txtScubbedMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReplace;
    }
}

