﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GraphCalculator
{
    class Graph
    {
        private Graphics grp; // Đối tượng đồ hoạ để vẽ
        private int magnification; // Độ thu phóng của đồ thị
        private float cellSize; // Kích thước một ô của đồ thị
        private PointF rootPoint; // Toạ độ điểm gốc O
        private int panelWidth; // Chiều rộng của panel vẽ
        private int panelHeight; // Chiều cao của panel vẽ

        public Graph(Panel panelGrid)
        {
            this.grp = panelGrid.CreateGraphics();
            this.magnification = 5;
            this.cellSize = 10 * magnification;
            //
            this.panelWidth = panelGrid.Width;
            this.panelHeight = panelGrid.Height;
            //
            this.rootPoint.X = panelWidth / 2;
            this.rootPoint.Y = panelHeight / 2;
        }

        /// <summary>
        /// Vẽ các đường lưới
        /// </summary>
        public void drawGrid()
        {
            Pen penXY = new Pen(Brushes.Black, 1);
            // Điều chỉnh đầu bút thành mũi tên đặc
            AdjustableArrowCap arrowCap = new AdjustableArrowCap(5, 5, true);
            penXY.CustomEndCap = arrowCap;
            Pen penGrid = new Pen(Color.FromArgb(50, 0, 0, 0), 1);

            // Hiệu chỉnh kích cỡ ô theo độ thu phóng
            this.cellSize = 10 * magnification;

            // Vẽ các trục oxy và ký hiệu oxy
            grp.DrawLine(penXY, new PointF(panelWidth / 2, panelHeight), new PointF(panelWidth / 2, 0));
            grp.DrawLine(penXY, new PointF(0, panelHeight / 2), new PointF(panelWidth, panelHeight / 2));
            grp.DrawString("O", new Font("Tahoma", 14), Brushes.Black, new PointF(panelWidth / 2 + 2, panelHeight / 2 + 2));
            grp.DrawString("x", new Font("Tahoma", 14), Brushes.Black, new PointF(panelWidth - 12, panelHeight / 2));
            grp.DrawString("y", new Font("Tahoma", 14), Brushes.Black, new PointF(panelWidth / 2 + 5, 0));

            // Vẽ đường lưới
            for (int i = 1; i < (panelWidth / cellSize) / 2; i++)
            {
                // Vẽ đối xứng các đường lưới
                grp.DrawLine(penGrid, new PointF(panelWidth / 2 + i * cellSize, 0), new PointF(panelWidth / 2 + i * cellSize, panelHeight));
                grp.DrawLine(penGrid, new PointF(panelWidth / 2 + -i * cellSize, 0), new PointF(panelWidth / 2 + -i * cellSize, panelHeight));

                grp.DrawLine(penGrid, new PointF(0, panelHeight / 2 + i * cellSize), new PointF(panelWidth, panelHeight / 2 + i * cellSize));
                grp.DrawLine(penGrid, new PointF(0, panelHeight / 2 + -i * cellSize), new PointF(panelWidth, panelHeight / 2 + -i * cellSize));

                //Nếu độ lớn của cột đủ rộng thì vẽ thêm các giá trị trục x,y
                if (magnification >= 3)
                {
                    grp.DrawString((i * 10).ToString(), new Font("Tahoma", 8), Brushes.Black, new PointF(panelWidth / 2 + i * cellSize, panelHeight / 2));
                    grp.DrawString((-i * 10).ToString(), new Font("Tahoma", 8), Brushes.Black, new PointF(panelWidth / 2 + -i * cellSize, panelHeight / 2));

                    grp.DrawString((i * 10).ToString(), new Font("Tahoma", 8), Brushes.Black, new PointF(panelWidth / 2 - 15, panelHeight / 2 + -i * cellSize));
                    grp.DrawString((-i * 10).ToString(), new Font("Tahoma", 8), Brushes.Black, new PointF(panelWidth / 2 - 20, panelHeight / 2 + i * cellSize));
                }

            }

        }

        /// <summary>
        /// Vẽ đồ thị
        /// </summary>
        /// <param name="expString"></param>
        public void drawGraph(string expString)
        {
            using (Pen p = new Pen(Brushes.Black, 1))
            using (GraphicsPath gP = new GraphicsPath())
            {
                // Đồ thị sẽ được vẽ từ x = start -> end
                float start = -((panelWidth/magnification/2)-3);
                float end = -start;

               
                // Dùng thư viện Ncalc để chuyển chuỗi thành một biểu thức
                NCalc.Expression exp = new NCalc.Expression(expString);
                float step = 0.1f;
                for (float x = start; x <= end; x += step)
                {
                    // Thay thế biến x trong biểu thức thành giá trị x
                    exp.Parameters["x"] = x;
                    /* Chuyển kết quả về chuỗi vì trong thư viện NCalc sẽ trả về object(double) với biểu thức chưa sin()/cos/tan và object(float) với các biểu thức còn lại. Vì kiểu float không thể giữ nhiều chữ số hàng thập phân */
                    string curY = exp.Evaluate().ToString(); 

                    exp.Parameters["x"] = x + step;
                    string nextY = exp.Evaluate().ToString();

                    // Đưa về vị trí chuẩn trong hệ quy chiếu oxy
                    float x1 = rootPoint.X + x * magnification;
                    float y1 = rootPoint.Y - float.Parse(curY)* magnification;
                    float x2 = rootPoint.X + (x + step) * magnification;
                    float y2 = rootPoint.Y - float.Parse(nextY) * magnification;

                    gP.AddLine(x1, y1, x2, y2);
                }

                // Tạo một bitmap để vẽ lên rồi sau đó chuyển lên form -> giảm giật
                Bitmap bmp = new Bitmap(panelWidth, panelHeight);
                using (Graphics temp = Graphics.FromImage(bmp))
                {
                    temp.SmoothingMode = SmoothingMode.AntiAlias;
                    temp.DrawPath(p, gP);
                }
                grp.DrawPath(p, gP);
            }
        }


        /// <summary>
        /// Xoá đồ thị cũ
        /// </summary>
        public void clearPanel()
        {
            grp.Clear(Color.White);
            this.drawGrid();
        }

        public int Magnification
        {
            get
            {
                return magnification;
            }

            set
            {
                magnification = value;
            }
        }

        public float CellSize
        {
            get
            {
                return cellSize;
            }

            set
            {
                cellSize = value;
            }
        }

        public PointF RootPoint
        {
            get
            {
                return rootPoint;
            }

            set
            {
                rootPoint = value;
            }
        }

        public int Width
        {
            get
            {
                return panelWidth;
            }

            set
            {
                panelWidth = value;
            }
        }

        public int Height
        {
            get
            {
                return panelHeight;
            }

            set
            {
                panelHeight = value;
            }
        }
    }
}
