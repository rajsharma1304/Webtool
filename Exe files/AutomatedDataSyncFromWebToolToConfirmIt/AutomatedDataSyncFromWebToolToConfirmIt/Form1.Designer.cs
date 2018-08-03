namespace AutomatedDataSyncFromWebToolToConfirmIt
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
            this.btnInternational = new System.Windows.Forms.Button();
            this.btnEurope = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInternational
            // 
            this.btnInternational.Location = new System.Drawing.Point(101, 48);
            this.btnInternational.Name = "btnInternational";
            this.btnInternational.Size = new System.Drawing.Size(75, 23);
            this.btnInternational.TabIndex = 0;
            this.btnInternational.Text = "International";
            this.btnInternational.UseVisualStyleBackColor = true;
            //this.btnInternational.Click += new System.EventHandler(this.btnInternational_Click);
            // 
            // btnEurope
            // 
            this.btnEurope.Enabled = false;
            this.btnEurope.Location = new System.Drawing.Point(105, 120);
            this.btnEurope.Name = "btnEurope";
            this.btnEurope.Size = new System.Drawing.Size(75, 23);
            this.btnEurope.TabIndex = 1;
            this.btnEurope.Text = "Europe";
            this.btnEurope.UseVisualStyleBackColor = true;
            this.btnEurope.Visible = false;
            //this.btnEurope.Click += new System.EventHandler(this.btnEurope_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(105, 187);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnEurope);
            this.Controls.Add(this.btnInternational);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInternational;
        private System.Windows.Forms.Button btnEurope;
        private System.Windows.Forms.Button button3;
    }
}

