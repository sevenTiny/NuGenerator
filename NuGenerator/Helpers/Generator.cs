using RazorEngine;
using RazorEngine.Templating;
using SevenTiny.Bantina.Extensions;
using SevenTiny.Bantina.Validation;
using SevenTinySoftware.NuGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace SevenTinySoftware.NuGenerator.Helpers
{
    public class Generator
    {
        private static IDictionary<string, object> GetInputDic()
        {
            if (BindingContext.Current.InputSetting == null)
                return new Dictionary<string, object>();

            var result = new Dictionary<string, object>();

            foreach (var item in BindingContext.Current.InputSetting)
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value) && !string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                    result.AddOrUpdate(item.Key, item.Value);
            }

            return result;
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="template">模板内容</param>
        /// <returns></returns>
        public static string GenerateCode(string template)
        {
            return RunCompile(template, GetRazorModel(GetInputDic()));
        }

        /// <summary>
        /// 生成代码文件
        /// </summary>
        public static int GenerateCodeFile(IProgress<(double precent, string text)> onGeneratedOne, IProgress<int> onFinished)
        {
            int count = 0;
            //待输出节点
            ObservableCollection<CheckBoxTreeViewNode> allSelectedNodes = GetCheckBoxTreeViewNodes(BindingContext.Current.TreeNodes);

            foreach (var item in allSelectedNodes)
            {
                //模板
                var template = File.ReadAllText(item.FullPath);

                string outPutName = string.Empty;

                //输出配置
                var outPutPath = OutPutSettingHelper.GetOutPutPath(item.RelationPath);

                //如果没有配置输出名称，则默认取模板名称
                if (string.IsNullOrEmpty(outPutPath))
                    outPutName = item.RelationPath;
                //如果配置了输出名称，则支持模板生成名称
                else
                    outPutName = GenerateCode(outPutPath);

                //拼接输出文件路径
                var outPutFilePath = Path.Combine(Common.GetDefaultOutPutPath(), outPutName);

                //根据模板生成代码，并输出
                var result = GenerateCode(template);

                Directory.CreateDirectory(outPutFilePath.Replace(Path.GetFileName(outPutFilePath), string.Empty));

                File.WriteAllText(outPutFilePath, result, System.Text.Encoding.UTF8);

                count++;
                onGeneratedOne.Report(((double)count / allSelectedNodes.Count, $"[{count}] {item.RelationPath} ===> {outPutName}"));
            }

            onFinished.Report(count);
            return count;

            static ObservableCollection<CheckBoxTreeViewNode> GetCheckBoxTreeViewNodes(ObservableCollection<CheckBoxTreeViewNode> source)
            {
                var container = new ObservableCollection<CheckBoxTreeViewNode>();

                foreach (var item in source)
                {
                    if (item.IsChecked && item.IsItem)
                    {
                        container.Add(item);
                    }
                    else if (item.Children.Any())
                    {
                        foreach (var child in GetCheckBoxTreeViewNodes(item.Children))
                        {
                            container.Add(child);
                        }
                    }
                }

                return container;
            }
        }

        /// <summary>
        /// 编译标识
        /// </summary>
        private static HashSet<string> _compileMark = new HashSet<string>();

        /// <summary>
        /// 编译并运行模板
        /// </summary>
        /// <param name="template">模板文本</param>
        /// <param name="model">参数模型</param>
        /// <returns></returns>
        public static string RunCompile(string template, object model)
        {
            Ensure.ArgumentNotNullOrEmpty(template, nameof(template));

            var templateKey = template.GetHashCode().ToString();

            if (!_compileMark.Contains(templateKey))
            {
                Engine.Razor.Compile(template, templateKey);
                _compileMark.Add(templateKey);
            }

            return Engine.Razor.Run(templateKey, null, model);
        }

        /// <summary>
        /// 构造Razor模型
        /// </summary>
        /// <param name="arguments">参数</param>
        /// <returns></returns>
        private static object GetRazorModel(IDictionary<string, object> arguments)
        {
            dynamic result = new ExpandoObject();

            foreach (var item in arguments)
                ((IDictionary<string, object>)result).Add(item);

            return result;
        }
    }
}
