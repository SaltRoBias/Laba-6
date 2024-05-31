using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Timers;

namespace Лаба_6
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer updateTimer;
        private DateTime currentTime;
        private float bulldozerX;
        private float shovelAngle;
        private bool shovelUp;

        private enum DrawingMode { None, StaticFigures, Face, Clock, Bulldozer }
        private DrawingMode currentMode = DrawingMode.None;

        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(OnPaint);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            switch (currentMode)
            {
                case DrawingMode.StaticFigures:
                    DrawStaticFigures(g);
                    break;
                case DrawingMode.Face:
                    DrawFace(g);
                    break;
                case DrawingMode.Clock:
                    DrawClock(g);
                    break;
                case DrawingMode.Bulldozer:
                    DrawBulldozer(g, bulldozerX, 300, shovelAngle);
                    break;
            }
        }


        private void DrawStaticFigures(Graphics g)
        {
            // Зафарбований сегмент кола
            g.FillPie(Brushes.Blue, 70, 70, 100, 100, 0, 120);

            // Правильний шестикутник
            PointF[] hexagonPoints = GetPolygonPoints(6, 500, 150, 60);
            g.DrawPolygon(Pens.Black, hexagonPoints);

            // Зафарбований трикутник
            PointF[] trianglePoints = GetPolygonPoints(3, 100, 300, 50);
            g.FillPolygon(Brushes.Red, trianglePoints);

            // Дуга еліпса
            g.DrawArc(Pens.Green, 400, 270, 150, 100, 45, 270);
        }

        private PointF[] GetPolygonPoints(int sides, float centerX, float centerY, float radius)
        {
            PointF[] points = new PointF[sides];
            double angle = Math.PI * 2 / sides;

            for (int i = 0; i < sides; i++)
            {
                points[i] = new PointF(
                    centerX + radius * (float)Math.Cos(i * angle),
                    centerY + radius * (float)Math.Sin(i * angle)
                );
            }

            return points;
        }

        private void DrawFace(Graphics g)
        {
            // Обличчя
            g.FillEllipse(Brushes.Yellow, 315, 100, 170, 200);

            // Очі
            g.FillEllipse(Brushes.White, 350, 150, 30, 30);
            g.FillEllipse(Brushes.White, 420, 150, 30, 30);
            g.FillEllipse(Brushes.Black, 360, 160, 10, 10);
            g.FillEllipse(Brushes.Black, 430, 160, 10, 10);

            // Ніс
            g.FillPolygon(Brushes.Orange, new PointF[] {
                new PointF(390, 220),
                new PointF(410, 220),
                new PointF(400, 180)
            });

            // Рот
            g.DrawArc(Pens.Red, 360, 210, 80, 40, 0, 180);
        }

        private void InitializeClock()
        {
            if (updateTimer != null)
            {
                updateTimer.Stop();
            }
            updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 1000; // 1 секунда
            updateTimer.Elapsed += (sender, e) => { this.Invalidate(); };
            updateTimer.Start();
        }

        private void DrawClock(Graphics g)
        {
            currentTime = DateTime.Now;

            // Центр годинника
            float centerX = this.ClientSize.Width / 2;
            float centerY = this.ClientSize.Height / 2;

            // Малювання циферблату
            g.DrawEllipse(Pens.Black, centerX - 100, centerY - 100, 200, 200);

            // Малювання годинної стрілки
            DrawHand(g, centerX, centerY, 50, (currentTime.Hour % 12 + currentTime.Minute / 60f) * 30, Pens.Black);

            // Малювання хвилинної стрілки
            DrawHand(g, centerX, centerY, 80, currentTime.Minute * 6, Pens.Blue);

            // Малювання секундної стрілки
            DrawHand(g, centerX, centerY, 90, currentTime.Second * 6, Pens.Red);
        }

        private void DrawHand(Graphics g, float centerX, float centerY, float length, float angle, Pen pen)
        {
            float angleRad = angle * (float)Math.PI / 180;
            float x = centerX + length * (float)Math.Sin(angleRad);
            float y = centerY - length * (float)Math.Cos(angleRad);
            g.DrawLine(pen, centerX, centerY, x, y);
        }

        private void InitializeBulldozer()
        {
            if (updateTimer != null)
            {
                updateTimer.Stop();
            }
            bulldozerX = -100;
            shovelAngle = 0; // Початковий кут для лопати
            shovelUp = true;

            updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 200; // 0.2 секунди
            updateTimer.Elapsed += (sender, e) =>
            {
                bulldozerX += 5;
                if (bulldozerX > this.ClientSize.Width) bulldozerX = -100;
                shovelAngle = shovelUp ? -5 : 0; // Повернути лопату вгору і вниз
                shovelUp = !shovelUp;
                this.Invalidate();
            };
            updateTimer.Start();
        }

        private void DrawBulldozer(Graphics g, float x, float y, float shovelAngle)
        {
            // Тіло бульдозера
            g.FillRectangle(Brushes.Yellow, x, y - 40, 100, 40); // Жовтий корпус

            // Кабіна бульдозера
            g.FillRectangle(Brushes.Black, x , y - 70, 50, 30); // Чорна кабіна
            g.FillRectangle(Brushes.LightBlue, x + 5, y - 65, 40, 20); // Блакитне скло
            g.FillRectangle(Brushes.Yellow, x , y - 75, 50, 5); // Жовта криша

            // Гусениці
            g.FillRectangle(Brushes.Black, x, y -10, 100, 30); // Чорні гусениці
            g.FillEllipse(Brushes.Black, x - 10 , y - 10, 30, 30); // Чорне колесо ліве
            g.FillEllipse(Brushes.Black, x + 80, y - 10, 30, 30); // Чорне колесо праве
            g.FillRectangle(Brushes.Gray, x + 10, y - 5, 80, 20); // Внутрішні сірі гусениці
            g.FillEllipse(Brushes.Gray, x , y - 5, 20, 20); // Сіре внутрішнє колесо ліве
            g.FillEllipse(Brushes.Gray, x + 80, y - 5, 20, 20); // Сіре внутрішнє колесо праве

            // Лопата бульдозера
            PointF[] shovelPoints = {
                new PointF(x + 120, y + 20),
                new PointF(x + 120, y - 40),
                new PointF(x + 150, y + 20)
            };

            PointF pivotPoint = new PointF(x + 20, y); // Ліва точка штуки що підіймає ковш
            using (Matrix transform = new Matrix())
            {
                transform.RotateAt(shovelAngle, pivotPoint);
                g.Transform = transform;

                // Штука що підіймає ковш
                g.FillRectangle(Brushes.Yellow, x + 10, y, 115, 5);

                // Лопата бульдозера
                g.FillPolygon(Brushes.Yellow, shovelPoints);

                g.ResetTransform();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.StaticFigures;
            this.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Face;
            this.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Clock;
            InitializeClock();
            this.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Bulldozer;
            InitializeBulldozer();
            this.Invalidate();
        }
    }
}
