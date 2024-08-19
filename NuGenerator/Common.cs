using SevenTinySoftware.NuGenerator.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace SevenTinySoftware.NuGenerator
{
    /// <summary>
    /// 全局公共
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 默认模板相对路径
        /// </summary>
        public const string DefaultTemplatePath = "Templates";
        /// <summary>
        /// 默认输出相对路径
        /// </summary>
        public const string DefaultOutPutPath = "OutPut";
        /// <summary>
        /// 默认配置路径
        /// </summary>
        public const string DefaultSettingPath = "Settings";
        /// <summary>
        /// 获取默认模板路径
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultTemplatePath() => Path.Combine(Environment.CurrentDirectory, DefaultTemplatePath);
        /// <summary>
        /// 获取默认输出路径
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultOutPutPath() => Path.Combine(Environment.CurrentDirectory, DefaultOutPutPath);
        /// <summary>
        /// 获取输出全路径
        /// </summary>
        /// <param name="relationPath"></param>
        /// <returns></returns>
        public static string GetOutPutFullPath(string relationPath) => Path.Combine(GetDefaultOutPutPath(), relationPath);

        /// <summary>
        /// 获取Demo配置模板
        /// </summary>
        /// <returns></returns>
        public static string GetDemoTemplate()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, DefaultTemplatePath, "demo.cshtml");

            if (File.Exists(filePath))
                return File.ReadAllText(filePath);

            return string.Empty;
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="exception"></param>
        public static void OutPut(Exception exception)
        {
            if (BindingContext.Current.TextEditorOutPut != null)
                BindingContext.Current.TextEditorOutPut.Text = exception.ToString();
        }

        /// <summary>
        /// 输出日志（补充）
        /// </summary>
        /// <param name="exception"></param>
        public static void OutPutAppend(Exception exception)
        {
            if (BindingContext.Current.TextEditorOutPut != null)
            {
                BindingContext.Current.TextEditorOutPut.Text += "\n";
                BindingContext.Current.TextEditorOutPut.Text += exception.ToString();
            }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        public static void OutPut(string msg)
        {
            if (BindingContext.Current.TextEditorOutPut != null)
                BindingContext.Current.TextEditorOutPut.Text = msg;
        }

        /// <summary>
        /// 输出日志（补充）
        /// </summary>
        /// <param name="msg"></param>
        public static void OutPutAppend(string msg)
        {
            if (BindingContext.Current.TextEditorOutPut != null)
            {
                BindingContext.Current.TextEditorOutPut.Text += "\n";
                BindingContext.Current.TextEditorOutPut.Text += msg;
            }
        }

        public static void OpenTemplateDirectory()
        {
            OpenDirectory(GetDefaultTemplatePath());
        }

        public static void OpenOutPutDirectory()
        {
            OpenDirectory(GetDefaultOutPutPath());
        }

        public static void OpenDirectory(string path)
        {
            //microsoft store 不支持调用 process.start
            //if (!PlugInManager.IsPlugIn)
            //{
            //    OutPut($"自动打开目录功需要安装插件后才能使用，请重启以自动安装插件。");
            //    OutPut($"或手动从资源管理器打开目录: {path}");
            //    return;
            //}

            //PlugInManager.FileManagerType.GetMethod("OpenDirectory").Invoke(null, new[] { path });
            Process.Start("explorer.exe", path);
        }
    }
}
