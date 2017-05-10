using ChamDiem;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace CameraApp.Help
{
    class ExcelHelp
    {
        public static void ExportExcel(DataTable dtBia1, DataTable dtBia2, DataTable dtBia3)
        {
            try
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "xls files (*.xls)|*.xls";
                sd.FilterIndex = 2;
                sd.RestoreDirectory = true;

                int[] tongDiem = new int[dtBia1.Rows.Count];

                if (sd.ShowDialog() == DialogResult.OK)
                {
                    object misValue = System.Reflection.Missing.Value;
                    Excel.Application xlApp = new Excel.Application();

                    if (xlApp == null)
                    {
                        Console.WriteLine("EXCEL could not be started. Check that your office installation and project references are correct.");
                        return;
                    }

                    Excel.Workbook workbook = xlApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                    // diem bia 1
                    Excel.Worksheet worksheetBia1 = (Excel.Worksheet)workbook.Worksheets[1];
                    worksheetBia1.Name = "Điểm số bia 1";
                    //tao ten collumn
                    worksheetBia1.Cells[1, 1] = "Họ và tên";
                    worksheetBia1.Cells[1, 2] = "Lần 1";
                    worksheetBia1.Cells[1, 3] = "Lần 2";
                    worksheetBia1.Cells[1, 4] = "Lần 3";
                    worksheetBia1.Cells[1, 5] = "Tổng";
                    for (int i = 0; i <= dtBia1.Rows.Count - 1; i++)
                    {
                        // tru them 2 cot tong diem va xep loai nen la -3
                        for (int j = 0; j <= dtBia1.Columns.Count - 3; j++)
                        {
                            string data = dtBia1.Rows[i].ItemArray[j].ToString();
                            // table excel bat dau tu 1 nen phai +1 so vs table C#
                            worksheetBia1.Cells[i + 1 + 1, j + 1] = data;
                            //cong tong diem, j = 0 la ten
                            if(j > 0 && j < dtBia1.Columns.Count - 3)
                            {
                                try
                                {
                                    tongDiem[i] += Int16.Parse(data);
                                }
                                catch (Exception exInt)
                                { }
                            }
                        }
                    }
                    worksheetBia1.Columns.AutoFit();
                    // diem bia 2
                    Excel.Worksheet worksheetBia2 = (Excel.Worksheet)workbook.Worksheets.Add(misValue, misValue, misValue, misValue); //(Excel.Worksheet)workbook.Worksheets[2];
                    worksheetBia2.Name = "Điểm số bia 2";
                    //tao ten collumn
                    worksheetBia2.Cells[1, 1] = "Họ và tên";
                    worksheetBia2.Cells[1, 2] = "Lần 1";
                    worksheetBia2.Cells[1, 3] = "Lần 2";
                    worksheetBia2.Cells[1, 4] = "Lần 3";
                    worksheetBia2.Cells[1, 5] = "Tổng";
                    for (int i = 0; i <= dtBia2.Rows.Count - 1; i++)
                    {
                        // tru them 2 cot tong diem va xep loai nen la -3
                        for (int j = 0; j <= dtBia2.Columns.Count - 3; j++)
                        {
                            string data = dtBia2.Rows[i].ItemArray[j].ToString();
                            worksheetBia2.Cells[i + 1 + 1, j + 1] = data;
                            //cong tong diem, j = 0 la ten
                            if (j > 0 && j < dtBia2.Columns.Count - 3)
                            {
                                try
                                {
                                    tongDiem[i] += Int16.Parse(data);
                                }
                                catch (Exception exInt)
                                { }
                            }
                        }
                    }
                    worksheetBia2.Columns.AutoFit();
                    // diem bia 3
                    Excel.Worksheet worksheetBia3 = (Excel.Worksheet)workbook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    worksheetBia3.Name = "Điểm số bia 3";
                    //tao ten collumn
                    worksheetBia3.Cells[1, 1] = "Họ và tên";
                    worksheetBia3.Cells[1, 2] = "Lần 1";
                    worksheetBia3.Cells[1, 3] = "Lần 2";
                    worksheetBia3.Cells[1, 4] = "Lần 3";
                    worksheetBia3.Cells[1, 5] = "Tổng";
                    for (int i = 0; i <= dtBia3.Rows.Count - 1; i++)
                    {
                        // tru them 2 cot tong diem va xep loai nen la -3
                        for (int j = 0; j <= dtBia3.Columns.Count - 3; j++)
                        {
                            string data = dtBia3.Rows[i].ItemArray[j].ToString();
                            worksheetBia3.Cells[i + 1 + 1, j + 1] = data;
                            //cong tong diem, j = 0 : la ten, j = dtBia3.Columns.Count - 3 : la tong diem
                            if (j > 0 && j < dtBia3.Columns.Count - 3)
                            {
                                try
                                {
                                    tongDiem[i] += Int16.Parse(data);
                                }
                                catch (Exception exInt)
                                { }
                            }
                        }
                    }
                    worksheetBia3.Columns.AutoFit();

                    // Worksheet tong diem va ti le phan tram
                    Excel.Worksheet worksheetTong = (Excel.Worksheet)workbook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    worksheetTong.Name = "Điểm số tổng";
                    //tao ten collumn
                    worksheetTong.Cells[1, 1] = "Họ và tên";
                    worksheetTong.Cells[1, 2] = "Tổng điểm 3 bia";
                    worksheetTong.Cells[1, 3] = "Xếp loại";
                    /*
                    + Giỏi: bắn : 72 điểm trở lên.
                    + Khá: 59 đến 71 điểm.
                    + Đạt: 45 đến 58 điểm.
                    + Không đạt: Dưới 45 điểm.
                    * */
                    int numGioi = 0;
                    int numKha = 0;
                    int numDat = 0;
                    int numKhongDat = 0;
                    for (int i = 0; i <= dtBia3.Rows.Count - 1; i++)
                    {
                        string data = dtBia3.Rows[i].ItemArray[0].ToString();
                        worksheetTong.Cells[i + 1 + 1, 1] = data;
                        worksheetTong.Cells[i + 1 + 1, 2] = tongDiem[i];
                        string xeploai = "Không đạt";// ngoai dieu kien ben duoi la khong dat thanh tich
                        if(tongDiem[i] >= 72)
                        {
                            xeploai = "Giỏi";
                            numGioi++;
                        }
                        else if(tongDiem[i] >= 59 && tongDiem[i] <= 71)
                        {
                            xeploai = "Khá";
                            numKha++;
                        }
                        else if (tongDiem[i] >= 45 && tongDiem[i] <= 58)
                        {
                            xeploai = "Đạt";
                            numDat++;
                        }
                        else
                        {
                            numKhongDat++;
                        }

                        worksheetTong.Cells[i + 1 + 1, 3] = xeploai;
                    }

                    worksheetTong.Cells[dtBia3.Rows.Count + 3, 1] = "Tỉ lệ % xếp loại giỏi";
                    float tlGioi = (float)numGioi / (float)dtBia3.Rows.Count * 100;
                    worksheetTong.Cells[dtBia3.Rows.Count + 3, 2] = tlGioi + "%";

                    worksheetTong.Cells[dtBia3.Rows.Count + 4, 1] = "Tỉ lệ % xếp loại khá";
                    float tlKha = (float)numKha / (float)dtBia3.Rows.Count * 100;
                    worksheetTong.Cells[dtBia3.Rows.Count + 4, 2] = tlKha + "%";

                    worksheetTong.Cells[dtBia3.Rows.Count + 5, 1] = "Tỉ lệ % xếp loại đạt";
                    float tlDat = (float)numDat / (float)dtBia3.Rows.Count * 100;
                    worksheetTong.Cells[dtBia3.Rows.Count + 5, 2] = tlDat + "%";

                    worksheetTong.Cells[dtBia3.Rows.Count + 6, 1] = "Tỉ lệ % xếp loại không đạt";
                    float tlKhongDat = (float)numKhongDat / (float)dtBia3.Rows.Count * 100;
                    worksheetTong.Cells[dtBia3.Rows.Count + 6, 2] = tlKhongDat + "%";

                    worksheetTong.Columns.AutoFit();

                    workbook.SaveAs(sd.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    workbook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    releaseObject(worksheetBia1);
                    releaseObject(worksheetBia2);
                    releaseObject(worksheetBia3);
                    releaseObject(worksheetTong);
                    releaseObject(workbook);
                    releaseObject(xlApp);
                    MessageBox.Show("Xuất file hoàn tất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        public static void ExportExcel1Bia(DataTable dtBia)
        {
            try
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "xls files (*.xls)|*.xls";
                sd.FilterIndex = 2;
                sd.RestoreDirectory = true;

                int[] tongDiem = new int[dtBia.Rows.Count];

                if (sd.ShowDialog() == DialogResult.OK)
                {
                    object misValue = System.Reflection.Missing.Value;
                    Excel.Application xlApp = new Excel.Application();

                    if (xlApp == null)
                    {
                        Console.WriteLine("EXCEL could not be started. Check that your office installation and project references are correct.");
                        return;
                    }

                    Excel.Workbook workbook = xlApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                    // diem bia 1
                    Excel.Worksheet worksheetBia1 = (Excel.Worksheet)workbook.Worksheets[1];
                    worksheetBia1.Name = "Điểm số";
                    //tao ten collumn
                    worksheetBia1.Cells[1, 1] = "Họ và tên";
                    worksheetBia1.Cells[1, 2] = "Lần 1";
                    worksheetBia1.Cells[1, 3] = "Lần 2";
                    worksheetBia1.Cells[1, 4] = "Lần 3";
                    worksheetBia1.Cells[1, 5] = "Tổng";
                    for (int i = 0; i <= dtBia.Rows.Count - 1; i++)
                    {
                        // tru them 2 cot tong diem va xep loai nen la -3
                        for (int j = 0; j <= dtBia.Columns.Count - 2; j++)
                        {
                            string data = dtBia.Rows[i].ItemArray[j].ToString();
                            // table excel bat dau tu 1 nen phai +1 so vs table C#
                            worksheetBia1.Cells[i + 1 + 1, j + 1] = data;
                            //cong tong diem, j = 0 la ten
                            if (j == dtBia.Columns.Count - 2)
                            {
                                try
                                {
                                    tongDiem[i] = Int16.Parse(data);
                                }
                                catch (Exception exInt)
                                { }
                            }
                        }
                    }
                    worksheetBia1.Columns.AutoFit();
                    
                    // Worksheet tong diem va ti le phan tram
                    Excel.Worksheet worksheetTong = (Excel.Worksheet)workbook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    worksheetTong.Name = "Điểm số tổng";
                    //tao ten collumn
                    worksheetTong.Cells[1, 1] = "Họ và tên";
                    worksheetTong.Cells[1, 2] = "Tổng điểm";
                    worksheetTong.Cells[1, 3] = "Xếp loại";
                    /*
                    + Giỏi: bắn : 24 điểm trở lên.
                    + Khá: 19 đến 23 điểm.
                    + Đạt: 15 đến 18 điểm.
                    + Không đạt: Dưới 15 điểm.
                    * */
                    int numGioi = 0;
                    int numKha = 0;
                    int numDat = 0;
                    int numKhongDat = 0;
                    for (int i = 0; i <= dtBia.Rows.Count - 1; i++)
                    {
                        string data = dtBia.Rows[i].ItemArray[0].ToString();
                        worksheetTong.Cells[i + 1 + 1, 1] = data;
                        worksheetTong.Cells[i + 1 + 1, 2] = tongDiem[i];
                        string xeploai = "Không đạt";// ngoai dieu kien ben duoi la khong dat thanh tich
                        if (tongDiem[i] >= 24)
                        {
                            xeploai = "Giỏi";
                            numGioi++;
                        }
                        else if (tongDiem[i] >= 19 && tongDiem[i] <= 23)
                        {
                            xeploai = "Khá";
                            numKha++;
                        }
                        else if (tongDiem[i] >= 15 && tongDiem[i] <= 18)
                        {
                            xeploai = "Đạt";
                            numDat++;
                        }
                        else
                        {
                            numKhongDat++;
                        }

                        worksheetTong.Cells[i + 1 + 1, 3] = xeploai;
                    }

                    worksheetTong.Cells[dtBia.Rows.Count + 3, 1] = "Tỉ lệ % xếp loại giỏi";
                    float tlGioi = (float)numGioi / (float)dtBia.Rows.Count * 100;
                    worksheetTong.Cells[dtBia.Rows.Count + 3, 2] = tlGioi + "%";

                    worksheetTong.Cells[dtBia.Rows.Count + 4, 1] = "Tỉ lệ % xếp loại khá";
                    float tlKha = (float)numKha / (float)dtBia.Rows.Count * 100;
                    worksheetTong.Cells[dtBia.Rows.Count + 4, 2] = tlKha + "%";

                    worksheetTong.Cells[dtBia.Rows.Count + 5, 1] = "Tỉ lệ % xếp loại đạt";
                    float tlDat = (float)numDat / (float)dtBia.Rows.Count * 100;
                    worksheetTong.Cells[dtBia.Rows.Count + 5, 2] = tlDat + "%";

                    worksheetTong.Cells[dtBia.Rows.Count + 6, 1] = "Tỉ lệ % xếp loại không đạt";
                    float tlKhongDat = (float)numKhongDat / (float)dtBia.Rows.Count * 100;
                    worksheetTong.Cells[dtBia.Rows.Count + 6, 2] = tlKhongDat + "%";

                    worksheetTong.Columns.AutoFit();

                    workbook.SaveAs(sd.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    workbook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    releaseObject(worksheetBia1);
                    releaseObject(worksheetTong);
                    releaseObject(workbook);
                    releaseObject(xlApp);
                    MessageBox.Show("Xuất file hoàn tất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
        }

        public static DataTable getDataTableExcelFor1Bia(string fileName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Họ và tên");
            dt.Columns.Add("Lần 1");
            dt.Columns.Add("Lần 2");
            dt.Columns.Add("Lần 3");
            dt.Columns.Add("Tổng");
            dt.Columns.Add("Xếp loại");
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook workBook = app.Workbooks.Open(fileName);
                Excel.Worksheet workSheet = (Excel.Worksheet)workBook.ActiveSheet;

                int index = 0;
                object rowIndex = 2;

                while (((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1]).Value2 != null)
                {
                    rowIndex = 2 + index;
                    Excel.Range range = (Excel.Range)workSheet.Cells[rowIndex, 1];
                    string value = range.Value2.ToString();

                    if (!value.Equals(""))
                    {
                        DataRow row;
                        row = dt.NewRow();
                        row[0] = value;
                        dt.Rows.Add(row);
                    }
                    index++;
                }
                workBook.Close();
                app.Quit();
                Marshal.ReleaseComObject(workSheet);
                Marshal.ReleaseComObject(workBook);
                Marshal.ReleaseComObject(app);
                workSheet = null;
                workBook = null;
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
            return dt;
        }


        public static DataTable getDataTableExcelFor3Bia(string fileName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Họ và tên");
            dt.Columns.Add("Lần 1");
            dt.Columns.Add("Lần 2");
            dt.Columns.Add("Lần 3");
            dt.Columns.Add("Tổng");
            dt.Columns.Add("Tổng 3 bia");
            dt.Columns.Add("Xếp loại");
            try
            {
                Excel.Application app = new Excel.Application();
                //Excel.Workbook workBook = app.Workbooks.Open(fileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Excel.Workbook workBook = app.Workbooks.Open(fileName);
                Excel.Worksheet workSheet = (Excel.Worksheet)workBook.ActiveSheet;

                int index = 0;
                object rowIndex = 2;

                while (((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1]).Value2 != null)
                {
                    rowIndex = 2 + index;
                    Excel.Range range = (Excel.Range)workSheet.Cells[rowIndex, 1];
                    string value = range.Value2.ToString();//Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1]).Value2);
                    
                    if (!value.Equals(""))
                    {
                        DataRow row;
                        row = dt.NewRow();
                        row[0] = value;// Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1]).Value2);
                        //row[1] = "";//Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 2]).Value2);
                        //row[2] = "";//Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 3]).Value2);
                        //row[3] = "";//Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 4]).Value2);
                        //row[4] = "";//Convert.ToString(((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 5]).Value2);
                        //row[5] = "";
                        //row[6] = "";
                        dt.Rows.Add(row);
                    }
                    index++;
                }
                workBook.Close();
                app.Quit();
                //releaseObject(workSheet);
                //releaseObject(workBook);
                //releaseObject(app);
                Marshal.ReleaseComObject(workSheet);
                Marshal.ReleaseComObject(workBook);
                Marshal.ReleaseComObject(app);
                workSheet = null;
                workBook = null;
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString());
            }
            return dt;
        }

        public static DataTable OpenFile(string fileName)
        {
            //var fullFileName = string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), fileName);
            //if (!File.Exists(fullFileName))
            if (!File.Exists(fileName))
            {
                System.Windows.Forms.MessageBox.Show("File not found");
                return null;
            }
            //var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fullFileName);
            var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName);
            var adapter = new OleDbDataAdapter("select * from [Sheet1$]", connectionString);
            var ds = new DataSet();
            string tableName = "excelData";
            adapter.Fill(ds, tableName);
            DataTable data = ds.Tables[tableName];
            return data;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                File.AppendAllText("log.txt", ex.ToString());
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
