using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Subsetter;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace fontsubset_gui;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        btnInputFont.Click += OnBtnInputFont;
        btnOutputFont.Click += OnBtnOutputFont;
        btnInputCharsPath.Click += OnBtnInputCharsPath;
        btnStart.Click += OnBtnStartSubset;

        togCustomMatch.IsCheckedChanged += CustomMatch_IsCheckedChanged;
        togCustomMatch.IsChecked = true;
        togCustomMatch.IsChecked = false;

    }

    async void OnBtnInputFont(object? sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog()
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>()
                {
                    new FileDialogFilter() { Name = "Font files", Extensions = new List<string> { "ttf", "otf" } }
                }
        };
        var result = await dlg.ShowAsync(this);

        if (result != null && result.Length > 0)
        {
            inputFont.Text = result[0];
        }
    }

    async void OnBtnOutputFont(object? sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "Font", Extensions = { "ttf", "otf" } });
        var result = await dialog.ShowAsync(this);

        if (result != null)
        {
            outputFont.Text = result;
        }
    }


    async void OnBtnInputCharsPath(object? sender, RoutedEventArgs e)
    {
        // 创建一个新的OpenFolderDialog实例
        OpenFolderDialog dialog = new OpenFolderDialog();

        // 显示对话框并获取用户选择的文件夹路径
        string result = await dialog.ShowAsync(this);

        // result现在包含用户选择的文件夹路径，或者如果用户取消了操作则为空
        if (!string.IsNullOrWhiteSpace(result))
        {
            inputCharsPath.Text = result;
        }
    }

    private void CustomMatch_IsCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (!togCustomMatch.IsChecked.Value)
        {
            charsFileMatch.Text = @".+\.txt|.+\.lua|.+\.asset";
            charsFileMatch.IsEnabled = false;
        }
        else
        {
            charsFileMatch.IsEnabled = true;
        }
    }


    /// <summary>
    /// 开始裁剪字体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void OnBtnStartSubset(object? sender, RoutedEventArgs e)
    {
        if (!File.Exists(inputFont.Text))
        {

            var box = MessageBoxManager.GetMessageBoxStandard("Error", "Input Font not exist:" + inputFont.Text, ButtonEnum.Ok);
            await box.ShowAsync();
            return;
        }
        if (!Directory.Exists(inputCharsPath.Text))
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", "Input Characters set Directory not exist:" + inputCharsPath.Text, ButtonEnum.Ok);
            await box.ShowAsync();
            return;
        }
        var ex = await SubsetterUtil.StartSubset(inputFont.Text, outputFont.Text, inputCharsPath.Text, charsFileMatch.Text, togContatinAscii.IsChecked.Value, togStripTable.IsChecked.Value);
        if (ex != null)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", "Subset Failed, Exception:" + ex.ToString(), ButtonEnum.Ok);
            await box.ShowAsync();
            return;
        }

        var box1 = MessageBoxManager.GetMessageBoxStandard("Success", "Subset Success", ButtonEnum.Ok);
        await box1.ShowAsync();

        //打开目录
        string outputPath = outputFont.Text;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using Process fileOpener = new Process();
            fileOpener.StartInfo.FileName = "explorer";
            fileOpener.StartInfo.Arguments = "/select," + outputPath + "\"";
            fileOpener.Start();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            using Process fileOpener = new Process();
            fileOpener.StartInfo.FileName = "explorer";
            fileOpener.StartInfo.Arguments = "-R " + outputPath;
            fileOpener.Start();
        }
    }


}