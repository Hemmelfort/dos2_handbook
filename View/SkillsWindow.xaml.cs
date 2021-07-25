using DOS2_Handbook.Model;
using DOS2_Handbook.Service;
using DOS2_Handbook.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
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

namespace DOS2_Handbook.View
{
    /// <summary>
    /// SkillsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SkillsWindow : Window
    {
        readonly SkillWindowViewModel viewModel;

        public SkillsWindow()
        {
            CheckResourceFiles();
            InitializeComponent();
            LoadWindowSettings();

            viewModel = new SkillWindowViewModel();
            DataContext = viewModel;




            //快速退出
            KeyDown += new KeyEventHandler((o, e) =>
            {
                if (e.Key == Key.Escape)
                    Close();
            });
        }

        /// <summary>
        /// Check resource files before startup.
        /// </summary>
        private void CheckResourceFiles()
        {
            if (!FileServices.ResourceFileExists())
            {
                MessageBox.Show("找不到资源文件，程序无法运行。", "文件丢失");
                Close();
            }
        }

        private void LoadWindowSettings()
        {
            Width = Properties.Settings.Default.SkillWindowWidth;
            Height = Properties.Settings.Default.SkillWindowHeight;
            Closing += (s, e) =>
            {
                Properties.Settings.Default.SkillWindowWidth = (int)Width;
                Properties.Settings.Default.SkillWindowHeight = (int)Height;
                Properties.Settings.Default.Save();
            };
        }


        /// <summary>
        /// 单击折叠技能树，再单击则展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrvSkills_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            item.IsExpanded = !item.IsExpanded;
            viewModel.SelectedItem = item.DataContext;  //这是什么操作？？
            e.Handled = true;
        }

        /// <summary>
        /// 禁止编辑技能属性表格（DataGrid的ItemsSource是列表的时候才能编辑，太坑爹）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgProperties_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// 在技能属性表格点击菜单中的复制选项后，复制所选内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var itemList = dgSkillProperties.SelectedItems;
            string result = "";
            foreach (KeyValuePair<string, string> kvp in itemList)
            {
                result = result + "\n" + kvp.Value;
            }
            Clipboard.SetText(result.Trim());
        }

        /// <summary>
        /// Button to fold all skill abilities.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFoldAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in trvSkills.Items)
            {
                if (trvSkills.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                    treeItem.IsExpanded = false;
            }
        }

        /// <summary>
        /// Show the details when user double click on an item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgSkillProperties_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var val = dgSkillProperties.SelectedValue.ToString();
            MessageBox.Show(val, "原始内容");
        }


        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var process = new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri);
            System.Diagnostics.Process.Start(process);
            e.Handled = true;
        }
    }


    /// <summary>
    /// 行动点数的转换：把数字转换成一个阵列。比如 3 转换成 [0,1,2]
    /// </summary>
    public class ActionPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = int.Parse(value.ToString());
            return Enumerable.Range(0, number);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 图标转换：把图标名转换成一个BitmapSource用来显示
    /// </summary>
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconName = value.ToString();
            //var bitmapSource = ImageServies.Instance.GetSkillIconBitmapImage(iconName);
            var bitmapSource = DOS2_Utils.GetSkillIcon(iconName);
            return bitmapSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    /// <summary>
    /// 技能属性名词翻译：把英文属性名转换成中文
    /// </summary>
    public class SkillPropertyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var translatedName = DOS2_Utils.TranslateString(value.ToString());
            return translatedName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 从字典中查找相应词条的中文翻译
    /// </summary>
    public class StatsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var translatedName = DOS2_Utils.TranslateString(value.ToString());
            return translatedName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 从字典中查找相应学派的中文翻译
    /// </summary>
    public class AbilityStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var translatedName = DOS2_Utils.TranslateAbilitySchoolName(value.ToString());
            return translatedName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 把参数填到技能描述里面
    /// </summary>
    public class DescriptionStringConverter : IValueConverter
    {
        //TODO: 在这里加一个委托，让它能使用 StatsStringConverter 的方法，要不然没法翻译成中文

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filledDescription = value.ToString();
            return filledDescription + "\n\nnot filled yet";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 把记忆槽位数转化成 槽位要不要显示
    /// </summary>
    public class MemoryCostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int memoryCost) &&
                int.TryParse(parameter.ToString(), out int slot))
            {
                switch (memoryCost)
                {
                    case 1:
                        if (slot == 1)
                            return true;
                        break;
                    case 2:
                        if (slot == 1 || slot == 2)
                            return true;
                        break;
                    case 3:
                        if (slot == 1 || slot == 2 || slot == 3)
                            return true;
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
