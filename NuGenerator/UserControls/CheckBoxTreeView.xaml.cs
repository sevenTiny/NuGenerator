using Microsoft.VisualBasic.FileIO;
using SevenTinySoftware.NuGenerator.Helpers;
using SevenTinySoftware.NuGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SevenTinySoftware.NuGenerator.UserControls
{
    public partial class CheckBoxTreeView : UserControl
    {
        /// <summary>
        /// 控件数据
        /// </summary>
        private IList<CheckBoxTreeViewNode> _itemsSourceData;

        /// <summary>
        /// 构造
        /// </summary>
        public CheckBoxTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件数据
        /// </summary>
        public IList<CheckBoxTreeViewNode> ItemsSourceData
        {
            get { return _itemsSourceData; }
            set
            {
                _itemsSourceData = value;
                checkBoxTreeView.ItemsSource = _itemsSourceData;
            }
        }

        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id, IList<CheckBoxTreeViewNode> treeList)
        {
            foreach (var tree in treeList)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }
        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id)
        {
            foreach (var tree in ItemsSourceData)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns></returns>
        public IList<CheckBoxTreeViewNode> CheckedItemsIgnoreRelation()
        {
            return GetCheckedItemsIgnoreRelation(_itemsSourceData);
        }

        /// <summary>
        /// 私有方法，忽略层次关系的情况下，获取选中项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList<CheckBoxTreeViewNode> GetCheckedItemsIgnoreRelation(IList<CheckBoxTreeViewNode> list)
        {
            IList<CheckBoxTreeViewNode> treeList = new List<CheckBoxTreeViewNode>();
            foreach (var tree in list)
            {
                if (tree.IsChecked)
                {
                    treeList.Add(tree);
                }
                foreach (var child in GetCheckedItemsIgnoreRelation(tree.Children))
                {
                    treeList.Add(child);
                }
            }
            return treeList;
        }

        /// <summary>
        /// 选中所有子项菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAllChild_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTreeView.SelectedItem != null)
            {
                CheckBoxTreeViewNode tree = (CheckBoxTreeViewNode)checkBoxTreeView.SelectedItem;
                tree.IsChecked = true;
                tree.SetChildrenChecked(true);
            }
        }

        /// <summary>
        /// 全部展开菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBoxTreeViewNode tree in checkBoxTreeView.ItemsSource)
            {
                tree.IsExpanded = true;
                tree.SetChildrenExpanded(true);
            }
        }

        /// <summary>
        /// 全部折叠菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBoxTreeViewNode tree in checkBoxTreeView.ItemsSource)
            {
                tree.IsExpanded = false;
                tree.SetChildrenExpanded(false);
            }
        }

        /// <summary>
        /// 全部选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBoxTreeViewNode tree in checkBoxTreeView.ItemsSource)
            {
                tree.IsChecked = true;
                tree.SetChildrenChecked(true);
            }
        }

        /// <summary>
        /// 全部取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBoxTreeViewNode tree in checkBoxTreeView.ItemsSource)
            {
                tree.IsChecked = false;
                tree.SetChildrenChecked(false);
            }
        }

        /// <summary>
        /// 鼠标右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;

            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        /// <summary>
        /// 打开文件所在目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTreeView.SelectedItem != null)
            {
                CheckBoxTreeViewNode node = (CheckBoxTreeViewNode)checkBoxTreeView.SelectedItem;
                Common.OpenDirectory(Path.GetDirectoryName(node.FullPath));
            }
        }

        private void menuEdit_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTreeView.SelectedItem != null)
            {
                CheckBoxTreeViewNode node = (CheckBoxTreeViewNode)checkBoxTreeView.SelectedItem;

                //节点
                if (node.IsItem)
                {
                    if (BindingContext.Current.IsPreviewTextChanged)
                    {
                        if (System.Windows.Forms.MessageBox.Show($"预览窗口编辑内容未保存，是否丢弃更改并切换模板？", "切换模板", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            ChangeBinding(node);
                        }
                    }
                    else
                    {
                        ChangeBinding(node);
                    }
                }
            }

            void ChangeBinding(CheckBoxTreeViewNode node)
            {
                BindingContext.Current.TextEditorPreview.Text = File.ReadAllText(node.FullPath);
                BindingContext.Current.IsPreviewTextChanged = false;
                BindingContext.Current.StatusLabelText = node.FullPath;
                BindingContext.Current.CurrentEditNode = node;
                OutPutSettingHelper.SwitchOutPutSetting();
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;

            var node = item.DataContext as CheckBoxTreeViewNode;

            if (node != null)
            {
                BindingContext.Current.CurrentSelectNode = node;

                //双击
                if (e.ClickCount >= 2)
                {
                    menuEdit_Click(sender, null);
                }
            }
        }

        private void menuDel_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTreeView.SelectedItem != null)
            {
                CheckBoxTreeViewNode node = (CheckBoxTreeViewNode)checkBoxTreeView.SelectedItem;

                if (System.Windows.Forms.MessageBox.Show($"确认删除？", "删除", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (File.Exists(node.FullPath))
                        FileSystem.DeleteFile(node.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                    //移除节点
                    FildAndDeleteNode(BindingContext.Current.TreeNodes, node.FullPath);
                }
            }

            void FildAndDeleteNode(ObservableCollection<CheckBoxTreeViewNode> source, string nodeFullPath)
            {
                foreach (var item in source)
                {
                    if (item.IsItem)
                    {
                        if (item.FullPath == nodeFullPath)
                        {
                            source.Remove(item);
                            return;
                        }
                    }
                    else
                    {
                        FildAndDeleteNode(item.Children, nodeFullPath);
                    }
                }
            }
        }

        private void menuAdd_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTreeView.SelectedItem != null)
            {
                CheckBoxTreeViewNode node = (CheckBoxTreeViewNode)checkBoxTreeView.SelectedItem;

                if (node != null)
                {
                    NewFile(node.FullPath, false);
                }
            }
        }

        private void menuAllAdd_Click(object sender, RoutedEventArgs e)
        {
            NewFile(Common.GetDefaultTemplatePath(), true);
        }

        private void NewFile(string openPath, bool isTop)
        {
            var dlg = new System.Windows.Forms.SaveFileDialog
            {
                InitialDirectory = openPath,
                FileName = "NewTemplate.cshtml"
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string demoTemplate = Common.GetDemoTemplate();

                File.WriteAllText(dlg.FileName, demoTemplate, Encoding.UTF8);

                //更新菜单数据源
                var newNode = new CheckBoxTreeViewNode
                {
                    Name = Path.GetFileName(dlg.FileName),
                    Children = new ObservableCollection<CheckBoxTreeViewNode>(),
                    //RelationPath = ,//在下面赋值
                    FullPath = dlg.FileName,
                    Icon = "/Images/T.png",
                    IsItem = true
                };

                if (isTop)
                {
                    newNode.RelationPath = newNode.Name;

                    if (!BindingContext.Current.TreeNodes.Any(t => t.RelationPath == newNode.RelationPath))
                        BindingContext.Current.TreeNodes.Add(newNode);
                }
                else
                {
                    FildAndAddNode(BindingContext.Current.TreeNodes, newNode, Path.GetDirectoryName(dlg.FileName));
                }
            }

            void FildAndAddNode(ObservableCollection<CheckBoxTreeViewNode> source, CheckBoxTreeViewNode newNode, string addToPath)
            {
                foreach (var item in source)
                {
                    //文件夹
                    if (!item.IsItem)
                    {
                        if (item.FullPath.Equals(addToPath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            newNode.RelationPath = Path.Combine(item.Name, newNode.Name);

                            //如果有重名的，则不添加
                            if (item.Children.Any(t => t.RelationPath == newNode.RelationPath))
                                return;

                            item.Children.Add(newNode);
                            return;
                        }
                        else
                        {
                            FildAndAddNode(item.Children, newNode, addToPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAllReferesh_Click(object sender, RoutedEventArgs e)
        {
            BindingContext.Current.InitTreeNodes();
        }

        private void menuOpenFolderFolder_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTreeView.SelectedItem != null)
            {
                CheckBoxTreeViewNode node = (CheckBoxTreeViewNode)checkBoxTreeView.SelectedItem;
                Common.OpenDirectory(node.FullPath);
            }
        }

        private void menu_OpenTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            Common.OpenTemplateDirectory();
        }
    }
}