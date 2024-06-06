using Rpn_Logic;
using System.Globalization;
using System.Windows;

namespace WpfAppRPN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CanvasGraph.Children.Clear();
            DrawCanvas();
        }

        private void DrawCanvas()
        {
            string input = Expression.Text;
            float start = float.Parse(tbStart.Text);
            float end = float.Parse(tbEnd.Text);    
            float scale = float.Parse(tbScale.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            float step = float.Parse(tbStep.Text);

            var canvasGraph = CanvasGraph;

            var print = new Printer(canvasGraph, start, end, step, scale);
            print.DrawAxis();

      

            var calculator = new RpnCalculator(input);
            List<Point> pointsChart = [];

            for (float x = start; x <= end; x += step / 50)
            {
                var y = calculator.Calculate(x);
                pointsChart.Add(new Point(x, y));
            }

            List<Point> pointsScale = [];

            for (float x = start; x <= end; x += step)
            {
                var y = calculator.Calculate(x);
                pointsScale.Add(new Point(x, y));
            }

            print.PlotGraph(pointsChart, pointsScale);
        }
    }

    class Point(double x, double y)
    {
        public readonly double X = x;
        public readonly double Y = y;
    }

}