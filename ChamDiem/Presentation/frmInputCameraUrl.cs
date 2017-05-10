using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace ChamDiem
{
    public partial class frmInputCameraUrl : DevExpress.XtraEditors.XtraForm
    {
        //3 bia moi bia 8 be
        const int SO_BE_BAN = 24;

        //bien de kiem tra co thay doi url hay k
        public static bool isOkClick = false;
        public frmInputCameraUrl()
        {
            InitializeComponent();
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            string[] urlArr = new string[SO_BE_BAN];
            try
            {
                DataTable dt = (DataTable)_gridControlUrl.DataSource;
                for (int i = 0; i <= SO_BE_BAN; i++)
                {
                    //lay phan tu trong url ghi vao mang va luu xuong
                    urlArr[i] = dt.Rows[i].ItemArray[1].ToString();
                }
            }
            catch(Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
            frmMain.WriteUrlCameraArray(urlArr);
            isOkClick = true;
            this.Close();
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            isOkClick = false;
            this.Close();
        }

        private void frmInputCameraUrl_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Bệ bắn");
            dt.Columns.Add("Đường dẫn url");
            // 3 bia, moi bia 8 be
            for(int i = 1; i <= SO_BE_BAN; i++)
            {
                DataRow row = dt.NewRow();
                if(i < 9)
                {
                    row[0] = "Bia 4 bệ số " + i;
                }
                else if(i < 17)
                {
                    row[0] = "Bia 7 bệ số " + (i - 8);
                }
                else
                {
                    row[0] = "Bia 8 bệ số " + (i - 17);
                }
                // urlCamera bat dau tu 0
                row[1] = frmMain.urlCamera[i - 1];
                dt.Rows.Add(row);
            }

            _gridViewUrl.Columns.Clear();
            _gridControlUrl.DataSource = null;
            _gridControlUrl.DataSource = dt;
            _gridViewUrl.BestFitColumns();
            isOkClick = false;
        }
    }
}