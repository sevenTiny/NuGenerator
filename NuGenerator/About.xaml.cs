using MahApps.Metro.Controls;
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

namespace SevenTinySoftware.NuGenerator
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();

            textEditorAbout.Text =
@"
* 软件版本

V1.0.0

* 鸣谢

Technical Support
sevenTiny Team
北京市海淀区上地东路35号 ShangDi HaiDian Area BeiJing 100089 P.R.China
郵箱Email: dong@7tiny.com , sevenTiny@foxmail.com
網址Http: http://www.7tiny.com
Create: 2021-09-05 ZhongHe Chengdu SiChuan 

* 了解更多

https://www.cnblogs.com/7tiny/p/15234048.html
";
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BindingContext.Current.AboutWindow = null;
        }
    }
}
