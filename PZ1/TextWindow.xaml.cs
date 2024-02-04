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
    /// Interaction logic for TextWindow.xaml
    /// </summary>
    public partial class TextWindow : Window
    {
        public TextWindow()
        {
            InitializeComponent();
        }

        public TextWindow(string text, int size, Color txtColor)
        {
            InitializeComponent();
            textBoxText.Text = text;
            textSize.Text = size.ToString();
            textColor.SelectedColor = txtColor;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            int txtSize;

            if (!string.IsNullOrWhiteSpace(textBoxText.Text) && int.TryParse(textSize.Text, out txtSize)
                && textColor.SelectedColor.HasValue)
            {
                ((MainWindow)Application.Current.MainWindow).DrawText(textBoxText.Text, txtSize, textColor.SelectedColor.Value);
                this.Close();
            }
        }
    }
}
