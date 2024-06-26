﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;
using NCalc;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace Graph_Calculator
{
    class Graph
    {
        Bitmap originBmp;
        string expString;
        Grid grid;
        
        public Graph(Grid grid, string expString)
        {
            this.grid = grid;
            this.expString = expString;
            originBmp = new Bitmap(grid.Width, grid.Height);
        }

        public void drawGraph()
        {
            originBmp = new Bitmap(grid.Width, grid.Height);
            Graphics temp = Graphics.FromImage(originBmp);
            temp.SmoothingMode = SmoothingMode.HighSpeed;

            Pen p = new Pen(Brushes.Black, 1);

            using (GraphicsPath gP = new GraphicsPath())
            {
                // Đồ thị sẽ được vẽ từ x = start -> end
                float start;
                float end;
                float step = 0.1f;
    
                if (expString.Contains("Log"))
                {
                    start = 0.1f;
                    end = ((grid.Width / grid.Magnification / 2));
                }
                else {
                    start = -((grid.Width / grid.Magnification / 2));
                    end = -start;
                }


                // Dùng thư viện Ncalc để chuyển chuỗi thành một biểu thức
                NCalc.Expression exp = new NCalc.Expression(expString);
                for (float x = start; x <= end; x += step)
                {
                    try
                    {
                        // Thay thế biến x trong biểu thức thành giá trị x
                        exp.Parameters["x"] = x;
                        /* Chuyển kết quả về chuỗi vì trong thư viện NCalc sẽ trả về object(double) với biểu thức chưa sin()/cos/tan và object(float) với các biểu thức còn lại. Vì kiểu float không thể giữ nhiều chữ số hàng thập phân */
                        string curY = exp.Evaluate().ToString();

                        exp.Parameters["x"] = x + step;
                        string nextY = exp.Evaluate().ToString();

                        // Nếu với x không thể tính ra được kết quả
                        if (curY == "NaN" || nextY == "NaN")
                            continue;

                        // Đưa về vị trí chuẩn trong hệ quy chiếu oxy
                        float x1 = grid.RootPoint.X + x * grid.Magnification;
                        float y1 = grid.RootPoint.Y - float.Parse(curY) * grid.Magnification;
                        float x2 = grid.RootPoint.X + (x + step) * grid.Magnification;
                        float y2 = grid.RootPoint.Y - float.Parse(nextY) * grid.Magnification;

                         //Với hàm log(2,x) hoặc log(x,2) thì sẽ không liên tục nên phải tách ra để tránh sai xót
                        if (y1 < 0 && y2 > 0 || y1 > 0 && y2 < 0)
                        {
                            temp.DrawPath(p, gP);
                            gP.Reset();
                        }
                        else
                        gP.AddLine(x1, y1, x2, y2);


                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("Hãy kiểm tra lại phương trình");
                        return;
                    }
                    catch (EvaluationException)
                    {
                        MessageBox.Show("Hãy kiểm tra lại phương trình");
                        return;
                    }

                    // Tạo một bitmap để vẽ lên rồi sau đó chuyển lên form -> giảm giật
                    try
                    {
                        temp.DrawPath(p, gP);
                    }
                    catch (OverflowException)
                    {
                        //MessageBox.Show("Hiện tại chưa thể xử lý được phương trình này. Hãy thử phóng to và thử lại!");
                        return;
                    }
            }
        }
        }

        public Bitmap OriginBmp
        {
            get
            {
                return originBmp;
            }

            set
            {
                originBmp = value;
            }
        }


        public string ExpString
        {
            get
            {
                return expString;
            }

            set
            {
                expString = value;
            }
        }

        internal Grid Grid
        {
            get
            {
                return grid;
            }

            set
            {
                grid = value;
            }
        }
    }
    
}
