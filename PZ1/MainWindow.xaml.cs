using GMap.NET.MapProviders;
using Microsoft.Win32;
using PZ1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string elementToDraw = "";
        private System.Windows.Point startPosition;
        private List<System.Windows.Point> PolygonPoints = new List<System.Windows.Point>();
        private UIElement Element = new UIElement();
        private List<UIElement> UndoElements = new List<UIElement>();
        private UIElement RedoElement = null;

        public MainWindow()
        {
            InitializeComponent();        
            GridHelper.GenerateRowsAndColoumns(Grid);
           
        }

        private void DrawButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    DefaultExt = "xml",
                    Filter = "XML Files|*.xml"
                };

                if (openFileDialog.ShowDialog().GetValueOrDefault())
                {
                    Network network = new Network();
                    network = NetworkHelper.ParseXML(openFileDialog.FileName);

                    NetworkHelper.ScalePositions(network);
                    NetworkHelper.DrawEntities(network, Grid, Canvas);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Invalid file", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DrawEllipse_Click(object sender, RoutedEventArgs e)
        {
            elementToDraw = "Ellipse";
            menuItemEllipse.Background = Brushes.Gray;
            menuItemPolygon.Background = Brushes.LightGray;
            menuItemText.Background = Brushes.LightGray;
            menuItemClear.Background = Brushes.LightGray;
            menuItemRedo.Background = Brushes.LightGray;
            menuItemUndo.Background = Brushes.LightGray;
        }

        public void DrawEllipse(int radiusX, int radiusY, int borderThickness, Color fillColor, Color borderColor)
        {
            Ellipse ellipse = new Ellipse
            {
                Fill = new SolidColorBrush(fillColor),
                Stroke = new SolidColorBrush(borderColor),
                StrokeThickness = borderThickness
            };

            Canvas.SetTop(ellipse, startPosition.Y);
            Canvas.SetLeft(ellipse, startPosition.X);

            ellipse.Width = radiusX;
            ellipse.Height = radiusY;

            Canvas.Children.Add(ellipse);

            ellipse.MouseLeftButtonDown += new MouseButtonEventHandler(EditEllipse);
        }

        private void EditEllipse(object sender, MouseButtonEventArgs e)
        {
            Element = sender as UIElement;
            Ellipse el = (Ellipse)Element;
            
            startPosition.Y = Canvas.GetTop(Element);
            startPosition.X = Canvas.GetLeft(Element);

            EllipseWindow ellipse = new EllipseWindow((int)el.Width, (int)el.Height, (int)el.StrokeThickness, ((SolidColorBrush)el.Fill).Color, ((SolidColorBrush)el.Stroke).Color);
            ellipse.Show();

            Canvas.Children.Remove(el);      
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPosition = e.GetPosition(Canvas);

            switch (elementToDraw)
            {
                case "Ellipse":
                    EllipseWindow ellipseWindow = new EllipseWindow();
                    ellipseWindow.Show();
                    menuItemEllipse.Background = Brushes.LightGray;
                    elementToDraw = "";
                    break;
                case "Polygon":
                    PolygonPoints.Add(startPosition);
                    break;
                case "Text":
                    TextWindow textWindow = new TextWindow();
                    textWindow.Show();
                    menuItemText.Background = Brushes.LightGray;
                    elementToDraw = "";
                    break;
            }
        }

        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            elementToDraw = "Polygon";
            menuItemPolygon.Background = Brushes.Gray;
            menuItemEllipse.Background = Brushes.LightGray;
            menuItemText.Background = Brushes.LightGray;
            menuItemClear.Background = Brushes.LightGray;
            menuItemRedo.Background = Brushes.LightGray; 
            menuItemUndo.Background = Brushes.LightGray; 
        }

        public void DrawPolygon(List<System.Windows.Point> points, int borderThickness, Color fillColor, Color borderColor)
        {
            Polygon polygon = new Polygon
            {
                Fill = new SolidColorBrush(fillColor),
                Stroke = new SolidColorBrush(borderColor),
                StrokeThickness = borderThickness
            };

            foreach (System.Windows.Point p in points)
                polygon.Points.Add(p);

            Canvas.Children.Add(polygon);

            PolygonPoints.Clear();
            polygon.MouseLeftButtonDown += new MouseButtonEventHandler(EditPolygon);
        }

        private void EditPolygon(object sender, MouseButtonEventArgs e)
        {
            Element = sender as UIElement;
            Polygon p = (Polygon)Element;

            PolygonWindow polygon = new PolygonWindow(p.Points.ToList(), (int)p.StrokeThickness, ((SolidColorBrush)p.Fill).Color, ((SolidColorBrush)p.Stroke).Color);
            polygon.Show();

            Canvas.Children.Remove(p);
        }


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (elementToDraw == "Polygon" && PolygonPoints.Count >= 3)
            {
                PolygonWindow polygonWindow = new PolygonWindow(PolygonPoints);
                polygonWindow.Show();
                menuItemPolygon.Background = Brushes.LightGray;
                elementToDraw = "";
            }
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            elementToDraw = "Text";
            menuItemText.Background = Brushes.Gray;
            menuItemPolygon.Background = Brushes.LightGray;
            menuItemEllipse.Background = Brushes.LightGray;          
            menuItemClear.Background = Brushes.LightGray;
            menuItemRedo.Background = Brushes.LightGray;
            menuItemUndo.Background = Brushes.LightGray;
        }

        public void DrawText(string text, int size, Color textColor)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontSize = size,
                Foreground = new SolidColorBrush(textColor)
            };

            Canvas.SetTop(textBlock, startPosition.Y);
            Canvas.SetLeft(textBlock, startPosition.X);

            Canvas.Children.Add(textBlock);

            textBlock.MouseLeftButtonDown += new MouseButtonEventHandler(EditText);
        }

        private void EditText(object sender, MouseButtonEventArgs e)
        {
            Element = sender as UIElement;
            TextBlock el = (TextBlock)Element;

            startPosition.Y = Canvas.GetTop(Element);
            startPosition.X = Canvas.GetLeft(Element);

            TextWindow textBlock = new TextWindow(el.Text, (int)el.FontSize, ((SolidColorBrush)el.Foreground).Color);
            textBlock.Show();

            Canvas.Children.Remove(el);
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {         
            if (Canvas.Children.Count > 0)
            {
                foreach (UIElement item in Canvas.Children)
                        UndoElements.Add(item);

                Canvas.Children.Clear();
            }
            
        }

        private void MenuItemUndo_Click(object sender, RoutedEventArgs e)
        {
            if (UndoElements.Count > 0)
            {
                foreach (UIElement el in UndoElements)
                    Canvas.Children.Add(el);

                UndoElements.Clear();
            }
            else if (Canvas.Children.Count > 1)
            {
                RedoElement = Canvas.Children[(Canvas.Children.Count - 1)];
                Canvas.Children.RemoveAt(Canvas.Children.Count - 1);
            }
        }

        private void MenuItemRedo_Click_(object sender, RoutedEventArgs e)
        {
            if (RedoElement != null)
            {
                Canvas.Children.Add(RedoElement);
                RedoElement = null;
            }
        }
    }
}
