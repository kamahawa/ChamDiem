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
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //bia de chon ban
        const int SELECT_BIA_4 = 1;
        const int SELECT_BIA_7 = 2;
        const int SELECT_BIA_8 = 3;
        const int SELECT_3_BIA = 4;

        //bien mac dinh ve luot
        const int RESET_LUOT_KHOI_DAU = 1;
        const int RESET_MEMBER_KHOI_DAU = 0;
        
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

        //array camera url
        public static string[] urlCamera = new string[SO_BE_BAN];

        //luot ban
        private int _luotBia1 = RESET_LUOT_KHOI_DAU, _luotBia2 = RESET_LUOT_KHOI_DAU, _luotBia3 = RESET_LUOT_KHOI_DAU;

        //nguoi ban hien tai o cac bia
        private int _currentMemberBia4 = 0, _currentMemberBia7 = 0, _currentMemberBia8 = 0;

        //luot ban hien tai o cac bia
        private int _currentTurn1 = 0, _currentTurn2 = 0, _currentTurn3 = 0;
        
        private Solider[] _solider;//so nguoi ban

        //chon bia de ban
        private int _selectedBia = SELECT_3_BIA;// mac dinh la chon 3 bia

        private Size _cameraSmallSize;//size of camera small
        private Point _cameraSmallLocation4;//location of camera small bia 4 before zoom
        private Point _cameraSmallLocation7;//location of camera small bia 7 before zoom
        private Point _cameraSmallLocation8;//location of camera small bia 8 before zoom

        private int _controlZoom4 = NO_CAMERA_SMALL;//control camera bia 4 is zooming
        private int _controlZoom7 = NO_CAMERA_SMALL;//control camera bia 7 is zooming
        private int _controlZoom8 = NO_CAMERA_SMALL;//control camera bia 8 is zooming

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
            _cameraSmallSize = _spcSmallCamera41.Size;
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
                ChamDiem(0, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, 1);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1)
            {
                _lblScore7.Text = "0";
                ChamDiem(0, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, 2);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2)
            {
                _lblScore8.Text = "0";
                ChamDiem(0, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, 3);
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
                    else if(_selectedBia == SELECT_BIA_4)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore4.Columns.Clear();
                        _gridControlScore4.DataSource = null;
                        _gridControlScore4.DataSource = table;
                        _gridViewScore4.BestFitColumns();
                        LoadNameBia4();//table);
                    }
                    else if (_selectedBia == SELECT_BIA_7)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore7.Columns.Clear();
                        _gridControlScore7.DataSource = null;
                        _gridControlScore7.DataSource = table;
                        _gridViewScore7.BestFitColumns();
                        LoadNameBia7();//table);
                    }
                    else //if (_selectedBia == BAN_BIA_8)
                    {
                        DataTable table = ExcelHelp.getDataTableExcelFor1Bia(op.FileName);
                        _gridViewScore8.Columns.Clear();
                        _gridControlScore8.DataSource = null;
                        _gridControlScore8.DataSource = table;
                        _gridViewScore8.BestFitColumns();
                        LoadNameBia8();//table);
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
            AddShotIcon(e.X, e.Y, _panCam4);
            XuLyBe1(e.X, e.Y);
        }

        private void _transpCtrl7_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore7.DataSource == null)
            {
                return;
            }
            AddShotIcon(e.X, e.Y, _panCam7);
            XuLyBe2(e.X, e.Y);
        }

        private void _transpCtrl8_MouseDown(object sender, MouseEventArgs e)
        {
            //neu chua co danh sach thi khong cho click
            if (_gridControlScore8.DataSource == null)
            {
                return;
            }
            AddShotIcon(e.X, e.Y, _panCam8);
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
            if (_tabControlCamera.SelectedTabPageIndex == 0 && _currentTurn1 + 8 < _solider.Length)
            {
                _currentTurn1 += 8;
                LoadNameBia4();
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1 && _currentTurn2 + 8 < _solider.Length)
            {
                _currentTurn2 += 8;
                LoadNameBia7();
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2 && _currentTurn3 + 8 < _solider.Length)
            {
                _currentTurn3 += 8;
                LoadNameBia8();
            }
        }

        private void _spcSmallCamera41_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera41);
            _controlZoom4 = CAMERA_SMALL_41;
            try
            {
                _lblName4.Text = _currentTurn1 < _solider.Length ? _solider[_currentTurn1].name : "";
                _currentMemberBia4 = _currentTurn1;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch(Exception ex)
            { }
        }

        private void _spcSmallCamera42_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera42);
            _controlZoom4 = CAMERA_SMALL_42;
            try
            {
                _lblName4.Text = (_currentTurn1 + 1) < _solider.Length ? _solider[_currentTurn1 + 1].name : "";
                _currentMemberBia4 = _currentTurn1 + 1;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera43_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera43);
            _controlZoom4 = CAMERA_SMALL_43;
            try
            {
                _lblName4.Text = (_currentTurn1 + 2) < _solider.Length ? _solider[_currentTurn1 + 2].name : "";
                _currentMemberBia4 = _currentTurn1 + 2;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera44_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera44);
            _controlZoom4 = CAMERA_SMALL_44;
            try
            {
                _lblName4.Text = (_currentTurn1 + 3) < _solider.Length ? _solider[_currentTurn1 + 3].name : "";
                _currentMemberBia4 = _currentTurn1 + 3;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera45_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera45);
            _controlZoom4 = CAMERA_SMALL_45;
            try
            {
                _lblName4.Text = (_currentTurn1 + 4) < _solider.Length ? _solider[_currentTurn1 + 4].name : "";
                _currentMemberBia4 = _currentTurn1 + 4;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera46_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera46);
            _controlZoom4 = CAMERA_SMALL_46;
            try
            {
                _lblName4.Text = (_currentTurn1 + 5) < _solider.Length ? _solider[_currentTurn1 + 5].name : "";
                _currentMemberBia4 = _currentTurn1 + 5;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera47_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera47);
            _controlZoom4 = CAMERA_SMALL_47;
            try
            {
                _lblName4.Text = (_currentTurn1 + 6) < _solider.Length ? _solider[_currentTurn1 + 6].name : "";
                _currentMemberBia4 = _currentTurn1 + 6;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera48_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall4(_spcSmallCamera48);
            _controlZoom4 = CAMERA_SMALL_48;
            try
            {
                _lblName4.Text = (_currentTurn1 + 7) < _solider.Length ? _solider[_currentTurn1 + 7].name : "";
                _currentMemberBia4 = _currentTurn1 + 7;
                _luotBia1 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera71_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera71);
            _controlZoom7 = CAMERA_SMALL_71;
            try
            {
                _lblName7.Text = _currentTurn2 < _solider.Length ? _solider[_currentTurn2].name : "";
                _currentMemberBia7 = _currentTurn2;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera72_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera72);
            _controlZoom7 = CAMERA_SMALL_72;
            try
            {
                _lblName7.Text = (_currentTurn2 + 1) < _solider.Length ? _solider[_currentTurn2 + 1].name : "";
                _currentMemberBia7 = _currentTurn2 + 1;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera73_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera73);
            _controlZoom7 = CAMERA_SMALL_73;
            try
            {
                _lblName7.Text = (_currentTurn2 + 2) < _solider.Length ? _solider[_currentTurn2 + 2].name : "";
                _currentMemberBia7 = _currentTurn2 + 2;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera74_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera74);
            _controlZoom7 = CAMERA_SMALL_74;
            try
            {
                _lblName7.Text = (_currentTurn2 + 3) < _solider.Length ? _solider[_currentTurn2 + 3].name : "";
                _currentMemberBia7 = _currentTurn2 + 3;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera75_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera75);
            _controlZoom7 = CAMERA_SMALL_75;
            try
            {
                _lblName7.Text = (_currentTurn2 + 4) < _solider.Length ? _solider[_currentTurn2 + 4].name : "";
                _currentMemberBia7 = _currentTurn2 + 4;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera76_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera76);
            _controlZoom7 = CAMERA_SMALL_76;
            try
            {
                _lblName7.Text = (_currentTurn2 + 5) < _solider.Length ? _solider[_currentTurn2 + 5].name : "";
                _currentMemberBia7 = _currentTurn2 + 5;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera77_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera77);
            _controlZoom7 = CAMERA_SMALL_77;
            try
            {
                _lblName7.Text = (_currentTurn2 + 6) < _solider.Length ? _solider[_currentTurn2 + 6].name : "";
                _currentMemberBia7 = _currentTurn2 + 6;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera78_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall7(_spcSmallCamera78);
            _controlZoom7 = CAMERA_SMALL_78;
            try
            {
                _lblName7.Text = (_currentTurn2 + 7) < _solider.Length ? _solider[_currentTurn2 + 7].name : "";
                _currentMemberBia7 = _currentTurn2 + 7;
                _luotBia2 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }
        
        private void _spcSmallCamera81_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera81);
            _controlZoom8 = CAMERA_SMALL_81;
            try
            {
                _lblName8.Text = _currentTurn3 < _solider.Length ? _solider[_currentTurn3].name : "";
                _currentMemberBia8 = _currentTurn3;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera82_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera82);
            _controlZoom8 = CAMERA_SMALL_82;
            try
            {
                _lblName8.Text = (_currentTurn3 + 1) < _solider.Length ? _solider[_currentTurn3 + 1].name : "";
                _currentMemberBia8 = _currentTurn3 + 1;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera83_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera83);
            _controlZoom8 = CAMERA_SMALL_83;
            try
            {
                _lblName8.Text = (_currentTurn3 + 2) < _solider.Length ? _solider[_currentTurn3 + 2].name : "";
                _currentMemberBia8 = _currentTurn3 + 2;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera84_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera84);
            _controlZoom8 = CAMERA_SMALL_84;
            try
            {
                _lblName8.Text = (_currentTurn3 + 3) < _solider.Length ? _solider[_currentTurn3 + 3].name : "";
                _currentMemberBia8 = _currentTurn3 + 3;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera85_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera85);
            _controlZoom8 = CAMERA_SMALL_85;
            try
            {
                _lblName8.Text = (_currentTurn3 + 4) < _solider.Length ? _solider[_currentTurn3 + 4].name : "";
                _currentMemberBia8 = _currentTurn3 + 4;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera86_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera86);
            _controlZoom8 = CAMERA_SMALL_86;
            try
            {
                _lblName8.Text = (_currentTurn3 + 5) < _solider.Length ? _solider[_currentTurn3 + 5].name : "";
                _currentMemberBia8 = _currentTurn3 + 5;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera87_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera87);
            _controlZoom8 = CAMERA_SMALL_87;
            try
            {
                _lblName8.Text = (_currentTurn3 + 6) < _solider.Length ? _solider[_currentTurn3 + 6].name : "";
                _currentMemberBia8 = _currentTurn3 + 6;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _spcSmallCamera88_MouseClick(object sender, MouseEventArgs e)
        {
            ZoomCameraSmall8(_spcSmallCamera88);
            _controlZoom8 = CAMERA_SMALL_88;
            try
            {
                _lblName8.Text = (_currentTurn3 + 7) < _solider.Length ? _solider[_currentTurn3 + 7].name : "";
                _currentMemberBia8 = _currentTurn3 + 7;
                _luotBia3 = RESET_LUOT_KHOI_DAU;
            }
            catch (Exception ex)
            { }
        }

        private void _btnBack_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_tabControlCamera.SelectedTabPageIndex == 0)
            {
                //khong co chon camera nao thi khong duoc zoomback
                if (_controlZoom4 == NO_CAMERA_SMALL)
                    return;
                // cho bia so 4 zoom nho lai
                ZoomBack(4);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 1)
            {
                //khong co chon camera nao thi khong duoc zoomback
                if (_controlZoom7 == NO_CAMERA_SMALL)
                    return;
                // cho bia so 7 zoom nho lai
                ZoomBack(7);
            }
            else if (_tabControlCamera.SelectedTabPageIndex == 2)
            {
                //khong co chon camera nao thi khong duoc zoomback
                if (_controlZoom8 == NO_CAMERA_SMALL)
                    return;
                // cho bia so 7 zoom nho lai
                ZoomBack(8);
            }
        }

        #region ---------- XU LY CHAM DIEM ----------

        private void AddShotIcon(int x, int y, PanelControl panCam)
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

        private void SoNguoiBan(DataTable dt)
        {
            _solider = new Solider[dt.Rows.Count];
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                _solider[i] = new Solider();
                _solider[i].name = dt.Rows[i][0].ToString();
            }
        }

        private void ChamDiem(int diem, ref int luot, GridControl dtgScore, LabelControl lblName, LabelControl lblScore, int be)
        {
            try
            {
                int currentMember = 0;
                //be == 1 la bia 4, be == 2 la bia 7, con khong la bia 8
                if (be == 1)
                {
                    currentMember = _currentMemberBia4;
                }
                else if (be == 2)
                {
                    currentMember = _currentMemberBia7;
                }
                else
                {
                    currentMember = _currentMemberBia8;
                }

                DataTable dt = (DataTable)dtgScore.DataSource;
                dt.Rows[currentMember][luot] = diem;

                if(_selectedBia == SELECT_3_BIA)
                {
                    //set tong diem
                    //_tong3Bia[currentMember] += diem;

                    //set tong diem
                    setTongDiem3Bia(currentMember);
                }

                docDiem(diem);

                //set ten va diem 
                if (luot == 1)
                {
                    lblName.Text = dt.Rows[currentMember][0].ToString();
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
            DataTable dt = (DataTable)_gridControlScore4.DataSource;
            dt.Rows[currentMember][5] = _tong3Bia[currentMember];
            dt.Rows[currentMember][6] = xeploai;

            DataTable dt2 = (DataTable)_gridControlScore7.DataSource;
            dt2.Rows[currentMember][5] = _tong3Bia[currentMember];
            dt2.Rows[currentMember][6] = xeploai;

            DataTable dt3 = (DataTable)_gridControlScore8.DataSource;
            dt3.Rows[currentMember][5] = _tong3Bia[currentMember];
            dt3.Rows[currentMember][6] = xeploai;
            */
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
                LoadCamera3Bia();
                /*
                if (_tabControlCamera.SelectedTabPageIndex == 0)
                {
                    LoadCameraBia4();
                }
                else if (_tabControlCamera.SelectedTabPageIndex == 1)
                {
                    LoadCameraBia7();
                }
                else if (_tabControlCamera.SelectedTabPageIndex == 2)
                {
                    LoadCameraBia8();
                }
                */
            }
            catch (Exception ex)
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

        void StopCameraSmall(StreamPlayerControl spc)
        {
            if (spc.IsPlaying)
            {
                spc.Stop();
            }
        }

        //zoom camera nho thanh camra lon de cham diem bia so 4
        void ZoomCameraSmall4(StreamPlayerControl spc)
        {
            _cameraSmallLocation4 = spc.Location;
            spc.Size = _transpCtrl4.Size;
            spc.Location = _transpCtrl4.Location;
            _transpCtrl4.Enabled = true;
            //cho camera len tren cung, khong bi cac control khac de len
            spc.BringToFront();
            _transpCtrl4.BringToFront();
        }

        //zoom camera nho thanh camra lon de cham diem bia so 7
        void ZoomCameraSmall7(StreamPlayerControl spc)
        {
            _cameraSmallLocation7 = spc.Location;
            spc.Size = _transpCtrl7.Size;
            spc.Location = _transpCtrl7.Location;
            _transpCtrl7.Enabled = true;
            //cho camera len tren cung, khong bi cac control khac de len
            spc.BringToFront();
            _transpCtrl7.BringToFront();
        }

        //zoom camera nho thanh camra lon de cham diem bia so 8
        void ZoomCameraSmall8(StreamPlayerControl spc)
        {
            _cameraSmallLocation8 = spc.Location;
            spc.Size = _transpCtrl8.Size;
            spc.Location = _transpCtrl8.Location;
            _transpCtrl8.Enabled = true;
            //cho camera len tren cung, khong bi cac control khac de len
            spc.BringToFront();
            _transpCtrl8.BringToFront();
        }

        // back tu zoom camera o man hinh chinh thanh camera nho
        void ZoomBack(int bia)
        {
            try
            {
                if (bia == 4)
                {
                    switch (_controlZoom4)
                    {
                        case CAMERA_SMALL_41:
                            _spcSmallCamera41.Size = _cameraSmallSize;
                            _spcSmallCamera41.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_42:
                            _spcSmallCamera42.Size = _cameraSmallSize;
                            _spcSmallCamera42.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_43:
                            _spcSmallCamera43.Size = _cameraSmallSize;
                            _spcSmallCamera43.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_44:
                            _spcSmallCamera44.Size = _cameraSmallSize;
                            _spcSmallCamera44.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_45:
                            _spcSmallCamera45.Size = _cameraSmallSize;
                            _spcSmallCamera45.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_46:
                            _spcSmallCamera46.Size = _cameraSmallSize;
                            _spcSmallCamera46.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_47:
                            _spcSmallCamera47.Size = _cameraSmallSize;
                            _spcSmallCamera47.Location = _cameraSmallLocation4;
                            break;
                        case CAMERA_SMALL_48:
                            _spcSmallCamera48.Size = _cameraSmallSize;
                            _spcSmallCamera48.Location = _cameraSmallLocation4;
                            break;
                    }
                    _controlZoom4 = NO_CAMERA_SMALL;
                    _transpCtrl4.Enabled = false;
                    XoaDiemBan();
                }
                else if (bia == 7)
                {
                    switch (_controlZoom7)
                    {
                        case CAMERA_SMALL_71:
                            _spcSmallCamera71.Size = _cameraSmallSize;
                            _spcSmallCamera71.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_72:
                            _spcSmallCamera72.Size = _cameraSmallSize;
                            _spcSmallCamera72.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_73:
                            _spcSmallCamera73.Size = _cameraSmallSize;
                            _spcSmallCamera73.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_74:
                            _spcSmallCamera74.Size = _cameraSmallSize;
                            _spcSmallCamera74.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_75:
                            _spcSmallCamera75.Size = _cameraSmallSize;
                            _spcSmallCamera75.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_76:
                            _spcSmallCamera76.Size = _cameraSmallSize;
                            _spcSmallCamera76.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_77:
                            _spcSmallCamera77.Size = _cameraSmallSize;
                            _spcSmallCamera77.Location = _cameraSmallLocation7;
                            break;
                        case CAMERA_SMALL_78:
                            _spcSmallCamera78.Size = _cameraSmallSize;
                            _spcSmallCamera78.Location = _cameraSmallLocation7;
                            break;
                    }
                    _controlZoom7 = NO_CAMERA_SMALL;
                    _transpCtrl7.Enabled = false;
                    XoaDiemBan();
                }
                else
                {
                    switch (_controlZoom8)
                    {
                        case CAMERA_SMALL_81:
                            _spcSmallCamera81.Size = _cameraSmallSize;
                            _spcSmallCamera81.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_82:
                            _spcSmallCamera82.Size = _cameraSmallSize;
                            _spcSmallCamera82.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_83:
                            _spcSmallCamera83.Size = _cameraSmallSize;
                            _spcSmallCamera83.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_84:
                            _spcSmallCamera84.Size = _cameraSmallSize;
                            _spcSmallCamera84.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_85:
                            _spcSmallCamera85.Size = _cameraSmallSize;
                            _spcSmallCamera85.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_86:
                            _spcSmallCamera86.Size = _cameraSmallSize;
                            _spcSmallCamera86.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_87:
                            _spcSmallCamera87.Size = _cameraSmallSize;
                            _spcSmallCamera87.Location = _cameraSmallLocation8;
                            break;
                        case CAMERA_SMALL_88:
                            _spcSmallCamera88.Size = _cameraSmallSize;
                            _spcSmallCamera88.Location = _cameraSmallLocation8;
                            break;
                    }
                    _controlZoom8 = NO_CAMERA_SMALL;
                    _transpCtrl8.Enabled = false;
                    XoaDiemBan();
                }
            }
            catch(Exception ex)
            {

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
            catch(Exception ex)
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
            catch(Exception ex)
            { }
        }

        void LoadCamera3Bia()
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
        }

        void LoadCameraBia4()
        {
            LoadCameraSmall(urlCamera[0], _spcSmallCamera41);
            LoadCameraSmall(urlCamera[1], _spcSmallCamera42);
            LoadCameraSmall(urlCamera[2], _spcSmallCamera43);
            LoadCameraSmall(urlCamera[3], _spcSmallCamera44);
            LoadCameraSmall(urlCamera[4], _spcSmallCamera45);
            LoadCameraSmall(urlCamera[5], _spcSmallCamera46);
            LoadCameraSmall(urlCamera[6], _spcSmallCamera47);
            LoadCameraSmall(urlCamera[7], _spcSmallCamera48);
        }

        void LoadCameraBia7()
        {
            LoadCameraSmall(urlCamera[8], _spcSmallCamera71);
            LoadCameraSmall(urlCamera[9], _spcSmallCamera72);
            LoadCameraSmall(urlCamera[10], _spcSmallCamera73);
            LoadCameraSmall(urlCamera[11], _spcSmallCamera74);
            LoadCameraSmall(urlCamera[12], _spcSmallCamera75);
            LoadCameraSmall(urlCamera[13], _spcSmallCamera76);
            LoadCameraSmall(urlCamera[14], _spcSmallCamera77);
            LoadCameraSmall(urlCamera[15], _spcSmallCamera78);
        }

        void LoadCameraBia8()
        {
            LoadCameraSmall(urlCamera[16], _spcSmallCamera81);
            LoadCameraSmall(urlCamera[17], _spcSmallCamera82);
            LoadCameraSmall(urlCamera[18], _spcSmallCamera83);
            LoadCameraSmall(urlCamera[19], _spcSmallCamera84);
            LoadCameraSmall(urlCamera[20], _spcSmallCamera85);
            LoadCameraSmall(urlCamera[21], _spcSmallCamera86);
            LoadCameraSmall(urlCamera[22], _spcSmallCamera87);
            LoadCameraSmall(urlCamera[23], _spcSmallCamera88);
        }

        void StopCameraBia4()
        {
            StopCameraSmall(_spcSmallCamera41);
            StopCameraSmall(_spcSmallCamera42);
            StopCameraSmall(_spcSmallCamera43);
            StopCameraSmall(_spcSmallCamera44);
            StopCameraSmall(_spcSmallCamera45);
            StopCameraSmall(_spcSmallCamera46);
            StopCameraSmall(_spcSmallCamera47);
            StopCameraSmall(_spcSmallCamera48);
        }

        void StopCameraBia7()
        {
            StopCameraSmall(_spcSmallCamera71);
            StopCameraSmall(_spcSmallCamera72);
            StopCameraSmall(_spcSmallCamera73);
            StopCameraSmall(_spcSmallCamera74);
            StopCameraSmall(_spcSmallCamera75);
            StopCameraSmall(_spcSmallCamera76);
            StopCameraSmall(_spcSmallCamera77);
            StopCameraSmall(_spcSmallCamera78);
        }

        void StopCameraBia8()
        {
            StopCameraSmall(_spcSmallCamera81);
            StopCameraSmall(_spcSmallCamera82);
            StopCameraSmall(_spcSmallCamera83);
            StopCameraSmall(_spcSmallCamera84);
            StopCameraSmall(_spcSmallCamera85);
            StopCameraSmall(_spcSmallCamera86);
            StopCameraSmall(_spcSmallCamera87);
            StopCameraSmall(_spcSmallCamera88);
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
            //reset bien
            resetVariable();
        }

        private void resetVariable()
        {
            _luotBia1 = RESET_LUOT_KHOI_DAU;
            _luotBia2 = RESET_LUOT_KHOI_DAU;
            _luotBia3 = RESET_LUOT_KHOI_DAU;

            _currentMemberBia4 = RESET_MEMBER_KHOI_DAU;
            _currentMemberBia7 = RESET_MEMBER_KHOI_DAU;
            _currentMemberBia8 = RESET_MEMBER_KHOI_DAU;

            //_tong3Bia = null;
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
                        ChamDiem(10, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //9 diem
                        _lblScore4.Text = "9";
                        ChamDiem(9, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                }
                else
                {
                    // neu khong nam trong 9 thi la 8 diem
                    //8 diem
                    _lblScore4.Text = "8";
                    ChamDiem(8, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
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
                        ChamDiem(7, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //6 diem
                        _lblScore4.Text = "6";
                        ChamDiem(6, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
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
                        ChamDiem(5, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
                    }
                    else
                    {
                        //0 diem
                        _lblScore4.Text = "0";
                        ChamDiem(0, ref _luotBia1, _gridControlScore4, _lblName4, _lblScore4, be);
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
                            ChamDiem(10, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //9 diem
                            _lblScore7.Text = "9";
                            ChamDiem(9, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        _lblScore7.Text = "8";
                        ChamDiem(8, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
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
                            ChamDiem(7, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //6 diem
                            _lblScore7.Text = "6";
                            ChamDiem(6, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        //5 diem
                        _lblScore7.Text = "5";
                        ChamDiem(5, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
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
                        ChamDiem(4, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                    }
                    else
                    {
                        //3 diem
                        _lblScore7.Text = "3";
                        ChamDiem(3, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
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
                            ChamDiem(2, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                        else
                        {
                            //1 diem
                            _lblScore7.Text = "1";
                            ChamDiem(1, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
                        }
                    }
                    else
                    {
                        //0 diem
                        _lblScore7.Text = "0";
                        ChamDiem(0, ref _luotBia2, _gridControlScore7, _lblName7, _lblScore7, be);
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
                            ChamDiem(10, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //9 diem
                            _lblScore8.Text = "9";
                            ChamDiem(9, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        _lblScore8.Text = "8";
                        ChamDiem(8, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
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
                            ChamDiem(7, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //6 diem
                            _lblScore8.Text = "6";
                            ChamDiem(6, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        //5 diem
                        _lblScore8.Text = "5";
                        ChamDiem(5, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
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
                        ChamDiem(4, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                    else
                    {
                        //3 diem
                        _lblScore8.Text = "3";
                        ChamDiem(3, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
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
                            ChamDiem(2, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                        else
                        {
                            //1 diem
                            _lblScore8.Text = "1";
                            ChamDiem(1, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                        }
                    }
                    else
                    {
                        //0 diem
                        _lblScore8.Text = "0";
                        ChamDiem(0, ref _luotBia3, _gridControlScore8, _lblName8, _lblScore8, be);
                    }
                }
            }
        }

        #endregion
    }
}
