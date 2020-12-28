using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Downloader
{
    public partial class MainWindow : Window
    {
        string currentDirectory = @"C:\nikita\test";
        bool IsDownloadInProgress = false;
        public string currentVid;
        public FileInfo currentFiVid;
        string currentTreeDirectory;
        public MainWindow()
        {
            InitializeComponent();

            DirectoryInfo vlcLibDirectory;
            if (IntPtr.Size == 4)
                vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory, @"libvlc\win-x86"));
            else
                vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory, @"libvlc\win-x64"));

            var options = new string[] { };

            this.MyControl.SourceProvider.CreatePlayer(vlcLibDirectory, options);

            this.channelsTree.Items.Clear();
            IEnumerable<DirectoryInfo> directoryInfos = DirUtil.EnumerateDir(currentDirectory);
            if (directoryInfos == null)
            {
                int num = (int)MessageBox.Show("Пустой dir");
            }
            else
            {
                foreach (FileSystemInfo fileSystemInfo in directoryInfos)
                    this.channelsTree.Items.Add(fileSystemInfo.Name);
            }
            //this.MyControl.SourceProvider.MediaPlayer.Play(new FileInfo(@"D:\Nikita\test.mp4"));

            //CoubDownloader downloader = new CoubDownloader();
            //Task task1 = new Task((Action)(() => this.IsDownloadInProgress = true));
            //Task<string> task2 = new Task<string>((Func<string>)(() => downloader.Download(@"https://coub.com/view/2h3pe6", this,
            //    @"C:\nikita\test\")));
            //task2.ContinueWith((Action<Task<string>>)(td => this.UpdateLog(td.Result)));
            //task2.Start();

            //FFmpegParser ffmpegParser = new FFmpegParser();
            //ffmpegParser.GetThumbnail(@"C:\nikita\test\2h3pe6.mp4", @"C:\nikita\test\124.jpg", "640x480");
        }

        public void UpdateLog(string message)
        {
            //if (this.logTextBox.)
            this.Dispatcher.Invoke((Delegate)new MainWindow.UpdateLogDelegate(this.UpdateLogItemCallback), (object)message);
            //else
            //    this.UpdateLogItemCallback(message);
        }

        private void UpdateLogItemCallback(string message) => this.logTextBox.AppendText(message);

        private delegate void UpdateLogDelegate(string message);

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            currentTreeDirectory = e.NewValue.ToString();
            string path = currentDirectory + "\\" + e.NewValue + "\\";
            FFmpegParser ffmpegParser = new FFmpegParser();
            IEnumerable<FileInfo> fileInfos = DirUtil.EnumerateFiles(path);
            if (fileInfos == null)
            {
                int num = (int)MessageBox.Show("Пустой files");
            }
            else
            {
                this.toolStripStatusLabel1.Content = "Начало загрузки фреймов...";
                //this.previewList.Dispatcher.BeginInvoke();
                this.previewList.Items.Clear();
                //ImageList imageList = new ImageList();
                //imageList.ImageSize = new Size(64, 64);
                //imageList.ColorDepth = ColorDepth.Depth32Bit;
                //this.previewList. = imageList;
                if (!Directory.Exists(path + "images"))
                    Directory.CreateDirectory(path + "images");
                var Data = new List<VideoData>();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    var Video = new VideoData();
                    Video.Title = fileInfo.Name;
                    string str = "images\\" + fileInfo.Name + ".jpg";
                    if (!File.Exists(path + str))
                        ffmpegParser.GetThumbnail(path + fileInfo.Name, path + str, "640x480");
                    var uri = new System.Uri(path + str);
                    BitmapImage bitmap = new BitmapImage(uri);
                    //    imageList.Images.Add(fileInfo.Name, (Image)bitmap);
                    //this.previewList.Items.Add(fileInfo.Name);
                    Video.ImageData = bitmap;
                    Data.Add(Video);
                    //    this.toolStripStatusLabel1.Text = fileInfo.Name;
                }
                //this.previewList.EndUpdate();
                this.previewList.ItemsSource = Data;
                this.toolStripStatusLabel1.Content = "Фреймы загружены";
            }
        }

        private void previewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //    this.currentVid = currentDirectory + "\\" + currentTreeDirectory + "\\" + this.previewList.SelectedItems[0].ToString();
            //    this.currentFiVid = new FileInfo(this.currentVid);
            //    this.MyControl.SourceProvider.MediaPlayer.SetMedia(this.currentFiVid);
            //    this.MyControl.SourceProvider.MediaPlayer.Play();
        }

        private void previewList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var Video = this.previewList.SelectedItems[0] as VideoData;   
            this.currentVid = currentDirectory + "\\" + currentTreeDirectory + "\\" + Video.Title;
            this.currentFiVid = new FileInfo(this.currentVid);
            this.MyControl.SourceProvider.MediaPlayer.SetMedia(this.currentFiVid);
            this.MyControl.SourceProvider.MediaPlayer.Play();
        }
    }
}
