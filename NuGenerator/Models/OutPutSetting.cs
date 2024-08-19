using SevenTinySoftware.NuGenerator.Helpers;
using System;
using System.ComponentModel;
using System.Linq;

namespace SevenTinySoftware.NuGenerator.Models
{
    /// <summary>
    /// 输出设置
    /// </summary>
    public class OutPutSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// 用RelationPath当作key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 显示的名称
        /// </summary>
        public string Name { get; set; }

        private string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                NotifyPropertyChanged(nameof(Value));
                OutPutSettingHelper.FlushConfig();
            }
        }
    }
}
