using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace SevenTinySoftware.NuGenerator.Helpers
{
    /// <summary>
    /// 插件管理
    /// </summary>
    public static class PlugInManager
    {
        private const string _PlugInSaveFolder = "PlugIn";
        /// <summary>
        /// 插件保存路径
        /// </summary>
        private static string PlugInSaveFolder => Path.Combine(Environment.CurrentDirectory, _PlugInSaveFolder);
        /// <summary>
        /// 是否安装插件
        /// </summary>
        public static bool IsPlugIn { get; set; }
        /// <summary>
        /// 文件管理插件类
        /// </summary>
        public static Type FileManagerType { get; set; }

        public static void LoadPlugIn()
        {
            try
            {
                if (!ExistLocalPlugIn())
                    DownloadPlugIn();

                Common.OutPut($"开始加载插件...");

                var assembly = Assembly.LoadFile(Path.Combine(PlugInSaveFolder, "PlugIn.FileManagement.dll"));

                FileManagerType = assembly.GetType("PlugIn.FileManagement.FileManager");

                if (FileManagerType == null)
                {
                    Common.OutPutAppend("插件加载失败，PlugIn.FileManagement 中没有找到类型 FileManager");
                    IsPlugIn = false;
                    return;
                }

                Common.OutPutAppend($"插件加载完成");

                IsPlugIn = true;
            }
            catch (Exception ex)
            {
                Common.OutPutAppend(ex);
            }
        }

        private static void DownloadPlugIn()
        {
            var logInSaveFolder = PlugInSaveFolder;

            if (!Directory.Exists(logInSaveFolder))
                Directory.CreateDirectory(logInSaveFolder);

            Common.OutPut($"开始下载插件...");

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile("https://gitee.com/seventiny/public-resource/raw/master/PlugIn.FileManagement/PlugIn.FileManagement.dll", Path.Combine(logInSaveFolder, "PlugIn.FileManagement.dll"));
                Common.OutPutAppend($"[0] PlugIn.FileManagement.dll finished");
                webClient.DownloadFile("https://gitee.com/seventiny/public-resource/raw/master/PlugIn.FileManagement/PlugIn.FileManagement.pdb", Path.Combine(logInSaveFolder, "PlugIn.FileManagement.pdb"));
                Common.OutPutAppend($"[0] PlugIn.FileManagement.pdb finished");
            }

            Common.OutPutAppend($"插件下载完成");
        }

        /// <summary>
        /// 插件文件是否存在
        /// </summary>
        /// <returns></returns>
        private static bool ExistLocalPlugIn()
        {
            return File.Exists(Path.Combine(PlugInSaveFolder, "PlugIn.FileManagement.dll"));
        }
    }
}
