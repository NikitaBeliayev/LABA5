using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using ScottPlot.Styles;

namespace LABA5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public class Item
    {
        public string x1 { get; set; }
        public string x2 { get; set; }
        public string y1 { get; set; }
        public string y2 { get; set; }
    }
    public class SecondItem
    {
        public string second_x1 { get; set; }
        public string second_y1 { get; set; }
    }

    public partial class MainWindow : Window
    {
        Window1 second_window;
        List<double> coordinates = new List<double>();
        const int INSIDE = 0; //0000
        const int LEFT = 1;//0001
        const int RIGHT = 2;//0010
        const int TOP = 4; //0100
        const int BOTTOM = 8;//1000
        public double x_max { get; set; }
        internal double y_max { get; set; }
        internal double x_min { get; set; }
        internal double y_min { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            System.Drawing.Color color = ColorTranslator.FromHtml("#1f24c7");
            Plot.Plot.XAxis.Label("X", color);
            Plot.Plot.YAxis.Label("Y", color);
        }
        private void add_line_btn_Click(object sender, RoutedEventArgs e)
        {
            Item item = new Item();
            double x1, x2, y1, y2;
            if (text_block_x1.Text != string.Empty && text_block_y1.Text != string.Empty && text_block_x2.Text != string.Empty && text_block_y2.Text != string.Empty)
            {
                if (double.TryParse(text_block_x1.Text, out x1) && double.TryParse(text_block_x2.Text, out x2) 
                    && double.TryParse(text_block_y1.Text, out y1) && double.TryParse(text_block_y2.Text, out y2))
                {
                    item.x1 = x1.ToString();
                    item.x2 = x2.ToString();
                    item.y1 = y1.ToString();
                    item.y2 = y2.ToString();
                    DrawLine(double.Parse(text_block_x1.Text), double.Parse(text_block_y1.Text), double.Parse(text_block_x2.Text), double.Parse(text_block_y2.Text));
                    table.Items.Add(item);
                    coordinates.Add(double.Parse(text_block_x1.Text));
                    coordinates.Add(double.Parse(text_block_y1.Text));
                    coordinates.Add(double.Parse(text_block_x2.Text));
                    coordinates.Add(double.Parse(text_block_y2.Text));
                } 
            }
            else if (polygon_radio_btn.IsChecked!.Value && text_block_x1.Text != string.Empty && text_block_y1.Text != string.Empty)
            {
                if (double.TryParse(text_block_x1.Text, out x1) && double.TryParse(text_block_y1.Text, out y1))
                {
                    item.x1 = text_block_x1.Text;
                    item.y1 = text_block_y1.Text;
                    coordinates.Add(double.Parse(item.x1));
                    coordinates.Add(double.Parse(item.y1));
                    System.Drawing.Color color = ColorTranslator.FromHtml("#008000");
                    table.Items.Add(item);
                    Plot.Plot.AddPoint(double.Parse(text_block_x1.Text), double.Parse(text_block_y1.Text), color);
                    Plot.Refresh();
                    if (coordinates.Count > 2)
                    {
                        double prev_x1 = coordinates[^4];
                        double prev_y1 = coordinates[^3];
                        DrawLine(prev_x1, prev_y1, double.Parse(item.x1), double.Parse(item.y1));
                    }

                }
            }
        }

        private void show_Click(object sender, RoutedEventArgs e)
        {
            second_window = new Window1();
            second_window.Owner = this;
            second_window.myDelegate = GetCoordinates;
            second_window.Show();
        }
        public void GetCoordinates(double x_min, double x_max, double y_min, double y_max)
        {
            this.x_min = x_min;
            this.x_max = x_max;
            this.y_min = y_min;
            this.y_max = y_max;
            ReDrawPlot();
            DrawRectangle(this.x_max, this.y_max, this.x_min, this.y_min);
        }


        private void ReDrawPlot()
        {
            Plot.Plot.Clear();
            int i = 0;
            if (line_radio_btn.IsChecked!.Value)
            {
                while (i != coordinates.Count)
                {
                    double x1 = coordinates[i];
                    double y1 = coordinates[i + 1];
                    double x2 = coordinates[i + 2];
                    double y2 = coordinates[i + 3];
                    DrawLine(x1, y1, x2, y2);
                    i += 4;
                }
            }
            else if (polygon_radio_btn.IsChecked!.Value)
            {
                double prev_x = 0, prev_y = 0, current_x = 0, current_y = 0;
                System.Drawing.Color color = ColorTranslator.FromHtml("#008000");
                while (i != coordinates.Count)
                {
                    current_x = coordinates[i];
                    current_y = coordinates[i + 1];
                    Plot.Plot.AddPoint(current_x, current_y, color);
                    Plot.Refresh();
                    if (i != 0)
                    {
                        prev_x = coordinates[i - 2];
                        prev_y = coordinates[i - 1];
                        DrawLine(prev_x, prev_y, current_x, current_y);
                    }
                    i += 2;
                }
            }
        }
        private void cut_Click(object sender, RoutedEventArgs e)
        {
            if (line_radio_btn.IsChecked!.Value)
            {
                Plot.Plot.Clear();
                DrawRectangle(x_max, y_max, x_min, y_min);
                table.Items.Clear();
                int i = 0;
                while (i != coordinates.Count)
                {
                    double x1 = coordinates[i];
                    double y1 = coordinates[i + 1];
                    double x2 = coordinates[i + 2];
                    double y2 = coordinates[i + 3];
                    CohenSutherlandAlghoritm(x1, y1, x2, y2);
                    i += 4;
                }
                coordinates.Clear();
                for (int j = 0; j < table.Items.Count; j++)
                {
                    if (table.Items[j] is Item current_item)
                    {
                        coordinates.Add(double.Parse(current_item.x1));
                        coordinates.Add(double.Parse(current_item.x2));
                        coordinates.Add(double.Parse(current_item.y1));
                        coordinates.Add(double.Parse(current_item.y2));
                    }
                }
            }
            else if (polygon_radio_btn.IsChecked!.Value)
            {
                if (table.Items.Count != 0)
                {
                    double[][] clip_area = new double[][]
                    {
                        new double[]{ x_min, y_min },
                        new double[]{ x_min, y_max },
                        new double[]{ x_max, y_max },
                        new double[]{ x_max, y_min },
                    };
                    int rows = table.Items.Count - 1;
                    double[][] poly_area = new double[20][];
                    for (int i = 0; i < poly_area.Length; i++)
                    {
                        poly_area[i] = new double[2];
                    }
                    int j = 0;
                    int index = 0;
                    while (j != coordinates.Count - 2)
                    {
                        double x = coordinates[j];
                        double y = coordinates[j + 1];
                        poly_area[index][0] = x;
                        poly_area[index][1] = y;
                        index += 1;
                        j += 2;
                    }
                    PolynomCutAlghoritm(rows, poly_area, clip_area);
                }
            }

        }

        private void clip(double[][] poly_area, double x1, double y1, double x2, double y2, ref int poly_size)
        {
            double[][] new_points = new double[20][];
            for (int i = 0; i < new_points.Length; i++)
            {
                new_points[i] = new double[2];
            }
            int new_poly_size = 0;
            for (int i = 0; i < poly_size; i++)
            {
                int k = (i + 1) % poly_size;
                double ix = poly_area[i][0], iy = poly_area[i][1];
                double kx = poly_area[k][0], ky = poly_area[k][1];
                double i_pos = (x2 - x1) * (iy - y1) - (y2 - y1) * (ix - x1);
                double k_pos = (x2 - x1) * (ky - y1) - (y2 - y1) * (kx - x1);
                if (i_pos < 0 && k_pos < 0)
                {
                    new_points[new_poly_size][0] = kx;
                    new_points[new_poly_size][1] = ky;
                    new_poly_size++;
                }
                else if (i_pos >= 0 && k_pos < 0)
                {
                    new_points[new_poly_size][0] = x_intersect(x1, y1, x2, y2, ix, iy, kx, ky);
                    new_points[new_poly_size][1] = y_intersect(x1, y1, x2, y2, ix, iy, kx, ky);
                    new_poly_size++;

                    new_points[new_poly_size][0] = kx;
                    new_points[new_poly_size][1] = ky;
                    new_poly_size++;
                }
                else if (i_pos < 0 && k_pos >= 0)
                {
                    new_points[new_poly_size][0] = x_intersect(x1, y1, x2, y2, ix, iy, kx, ky);
                    new_points[new_poly_size][1] = y_intersect(x1, y1, x2, y2, ix, iy, kx, ky);
                    new_poly_size++;
                }
            }
            poly_size = new_poly_size;
            for (int i = 0; i < poly_size; i++)
            {
                poly_area[i][0] = new_points[i][0];
                poly_area[i][1] = new_points[i][1];
            }
        }
        private void PolynomCutAlghoritm(int polynom_size, double[][] poly_area, double[][] clip_area)
        {
            for (int i = 0; i < 4; i++)
            {
                int k = (i + 1) % 4;
                clip(poly_area, clip_area[i][0], clip_area[i][1], clip_area[k][0], clip_area[k][1], ref polynom_size);
            }
            Plot.Plot.Clear();
            coordinates.Clear();
            int a = 0; 
            while (a != polynom_size)
            {
                coordinates.Add(poly_area[a][0]);
                coordinates.Add(poly_area[a][1]);
                a++;
            }
            coordinates.Add(poly_area[0][0]);
            coordinates.Add(poly_area[0][1]);
            ReDrawPlot();
            DrawRectangle(this.x_max, this.y_max, this.x_min, this.y_min);

        }
        private double x_intersect(double x1, double y1, double x2, double y2,
                                   double x3, double y3, double x4, double y4)
        {
            double num = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
            double den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            return num / den;
        }

        private double y_intersect(double x1, double y1, double x2, double y2,
                                  double x3, double y3, double x4, double y4)
        {
            double num = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
            double den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            return num / den;
        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            double[] x = new double[] { x1, x2 };
            double[] y = new double[] { y1, y2 };
            System.Drawing.Color color = ColorTranslator.FromHtml("#008000");
            Plot.Plot.AddScatter(x, y, color);
            Plot.Refresh();
        }

        private void DrawRectangle(double x_max, double y_max, double x_min, double y_min)
        {
            System.Drawing.Color color = ColorTranslator.FromHtml("#FF0000");
            double[] xs = new double[] { x_min, x_max };
            double[] ys = new double[] { y_max, y_max };
            Plot.Plot.AddScatter(xs, ys, color);
            Plot.Refresh();
            xs = new double[] { x_max, x_max };
            ys = new double[] { y_max, y_min };
            Plot.Plot.AddScatter(xs, ys, color);
            Plot.Refresh();
            xs = new double[] { x_max, x_min };
            ys = new double[] { y_min, y_min };
            Plot.Plot.AddScatter(xs, ys, color);
            Plot.Refresh();
            xs = new double[] { x_min, x_min };
            ys = new double[] { y_min, y_max };
            Plot.Plot.AddScatter(xs, ys, color);
            Plot.Refresh();
        }

        private void CohenSutherlandAlghoritm(double x1, double y1, double x2, double y2)
        {
            int code1 = ComputeCode(x1, y1);
            int code2 = ComputeCode(x2, y2);

            bool accept = false;

            while (true)
            {
                if ((code1 == 0) && (code2 == 0))
                {
                    accept = true;
                    break;
                }
                else if (Convert.ToBoolean(code1 & code2))
                {
                    break;
                }
                else
                {
                    int code_out;
                    double x = 0, y = 0;

                    if (code1 != 0)
                    {
                        code_out = code1;
                    }
                    else
                    {
                        code_out = code2;
                    }

                    if (Convert.ToBoolean(code_out & TOP))
                    {
                        x = x1 + (x2 - x1) * (y_max - y1) / (y2 - y1);
                        y = y_max;
                    }
                    else if (Convert.ToBoolean(code_out & BOTTOM))
                    {
                        x = x1 + (x2 - x1) * (y_min - y1) / (y2 - y1);
                        y = y_min;
                    }
                    else if (Convert.ToBoolean(code_out & RIGHT))
                    {
                        y = y1 + (y2 - y1) * (x_max - x1) / (x2 - x1);
                        x = x_max;
                    }
                    else if (Convert.ToBoolean(code_out & LEFT))
                    {
                        y = y1 + (y2 - y1) * (x_min - x1) / (x2 - x1);
                        x = x_min;
                    }

                    if (code_out == code1)
                    {
                        x1 = x;
                        y1 = y;
                        code1 = ComputeCode(x1, y1);
                    }
                    else
                    {
                        x2 = x;
                        y2 = y;
                        code2 = ComputeCode(x2, y2);
                    }
                }
            }
            if (accept)
            {
                DrawLine(x1, y1, x2, y2);
                Item item = new Item();
                item.x1 = x1.ToString();
                item.y1 = y1.ToString();
                item.x2 = x2.ToString();
                item.y2 = y2.ToString();
                table.Items.Add(item);
            }

        }

        private int ComputeCode(double x, double y)
        {
            int code = INSIDE;
            if (x < x_min)
            {
                code |= LEFT;
            }
            else if (x > x_max)
            {
                code |= RIGHT;
            }
            if (y < y_min)
            {
                code |= BOTTOM;
            }
            else if (y > y_max)
            {
                code |= TOP;
            }
            return code;
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            table.Items.Clear();
            Plot.Plot.Clear();
            text_block_x1.Clear();
            text_block_x2.Clear();
            text_block_y1.Clear();
            text_block_y2.Clear();
            coordinates.Clear();
        }

        private void line_radio_btn_Checked(object sender, RoutedEventArgs e)
        {
            if (add_line_btn.Content.ToString() == "Add point")
            {
                add_line_btn.Content = "Add line";
            }
            add_line_btn.IsEnabled = true;
            show_btn.IsEnabled = true;
            remove_btn.IsEnabled = true;
            cut_btn.IsEnabled = true;
            x1_word.Visibility = Visibility.Visible;
            x2_word.Visibility = Visibility.Visible;
            y1_word.Visibility = Visibility.Visible;
            y2_word.Visibility = Visibility.Visible;
            text_block_x1.Visibility = Visibility.Visible;
            text_block_x2.Visibility = Visibility.Visible;
            text_block_y1.Visibility = Visibility.Visible;
            text_block_y2.Visibility = Visibility.Visible;
            x1_word.Text = "X1:";
            x2_word.Text = "X2:";
            y1_word.Text = "Y1:";
            y2_word.Text = "Y2:";
            table.Items.Clear();
            Plot.Plot.Clear();
            if (table.Columns.Count != 4)
            {
                DataGridTextColumn coloumn1 = new DataGridTextColumn { Header = "x2" };
                DataGridTextColumn coloumn2 = new DataGridTextColumn { Header = "y2" };
                Binding binding1 = new Binding("x2");
                Binding binding2 = new Binding("y2");
                coloumn1.Binding = binding1;
                coloumn2.Binding = binding2;
                DataGridLength length = new DataGridLength(147.7d);
                coloumn1.Width = length;
                coloumn2.Width = length;
                table.Columns[0].Width = length;
                table.Columns[1].Width = length;
                table.Columns.Insert(1, coloumn1);
                table.Columns.Insert(3, coloumn2);
                table.CanUserResizeColumns = false;
            }
            coordinates.Clear();
        }

        private void polygon_radio_btn_Checked(object sender, RoutedEventArgs e)
        {
            add_line_btn.Content = "Add point";
            add_line_btn.IsEnabled = true;
            show_btn.IsEnabled = true;
            remove_btn.IsEnabled = true;
            cut_btn.IsEnabled = true;
            x1_word.Visibility = Visibility.Visible;
            x2_word.Visibility = Visibility.Hidden;
            y1_word.Visibility = Visibility.Visible;
            y2_word.Visibility = Visibility.Hidden;
            text_block_x1.Visibility = Visibility.Visible;
            text_block_x2.Visibility = Visibility.Hidden;
            text_block_y1.Visibility = Visibility.Visible;
            text_block_y2.Visibility = Visibility.Hidden;
            text_block_x1.Text = String.Empty;
            text_block_y1.Text = String.Empty;
            x1_word.Text = "X1:";
            x2_word.Text = "X2:";
            y1_word.Text = "Y1:";
            y2_word.Text = "Y2:";
            table.Items.Clear();
            Plot.Plot.Clear();
            table.Columns.Clear();
            DataGridTextColumn coloumn1 = new DataGridTextColumn { Header = "x1" };
            DataGridTextColumn coloumn2 = new DataGridTextColumn { Header = "y1" };
            Binding binding1 = new Binding("x1");
            Binding binding2 = new Binding("y1");
            coloumn1.Binding = binding1;
            coloumn2.Binding = binding2;
            DataGridLength length = new DataGridLength(295.4d);
            coloumn1.Width = length;
            coloumn2.Width = length;
            table.Columns.Add(coloumn1);
            table.Columns.Add(coloumn2);
            coordinates.Clear();
        }
    }
}
