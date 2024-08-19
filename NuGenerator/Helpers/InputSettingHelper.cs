using System.Collections.ObjectModel;
using System.Linq;

namespace SevenTinySoftware.NuGenerator.Helpers
{
    public class InputSettingConfig
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
    }

    internal static class InputSettingHelper
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        private static string _Name = "input.setting.json";

        /// <summary>
        /// 配置
        /// </summary>
        public static ObservableCollection<InputSettingConfig> Config = SettingsHelper.GetConfig<ObservableCollection<InputSettingConfig>>(_Name);

        /// <summary>
        /// 刷新配置到文件
        /// </summary>
        public static void FlushConfig()
        {
            if (Config != null)
            {
                SettingsHelper.FlushConfig(_Name, Config.Where(t => !string.IsNullOrEmpty(t.Key) && !string.IsNullOrEmpty(t.Value)).ToList());
            }
        }
    }
}
