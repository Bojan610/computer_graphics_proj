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
    /// Interaction logic for PolygonWindow.xaml
    /// </summary>
    public partial class PolygonWindow : Window
    {
        private List<System.Windows.Point> points = new List<Point>();

        public PolygonWindow()
        {
            InitializeComponent();
        }

        public PolygonWindow(List<System.Windows.Point> points)
        {
            InitializeComponent();
            this.points = points;
        }

        public PolygonWindow(List<System.Windows.Point> points, int thic, Color fillCol, Color borColor)
        {
            this.points = points;
            InitializeComponent();
            borderThickness.Text = thic.ToString();
            colorFill.SelectedColor = fillCol;
            colorBorder.SelectedColor = borColor;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            int thic;

            if (int.TryParse(borderThickness.Text, out thic) && colorBorder.SelectedColor.HasValue && colorFill.SelectedColor.HasValue)
            {
                ((MainWindow)Application.Current.MainWindow).DrawPolygon(points, thic, colorFill.SelectedColor.Value, colorBorder.SelectedColor.Value);
                this.Close();
            }
        }
    }
}
