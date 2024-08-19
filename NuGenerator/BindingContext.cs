using SevenTinySoftware.NuGenerator.Helpers;
using SevenTinySoftware.NuGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SevenTinySoftware.NuGenerator
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public class BindingContext : INotifyPropertyChanged
    {
        private BindingContext() { }

        public static BindingContext Current = new BindingContext();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// 预览编辑器
        /// </summary>
        public ICSharpCode.AvalonEdit.TextEditor TextEditorPreview { get; set; }

        /// <summary>
        /// 输出编辑器
        /// </summary>
        public ICSharpCode.AvalonEdit.TextEditor TextEditorOutPut { get; set; }

        /// <summary>
        /// 状态栏Label
        /// </summary>
        public string StatusLabelText
        {
            get
            {
                return _StatusLabelText;
            }
            set
            {
                _StatusLabelText = value;
                NotifyPropertyChanged(nameof(StatusLabelText));
            }
        }
        private string _StatusLabelText;

        /// <summary>
        /// 所有树形菜单数据
        /// </summary>
        public ObservableCollection<CheckBoxTreeViewNode> TreeNodes { get; set; }

        /// <summary>
        /// 当前选择的节点
        /// </summary>
        public CheckBoxTreeViewNode CurrentSelectNode { get; set; }

        /// <summary>
        /// 当前编辑的节点
        /// </summary>
        public CheckBoxTreeViewNode CurrentEditNode { get; set; }

        /// <summary>
        /// 是否预览窗口文本被编辑了
        /// </summary>
        public bool IsPreviewTextChanged { get; set; } = false;

        /// <summary>
        /// 当前编辑的节点输出配置
        /// </summary>
        public ObservableCollection<OutPutSetting> OutPutSetting { get; set; } = OutPutSettingHelper.OutPutSettings;
        /// <summary>
        /// 输入配置
        /// </summary>
        public ObservableCollection<InputSettingConfig> InputSetting { get; set; } = InputSettingHelper.Config;

        /// <summary>
        /// 进度条进度
        /// </summary>
        public int ProgressBarValue
        {
            get
            {
                return _ProgressBarValue;
            }
            set
            {
                _ProgressBarValue = value;
                NotifyPropertyChanged(nameof(ProgressBarValue));
            }
        }
        private int _ProgressBarValue;

        /// <summary>
        /// 设置进度条
        /// </summary>
        /// <param name="percent">百分比（小数）</param>
        public void SetProgress(double percent)
        {
            var aim = (int)(100 * percent);
            ProgressBarValue = aim;
        }

        /// <summary>
        /// 初始化树菜单
        /// </summary>
        public void InitTreeNodes()
        {
            if (Current.TreeNodes == null)
            {
                Current.TreeNodes = GetItems(Common.DefaultTemplatePath);
            }
            else
            {
                //移除
                for (int i = Current.TreeNodes.Count; i > 0; i--)
                {
                    Current.TreeNodes.RemoveAt(i - 1);
                }

                //重新添加
                foreach (var item in GetItems(Common.DefaultTemplatePath))
                {
                    Current.TreeNodes.Add(item);
                }
            }

            //获取节点
            ObservableCollection<CheckBoxTreeViewNode> GetItems(string currentPath)
            {
                var nodes = new ObservableCollection<CheckBoxTreeViewNode>();
                //目录
                foreach (var item in Directory.GetDirectories(currentPath, "*", SearchOption.TopDirectoryOnly))
                {
                    nodes.Add(new CheckBoxTreeViewNode
                    {
                        Name = Path.GetFileName(item),
                        Children = GetItems(item),
                        RelationPath = item.Replace(Common.DefaultTemplatePath + "\\", string.Empty),
                        FullPath = Path.GetFullPath(item),
                        Icon = "/Images/folder.png",
                        IsItem = false
                    });
                }
                //文件
                foreach (var item in Directory.GetFiles(currentPath, "*", SearchOption.TopDirectoryOnly))
                {
                    nodes.Add(new CheckBoxTreeViewNode
                    {
                        Name = Path.GetFileName(item),
                        Children = new ObservableCollection<CheckBoxTreeViewNode>(),
                        RelationPath = item.Replace(Common.DefaultTemplatePath + "\\", string.Empty),
                        FullPath = Path.GetFullPath(item),
                        Icon = "/Images/T.png",
                        IsItem = true
                    });
                }

                //绑定父子关系
                foreach (var item in nodes)
                {
                    foreach (var child in item.Children)
                    {
                        child.Parent = item;
                    }
                }

                return nodes;
            }
        }

        /// <summary>
        /// 关于窗体
        /// </summary>
        public About AboutWindow { get; set; }
    }
}
