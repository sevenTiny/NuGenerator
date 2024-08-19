using SevenTinySoftware.NuGenerator.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SevenTinySoftware.NuGenerator.Helpers
{
    internal class OutPutSettingConfig
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutPutPath { get; set; }
    }

    internal static class OutPutSettingHelper
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        private static string _Name = "output.setting.json";

        /// <summary>
        /// 配置
        /// </summary>
        public static List<OutPutSettingConfig> Config = SettingsHelper.GetConfig<List<OutPutSettingConfig>>(_Name);

        /// <summary>
        /// 界面展示的配置
        /// </summary>
        public static ObservableCollection<OutPutSetting> OutPutSettings = new ObservableCollection<OutPutSetting>();

        /// <summary>
        /// 刷新配置到文件
        /// </summary>
        public static void FlushConfig()
        {
            if (Config == null)
                return;

            //当前编辑的节点目录
            var eitNode = BindingContext.Current.CurrentEditNode;

            if (eitNode == null)
                return;

            bool needFlush = false;

            //找到当前配置的节点
            var currentConfig = Config.FirstOrDefault(t => t.Key.Equals(eitNode.RelationPath));

            foreach (var item in OutPutSettings)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;

                //输出路径配置
                if (item.Key.Equals("OutPutPath"))
                {
                    if (currentConfig != null)
                        currentConfig.OutPutPath = item.Value;
                    else
                        Config.Add(currentConfig = new OutPutSettingConfig { Key = eitNode.RelationPath, OutPutPath = item.Value });

                    needFlush = true;
                }
                //其他配置节点...
            }

            if (needFlush)
                SettingsHelper.FlushConfig(_Name, Config);
        }

        /// <summary>
        /// 切换设置
        /// </summary>
        public static void SwitchOutPutSetting()
        {
            //当前编辑的节点目录
            var eitNode = BindingContext.Current.CurrentEditNode;

            if (eitNode == null)
                return;

            var currentConfig = Config.FirstOrDefault(t => t.Key.Equals(eitNode.RelationPath));

            //先全部移除，然后后续再添加
            for (int i = OutPutSettings.Count; i > 0; i--)
                OutPutSettings.RemoveAt(i - 1);

            //如果已经配置了，则取配置好的
            if (currentConfig != null)
            {
                OutPutSettings.Add(new OutPutSetting { Key = "OutPutPath", Name = "输出路径", Value = currentConfig.OutPutPath });
            }
            else
            {
                OutPutSettings.Add(new OutPutSetting { Key = "OutPutPath", Name = "输出路径", Value = eitNode.RelationPath });
            }
        }

        /// <summary>
        /// 获取某节点的输出目录
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetOutPutPath(string key)
        {
            return Config.FirstOrDefault(t => t.Key == key)?.OutPutPath;
        }
    }
}
