using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SevenTinySoftware.NuGenerator.Helpers
{
    internal static class SettingsHelper
    {
        /// <summary>
        /// 获取输出配置路径
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        internal static string GetOutPutSettingPath(string configName) => Path.Combine(Environment.CurrentDirectory, "Settings", configName);
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        internal static T GetConfig<T>(string configName) => JsonConvert.DeserializeObject<T>(File.ReadAllText(GetOutPutSettingPath(configName), Encoding.UTF8));
        /// <summary>
        /// 刷新配置到文件
        /// </summary>
        internal static void FlushConfig<T>(string configName, T config)
        {
            File.WriteAllText(GetOutPutSettingPath(configName), JsonConvert.SerializeObject(config), Encoding.UTF8);
        }
    }
}
