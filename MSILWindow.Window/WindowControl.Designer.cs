namespace MSILWindow.Window
{
    partial class WindowControl
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
            this.result = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // result
            // 
            this.result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.result.Multiline = true;
            this.result.ReadOnly = true;
            this.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace, 9f, System.Drawing.FontStyle.Bold);
            this.result.Location = new System.Drawing.Point(0, 0);
            this.result.Name = "result";
            this.result.Size = new System.Drawing.Size(525, 466);
            this.result.TabIndex = 0;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.result);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(525, 466);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox result;

    }
}
