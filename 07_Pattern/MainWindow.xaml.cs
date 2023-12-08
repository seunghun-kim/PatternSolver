using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace _07_Pattern
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private readonly List<List<int>> graph = new List<List<int>>{
            new List<int>{1, 3, 4, 5, 7 },  // 0
            new List<int>{0, 2, 3, 4, 5, 6, 8 },  // 1
            new List<int>{1, 3, 4, 5, 7 },  // 2
            new List<int>{0, 1, 2, 4, 6, 7, 8 },  // 3
            new List<int>{0, 1, 2, 3, 5, 6, 7, 8},  // 4
            new List<int>{0, 1, 2, 4, 6, 7, 8},  // 5
            new List<int>{1, 3, 4, 5, 7},  // 6
            new List<int>{0, 2, 3, 4, 5, 6, 8},  // 7
            new List<int>{1, 3, 4, 5, 7}  // 8
        };

        private async Task Solve()
        {
            string key = "32AF8308A7627D86966F4567809617FF498EEF38";
            //string key = "9A7149A5A7786BB368E06D08C5D77774EB43A49E";
            string? result = null;

            await Task.Run(() =>
            {
                for (int i = 0; i < 9; i++)
                {
                    string pattern = i.ToString();
                    result = Traverse(pattern, key);
                    if (result != null)
                    {
                        break;
                    }
                }
                if (result == null)
                {
                    MessageBox.Show("Fail");
                }
            });
        }

        private string? Traverse(string current_pattern, string key)
        {
            string? result = null;

            Dispatcher.Invoke(() =>
            {
                DrawPattern(current_pattern);
            });

            int current_position = current_pattern.Last() - '0';
            List<int> available_next_nodes = graph[current_position];

            // Check SHA1 hash before moving on to next pattern
            string current_pattern_hash = GetSHA1(current_pattern);
            if (current_pattern_hash == key)
            {
                return current_pattern;
            }

            // Generate next pattern and traverse
            foreach (int next in available_next_nodes)
            {
                if (current_pattern.Contains(next.ToString()))
                {
                    continue;
                }

                result = Traverse(current_pattern + next.ToString(), key);

                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        private string GetSHA1(List<int> pattern)
        {
            string input = "";
            foreach (int val in pattern)
            {
                input += val.ToString();
            }
            return GetSHA1(input);
        }
        private string GetSHA1(string pattern)
        {
            using var sha1 = SHA1.Create();
            return Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(pattern)));
        }

        private void DrawPattern(string pattern)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < 9; i++)
            {
                points.Add(((RadioButton)this.FindName("rb" + i.ToString())).TransformToAncestor(this).Transform(new Point(0, 0)));
            }

            canvasPattern.Children.Clear();

            Point? prevPoint = null;
            foreach (char pos in pattern)
            {
                int pos_int = pos - '0';
                if (prevPoint == null)
                {
                    prevPoint = points[pos_int];
                    continue;
                }

                Line line = new Line();
                line.Stroke = Brushes.Blue;
                line.StrokeThickness = 2;
                line.X1 = prevPoint.Value.X;
                line.Y1 = prevPoint.Value.Y;
                line.X2 = points[pos_int].X;
                line.Y2 = points[pos_int].Y;
                canvasPattern.Children.Add(line);

                prevPoint = points[pos_int];
            }
            canvasPattern.InvalidateVisual();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //string pattern_to_solve = "581637";
            //string pattern_to_solve = "012345678";
            //DrawPattern(pattern_to_solve);

            //string hash_to_solve = GetSHA1(pattern_to_solve);
            //MessageBox.Show(hash_to_solve);

            ((Button)sender).IsEnabled = false;
            await Solve();
            ((Button)sender).IsEnabled = true;
        }
    }
}
