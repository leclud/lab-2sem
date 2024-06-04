using RPN_Logic;
using System.Globalization;
using System.Windows;

namespace WpfAppRPN
{
    class Point(double x, double y)
    {
        public readonly double X = x;
        public readonly double Y = y;
    }

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
            string input = txtboxInput.Text;
            double start = double.Parse(txtboxStart.Text);
            double end = double.Parse(txtboxEnd.Text);
            double scale = double.Parse(txtboxScale.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double step = double.Parse(txtboxStep.Text);

            var canvasGraph = CanvasGraph;

            var print = new Printer(canvasGraph, start, end, step, scale);
            print.DrawAxis();

      

            var calculator = new RPNCalculator(input);
            List<Point> pointsChart = [];

            for (double x = start; x <= end; x += step / 50)
            {
                var y = calculator.CalculateWithX(calculator.Rpn, x);
                pointsChart.Add(new Point(x, y));
            }

            List<Point> pointsScale = [];

            for (double x = start; x <= end; x += step)
            {
                var y = calculator.CalculateWithX(calculator.Rpn, x);
                pointsScale.Add(new Point(x, y));
            }

            print.PlotGraph(pointsChart, pointsScale);
        }
    }
}