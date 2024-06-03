using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RPN_Logic;


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

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double xValue = double.Parse(tbVariable.Text);
                string expression = tbExpression.Text;
                expression = expression.Replace("x", xValue.ToString());
                expression = expression.Replace(" ", string.Empty);

                float result = RPNCalculator.Calculator(expression);

                tbResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid expression.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tbResult.Foreground = Brushes.Red;
            }
        }

    }
}
