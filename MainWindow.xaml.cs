using DOS2_Handbook.View;
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

namespace DOS2_Handbook
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 主窗口
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            #region 目前只有这个功能在用
            new SkillsWindow().Show();
            Close();
            #endregion
        }

        private void NavigateCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case "skills":
                    var win = new SkillsWindow();
                    win.Show();
                    break;
            }
        }

        private void ExitCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }

    class MWCommands
    {
        public static RoutedCommand NavigateCmd = new RoutedCommand();
        public static RoutedCommand ExitCmd = new RoutedCommand("ExitCmd", typeof(MWCommands),
            new InputGestureCollection { new KeyGesture(Key.Escape) });
    }
}
