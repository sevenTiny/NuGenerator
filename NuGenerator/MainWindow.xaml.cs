using MahApps.Metro.Controls;
using SevenTinySoftware.NuGenerator.Helpers;
using SevenTinySoftware.NuGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SevenTinySoftware.NuGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            //初始化左侧树状菜单
            BindingContext.Current.InitTreeNodes();
            checkBoxTreeView.ItemsSourceData = BindingContext.Current.TreeNodes;
            //初始化右侧设置区域
            inputSetting.SelectedCellsChanged += (sender, e) => InputSettingHelper.FlushConfig();
            inputSetting.DataContext = BindingContext.Current.InputSetting;
            outputSetting.SelectedCellsChanged += (sender, e) => OutPutSettingHelper.FlushConfig();
            outputSetting.DataContext = BindingContext.Current.OutPutSetting;
            //绑定控件
            BindingContext.Current.TextEditorPreview = textEditorPreview;
            BindingContext.Current.TextEditorOutPut = textEditorOutPut;
            statusLabel.DataContext = BindingContext.Current;
            progressBar1.DataContext = BindingContext.Current;

            //预览窗口绑定事件
            textEditorPreview.TextChanged += (sender, e) =>
            {
                BindingContext.Current.IsPreviewTextChanged = true;
            };
            //给文本编辑器的文本区赋值一个名字（获取当前焦点控件时用）
            textEditorPreview.TextArea.Name = "textEditorPreviewTextArea";
            textEditorPreview.Text = Common.GetDemoTemplate();
            BindingContext.Current.IsPreviewTextChanged = false;
            //最后加载插件
            //PlugInManager.LoadPlugIn();
        }

        /// <summary>
        /// 生成预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneratePreview_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IProgress<string> progress = new Progress<string>(val => Common.OutPut(val));

            BindingContext.Current.SetProgress(0.2);

            Common.OutPut($"{DateTime.Now} 开始生成...");
            var script = textEditorPreview.Text;

            Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(script))
                        progress.Report("空模板");
                    else
                        progress.Report(Generator.GenerateCode(script));

                    BindingContext.Current.SetProgress(1);
                }
                catch (Exception ex)
                {
                    progress.Report(ex.ToString());
                }
            });
        }

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateFiles_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                IProgress<(double precent, string text)> onGeneratedOne = new Progress<(double percent, string text)>(val =>
                {
                    BindingContext.Current.SetProgress(val.percent);
                    Common.OutPutAppend(val.text);
                });

                IProgress<int> onAllFinished = new Progress<int>(count =>
                {
                    if (count > 0)
                    {
                        Common.OutPutAppend($"{DateTime.Now} 生成成功！共生成{count}个资源。");

                        if (MessageBox.Show($"成功生成 {count} 个文件，是否打开输出目录？", "生成成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Common.OpenOutPutDirectory();
                        }
                    }
                    else
                    {
                        Common.OutPut("未选择任何模板");
                    }
                });

                IProgress<Exception> exceptionProgress = new Progress<Exception>(val => Common.OutPutAppend(val));

                Common.OutPut($"{DateTime.Now} 开始生成...");

                Task.Run(() =>
                {
                    try
                    {
                        Generator.GenerateCodeFile(onGeneratedOne, onAllFinished);
                    }
                    catch (Exception ex)
                    {
                        exceptionProgress.Report(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Common.OutPutAppend(ex);
            }
        }

        /// <summary>
        /// 预览页面保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreViewSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var text = textEditorPreview.Text;

            if (BindingContext.Current.CurrentEditNode != null)
            {
                File.WriteAllText(BindingContext.Current.CurrentEditNode.FullPath, text, Encoding.UTF8);
                BindingContext.Current.StatusLabelText = $"{DateTime.Now} 保存成功";
                BindingContext.Current.IsPreviewTextChanged = false;
            }
            else
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    InitialDirectory = Common.GetDefaultTemplatePath()
                };

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, text, Encoding.UTF8);
                    BindingContext.Current.StatusLabelText = $"{DateTime.Now} 保存成功";
                    BindingContext.Current.IsPreviewTextChanged = false;
                }
            }
        }

        /// <summary>
        /// 输出框右键另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutPutSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "output",
                InitialDirectory = Common.GetDefaultOutPutPath()
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, textEditorOutPut.Text, Encoding.UTF8);
            }
        }

        private void menu_OpenTemplateFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Common.OpenTemplateDirectory();
        }

        private void menu_OutPutFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Common.OpenOutPutDirectory();
        }

        private void menu_Exist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void About_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BindingContext.Current.AboutWindow == null)
            {
                BindingContext.Current.AboutWindow = new About()
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
                };
            }

            BindingContext.Current.AboutWindow.Show();
        }

        /// <summary>
        /// 保存快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Save(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (textEditorPreview.TextArea.IsFocused)
            {
                PreViewSave_Click(sender, e);
            }
            else if (textEditorOutPut.TextArea.IsFocused)
            {
                OutPutSave_Click(sender, e);
            }
        }

        /// <summary>
        /// 生成快捷键F5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Generate(object sender, ExecutedRoutedEventArgs e)
        {
            if (textEditorPreview.TextArea.IsFocused)
            {
                GeneratePreview_Click(sender, e);
            }
        }
    }
}
