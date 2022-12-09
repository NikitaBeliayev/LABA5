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

namespace LABA5
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public delegate void MyDelegate(double x_min, double x_max, double y_min, double y_max);
    public partial class Window1 : Window
    {
        public MyDelegate myDelegate { get; set; }
        public double x_min { get; private set; }
        public double x_max { get; private set; }
        public double y_min { get; private set; }
        public double y_max { get; private set; }
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (text_box_max_x.Text != String.Empty && text_box_max_y.Text != String.Empty && text_box_min_x.Text != String.Empty && text_box_min_y.Text != String.Empty)
            {
                myDelegate.Invoke(double.Parse(text_box_min_x.Text), double.Parse(text_box_max_x.Text), double.Parse(text_box_min_y.Text), double.Parse(text_box_max_y.Text));
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
