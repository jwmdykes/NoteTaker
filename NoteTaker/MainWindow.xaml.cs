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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace NoteTaker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        InkCanvas inkCanvas1;
        DrawingAttributes inkDA;
        DrawingAttributes highlighterDA;
        bool useHighlighter = false;
        bool useEraser = false;
        System.Windows.Media.SolidColorBrush backgroundColor;

        public MainWindow()
        {
            InitializeComponent();

            inkCanvas1 = new InkCanvas();
            backgroundColor = new System.Windows.Media.SolidColorBrush();
            backgroundColor.Color = Color.FromArgb(0xff, 0xA0, 0xE7, 0xE5);
            inkCanvas1.Background = backgroundColor;

            _ = canvasSlot.Children.Add(inkCanvas1);

            // Set up the DrawingAttributes for the pen.
            inkDA = new DrawingAttributes();
            inkDA.Color = Colors.Black;
            inkDA.Height = 5;
            inkDA.Width = 5;
            inkDA.FitToCurve = false;

            // Set up the DrawingAttributes for the highlighter.
            highlighterDA = new DrawingAttributes();
            highlighterDA.Color = Colors.Orchid;
            highlighterDA.IsHighlighter = true;
            highlighterDA.IgnorePressure = true;
            highlighterDA.StylusTip = StylusTip.Rectangle;
            highlighterDA.Height = 30;
            highlighterDA.Width = 10;

            inkCanvas1.DefaultDrawingAttributes = inkDA;

            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;
            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            // reload recent ink
            string[] paths = { Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NoteTaker", "Inks", "recentInk" };
            string data_dir = System.IO.Path.Combine(paths);
            if (File.Exists(data_dir))
            {
                FileStream fs = new FileStream(data_dir, FileMode.Open);
                inkCanvas1.Strokes = new StrokeCollection(fs);
                fs.Close();
            }

            // Add handler for window closing
            Closing += OnWindowClosing;
        }

        private void ToggleEraser(object sender, RoutedEventArgs e)
        {
            if (useEraser)
            {
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
            else
            {
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint;
                inkCanvas1.EraserShape = new EllipseStylusShape(30, 30);
            }
            
            useEraser = !useEraser;
        }

        private void ToggleHighlighter(object sender, RoutedEventArgs e)
        {
            if (!useHighlighter)
            {
                inkCanvas1.DefaultDrawingAttributes = highlighterDA;
            }
            else
            {
                inkCanvas1.DefaultDrawingAttributes = inkDA;
            }

            useHighlighter = !useHighlighter;
        }

        private void ClearInk(object sender, RoutedEventArgs e)
        {
            inkCanvas1.Strokes.Clear();
        }


        private void SaveInk(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "isf files (*.isf)|*.isf";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fs = new FileStream(saveFileDialog1.FileName,
                                               FileMode.Create);
                inkCanvas1.Strokes.Save(fs);
                fs.Close();
            }
        }

        private void LoadInk(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "isf files (*.isf)|*.isf";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog1.FileName,
                                               FileMode.Open);
                inkCanvas1.Strokes = new StrokeCollection(fs);
                fs.Close();
            }
        }

        private void NewWindow(object sender, RoutedEventArgs e)
        {
            MainWindow newWin = new MainWindow();
            newWin.Show();
            newWin.inkCanvas1.Strokes.Clear();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Save the current window size and state
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();

            // Save the current ink

            string[] inksPath = { Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NoteTaker", "Inks" };
            // Create folder if it doesn't exists
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(inksPath));

            string[] paths = { System.IO.Path.Combine(inksPath), "recentInk" };
            string targetPath = System.IO.Path.Combine(paths);
            FileStream fs = new FileStream(targetPath, FileMode.Create);
            inkCanvas1.Strokes.Save(fs);
            fs.Close();
        }
    }
}

