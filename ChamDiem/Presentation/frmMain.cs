using CameraApp.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FoxLearn.License;
using WebEye;
using ChamDiem.Presentation;
using System.Threading;
using System.Media;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using CameraApp.Business;

namespace ChamDiem
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //bia de chon ban
        const int SELECT_BIA_4 = 1;
        const int SELECT_BIA_7 = 2;
        const int SELECT_BIA_8 = 3;
        const int SELECT_3_BIA = 4;

        //bien mac dinh ve luot
        const int LUOT_KHOI_DAU = 1;
        const int MEMBER_KHOI_DAU = 0;
        
        //3 bia moi bia 8 be
        const int SO_BE_BAN = 24;

        //array camera url
        public static string[] urlCamera = new string[SO_BE_BAN];

        //luot ban
        private int _luotBia1 = 1, _luotBia2 = 1, _luotBia3 = 1;

        //nguoi ban hien tai o cac bia
        private int _currentMemberBia1 = 0, _currentMemberBia2 = 0, _currentMemberBia3 = 0;

        //nguoi ban hien tai o cac bia
        private int _currentTurn1 = 0, _currentTurn2 = 0, _currentTurn3 = 0;

        //tong diem 3 bia cua cac nguoi ban
        private int[] _tong3Bia;

        //chon bia de ban
        private int _selectedBia = SELECT_3_BIA;// mac dinh la chon 3 bia
        
        public frmMain()
        {
            InitializeComponent();
            //kiem tra license
            KeyManager km = new KeyManager(ComputerInfo.GetComputerId());
            LicenseInfo lic = new LicenseInfo();
            int value = km.LoadSuretyFile(string.Format(@"{0}\Key.lic", Application.StartupPath), ref lic);
            string productKey = lic.ProductKey;
            bool isActive = false;//bien kiem tra kich hoat
            if (km.ValidKey(ref productKey))
            {
                KeyValuesClass kv = new KeyValuesClass();
                if (km.DisassembleKey(productKey, ref kv))
                {
                    if (kv.Type == LicenseType.TRIAL)
                    {
                        //dung thu ma con ngay thi cho chay
                        if ((kv.Expiration - DateTime.Now.Date).Days > 0)
                        {
                            isActive = true;
                        }
                    }
                    else
                    {
                        isActive = true;
                    }
                }
            }

            if (isActive)
            {
                //da kich hoat thi load url camera
                LoadUrlCamera();
                LoadCamera();
            }
            else
            {
                using (frmRegistration frm = new frmRegistration())
                {
                    this.Hide();
                    frm.ShowDialog();
                    Application.Restart();
                }
            }
        }

        private void _btnExportScoreFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(_selectedBia == SELECT_3_BIA)
            {
                ExcelHelp.ExportExcel((DataTable)_gridControlScore4.DataSource, (DataTable)_gridControlScore7.DataSource, (DataTable)_gridControlScore8.DataSource);
            }
            else if (_selectedBia == SELECT_BIA_4)
            {
                ExcelHelp.ExportExcel1Bia((DataTable)_gridControlScore4.DataSource);
            }

            else if (_selectedBia == SELECT_BIA_7)
            {
                ExcelHelp.ExportExcel1Bia((DataTable)_gridControlScore7.DataSource);
            }

            else if (_selectedBia == SELECT_BIA_8)
            {
                ExcelHelp.ExportExcel1Bia((DataTable)_gridControlScore8.DataSource);
            }

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }

        private void _btnInputCameraUrl_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmInputCameraUrl frm = new frmInputCameraUrl();
            frm.ShowDialog();
            if(frmInputCameraUrl.isOkClick)
            {
                LoadUrlCamera();
                LoadCamera();
            }
        }

        private void _btnChangeTitle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmEditBox frm = new frmEditBox("Đổi tên form");
            frm.ShowDialog();
            if (frmEditBox.content.Trim() != "")
            {
                this.Text = frmEditBox.content;
            }
        }

        private void _btnAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.Show();
        }

        private void _btnRegister_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmRegistration frm = new frmRegistration();
            frm.ShowDialog();
        }

        private void _btnDeleteShot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDiemBan();
        }

        private void _btnMissShot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(_tabControlCamera.SelectedTabPageIndex == 0)
            {
                _lblScore4.Text = "0";
                ChamDiem(0, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, 1);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1)
            {
                _lblScore7.Text = "0";
                ChamDiem(0, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, 2);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2)
            {
                _lblScore8.Text = "0";
                ChamDiem(0, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, 3);
            }
        }

        private void _btnAddList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "xls, xlsx|*.xls;*.xlsx";
            try
            {
                if (op.ShowDialog() == DialogResult.OK)
                {
                    if(_selectedBia == SELECT_3_BIA)
                    {
                        //load info be so 1
                        DataTable table = ExcelHelp.getDataTableExcelFor3Bia(op.FileName);
                        _tong3Bia = new int[table.Rows.Count];

                        _gridViewScore4.Columns.Clear();
                        _gridControlScore4.DataSource = null;
                        _gridControlScore4.DataSource = table;
                        _gridViewScore4.BestFitColumns();
                        
                        //load info be so 2
                        DataTable table2 = table.Copy();//ExcelHelp.getDataTableExcel(op.FileName);
                        _gridViewScore7.Columns.Clear();
                        _gridControlScore7.DataSource = null;
                        _gridControlScore7.DataSource = table2;
                        _gridViewScore7.BestFitColumns();

                        //load info be so 3
                        DataTable table3 = table.Copy();//ExcelHelp.getDataTableExcel(op.FileName);
                        _gridViewScore8.Columns.Clear();
                        _gridControlScore8.DataSource = null;
                        _gridControlScore8.DataSource = table3;
                        _gridViewScore8.BestFitColumns();

                        //load ten vao cac camera
                        LoadNameBia4(table);
                        LoadNameBia7(table);
                        LoadNameBia8(table);
                    }                    
                    else if(_selectedBia == SELECT_BIA_4)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore4.Columns.Clear();
                        _gridControlScore4.DataSource = null;
                        _gridControlScore4.DataSource = table;
                        _gridViewScore4.BestFitColumns();
                        LoadNameBia4(table);
                    }
                    else if (_selectedBia == SELECT_BIA_7)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore7.Columns.Clear();
                        _gridControlScore7.DataSource = null;
                        _gridControlScore7.DataSource = table;
                        _gridViewScore7.BestFitColumns();
                        LoadNameBia7(table);
                    }
                    else //if (_selectedBia == BAN_BIA_8)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore8.Columns.Clear();
                        _gridControlScore8.DataSource = null;
                        _gridControlScore8.DataSource = table;
                        _gridViewScore8.BestFitColumns();
                        LoadNameBia8(table);
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }
        
        private void _spcCamera4_StreamFailed(object sender, StreamFailedEventArgs e)
        {
            _barStaticItem4.Caption = "Camera kết nối thất bại";
            _btnReconnect4.Enabled = true;
        }

        private void _spcCamera4_StreamStarted(object sender, EventArgs e)
        {
            _barStaticItem4.Caption = "Camera đang chạy";
            _btnReconnect4.Enabled = false;
        }

        private void _spcCamera7_StreamFailed(object sender, StreamFailedEventArgs e)
        {
            _barStaticItem7.Caption = "Camera kết nối thất bại";
            _btnReconnect7.Enabled = true;
        }

        private void _spcCamera7_StreamStarted(object sender, EventArgs e)
        {
            _barStaticItem7.Caption = "Camera đang chạy";
            _btnReconnect7.Enabled = false;
        }

        private void _spcCamera8_StreamFailed(object sender, StreamFailedEventArgs e)
        {
            _barStaticItem8.Caption = "Camera kết nối thất bại";
            _btnReconnect8.Enabled = true;
        }

        private void _spcCamera8_StreamStarted(object sender, EventArgs e)
        {
            _barStaticItem8.Caption = "Camera đang chạy";
            _btnReconnect8.Enabled = false;
        }

        private void _transpCtrl4_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore4.DataSource == null)
            {
                return;
            }
            addShotIcon(e.X, e.Y, _panCam4);
            XuLyBe1(e.X, e.Y);
        }

        private void _transpCtrl7_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore7.DataSource == null)
            {
                return;
            }
            addShotIcon(e.X, e.Y, _panCam7);
            XuLyBe2(e.X, e.Y);
        }

        private void _transpCtrl8_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore8.DataSource == null)
            {
                return;
            }
            addShotIcon(e.X, e.Y, _panCam8);
            XuLyBe3(e.X, e.Y);
        }
        
        private void _btnReconnect4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var uri = new Uri(urlCamera[0]);
            _spcCamera4.StartPlay(uri, TimeSpan.FromSeconds(15.0));
            _barStaticItem4.Caption = "Camera đang kết nối...";
        }

        private void _btnReconnect7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var uri = new Uri(urlCamera[1]);
            _spcCamera7.StartPlay(uri, TimeSpan.FromSeconds(15.0));
            _barStaticItem7.Caption = "Camera đang kết nối...";
        }

        private void _btnReconnect8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var uri = new Uri(urlCamera[2]);
            _spcCamera8.StartPlay(uri, TimeSpan.FromSeconds(15.0));
            _barStaticItem8.Caption = "Camera đang kết nối...";
        }

        private void _btnChooseBia4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //neu bia ban ma la bia hien tai thi k lam gi
            if (_selectedBia == SELECT_BIA_4)
            {
                return;
            }
            DialogResult dr = MessageBox.Show("Bạn có muốn chuyển sang bắn bia 4? Điểm bắn ở các bia khác sẽ bị mất.", "Thông báo", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                _selectedBia = SELECT_BIA_4;
                //enable tab page 4 and disable tab page 7, tab page 8
                _tabCamera4.PageEnabled = true;
                _tabCamera7.PageEnabled = false;
                _tabCamera8.PageEnabled = false;
                refreshControl();
            }
        }

        private void _btnChooseBia7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //neu bia ban ma la bia hien tai thi k lam gi
            if (_selectedBia == SELECT_BIA_7)
            {
                return;
            }
            DialogResult dr = MessageBox.Show("Bạn có muốn chuyển sang bắn bia 7? Điểm bắn ở các bia khác sẽ bị mất.", "Thông báo", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                _selectedBia = SELECT_BIA_7;
                //enable tab page 7 and disable tab page 4, tab page 8
                _tabCamera4.PageEnabled = false;
                _tabCamera7.PageEnabled = true;
                _tabCamera8.PageEnabled = false;
                refreshControl();
            }
        }

        private void _btnChooseBia8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //neu bia ban ma la bia hien tai thi k lam gi
            if (_selectedBia == SELECT_BIA_8)
            {
                return;
            }
            DialogResult dr = MessageBox.Show("Bạn có muốn chuyển sang bắn bia 8? Điểm bắn ở các bia khác sẽ bị mất.", "Thông báo", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                _selectedBia = SELECT_BIA_8;
                //enable tab page 8 and disable tab page 7, tab page 4
                _tabCamera4.PageEnabled = false;
                _tabCamera7.PageEnabled = false;
                _tabCamera8.PageEnabled = true;
                refreshControl();
            }
        }

        private void _btnChoose3Bia_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //neu bia ban ma la bia hien tai thi k lam gi
            if (_selectedBia == SELECT_3_BIA)
            {
                return;
            }
            DialogResult dr = MessageBox.Show("Bạn có muốn chuyển sang bắn 3 bia? Điểm bắn ở các bia cũ sẽ bị mất.", "Thông báo", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                _selectedBia = SELECT_3_BIA;
                //enable tab page 4, tab page 7, tab page 8
                _tabCamera4.PageEnabled = true;
                _tabCamera7.PageEnabled = true;
                _tabCamera8.PageEnabled = true;
                //chon tab 4 de show
                _tabControlCamera.SelectedTabPage = _tabCamera4;
                refreshControl();
            }
        }

        private void _btnNextTurns_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_tabControlCamera.SelectedTabPageIndex == 0)
            {
                _currentTurn1 += 8;
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1)
            {
                _currentTurn2 += 8;
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2)
            {
                _currentTurn3 += 8;
            }
        }
        
        #region ---------- XU LY CHAM DIEM ----------

        private void addShotIcon(int x, int y, PanelControl panCam)
        {
            PictureBox px = new PictureBox();
            px.Size = new Size(4, 4);
            px.BackColor = Color.Transparent;
            px.SizeMode = PictureBoxSizeMode.StretchImage;
            px.Image = Properties.Resources.x;
            // tru di 4 don vi de chinh giua hinh
            px.Location = new Point(x - 2, y - 2);
            //_panCam.Controls.Add(px);
            panCam.Controls.Add(px);
            px.BringToFront();
        }

        private void ChamDiem(int diem, ref int luot, ref int currentMember, GridControl dtgScore, LabelControl lblName, LabelControl lblScore, int be)
        {
            try
            {
                DataTable dt = (DataTable)dtgScore.DataSource;
                dt.Rows[currentMember][luot] = diem;

                if(_selectedBia == SELECT_3_BIA)
                {
                    //set tong diem
                    _tong3Bia[currentMember] += diem;
                    //set tong diem
                    setTongDiem3Bia(currentMember);
                }

                docDiem(diem);

                //set ten va diem 
                if (luot == 1)
                {
                    lblName.Text = dt.Rows[currentMember][0].ToString();
                    //neu con nguoi ban thi show nguoi ban ke tiep len
                    /*
                    if (currentMember + 1 < dt.Rows.Count)
                    {
                        lblNameTurns.Text = dt.Rows[currentMember + 1][0].ToString();
                    }
                    */
                }

                if (luot == 3)
                {
                    //ban het luot thi tinh tong diem
                    int l1 = Int32.Parse(dt.Rows[currentMember][1].ToString());
                    int l2 = Int32.Parse(dt.Rows[currentMember][2].ToString());
                    int l3 = Int32.Parse(dt.Rows[currentMember][3].ToString());

                    int tong = l1 + l2 + l3;
                    
                    //tong diem
                    dt.Rows[currentMember][4] = tong;

                    //neu khong phai la 3 bia thi cho xep loai luon o day.
                    if(_selectedBia != SELECT_3_BIA)
                    {
                        XepLoai1Bia(tong, currentMember, dt);
                    }

                    luot = 1;
                    currentMember++; // het luot thi nguoi khac vao ban
                    
                    Thread thread = new Thread(() => soundDiem(be, tong));
                    thread.Start();
                }
                else
                {
                    luot++;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        private void XepLoai1Bia(int tong, int currentMember, DataTable dt)
        {
            try
            {
                string xeploai = "Không đạt";// ngoai dieu kien ben duoi la khong dat thanh tich           
                if (tong >= 24)
                {
                    xeploai = "Giỏi";
                }
                else if (tong >= 19 && tong <= 23)
                {
                    xeploai = "Khá";
                }
                else if (tong >= 15 && tong <= 18)
                {
                    xeploai = "Đạt";
                }

                dt.Rows[currentMember][5] = xeploai;
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        private void soundDiem(int be, int tong)
        {

            //nghi 1 giay de doc so diem o luot 3
            Thread.Sleep(1000);
            //neu la 3 bia thi kiem tra xem dang o be nao, con khong thi chi doc tong
            if(_selectedBia == SELECT_3_BIA)
            {
                //neu la o be nao thi phat len tong o be do
                if (be == 1)
                {
                    //playSound(@"ScoreSound\Tong_diem_be_so_1_la.wav");
                    playSound(Properties.Resources.Tong_diem_be_so_1_la);
                }
                else if (be == 2)
                {
                    //playSound(@"ScoreSound\Tong_diem_be_so_2_la.wav");
                    playSound(Properties.Resources.Tong_diem_be_so_2_la);
                }
                else if (be == 3)
                {
                    //playSound(@"ScoreSound\Tong_diem_be_so_3_la.wav");
                    playSound(Properties.Resources.Tong_diem_be_so_3_la);
                }
                Thread.Sleep(2000);
            }
            else
            {
                playSound(Properties.Resources.Tong_diem_ban_la);
                Thread.Sleep(1500);
            }
            
            //nghi 2 giay de doc so diem o luot tong diem be
            docDiem(tong);
        }

        private void playSound(string fileName)
        {
            SoundPlayer simpleSound = new SoundPlayer(fileName);
            simpleSound.Play();
        }

        private void playSound(Stream file)
        {
            SoundPlayer simpleSound = new SoundPlayer(file);
            simpleSound.Play();
        }

        private void docDiem(int diem)
        {
            switch (diem)
            {
                case 0:
                    playSound(Properties.Resources._0_diem);
                    break;
                case 1:
                    playSound(Properties.Resources._1_diem);
                    break;
                case 2:
                    playSound(Properties.Resources._2_diem);
                    break;
                case 3:
                    playSound(Properties.Resources._3_diem);
                    break;
                case 4:
                    playSound(Properties.Resources._4_diem);
                    break;
                case 5:
                    playSound(Properties.Resources._5_diem);
                    break;
                case 6:
                    playSound(Properties.Resources._6_diem);
                    break;
                case 7:
                    playSound(Properties.Resources._7_diem);
                    break;
                case 8:
                    playSound(Properties.Resources._8_diem);
                    break;
                case 9:
                    playSound(Properties.Resources._9_diem);
                    break;
                case 10:
                    playSound(Properties.Resources._10_diem);
                    break;
                case 11:
                    playSound(Properties.Resources._11_diem);
                    break;
                case 12:
                    playSound(Properties.Resources._12_diem);
                    break;
                case 13:
                    playSound(Properties.Resources._13_diem);
                    break;
                case 14:
                    playSound(Properties.Resources._14_diem);
                    break;
                case 15:
                    playSound(Properties.Resources._15_diem);
                    break;
                case 16:
                    playSound(Properties.Resources._16_diem);
                    break;
                case 17:
                    playSound(Properties.Resources._17_diem);
                    break;
                case 18:
                    playSound(Properties.Resources._18_diem);
                    break;
                case 19:
                    playSound(Properties.Resources._19_diem);
                    break;
                case 20:
                    playSound(Properties.Resources._20_diem);
                    break;
                case 21:
                    playSound(Properties.Resources._21_diem);
                    break;
                case 22:
                    playSound(Properties.Resources._22_diem);
                    break;
                case 23:
                    playSound(Properties.Resources._23_diem);
                    break;
                case 24:
                    playSound(Properties.Resources._24_diem);
                    break;
                case 25:
                    playSound(Properties.Resources._25_diem);
                    break;
                case 26:
                    playSound(Properties.Resources._26_diem);
                    break;
                case 27:
                    playSound(Properties.Resources._27_diem);
                    break;
                case 28:
                    playSound(Properties.Resources._28_diem);
                    break;
                case 29:
                    playSound(Properties.Resources._29_diem);
                    break;
                case 30:
                    playSound(Properties.Resources._30_diem);
                    break;
            }
        }
        
        private void setTongDiem3Bia(int currentMember)
        {
            string xeploai = "Không đạt";// ngoai dieu kien ben duoi la khong dat thanh tich
            if (_tong3Bia[currentMember] >= 72)
            {
                xeploai = "Giỏi";
            }
            else if (_tong3Bia[currentMember] >= 59 && _tong3Bia[currentMember] <= 71)
            {
                xeploai = "Khá";
            }
            else if (_tong3Bia[currentMember] >= 45 && _tong3Bia[currentMember] <= 58)
            {
                xeploai = "Đạt";
            }
            DataTable dt = (DataTable)_gridControlScore4.DataSource;
            dt.Rows[currentMember][5] = _tong3Bia[currentMember];
            dt.Rows[currentMember][6] = xeploai;

            DataTable dt2 = (DataTable)_gridControlScore7.DataSource;
            dt2.Rows[currentMember][5] = _tong3Bia[currentMember];
            dt2.Rows[currentMember][6] = xeploai;

            DataTable dt3 = (DataTable)_gridControlScore8.DataSource;
            dt3.Rows[currentMember][5] = _tong3Bia[currentMember];
            dt3.Rows[currentMember][6] = xeploai;
        }
        
        #endregion

        #region --------- FUNCTION CODE ---------
        void LoadUrlCamera()
        {
            string path = @"UrlCamera.txt";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                MessageBox.Show("Vui lòng nhập đường dẫn camera.");
            }
            else
            {
                try
                {
                    //Pass the file path and file name to the StreamReader constructor
                    StreamReader sr = new StreamReader(path);

                    //Read the first line of text
                    String line = sr.ReadLine();

                    for (int i = 0; i < SO_BE_BAN; i++)
                    {
                        if (line != null)
                        {
                            urlCamera[i] = line;

                            //Read the next line
                            line = sr.ReadLine();
                        }
                    }
                    //close the file
                    sr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText("log.txt", ex.ToString());
                }
            }
        }

        void LoadCamera()
        {           
            try
            {
                LoadCameraSmall(urlCamera[0], _spcSmallCamera41);
                LoadCameraSmall(urlCamera[1], _spcSmallCamera42);
                LoadCameraSmall(urlCamera[2], _spcSmallCamera43);
                LoadCameraSmall(urlCamera[3], _spcSmallCamera44);
                LoadCameraSmall(urlCamera[4], _spcSmallCamera45);
                LoadCameraSmall(urlCamera[5], _spcSmallCamera46);
                LoadCameraSmall(urlCamera[6], _spcSmallCamera47);
                LoadCameraSmall(urlCamera[7], _spcSmallCamera48);
                LoadCameraSmall(urlCamera[8], _spcSmallCamera71);
                LoadCameraSmall(urlCamera[9], _spcSmallCamera72);
                LoadCameraSmall(urlCamera[10], _spcSmallCamera73);
                LoadCameraSmall(urlCamera[11], _spcSmallCamera74);
                LoadCameraSmall(urlCamera[12], _spcSmallCamera75);
                LoadCameraSmall(urlCamera[13], _spcSmallCamera76);
                LoadCameraSmall(urlCamera[14], _spcSmallCamera77);
                LoadCameraSmall(urlCamera[15], _spcSmallCamera78);
                LoadCameraSmall(urlCamera[16], _spcSmallCamera81);
                LoadCameraSmall(urlCamera[17], _spcSmallCamera82);
                LoadCameraSmall(urlCamera[18], _spcSmallCamera83);
                LoadCameraSmall(urlCamera[19], _spcSmallCamera84);
                LoadCameraSmall(urlCamera[20], _spcSmallCamera85);
                LoadCameraSmall(urlCamera[21], _spcSmallCamera86);
                LoadCameraSmall(urlCamera[22], _spcSmallCamera87);
                LoadCameraSmall(urlCamera[23], _spcSmallCamera88);

                /*
                if (urlCamera[0] != "")
                {
                    var uri = new Uri(urlCamera[0]);
                    _spcCamera4.StartPlay(uri, TimeSpan.FromSeconds(15.0));
                    _barStaticItem4.Caption = "Camera đang kết nối...";
                }
                else
                {
                    _barStaticItem4.Caption = "Vui lòng nhập đường dẫn cho camera bia 4";
                }
                if (urlCamera[1] != "")
                {
                    var uri = new Uri(urlCamera[1]);
                    _spcCamera7.StartPlay(uri, TimeSpan.FromSeconds(15.0));
                    _barStaticItem7.Caption = "Camera đang kết nối...";
                }
                else
                {
                    _barStaticItem7.Caption = "Vui lòng nhập đường dẫn cho camera bia 7";
                }
                if (urlCamera[2] != "")
                {
                    var uri = new Uri(urlCamera[2]);
                    _spcCamera8.StartPlay(uri, TimeSpan.FromSeconds(15.0));
                    _barStaticItem8.Caption = "Camera đang kết nối...";
                }
                else
                {
                    _barStaticItem8.Caption = "Vui lòng nhập đường dẫn cho camera bia 8";
                }
                */
            }
            catch(Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        void LoadCameraSmall(string url, StreamPlayerControl spc)
        {
            if (url != "")
            {
                var uri = new Uri(url);
                spc.StartPlay(uri, TimeSpan.FromSeconds(15.0));
            }
        }

        void LoadNameBia4(DataTable table)
        {
            _lblName41.Text = table.Rows[_currentTurn1 + 0][0].ToString();
            _lblName42.Text = table.Rows[_currentTurn1 + 1][0].ToString();
            _lblName43.Text = table.Rows[_currentTurn1 + 2][0].ToString();
            _lblName44.Text = table.Rows[_currentTurn1 + 3][0].ToString();
            _lblName45.Text = table.Rows[_currentTurn1 + 4][0].ToString();
            _lblName46.Text = table.Rows[_currentTurn1 + 5][0].ToString();
            _lblName47.Text = table.Rows[_currentTurn1 + 6][0].ToString();
            _lblName48.Text = table.Rows[_currentTurn1 + 7][0].ToString();
        }

        void LoadNameBia7(DataTable table)
        {
            _lblName71.Text = table.Rows[_currentTurn2 + 0][0].ToString();
            _lblName72.Text = table.Rows[_currentTurn2 + 1][0].ToString();
            _lblName73.Text = table.Rows[_currentTurn2 + 2][0].ToString();
            _lblName74.Text = table.Rows[_currentTurn2 + 3][0].ToString();
            _lblName75.Text = table.Rows[_currentTurn2 + 4][0].ToString();
            _lblName76.Text = table.Rows[_currentTurn2 + 5][0].ToString();
            _lblName77.Text = table.Rows[_currentTurn2 + 6][0].ToString();
            _lblName78.Text = table.Rows[_currentTurn2 + 7][0].ToString();
        }

        void LoadNameBia8(DataTable table)
        {
            _lblName81.Text = table.Rows[_currentTurn3 + 0][0].ToString();
            _lblName82.Text = table.Rows[_currentTurn3 + 1][0].ToString();
            _lblName83.Text = table.Rows[_currentTurn3 + 2][0].ToString();
            _lblName84.Text = table.Rows[_currentTurn3 + 3][0].ToString();
            _lblName85.Text = table.Rows[_currentTurn3 + 4][0].ToString();
            _lblName86.Text = table.Rows[_currentTurn3 + 5][0].ToString();
            _lblName87.Text = table.Rows[_currentTurn3 + 6][0].ToString();
            _lblName88.Text = table.Rows[_currentTurn3 + 7][0].ToString();
        }
        
        private void refreshControl()
        {
            //clear grid control
            _gridViewScore4.Columns.Clear();
            _gridControlScore4.DataSource = null;
            _gridViewScore7.Columns.Clear();
            _gridControlScore7.DataSource = null;
            _gridViewScore8.Columns.Clear();
            _gridControlScore8.DataSource = null;

            //reset ten va diem
            _lblName4.Text = "";
            _lblName7.Text = "";
            _lblName8.Text = "";
            _lblScore4.Text = "";
            _lblScore7.Text = "";
            _lblScore8.Text = "";

            //reset diem ban
            XoaDiemBan();
        }

        private void resetVariable()
        {
            _luotBia1 = LUOT_KHOI_DAU;
            _luotBia2 = LUOT_KHOI_DAU;
            _luotBia3 = LUOT_KHOI_DAU;

            _currentMemberBia1 = MEMBER_KHOI_DAU;
            _currentMemberBia2 = MEMBER_KHOI_DAU;
            _currentMemberBia3 = MEMBER_KHOI_DAU;

            _tong3Bia = null;
        }

        private void XoaDiemBan()
        {
            for (int i = _panCam4.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox control = _panCam4.Controls[i] as PictureBox;
                if (control == null)
                    continue;
                control.Dispose();
            }
            for (int i = _panCam7.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox control = _panCam7.Controls[i] as PictureBox;
                if (control == null)
                    continue;
                control.Dispose();
            }
            for (int i = _panCam8.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox control = _panCam8.Controls[i] as PictureBox;
                if (control == null)
                    continue;
                control.Dispose();
            }
            resetVariable();
        }

        public static void WriteUrlCameraArray(string[] urlArr)
        {
            string path = @"UrlCamera.txt";
            File.WriteAllLines(path, urlArr);
        }

        private void XuLyBe1(int x, int y)
        {
            Bitmap bmResize = ProcessImage.ResizeImage(Properties.Resources.bia4_8, _ptbCamera.Width, _ptbCamera.Height);
            Bitmap bitmap = bmResize;// Properties.Resources.bia4_8;
            int be = 1;// ban o be so 1
            Color c = bitmap.GetPixel(x, y);
            //dung thuat toan quicksort
            if (c.ToArgb().Equals(Color.Red.ToArgb()))
            {
                bmResize = ProcessImage.ResizeImage(Properties.Resources.bia4_9, _ptbCamera.Width, _ptbCamera.Height);
                bitmap = bmResize;//Properties.Resources.bia4_9;
                c = bitmap.GetPixel(x, y);
                if (c.ToArgb().Equals(Color.Red.ToArgb()))
                {
                    bmResize = ProcessImage.ResizeImage(Properties.Resources.bia4_10, _ptbCamera.Width, _ptbCamera.Height);
                    bitmap = bmResize;//Properties.Resources.bia4_10;
                    c = bitmap.GetPixel(x, y);
                    // neu nam o tam 10 thi la 10, khong la 9
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        //10 diem
                        _lblScore4.Text = "10";
                        ChamDiem(10, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //9 diem
                        _lblScore4.Text = "9";
                        ChamDiem(9, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                }
                else
                {
                    // neu khong nam trong 9 thi la 8 diem
                    //8 diem
                    _lblScore4.Text = "8";
                    ChamDiem(8, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                }
            }
            else
            {
                bmResize = ProcessImage.ResizeImage(Properties.Resources.bia4_6, _ptbCamera.Width, _ptbCamera.Height);
                bitmap = bmResize;//Properties.Resources.bia4_6;
                c = bitmap.GetPixel(x, y);
                if (c.ToArgb().Equals(Color.Red.ToArgb()))
                {
                    bmResize = ProcessImage.ResizeImage(Properties.Resources.bia4_7, _ptbCamera.Width, _ptbCamera.Height);
                    bitmap = bmResize;//Properties.Resources.bia4_7;
                    c = bitmap.GetPixel(x, y);
                    // neu nam o tam 7 thi la 7, khong la 6
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        //7 diem
                        _lblScore4.Text = "7";
                        ChamDiem(7, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //6 diem
                        _lblScore4.Text = "6";
                        ChamDiem(6, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                }
                else
                {
                    bmResize = ProcessImage.ResizeImage(Properties.Resources.bia4_5, _ptbCamera.Width, _ptbCamera.Height);
                    bitmap = bmResize;//Properties.Resources.bia4_5;
                    c = bitmap.GetPixel(x, y);
                    //neu nam o trong 5 thi la 5, con o ngoai la truot
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        //5 diem
                        _lblScore4.Text = "5";
                        ChamDiem(5, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //0 diem
                        _lblScore4.Text = "0";
                        ChamDiem(0, ref _luotBia1, ref _currentMemberBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                }
            }
        }

        private void XuLyBe2(int x, int y)
        {
            Bitmap bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_5, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_5;
            int be = 2;// ban o be so 2
            Color c = bitmap.GetPixel(x, y);
            //dung thuat toan quicksort
            if (c.ToArgb().Equals(Color.Red.ToArgb()))
            {
                bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_8, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_8;
                c = bitmap.GetPixel(x, y);
                if (c.ToArgb().Equals(Color.Red.ToArgb()))
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_9, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_9;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_10, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_10;
                        c = bitmap.GetPixel(x, y);
                        if (c.ToArgb().Equals(Color.Red.ToArgb()))
                        {
                            //10 diem
                            _lblScore7.Text = "10";
                            ChamDiem(10, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //9 diem
                            _lblScore7.Text = "9";
                            ChamDiem(9, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        _lblScore7.Text = "8";
                        ChamDiem(8, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                }
                else
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_6, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_6;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_7, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_7;
                        c = bitmap.GetPixel(x, y);
                        if (c.ToArgb().Equals(Color.Red.ToArgb()))
                        {
                            //7 diem
                            _lblScore7.Text = "7";
                            ChamDiem(7, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //6 diem
                            _lblScore7.Text = "6";
                            ChamDiem(6, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        //5 diem
                        _lblScore7.Text = "5";
                        ChamDiem(5, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                }
            }
            else
            {
                bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_3, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_3;
                c = bitmap.GetPixel(x, y);
                if (c.ToArgb().Equals(Color.Red.ToArgb()))
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_4, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_4;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        //4 diem
                        _lblScore7.Text = "4";
                        ChamDiem(4, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                    else
                    {
                        //3 diem
                        _lblScore7.Text = "3";
                        ChamDiem(3, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                }
                else
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_1, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_1;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        bitmap = ProcessImage.ResizeImage(Properties.Resources.bia7_2, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia7_2;
                        c = bitmap.GetPixel(x, y);
                        if (c.ToArgb().Equals(Color.Red.ToArgb()))
                        {
                            //2 diem
                            _lblScore7.Text = "2";
                            ChamDiem(2, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //1 diem
                            _lblScore7.Text = "1";
                            ChamDiem(1, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        //0 diem
                        _lblScore7.Text = "0";
                        ChamDiem(0, ref _luotBia2, ref _currentMemberBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                }
            }
        }

        private void XuLyBe3(int x, int y)
        {
            Bitmap bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_5, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_5;
            int be = 3;// ban o be so 3
            Color c = bitmap.GetPixel(x, y);
            //dung thuat toan quicksort
            if (c.ToArgb().Equals(Color.Red.ToArgb()))
            {
                bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_8, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_8;
                c = bitmap.GetPixel(x, y);
                if (c.ToArgb().Equals(Color.Red.ToArgb()))
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_9, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_9;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_10, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_10;
                        c = bitmap.GetPixel(x, y);
                        if (c.ToArgb().Equals(Color.Red.ToArgb()))
                        {
                            //10 diem
                            _lblScore8.Text = "10";
                            ChamDiem(10, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //9 diem
                            _lblScore8.Text = "9";
                            ChamDiem(9, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        _lblScore8.Text = "8";
                        ChamDiem(8, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                }
                else
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_6, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_6;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_7, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_7;
                        c = bitmap.GetPixel(x, y);
                        if (c.ToArgb().Equals(Color.Red.ToArgb()))
                        {
                            //7 diem
                            _lblScore8.Text = "7";
                            ChamDiem(7, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //6 diem
                            _lblScore8.Text = "6";
                            ChamDiem(6, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        //5 diem
                        _lblScore8.Text = "5";
                        ChamDiem(5, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                }
            }
            else
            {
                bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_3, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_3;
                c = bitmap.GetPixel(x, y);
                if (c.ToArgb().Equals(Color.Red.ToArgb()))
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_4, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_4;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        //4 diem
                        _lblScore8.Text = "4";
                        ChamDiem(4, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                    else
                    {
                        //3 diem
                        _lblScore8.Text = "3";
                        ChamDiem(3, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                }
                else
                {
                    bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_1, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_1;
                    c = bitmap.GetPixel(x, y);
                    if (c.ToArgb().Equals(Color.Red.ToArgb()))
                    {
                        bitmap = ProcessImage.ResizeImage(Properties.Resources.bia8_2, _ptbCamera.Width, _ptbCamera.Height);//Properties.Resources.bia8_2;
                        c = bitmap.GetPixel(x, y);
                        if (c.ToArgb().Equals(Color.Red.ToArgb()))
                        {
                            //2 diem
                            _lblScore8.Text = "2";
                            ChamDiem(2, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //1 diem
                            _lblScore8.Text = "1";
                            ChamDiem(1, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        //0 diem
                        _lblScore8.Text = "0";
                        ChamDiem(0, ref _luotBia3, ref _currentMemberBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                }
            }
        }

        #endregion
    }
}
