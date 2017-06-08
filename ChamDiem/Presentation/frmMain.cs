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
using DevExpress.XtraWaitForm;
using CameraApp;

namespace ChamDiem
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        #region ----- STATIC VARIABLE -----
        //bia de chon ban
        const int SELECT_BIA_4 = 1;
        const int SELECT_BIA_7 = 2;
        const int SELECT_BIA_8 = 3;
        const int SELECT_3_BIA = 4;

        //bien mac dinh ve luot
        const int RESET_LUOT_KHOI_DAU = 1;
        const int RESET_MEMBER_KHOI_DAU = 0;

        //luot ban o 1 bia
        const int LUOT_BAN_1_BIA = 3;

        //3 bia moi bia 8 be
        const int SO_BE_BAN = 24;

        //Nam of small Camera
        const int CAMERA_SMALL_41 = 41;
        const int CAMERA_SMALL_42 = 42;
        const int CAMERA_SMALL_43 = 43;
        const int CAMERA_SMALL_44 = 44;
        const int CAMERA_SMALL_45 = 45;
        const int CAMERA_SMALL_46 = 46;
        const int CAMERA_SMALL_47 = 47;
        const int CAMERA_SMALL_48 = 48;
        const int CAMERA_SMALL_71 = 71;
        const int CAMERA_SMALL_72 = 72;
        const int CAMERA_SMALL_73 = 73;
        const int CAMERA_SMALL_74 = 74;
        const int CAMERA_SMALL_75 = 75;
        const int CAMERA_SMALL_76 = 76;
        const int CAMERA_SMALL_77 = 77;
        const int CAMERA_SMALL_78 = 78;
        const int CAMERA_SMALL_81 = 81;
        const int CAMERA_SMALL_82 = 82;
        const int CAMERA_SMALL_83 = 83;
        const int CAMERA_SMALL_84 = 84;
        const int CAMERA_SMALL_85 = 85;
        const int CAMERA_SMALL_86 = 86;
        const int CAMERA_SMALL_87 = 87;
        const int CAMERA_SMALL_88 = 88;

        //khong co chon bat ky camera thi gan gia tri nay
        const int NO_CAMERA_SMALL = 0;

        #endregion

        //array camera url
        public static string[] _urlCamera = new string[SO_BE_BAN];

        //nguoi ban hien tai o cac bia
        private int _currentMemberBia4 = 0, _currentMemberBia7 = 0, _currentMemberBia8 = 0;

        //luot ban hien tai o cac bia
        private int _currentTurn1 = 0, _currentTurn2 = 0, _currentTurn3 = 0;

        //so nguoi 1 luot ban, hien tai la 8 nguoi
        private const int NUMBER_PERSON_SHOT = 8;

        private Solider[] _solider;//so nguoi ban

        //chon bia de ban
        private int _selectedBia = SELECT_3_BIA;// mac dinh la chon 3 bia
        
        private bool isActive = false;//bien kiem tra kich hoat

        List<PictureBox> _lstPbBia41 = new List<PictureBox>();
        List<PictureBox> _lstPbBia42 = new List<PictureBox>();
        List<PictureBox> _lstPbBia43 = new List<PictureBox>();
        List<PictureBox> _lstPbBia44 = new List<PictureBox>();
        List<PictureBox> _lstPbBia45 = new List<PictureBox>();
        List<PictureBox> _lstPbBia46 = new List<PictureBox>();
        List<PictureBox> _lstPbBia47 = new List<PictureBox>();
        List<PictureBox> _lstPbBia48 = new List<PictureBox>();
        List<PictureBox> _lstPbBia71 = new List<PictureBox>();
        List<PictureBox> _lstPbBia72 = new List<PictureBox>();
        List<PictureBox> _lstPbBia73 = new List<PictureBox>();
        List<PictureBox> _lstPbBia74 = new List<PictureBox>();
        List<PictureBox> _lstPbBia75 = new List<PictureBox>();
        List<PictureBox> _lstPbBia76 = new List<PictureBox>();
        List<PictureBox> _lstPbBia77 = new List<PictureBox>();
        List<PictureBox> _lstPbBia78 = new List<PictureBox>();
        List<PictureBox> _lstPbBia81 = new List<PictureBox>();
        List<PictureBox> _lstPbBia82 = new List<PictureBox>();
        List<PictureBox> _lstPbBia83 = new List<PictureBox>();
        List<PictureBox> _lstPbBia84 = new List<PictureBox>();
        List<PictureBox> _lstPbBia85 = new List<PictureBox>();
        List<PictureBox> _lstPbBia86 = new List<PictureBox>();
        List<PictureBox> _lstPbBia87 = new List<PictureBox>();
        List<PictureBox> _lstPbBia88 = new List<PictureBox>();
        
        private Color[] COLOR_FOR_EACH_TURN = new Color[] 
        {
            Color.Red,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Pink,
            Color.Orange,
            Color.Purple,
            Color.Brown,
            Color.YellowGreen,
            Color.Violet
        };

        //D8GXC-MVBVA-H3009-1YL09-828HF-PJYMC
        public frmMain()
        {
            InitializeComponent();
            //kiem tra license
            KeyManager km = new KeyManager(ComputerInfo.GetComputerId());
            LicenseInfo lic = new LicenseInfo();
            int value = km.LoadSuretyFile(string.Format(@"{0}\Key.lic", Application.StartupPath), ref lic);
            string productKey = lic.ProductKey;
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

            if (!isActive)
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
            Thread t = new Thread(ExportExcel);
            t.Start();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //da kich hoat thi load url camera
            LoadUrlCamera();
            LoadCamera();
        }

        private void _btnInputCameraUrl_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmInputCameraUrl frm = new frmInputCameraUrl();
            frm.ShowDialog();
            if (frmInputCameraUrl.isOkClick)
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
            if (_tabControlCamera.SelectedTabPageIndex == 0)
            {
                _lblScore4.Text = "0";
                ChamDiem(0, _gridControlScore4, _lblName4, _lblScore4, 1);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1)
            {
                _lblScore7.Text = "0";
                ChamDiem(0, _gridControlScore7, _lblName7, _lblScore7, 2);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2)
            {
                _lblScore8.Text = "0";
                ChamDiem(0, _gridControlScore8, _lblName8, _lblScore8, 3);
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
                    if (_selectedBia == SELECT_3_BIA)
                    {
                        //load info be so 1
                        DataTable table = ExcelHelp.getDataTableExcelFor3Bia(op.FileName);
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

                        //khoi tao so nguoi ban
                        SoNguoiBan(table);

                        //load ten vao cac camera
                        LoadNameBia4();//table);
                        LoadNameBia7();//table);
                        LoadNameBia8();//table);
                    }
                    else if (_selectedBia == SELECT_BIA_4)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore4.Columns.Clear();
                        _gridControlScore4.DataSource = null;
                        _gridControlScore4.DataSource = table;
                        _gridViewScore4.BestFitColumns();

                        //khoi tao so nguoi ban
                        SoNguoiBan(table);

                        LoadNameBia4();//table);
                    }
                    else if (_selectedBia == SELECT_BIA_7)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore7.Columns.Clear();
                        _gridControlScore7.DataSource = null;
                        _gridControlScore7.DataSource = table;
                        _gridViewScore7.BestFitColumns();

                        //khoi tao so nguoi ban
                        SoNguoiBan(table);

                        LoadNameBia7();//table);
                    }
                    else //if (_selectedBia == BAN_BIA_8)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore8.Columns.Clear();
                        _gridControlScore8.DataSource = null;
                        _gridControlScore8.DataSource = table;
                        _gridViewScore8.BestFitColumns();

                        //khoi tao so nguoi ban
                        SoNguoiBan(table);

                        LoadNameBia8();//table);
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        private void _transpCtrl4_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore4.DataSource == null)
            {
                return;
            }
            // vuot qua so luot thi k show nua
            if (_solider[_currentMemberBia4].luotBanBia4 == 4)
            {
                return;
            }
            AddShotIcon(e.X, e.Y, _panCam4, 4);
            XuLyBe1(e.X, e.Y);
        }

        private void _transpCtrl7_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore7.DataSource == null)
            {
                return;
            }
            // vuot qua so luot thi k show nua
            if (_solider[_currentMemberBia7].luotBanBia7 == 4)
            {
                return;
            }
            AddShotIcon(e.X, e.Y, _panCam7, 7);
            XuLyBe2(e.X, e.Y);
        }

        private void _transpCtrl8_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore8.DataSource == null)
            {
                return;
            }
            // vuot qua so luot thi k show nua
            if (_solider[_currentMemberBia8].luotBanBia8 == 4)
            {
                return;
            }
            AddShotIcon(e.X, e.Y, _panCam8, 8);
            XuLyBe3(e.X, e.Y);
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
                LoadCamera();
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
                LoadCamera();
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
                LoadCamera();
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
                LoadCamera();
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
            if (_tabControlCamera.SelectedTabPageIndex == 0 && _currentTurn1 + NUMBER_PERSON_SHOT < _solider.Length)
            {
                _currentTurn1 += NUMBER_PERSON_SHOT;
                LoadNameBia4();
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1 && _currentTurn2 + NUMBER_PERSON_SHOT < _solider.Length)
            {
                _currentTurn2 += NUMBER_PERSON_SHOT;
                LoadNameBia7();
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2 && _currentTurn3 + NUMBER_PERSON_SHOT < _solider.Length)
            {
                _currentTurn3 += NUMBER_PERSON_SHOT;
                LoadNameBia8();
            }
        }

        private void _btnLoadCameraBe41_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[0], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 1);

                _lblName4.Text = _currentTurn1 < _solider.Length ? _solider[_currentTurn1].name : "";
                _currentMemberBia4 = _currentTurn1;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void _btnLoadCameraBe42_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[1], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 2);

                _lblName4.Text = (_currentTurn1 + 1) < _solider.Length ? _solider[_currentTurn1 + 1].name : "";
                _currentMemberBia4 = _currentTurn1 + 1;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe43_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[2], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 3);

                _lblName4.Text = (_currentTurn1 + 2) < _solider.Length ? _solider[_currentTurn1 + 2].name : "";
                _currentMemberBia4 = _currentTurn1 + 2;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe44_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[3], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 4);

                _lblName4.Text = (_currentTurn1 + 3) < _solider.Length ? _solider[_currentTurn1 + 3].name : "";
                _currentMemberBia4 = _currentTurn1 + 3;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe45_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[4], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 5);

                _lblName4.Text = (_currentTurn1 + 4) < _solider.Length ? _solider[_currentTurn1 + 4].name : "";
                _currentMemberBia4 = _currentTurn1 + 4;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe46_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[5], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 6);

                _lblName4.Text = (_currentTurn1 + 5) < _solider.Length ? _solider[_currentTurn1 + 5].name : "";
                _currentMemberBia4 = _currentTurn1 + 5;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe47_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[6], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 7);

                _lblName4.Text = (_currentTurn1 + 6) < _solider.Length ? _solider[_currentTurn1 + 6].name : "";
                _currentMemberBia4 = _currentTurn1 + 6;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe48_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[7], _spcCamera4);
                //lan dau chua load camera thi bang false
                if (_transpCtrl4.Enabled == false)
                {
                    _transpCtrl4.Enabled = true;
                }
                LoadTransControl(4, 8);

                _lblName4.Text = (_currentTurn1 + 7) < _solider.Length ? _solider[_currentTurn1 + 7].name : "";
                _currentMemberBia4 = _currentTurn1 + 7;
                _lblScore4.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe71_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[8], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 1);

                _lblName7.Text = _currentTurn2 < _solider.Length ? _solider[_currentTurn2].name : "";
                _currentMemberBia7 = _currentTurn2;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe72_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[9], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 2);

                _lblName7.Text = (_currentTurn2 + 1) < _solider.Length ? _solider[_currentTurn2 + 1].name : "";
                _currentMemberBia7 = _currentTurn2 + 1;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe73_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[10], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 3);

                _lblName7.Text = (_currentTurn2 + 2) < _solider.Length ? _solider[_currentTurn2 + 2].name : "";
                _currentMemberBia7 = _currentTurn2 + 2;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe74_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[11], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 4);

                _lblName7.Text = (_currentTurn2 + 3) < _solider.Length ? _solider[_currentTurn2 + 3].name : "";
                _currentMemberBia7 = _currentTurn2 + 3;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe75_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[12], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 5);

                _lblName7.Text = (_currentTurn2 + 4) < _solider.Length ? _solider[_currentTurn2 + 4].name : "";
                _currentMemberBia7 = _currentTurn2 + 4;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe76_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[13], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 6);

                _lblName7.Text = (_currentTurn2 + 5) < _solider.Length ? _solider[_currentTurn2 + 5].name : "";
                _currentMemberBia7 = _currentTurn2 + 5;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe77_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[14], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 7);

                _lblName7.Text = (_currentTurn2 + 6) < _solider.Length ? _solider[_currentTurn2 + 6].name : "";
                _currentMemberBia7 = _currentTurn2 + 6;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe78_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[15], _spcCamera7);
                //lan dau chua load camera thi bang false
                if (_transpCtrl7.Enabled == false)
                {
                    _transpCtrl7.Enabled = true;
                }
                LoadTransControl(7, 8);

                _lblName7.Text = (_currentTurn2 + 7) < _solider.Length ? _solider[_currentTurn2 + 7].name : "";
                _currentMemberBia7 = _currentTurn2 + 7;
                _lblScore7.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe81_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[16], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 1);

                _lblName8.Text = _currentTurn3 < _solider.Length ? _solider[_currentTurn3].name : "";
                _currentMemberBia8 = _currentTurn3;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe82_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[17], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 2);

                _lblName8.Text = (_currentTurn3 + 1) < _solider.Length ? _solider[_currentTurn3 + 1].name : "";
                _currentMemberBia8 = _currentTurn3 + 1;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe83_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[18], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 3);

                _lblName8.Text = (_currentTurn3 + 2) < _solider.Length ? _solider[_currentTurn3 + 2].name : "";
                _currentMemberBia8 = _currentTurn3 + 2;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe84_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[19], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 4);

                _lblName8.Text = (_currentTurn3 + 3) < _solider.Length ? _solider[_currentTurn3 + 3].name : "";
                _currentMemberBia8 = _currentTurn3 + 3;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe85_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[20], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 5);

                _lblName8.Text = (_currentTurn3 + 4) < _solider.Length ? _solider[_currentTurn3 + 4].name : "";
                _currentMemberBia8 = _currentTurn3 + 4;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe86_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[21], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 6);

                _lblName8.Text = (_currentTurn3 + 5) < _solider.Length ? _solider[_currentTurn3 + 5].name : "";
                _currentMemberBia8 = _currentTurn3 + 5;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe87_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[22], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 7);

                _lblName8.Text = (_currentTurn3 + 6) < _solider.Length ? _solider[_currentTurn3 + 6].name : "";
                _currentMemberBia8 = _currentTurn3 + 6;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        private void _btnLoadCameraBe88_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCameraUrl(_urlCamera[23], _spcCamera8);
                //lan dau chua load camera thi bang false
                if (_transpCtrl8.Enabled == false)
                {
                    _transpCtrl8.Enabled = true;
                }
                LoadTransControl(8, 8);

                _lblName8.Text = (_currentTurn3 + 7) < _solider.Length ? _solider[_currentTurn3 + 7].name : "";
                _currentMemberBia8 = _currentTurn3 + 7;
                _lblScore8.Text = "";
            }
            catch (Exception ex)
            { }
        }

        #region ---------- XU LY CHAM DIEM ----------

        private void AddShotIcon(int x, int y, PanelControl panCam, int be)
        {
            PictureBox px = new PictureBox();
            px.Size = new Size(4, 4);
            if (be == 4)
            {
                px.BackColor = COLOR_FOR_EACH_TURN[_currentTurn1 % 10];//Color.Transparent;
            }
            else if (be == 7)
            {
                px.BackColor = COLOR_FOR_EACH_TURN[_currentTurn2 % 10];//Color.Transparent;
            }
            else
            {
                px.BackColor = COLOR_FOR_EACH_TURN[_currentTurn3 % 10];//Color.Transparent;
            }                

            // tru di 2 don vi de chinh giua hinh
            px.Location = new Point(x - 2, y - 2);
            panCam.Controls.Add(px);
            px.BringToFront();
            if (be == 4)
            {
                if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 0)
                {
                    _lstPbBia41.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 1)
                {
                    _lstPbBia42.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 2)
                {
                    _lstPbBia43.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 3)
                {
                    _lstPbBia44.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 4)
                {
                    _lstPbBia45.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 5)
                {
                    _lstPbBia46.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 6)
                {
                    _lstPbBia47.Add(px);
                }
                else if (_currentMemberBia4 % NUMBER_PERSON_SHOT == 7)
                {
                    _lstPbBia48.Add(px);
                }
            }
            else if (be == 7)
            {
                if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 0)
                {
                    _lstPbBia71.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 1)
                {
                    _lstPbBia72.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 2)
                {
                    _lstPbBia73.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 3)
                {
                    _lstPbBia74.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 4)
                {
                    _lstPbBia75.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 5)
                {
                    _lstPbBia76.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 6)
                {
                    _lstPbBia77.Add(px);
                }
                else if (_currentMemberBia7 % NUMBER_PERSON_SHOT == 7)
                {
                    _lstPbBia78.Add(px);
                }
            }
            else
            {
                if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 0)
                {
                    _lstPbBia81.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 1)
                {
                    _lstPbBia82.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 2)
                {
                    _lstPbBia83.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 3)
                {
                    _lstPbBia84.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 4)
                {
                    _lstPbBia85.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 5)
                {
                    _lstPbBia86.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 6)
                {
                    _lstPbBia87.Add(px);
                }
                else if (_currentMemberBia8 % NUMBER_PERSON_SHOT == 7)
                {
                    _lstPbBia88.Add(px);
                }
            }
        }

        private void EnableShot(List<PictureBox> lst, bool enable)
        {
            foreach (PictureBox px in lst)
            {
                px.Enabled = enable;
            }
        }

        private void LoadTransControl(int bia, int be)
        {
            if (bia == 4)
            {
                switch (be)
                {
                    case 1:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia41.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia41.ElementAt(i).Size;
                            px.BackColor = _lstPbBia41.ElementAt(i).BackColor;
                            px.Location = _lstPbBia41.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 2:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia42.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia42.ElementAt(i).Size;
                            px.BackColor = _lstPbBia42.ElementAt(i).BackColor;
                            px.Location = _lstPbBia42.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 3:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia43.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia43.ElementAt(i).Size;
                            px.BackColor = _lstPbBia43.ElementAt(i).BackColor;
                            px.Location = _lstPbBia43.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 4:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia44.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia44.ElementAt(i).Size;
                            px.BackColor = _lstPbBia44.ElementAt(i).BackColor;
                            px.Location = _lstPbBia44.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 5:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia45.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia45.ElementAt(i).Size;
                            px.BackColor = _lstPbBia45.ElementAt(i).BackColor;
                            px.Location = _lstPbBia45.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 6:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia46.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia46.ElementAt(i).Size;
                            px.BackColor = _lstPbBia46.ElementAt(i).BackColor;
                            px.Location = _lstPbBia46.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 7:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia47.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia47.ElementAt(i).Size;
                            px.BackColor = _lstPbBia47.ElementAt(i).BackColor;
                            px.Location = _lstPbBia47.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 8:
                        XoaDiemBanBeSo4();
                        for (int i = 0; i < _lstPbBia48.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia48.ElementAt(i).Size;
                            px.BackColor = _lstPbBia48.ElementAt(i).BackColor;
                            px.Location = _lstPbBia48.ElementAt(i).Location;
                            _panCam4.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                }
            }
            else if (bia == 7)
            {
                switch (be)
                {
                    case 1:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia71.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia71.ElementAt(i).Size;
                            px.BackColor = _lstPbBia71.ElementAt(i).BackColor;
                            px.Location = _lstPbBia71.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 2:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia72.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia72.ElementAt(i).Size;
                            px.BackColor = _lstPbBia72.ElementAt(i).BackColor;
                            px.Location = _lstPbBia72.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 3:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia73.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia73.ElementAt(i).Size;
                            px.BackColor = _lstPbBia73.ElementAt(i).BackColor;
                            px.Location = _lstPbBia73.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 4:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia74.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia74.ElementAt(i).Size;
                            px.BackColor = _lstPbBia74.ElementAt(i).BackColor;
                            px.Location = _lstPbBia74.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 5:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia75.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia75.ElementAt(i).Size;
                            px.BackColor = _lstPbBia75.ElementAt(i).BackColor;
                            px.Location = _lstPbBia75.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 6:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia76.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia76.ElementAt(i).Size;
                            px.BackColor = _lstPbBia76.ElementAt(i).BackColor;
                            px.Location = _lstPbBia76.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 7:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia77.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia77.ElementAt(i).Size;
                            px.BackColor = _lstPbBia77.ElementAt(i).BackColor;
                            px.Location = _lstPbBia77.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 8:
                        XoaDiemBanBeSo7();
                        for (int i = 0; i < _lstPbBia78.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia78.ElementAt(i).Size;
                            px.BackColor = _lstPbBia78.ElementAt(i).BackColor;
                            px.Location = _lstPbBia78.ElementAt(i).Location;
                            _panCam7.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                }
            }
            else if (bia == 8)
            {
                switch (be)
                {
                    case 1:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia81.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia81.ElementAt(i).Size;
                            px.BackColor = _lstPbBia81.ElementAt(i).BackColor;
                            px.Location = _lstPbBia81.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 2:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia82.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia82.ElementAt(i).Size;
                            px.BackColor = _lstPbBia82.ElementAt(i).BackColor;
                            px.Location = _lstPbBia82.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 3:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia83.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia83.ElementAt(i).Size;
                            px.BackColor = _lstPbBia83.ElementAt(i).BackColor;
                            px.Location = _lstPbBia83.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 4:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia84.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia84.ElementAt(i).Size;
                            px.BackColor = _lstPbBia84.ElementAt(i).BackColor;
                            px.Location = _lstPbBia84.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 5:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia85.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia85.ElementAt(i).Size;
                            px.BackColor = _lstPbBia85.ElementAt(i).BackColor;
                            px.Location = _lstPbBia85.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 6:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia86.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia86.ElementAt(i).Size;
                            px.BackColor = _lstPbBia86.ElementAt(i).BackColor;
                            px.Location = _lstPbBia86.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 7:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia87.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia87.ElementAt(i).Size;
                            px.BackColor = _lstPbBia87.ElementAt(i).BackColor;
                            px.Location = _lstPbBia87.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                    case 8:
                        XoaDiemBanBeSo8();
                        for (int i = 0; i < _lstPbBia88.Count; i++)
                        {
                            PictureBox px = new PictureBox();
                            px.Size = _lstPbBia88.ElementAt(i).Size;
                            px.BackColor = _lstPbBia88.ElementAt(i).BackColor;
                            px.Location = _lstPbBia88.ElementAt(i).Location;
                            _panCam8.Controls.Add(px);
                            px.BringToFront();
                        }
                        break;
                }
            }
        }

        private void SoNguoiBan(DataTable dt)
        {
            _solider = new Solider[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                _solider[i] = new Solider();
                _solider[i].name = dt.Rows[i][0].ToString();
            }
        }

        private void ChamDiem(int diem, GridControl dtgScore, LabelControl lblName, LabelControl lblScore, int be)
        {
            try
            {
                int currentMember = 0;//nguoi ban hien tai o bia 4/7/8
                int luot = 0;//luot ban cua nguoi do tai bia 4/7/8

                //be == 1 la bia 4, be == 2 la bia 7, con khong la bia 8
                if (be == 1)
                {
                    // lay nguoi ban hien tai
                    currentMember = _currentMemberBia4;
                    //lay luot hien tai
                    luot = _solider[currentMember].luotBanBia4 <= LUOT_BAN_1_BIA ? _solider[currentMember].luotBanBia4 : 0;
                }
                else if (be == 2)
                {
                    currentMember = _currentMemberBia7;
                    luot = _solider[currentMember].luotBanBia7 <= LUOT_BAN_1_BIA ? _solider[currentMember].luotBanBia7 : 0;
                }
                else
                {
                    currentMember = _currentMemberBia8;
                    luot = _solider[currentMember].luotBanBia8 <= LUOT_BAN_1_BIA ? _solider[currentMember].luotBanBia8 : 0;
                }

                if (luot == 0)
                    return;

                DataTable dt = (DataTable)dtgScore.DataSource;
                dt.Rows[currentMember][luot] = diem;

                if (be == 1)
                {
                    _solider[currentMember].SetTongDiemBia4(diem);
                }
                else if (be == 2)
                {
                    _solider[currentMember].SetTongDiemBia7(diem);
                }
                else
                {
                    _solider[currentMember].SetTongDiemBia8(diem);
                }

                if (_selectedBia == SELECT_3_BIA)
                {
                    //set tong diem
                    setTongDiem3Bia(currentMember);
                }

                docDiem(diem);

                if (luot == 3)
                {
                    int tong = 0;
                    if (be == 1)
                    {
                        tong = _solider[currentMember].GetTongDiemBia4();
                    }
                    else if (be == 2)
                    {
                        tong = _solider[currentMember].GetTongDiemBia7();
                    }
                    else
                    {
                        tong = _solider[currentMember].GetTongDiemBia8();
                    }

                    //ban het luot thi tinh tong diem
                    //int l1 = Int32.Parse(dt.Rows[currentMember][1].ToString());
                    //int l2 = Int32.Parse(dt.Rows[currentMember][2].ToString());
                    //int l3 = Int32.Parse(dt.Rows[currentMember][3].ToString());
                    //int tong = l1 + l2 + l3;                    
                    //tong diem
                    dt.Rows[currentMember][4] = tong;

                    //neu khong phai la 3 bia thi cho xep loai luon o day.
                    if (_selectedBia != SELECT_3_BIA)
                    {
                        if (be == 1)
                        {
                            dt.Rows[currentMember][5] = _solider[currentMember].XepLoai1Bia(_solider[currentMember].GetTongDiemBia4());
                        }
                        else if (be == 2)
                        {
                            dt.Rows[currentMember][5] = _solider[currentMember].XepLoai1Bia(_solider[currentMember].GetTongDiemBia7());
                        }
                        else
                        {
                            dt.Rows[currentMember][5] = _solider[currentMember].XepLoai1Bia(_solider[currentMember].GetTongDiemBia8());
                        }
                    }

                    Thread thread = new Thread(() => soundDiem(be, tong));
                    thread.Start();
                }

                //tang so luot cua nguoi ban len
                if (be == 1)
                {
                    _solider[currentMember].luotBanBia4++;
                }
                else if (be == 2)
                {
                    _solider[currentMember].luotBanBia7++;
                }
                else
                {
                    _solider[currentMember].luotBanBia8++;
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
            if (_selectedBia == SELECT_3_BIA)
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
            /*
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
            */
            int tong3Bia = _solider[currentMember].GetTongDiem3Bia();
            DataTable dt = (DataTable)_gridControlScore4.DataSource;
            dt.Rows[currentMember][5] = tong3Bia;
            dt.Rows[currentMember][6] = _solider[currentMember].XepLoai3Bia(tong3Bia);

            DataTable dt2 = (DataTable)_gridControlScore7.DataSource;
            dt2.Rows[currentMember][5] = _solider[currentMember].GetTongDiem3Bia();
            dt2.Rows[currentMember][6] = _solider[currentMember].XepLoai3Bia(tong3Bia);

            DataTable dt3 = (DataTable)_gridControlScore8.DataSource;
            dt3.Rows[currentMember][5] = _solider[currentMember].GetTongDiem3Bia();
            dt3.Rows[currentMember][6] = _solider[currentMember].XepLoai3Bia(tong3Bia);
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
                            _urlCamera[i] = line;

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

            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        void LoadCameraUrl(string url, StreamPlayerControl spc)
        {
            if(spc.IsPlaying)
            {
                spc.Stop();
            }
            if (url != "")
            {
                var uri = new Uri(url);
                spc.StartPlay(uri, TimeSpan.FromSeconds(15.0));
            }
        }

        private void ExportExcel()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(ExportExcel));
            }
            else
            {
                if (_selectedBia == SELECT_3_BIA)
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
        }

        void StopCameraSmall(StreamPlayerControl spc)
        {
            if (spc.IsPlaying)
            {
                spc.Stop();
            }
        }

        void LoadNameBia4()//DataTable table)
        {
            try
            {
                _lblName41.Text = (_currentTurn1 + 0) < _solider.Length ? _solider[_currentTurn1 + 0].name : "";// table.Rows[_currentTurn1 + 0][0].ToString();
                _lblName42.Text = (_currentTurn1 + 1) < _solider.Length ? _solider[_currentTurn1 + 1].name : "";// table.Rows[_currentTurn1 + 1][0].ToString();
                _lblName43.Text = (_currentTurn1 + 2) < _solider.Length ? _solider[_currentTurn1 + 2].name : "";// table.Rows[_currentTurn1 + 2][0].ToString();
                _lblName44.Text = (_currentTurn1 + 3) < _solider.Length ? _solider[_currentTurn1 + 3].name : "";// table.Rows[_currentTurn1 + 3][0].ToString();
                _lblName45.Text = (_currentTurn1 + 4) < _solider.Length ? _solider[_currentTurn1 + 4].name : "";// table.Rows[_currentTurn1 + 4][0].ToString();
                _lblName46.Text = (_currentTurn1 + 5) < _solider.Length ? _solider[_currentTurn1 + 5].name : "";// table.Rows[_currentTurn1 + 5][0].ToString();
                _lblName47.Text = (_currentTurn1 + 6) < _solider.Length ? _solider[_currentTurn1 + 6].name : "";// table.Rows[_currentTurn1 + 6][0].ToString();
                _lblName48.Text = (_currentTurn1 + 7) < _solider.Length ? _solider[_currentTurn1 + 7].name : "";// table.Rows[_currentTurn1 + 7][0].ToString();
            }
            catch (Exception ex)
            { }
        }

        void LoadNameBia7()//DataTable table)
        {
            try
            {
                _lblName71.Text = (_currentTurn2 + 0) < _solider.Length ? _solider[_currentTurn2 + 0].name : "";// table.Rows[_currentTurn2 + 0][0].ToString();
                _lblName72.Text = (_currentTurn2 + 1) < _solider.Length ? _solider[_currentTurn2 + 1].name : "";// table.Rows[_currentTurn2 + 1][0].ToString();
                _lblName73.Text = (_currentTurn2 + 2) < _solider.Length ? _solider[_currentTurn2 + 2].name : "";// table.Rows[_currentTurn2 + 2][0].ToString();
                _lblName74.Text = (_currentTurn2 + 3) < _solider.Length ? _solider[_currentTurn2 + 3].name : "";// table.Rows[_currentTurn2 + 3][0].ToString();
                _lblName75.Text = (_currentTurn2 + 4) < _solider.Length ? _solider[_currentTurn2 + 4].name : "";// table.Rows[_currentTurn2 + 4][0].ToString();
                _lblName76.Text = (_currentTurn2 + 5) < _solider.Length ? _solider[_currentTurn2 + 5].name : "";// table.Rows[_currentTurn2 + 5][0].ToString();
                _lblName77.Text = (_currentTurn2 + 6) < _solider.Length ? _solider[_currentTurn2 + 6].name : "";// table.Rows[_currentTurn2 + 6][0].ToString();
                _lblName78.Text = (_currentTurn2 + 7) < _solider.Length ? _solider[_currentTurn2 + 7].name : "";// table.Rows[_currentTurn2 + 7][0].ToString();
            }
            catch (Exception ex)
            { }
        }

        void LoadNameBia8()//DataTable table)
        {
            try
            {
                _lblName81.Text = (_currentTurn3 + 0) < _solider.Length ? _solider[_currentTurn3 + 0].name : "";// table.Rows[_currentTurn3 + 0][0].ToString();
                _lblName82.Text = (_currentTurn3 + 1) < _solider.Length ? _solider[_currentTurn3 + 1].name : "";// table.Rows[_currentTurn3 + 1][0].ToString();
                _lblName83.Text = (_currentTurn3 + 2) < _solider.Length ? _solider[_currentTurn3 + 2].name : "";// table.Rows[_currentTurn3 + 2][0].ToString();
                _lblName84.Text = (_currentTurn3 + 3) < _solider.Length ? _solider[_currentTurn3 + 3].name : "";// table.Rows[_currentTurn3 + 3][0].ToString();
                _lblName85.Text = (_currentTurn3 + 4) < _solider.Length ? _solider[_currentTurn3 + 4].name : "";// table.Rows[_currentTurn3 + 4][0].ToString();
                _lblName86.Text = (_currentTurn3 + 5) < _solider.Length ? _solider[_currentTurn3 + 5].name : "";// table.Rows[_currentTurn3 + 5][0].ToString();
                _lblName87.Text = (_currentTurn3 + 6) < _solider.Length ? _solider[_currentTurn3 + 6].name : "";// table.Rows[_currentTurn3 + 6][0].ToString();
                _lblName88.Text = (_currentTurn3 + 7) < _solider.Length ? _solider[_currentTurn3 + 7].name : "";// table.Rows[_currentTurn3 + 7][0].ToString();
            }
            catch (Exception ex)
            { }
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

            //xoa name bia 4
            _lblName41.Text = "";
            _lblName42.Text = "";
            _lblName43.Text = "";
            _lblName44.Text = "";
            _lblName45.Text = "";
            _lblName46.Text = "";
            _lblName47.Text = "";
            _lblName48.Text = "";

            //xoa name bia 7
            _lblName71.Text = "";
            _lblName72.Text = "";
            _lblName73.Text = "";
            _lblName74.Text = "";
            _lblName75.Text = "";
            _lblName76.Text = "";
            _lblName77.Text = "";
            _lblName78.Text = "";

            //xoa name bia 8
            _lblName81.Text = "";
            _lblName82.Text = "";
            _lblName83.Text = "";
            _lblName84.Text = "";
            _lblName85.Text = "";
            _lblName86.Text = "";
            _lblName87.Text = "";
            _lblName88.Text = "";

            //reset diem ban
            XoaDiemBan();
            //reset bien
            resetVariable();
        }

        private void resetVariable()
        {
            _currentMemberBia4 = RESET_MEMBER_KHOI_DAU;
            _currentMemberBia7 = RESET_MEMBER_KHOI_DAU;
            _currentMemberBia8 = RESET_MEMBER_KHOI_DAU;

            //_tong3Bia = null;
            _solider = null;
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
            _lstPbBia41.Clear();
            _lstPbBia42.Clear();
            _lstPbBia43.Clear();
            _lstPbBia44.Clear();
            _lstPbBia45.Clear();
            _lstPbBia46.Clear();
            _lstPbBia47.Clear();
            _lstPbBia48.Clear();
            _lstPbBia71.Clear();
            _lstPbBia72.Clear();
            _lstPbBia73.Clear();
            _lstPbBia74.Clear();
            _lstPbBia75.Clear();
            _lstPbBia76.Clear();
            _lstPbBia77.Clear();
            _lstPbBia78.Clear();
            _lstPbBia81.Clear();
            _lstPbBia82.Clear();
            _lstPbBia83.Clear();
            _lstPbBia84.Clear();
            _lstPbBia85.Clear();
            _lstPbBia86.Clear();
            _lstPbBia87.Clear();
            _lstPbBia88.Clear();
        }

        private void XoaDiemBanBeSo4()
        {
            for (int i = _panCam4.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox control = _panCam4.Controls[i] as PictureBox;
                if (control == null)
                    continue;
                control.Dispose();
            }
        }

        private void XoaDiemBanBeSo7()
        {
            for (int i = _panCam7.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox control = _panCam7.Controls[i] as PictureBox;
                if (control == null)
                    continue;
                control.Dispose();
            }
        }

        private void XoaDiemBanBeSo8()
        {
            for (int i = _panCam8.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox control = _panCam8.Controls[i] as PictureBox;
                if (control == null)
                    continue;
                control.Dispose();
            }
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
                        ChamDiem(10, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //9 diem
                        _lblScore4.Text = "9";
                        ChamDiem(9, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                }
                else
                {
                    // neu khong nam trong 9 thi la 8 diem
                    //8 diem
                    _lblScore4.Text = "8";
                    ChamDiem(8, _gridControlScore4, _lblName4, _lblScore4, be);
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
                        ChamDiem(7, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //6 diem
                        _lblScore4.Text = "6";
                        ChamDiem(6, _gridControlScore4, _lblName4, _lblScore4, be);
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
                        ChamDiem(5, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //0 diem
                        _lblScore4.Text = "0";
                        ChamDiem(0, _gridControlScore4, _lblName4, _lblScore4, be);
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
                            ChamDiem(10, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //9 diem
                            _lblScore7.Text = "9";
                            ChamDiem(9, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        _lblScore7.Text = "8";
                        ChamDiem(8, _gridControlScore7, _lblName7, _lblScore7, be);
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
                            ChamDiem(7, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //6 diem
                            _lblScore7.Text = "6";
                            ChamDiem(6, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        //5 diem
                        _lblScore7.Text = "5";
                        ChamDiem(5, _gridControlScore7, _lblName7, _lblScore7, be);
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
                        ChamDiem(4, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                    else
                    {
                        //3 diem
                        _lblScore7.Text = "3";
                        ChamDiem(3, _gridControlScore7, _lblName7, _lblScore7, be);
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
                            ChamDiem(2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //1 diem
                            _lblScore7.Text = "1";
                            ChamDiem(1, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        //0 diem
                        _lblScore7.Text = "0";
                        ChamDiem(0, _gridControlScore7, _lblName7, _lblScore7, be);
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
                            ChamDiem(10, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //9 diem
                            _lblScore8.Text = "9";
                            ChamDiem(9, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        _lblScore8.Text = "8";
                        ChamDiem(8, _gridControlScore8, _lblName8, _lblScore8, be);
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
                            ChamDiem(7, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //6 diem
                            _lblScore8.Text = "6";
                            ChamDiem(6, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        //5 diem
                        _lblScore8.Text = "5";
                        ChamDiem(5, _gridControlScore8, _lblName8, _lblScore8, be);
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
                        ChamDiem(4, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                    else
                    {
                        //3 diem
                        _lblScore8.Text = "3";
                        ChamDiem(3, _gridControlScore8, _lblName8, _lblScore8, be);
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
                            ChamDiem(2, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //1 diem
                            _lblScore8.Text = "1";
                            ChamDiem(1, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        //0 diem
                        _lblScore8.Text = "0";
                        ChamDiem(0, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                }
            }
        }

        #endregion
    }
}