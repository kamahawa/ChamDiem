namespace ChamDiem.Presentation
{
    partial class frmEditBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditBox));
            this._txtContent = new DevExpress.XtraEditors.TextEdit();
            this._btnOK = new DevExpress.XtraEditors.SimpleButton();
            this._btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this._txtContent.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // _txtContent
            // 
            this._txtContent.Location = new System.Drawing.Point(13, 13);
            this._txtContent.Name = "_txtContent";
            this._txtContent.Size = new System.Drawing.Size(435, 20);
            this._txtContent.TabIndex = 0;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(129, 43);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 1;
            this._btnOK.Text = "OK";
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(225, 43);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "Hủy";
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // frmEditBox
            // 
            this.AcceptButton = this._btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 78);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._txtContent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(476, 117);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(476, 117);
            this.Name = "frmEditBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thay đổi tên";
            ((System.ComponentModel.ISupportInitialize)(this._txtContent.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit _txtContent;
        private DevExpress.XtraEditors.SimpleButton _btnOK;
        private DevExpress.XtraEditors.SimpleButton _btnCancel;
    }
}