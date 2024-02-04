using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PZ1
{
    /// <summary>
    /// Interaction logic for EllipseWindow.xaml
    /// </summary>
    public partial class EllipseWindow : Window
    {
        public EllipseWindow()
        {
            InitializeComponent();
        }

        public EllipseWindow(int radX, int radY, int thick, Color fill, Color border)
        {
            InitializeComponent();
            radiusX.Text = radX.ToString();
            radiusY.Text = radY.ToString();
            borderThickness.Text = thick.ToString();
            colorFill.SelectedColor = fill;
            colorBorder.SelectedColor = border;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            int radX;
            int radY;
            int thic;

            if (int.TryParse(radiusX.Text, out radX) && int.TryParse(radiusY.Text, out radY) &&
                int.TryParse(borderThickness.Text, out thic) && colorBorder.SelectedColor.HasValue && colorFill.SelectedColor.HasValue)
            {
                ((MainWindow)Application.Current.MainWindow).DrawEllipse(radX, radY, thic, colorFill.SelectedColor.Value, colorBorder.SelectedColor.Value);
                this.Close();
            }
        }
    }
}
