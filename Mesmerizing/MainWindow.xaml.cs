using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mesmerizing
{
    public partial class MainWindow : Window
    {
        private const int MirrorCount = 40;
        private Brush canvasBackground;
        private Brush paintBrush;
        private Polyline fakeDrawingLine;
        private Point drawingPoint;
        private Polyline[] drawingLines = new Polyline[MirrorCount];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeCanvas()
        {
            canvasBackground = Brushes.Black;
            paintBrush = Brushes.White;

            CanvasPaint.Background = canvasBackground;

            double canvasWidth = CanvasPaint.ActualWidth;
            double canvasHeight = CanvasPaint.ActualHeight;
            double canvasCenterX = canvasWidth / 2;
            double canvasCenterY = canvasHeight / 2;

            double radianMultiplier = Math.PI / 180;
            for (int i = 0; i <= 360; i += (360 / MirrorCount))
            {
                double x = canvasWidth * Math.Cos(i * radianMultiplier) + canvasCenterX;
                double y = canvasHeight * Math.Sin(i * radianMultiplier) + canvasCenterY;

                Polyline skecthLine = BuildLine(paintBrush, 2);
                skecthLine.Points.Add(new Point(canvasCenterX, canvasCenterY));
                skecthLine.Points.Add(new Point(x, y));
                CanvasPaint.Children.Add(skecthLine);
            }

            fakeDrawingLine = BuildLine(Brushes.Transparent, 1);
            CanvasPaint.Children.Add(fakeDrawingLine);

            for (int i = 0; i < drawingLines.Length; i++)
            {
                drawingLines[i] = BuildLine(paintBrush, 3);
                CanvasPaint.Children.Add(drawingLines[i]);
            }
        }
        private Polyline BuildLine(Brush paint, int stroke)
        {
            Polyline line = new Polyline();
            line.Stroke = paint;
            line.StrokeThickness = stroke;

            return line;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCanvas();
        }

        private void CanvasPaint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                drawingPoint = e.GetPosition(this);
        }

        private void CanvasPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                drawingPoint = e.GetPosition(CanvasPaint);
                fakeDrawingLine.Points.Add(drawingPoint);

                int x = 360 / MirrorCount;
                int i = 0;
                foreach (Polyline polyLine in drawingLines)
                {
                    polyLine.Points.Clear();

                    for (int k = 0; k < fakeDrawingLine.Points.Count; k++)
                    {
                        if (k + 2 < fakeDrawingLine.Points.Count)
                        {
                            if ((fakeDrawingLine.Points[k + 2].X - 2 < fakeDrawingLine.Points[k].X && fakeDrawingLine.Points[k].X <= fakeDrawingLine.Points[k + 2].X + 2) && (fakeDrawingLine.Points[k + 2].Y - 2 < fakeDrawingLine.Points[k].Y && fakeDrawingLine.Points[k].Y <= fakeDrawingLine.Points[k + 2].Y + 2))
                            {

                                fakeDrawingLine.Points[k] = fakeDrawingLine.Points[k + 1];
                            }
                        }
                        polyLine.Points.Add(fakeDrawingLine.Points[k]);
                    }

                    Matrix matrix = new Matrix();
                    matrix.RotateAt(x * i, CanvasPaint.ActualWidth / 2, CanvasPaint.ActualHeight / 2);

                    var transformGroup = new TransformGroup();
                    transformGroup.Children.Add(new MatrixTransform(matrix));
                    if (i % 2 == 0)
                    {
                        transformGroup.Children.Add(new ScaleTransform(1, -1, CanvasPaint.ActualWidth / 2, CanvasPaint.ActualHeight / 2));
                        matrix.RotateAt(x * i - 90, CanvasPaint.ActualWidth / 2, CanvasPaint.ActualHeight / 2);
                        transformGroup.Children.Add(new MatrixTransform(matrix));
                    }

                    polyLine.RenderTransform = transformGroup;
                    i++;
                }
            }
        }

        private void CanvasPaint_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < drawingLines.Length; i++)
            {
                fakeDrawingLine.Points.Clear();
                drawingLines[i].Points.Clear();
            }
        }
    }
}