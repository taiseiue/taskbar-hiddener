using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace タスクバー表示切替
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process expp;
        public MainWindow()
        {
            InitializeComponent();
            checkBox1.IsChecked = !IsShowTaskBar;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtkey);

        private const string TARGET_PROCESS_NAME = "explorer";
        private void Timer_Tick(object sender, EventArgs e)
        {
            Process[] ps = Process.GetProcessesByName(TARGET_PROCESS_NAME);

            //配列から1つずつ取り出す
            if (ps.Length <= 0)
            {
                Process.Start(TARGET_PROCESS_NAME);
                Thread.Sleep(500);
                checkBox1.IsChecked = !IsShowTaskBar;
                return;
            }
            else
            {
                expp = ps[0];
            }
            if (!expp.Responding)
            {
                Thread.Sleep(500);
                checkBox1.IsChecked = !IsShowTaskBar;
                return;
            }
            checkBox1.IsChecked = !IsShowTaskBar;
            if (IsClickDown())
            {
                System.Drawing.Point p = System.Windows.Forms.Cursor.Position;//マウスカーソル位置取得
                int y2 = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 1;
                //ディスプレイの幅
                int x2 = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 1;

                int x = p.X;
                int y = p.Y;

                if ((y2 - y) < 10)
                {
                    checkBox1.IsChecked = !checkBox1.IsChecked;
                    Thread.Sleep(500);
                }

            }

        }
        private bool IsClickDown()
        {
            //マウス左ボタン(0x01)の状態、押されていたらマイナス値(-127)、なかったら0
            return GetKeyState(0x04) < 0;
        }

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            tskBarHide();
        }
        [DllImport("USER32.DLL", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL", CharSet = CharSet.Auto)]
        public static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("USER32.DLL", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        private static int SW_SHOW = 5;
        private static int SW_HIDE = 0;
        private IntPtr hwnd = FindWindow("Shell_TrayWnd", null);

        public void tskBarHide()
        {
            ShowWindow(hwnd, SW_HIDE);

        }
        public bool IsShowTaskBar
        {
            get
            {
                hwnd = FindWindow("Shell_TrayWnd", null);
                return IsWindowVisible(hwnd);
            }
        }
        public void tskBarDisp()
        {
            ShowWindow(hwnd, SW_SHOW);
        }
        private bool _dm = false;
        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            tskBarDisp();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsShowTaskBar)
            {
                if (MessageBox.Show(@"タスクバーが非表示になっています。
もう一度表示するにはこのプログラムを再度起動してください。
プログラムを終了してもよろしいですか？", "タスクバー表示切替", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Topmost = !Topmost;
            if (Topmost)
            {
                Title = "[#]タスクバー表示切替";
                mi1.IsChecked = true;
            }
            else
            {
                Title = "タスクバー表示切替";
                mi1.IsChecked = false;
            }

        }

        private void mi1_Click(object sender, RoutedEventArgs e)
        {
            Topmost = mi1.IsChecked;
            if (Topmost)
            {
                Title = "[#]タスクバー表示切替";
                mi1.IsChecked = true;
            }
            else
            {
                Title = "タスクバー表示切替";
                mi1.IsChecked = false;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            new About().Show();
        }
    }
}
