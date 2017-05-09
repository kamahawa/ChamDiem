namespace ChamDiem
{
    partial class frmRegistration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRegistration));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this._txtProductId = new DevExpress.XtraEditors.TextEdit();
            this._txtProductKey = new DevExpress.XtraEditors.TextEdit();
            this._btnRegister = new DevExpress.XtraEditors.SimpleButton();
            this._btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this._txtProductId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._txtProductKey.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(10, 10);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(70, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "ID sản phẩm : ";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(10, 40);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(73, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Mã sản phẩm : ";
            // 
            // _txtProductId
            // 
            this._txtProductId.Location = new System.Drawing.Point(90, 10);
            this._txtProductId.Name = "_txtProductId";
            this._txtProductId.Properties.ReadOnly = true;
            this._txtProductId.Size = new System.Drawing.Size(475, 20);
            this._txtProductId.TabIndex = 2;
            // 
            // _txtProductKey
            // 
            this._txtProductKey.Location = new System.Drawing.Point(90, 40);
            this._txtProductKey.Name = "_txtProductKey";
            this._txtProductKey.Size = new System.Drawing.Size(475, 20);
            this._txtProductKey.TabIndex = 3;
            // 
            // _btnRegister
            // 
            this._btnRegister.Location = new System.Drawing.Point(172, 66);
            this._btnRegister.Name = "_btnRegister";
            this._btnRegister.Size = new System.Drawing.Size(75, 23);
            this._btnRegister.TabIndex = 4;
            this._btnRegister.Text = "Đăng ký";
            this._btnRegister.Click += new System.EventHandler(this._btnRegister_Click);
            // 
            // _btnClose
            // 
            this._btnClose.Location = new System.Drawing.Point(265, 66);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(75, 23);
            this._btnClose.TabIndex = 5;
            this._btnClose.Text = "Đóng";
            this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
            // 
            // frmRegistration
            // 
            this.AcceptButton = this._btnRegister;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 96);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnRegister);
            this.Controls.Add(this._txtProductKey);
            this.Controls.Add(this._txtProductId);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 135);
            this.MinimumSize = new System.Drawing.Size(600, 135);
            this.Name = "frmRegistration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmRegistration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRegistration_FormClosing);
            this.Load += new System.EventHandler(this.frmRegistration_Load);
            ((System.ComponentModel.ISupportInitialize)(this._txtProductId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._txtProductKey.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit _txtProductId;
        private DevExpress.XtraEditors.TextEdit _txtProductKey;
        private DevExpress.XtraEditors.SimpleButton _btnRegister;
        private DevExpress.XtraEditors.SimpleButton _btnClose;
    }
}