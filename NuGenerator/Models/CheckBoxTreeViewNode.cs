using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SevenTinySoftware.NuGenerator.Models
{
    public class CheckBoxTreeViewNode : INotifyPropertyChanged
    {
        /// <summary>
        /// 构造
        /// </summary>
        public CheckBoxTreeViewNode()
        {
        }

        /// <summary>
        /// 键值
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 显示的字符
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelationPath { get; set; }
        /// <summary>
        /// 当前节点文件全路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 是否子项（没有下级）
        /// </summary>
        public bool IsItem { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 右键子菜单按钮可见
        /// </summary>
        public string ItemVisibility => IsItem ? "Visible" : "Collapsed";
        /// <summary>
        /// 右键父菜单按钮课件
        /// </summary>
        public string FolderVisibility => IsItem ? "Collapsed" : "Visible";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackGround
        {
            get
            {
                return _backGroud;
            }
            set
            {
                _backGroud = value;
                NotifyPropertyChanged(nameof(BackGround));
            }
        }
        private string _backGroud = "";

        /// <summary>
        /// 指针悬停时的显示说明
        /// </summary>
        public string ToolTip => string.Format("{0}-{1}", Id, Name);

        /// <summary>
        /// 选中状态
        /// </summary>
        private bool _isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    NotifyPropertyChanged("IsChecked");

                    if (_isChecked)
                    {
                        //如果都选中则父项也应该选中
                        if (Parent != null && !Parent.Children.Any(t => !t.IsChecked))
                            Parent.IsChecked = true;

                        //勾选时将所有子项勾选上
                        foreach (var item in Children)
                            item.IsChecked = true;

                        BackGround = "LightSkyBlue";
                    }
                    else
                    {
                        //如果取消选中子项也应该取消选中
                        foreach (var item in Children)
                            item.IsChecked = false;

                        BackGround = null;
                    }
                }
            }
        }

        /// <summary>
        /// 折叠状态
        /// </summary>
        private bool _isExpanded;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    //折叠状态改变
                    _isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// 父项
        /// </summary>
        public CheckBoxTreeViewNode Parent { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        public ObservableCollection<CheckBoxTreeViewNode> Children { get; set; }

        /// <summary>
        /// 设置所有子项的选中状态
        /// </summary>
        /// <param name="isChecked"></param>
        public void SetChildrenChecked(bool isChecked)
        {
            foreach (CheckBoxTreeViewNode child in Children)
            {
                child.IsChecked = IsChecked;
                child.SetChildrenChecked(IsChecked);
            }
        }

        /// <summary>
        /// 设置所有子项展开状态
        /// </summary>
        /// <param name="isExpanded"></param>
        public void SetChildrenExpanded(bool isExpanded)
        {
            foreach (CheckBoxTreeViewNode child in Children)
            {
                child.IsExpanded = isExpanded;
                child.SetChildrenExpanded(isExpanded);
            }
        }

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
