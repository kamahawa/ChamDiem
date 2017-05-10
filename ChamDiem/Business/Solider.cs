using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChamDiem
{
    class Solider
    {
        public string name = "";//ho va ten
        public int luotBanBia4 = 0;//so luot ban bia 4
        public int luotBanBia7 = 0;//so luot ban bia 7
        public int luotBanBia8 = 0;//so luot ban bia 8

        int tongDiemBia4 = 0;//tong diem luot ban o bia 4
        int tongDiemBia7 = 0;//tong diem luot ban o bia 7
        int tongDiemBia8 = 0;//tong diem luot ban o bia 8
        int tongDiem3Bia = 0;//tong diem 3 bia


        public int GetTongDiem3Bia()
        {
            return tongDiemBia4 + tongDiemBia7 + tongDiemBia8;
        }

        public int GetTongDiemBia4()
        {
            return tongDiemBia4;
        }

        public int GetTongDiemBia7()
        {
            return tongDiemBia7;
        }

        public int GetTongDiemBia8()
        {
            return tongDiemBia8;
        }

        public void SetTongDiemBia4(int diem)
        {
            //moi nguoi ban 3 luot, neu hon thi k tinh diem nua
            if(luotBanBia4 <= 3)
            {
                tongDiemBia4 += diem;
            }
        }

        public void SetTongDiemBia7(int diem)
        {
            //moi nguoi ban 3 luot, neu hon thi k tinh diem nua
            if (luotBanBia7 <= 3)
            {
                tongDiemBia7 += diem;
            }
        }

        public void SetTongDiemBia8(int diem)
        {
            //moi nguoi ban 3 luot, neu hon thi k tinh diem nua
            if (luotBanBia8 <= 3)
            {
                tongDiemBia8 += diem;
            }
        }

        public string XepLoai1Bia(int tong)
        {
            string xeploai = "Không đạt";
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
            return xeploai;
        }

        public string XepLoai3Bia(int tong)
        {
            string xeploai = "Không đạt";
            if (tong >= 72)
            {
                xeploai = "Giỏi";
            }
            else if (tong >= 59 && tong <= 71)
            {
                xeploai = "Khá";
            }
            else if (tong >= 45 && tong <= 58)
            {
                xeploai = "Đạt";
            }
            return xeploai;
        }

        public void reset()
        {
            luotBanBia4 = 0;//so luot ban bia 4
            luotBanBia7 = 0;//so luot ban bia 7
            luotBanBia8 = 0;//so luot ban bia 8

            tongDiemBia4 = 0;//tong diem luot ban o bia 4
            tongDiemBia7 = 0;//tong diem luot ban o bia 7
            tongDiemBia8 = 0;//tong diem luot ban o bia 8
            tongDiem3Bia = 0;//tong diem 3 bia
        }
    }
}
