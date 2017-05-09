namespace ChamDiem
{
    partial class frmInputCameraUrl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInputCameraUrl));
            this._btnOK = new DevExpress.XtraEditors.SimpleButton();
            this._btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this._gridControlUrl = new DevExpress.XtraGrid.GridControl();
            this._gridViewUrl = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this._gridControlUrl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewUrl)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnOK
            // 
            this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnOK.Location = new System.Drawing.Point(275, 595);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 8;
            this._btnOK.Text = "OK";
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnCancel.Location = new System.Drawing.Point(370, 595);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 8;
            this._btnCancel.Text = "Hủy";
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _gridControlUrl
            // 
            this._gridControlUrl.Location = new System.Drawing.Point(4, 5);
            this._gridControlUrl.MainView = this._gridViewUrl;
            this._gridControlUrl.Name = "_gridControlUrl";
            this._gridControlUrl.Size = new System.Drawing.Size(735, 584);
            this._gridControlUrl.TabIndex = 9;
            this._gridControlUrl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this._gridViewUrl});
            // 
            // _gridViewUrl
            // 
            this._gridViewUrl.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this._gridViewUrl.GridControl = this._gridControlUrl;
            this._gridViewUrl.Name = "_gridViewUrl";
            this._gridViewUrl.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Default;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Bệ bắn";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Đường dẫn url";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // frmInputCameraUrl
            // 
            this.AcceptButton = this._btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 627);
            this.Controls.Add(this._gridControlUrl);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(760, 687);
            this.MinimizeBox = false;
            this.Name = "frmInputCameraUrl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nhập địa chỉ camera";
            this.Load += new System.EventHandler(this.frmInputCameraUrl_Load);
            ((System.ComponentModel.ISupportInitialize)(this._gridControlUrl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewUrl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton _btnOK;
        private DevExpress.XtraEditors.SimpleButton _btnCancel;
        private DevExpress.XtraGrid.GridControl _gridControlUrl;
        private DevExpress.XtraGrid.Views.Grid.GridView _gridViewUrl;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
    }
}