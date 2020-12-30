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
using Vlc.DotNet.Core;

namespace Downloader
{
    public partial class MainWindow : Window
    {
        string currentDirectory = @"C:\nikita\test";
        bool IsDownloadInProgress = false;
        public string currentVid;
        public FileInfo currentFiVid;
        string currentTreeDirectory;
        bool autoplayEnabled = false;
        bool repeatEnabled = false;
        bool randomEnabled = false;
        VideoData Video;
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
            this.MyControl.SourceProvider.MediaPlayer.EndReached += new EventHandler<VlcMediaPlayerEndReachedEventArgs>(this.vlcControl1_EndReached);

            updateTree();
        }

        public void UpdateLog(string message)
        {
            //if (this.logTextBox.Dispatcher.Invoke.)
            this.Dispatcher.Invoke((Delegate)new MainWindow.UpdateLogDelegate(this.UpdateLogItemCallback), (object)message);
            //else
            //    this.UpdateLogItemCallback(message);
        }

        private void UpdateLogItemCallback(string message) => this.logTextBox.AppendText(message);

        private delegate void UpdateLogDelegate(string message);

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Directory.Exists(currentDirectory))
            {
                MessageBox.Show("Неверный путь");
            }
            currentTreeDirectory = e.NewValue.ToString();
            string path = currentDirectory + "\\" + e.NewValue + "\\";
            FFmpegParser ffmpegParser = new FFmpegParser();
            IEnumerable<FileInfo> fileInfos = DirUtil.EnumerateFiles(path);
            if (fileInfos == null)
            {
                MessageBox.Show("Папка пуста");
            }
            else
            {
                this.toolStripStatusLabel1.Content = "Начало загрузки фреймов...";
                //this.previewList.Dispatcher.BeginInvoke();
                this.previewList.ItemsSource = null;
                this.previewList.Items.Clear();
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
                    string pathUri = path + str;
                    var uri = new System.Uri(pathUri);
                    BitmapImage bitmap = new BitmapImage(uri);
                    Video.ImageData = bitmap;
                    Data.Add(Video);
                    this.toolStripStatusLabel1.Content = fileInfo.Name;
                }
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
            this.Video = this.previewList.SelectedItems[0] as VideoData;   
            this.currentVid = currentDirectory + "\\" + currentTreeDirectory + "\\" + Video.Title;
            this.currentFiVid = new FileInfo(this.currentVid);
            this.MyControl.SourceProvider.MediaPlayer.SetMedia(this.currentFiVid);
            this.MyControl.SourceProvider.MediaPlayer.Play();
        }

        private void pathToDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.currentDirectory = pathToDir.Text;

        }

        private void updateTree()
        {
            if (!Directory.Exists(currentDirectory))
            {
                MessageBox.Show("Неверный путь");
            }
            this.channelsTree.Items.Clear();
            IEnumerable<DirectoryInfo> directoryInfos = DirUtil.EnumerateDir(currentDirectory);
            if (directoryInfos == null)
            {
                MessageBox.Show("Пустая папка");
            }
            else
            {
                foreach (FileSystemInfo fileSystemInfo in directoryInfos)
                    this.channelsTree.Items.Add(fileSystemInfo.Name);
            }
        }

        private void pathToDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.currentDirectory = pathToDir.Text;
                updateTree();
            }
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(currentDirectory + "\\DOWNLOADS"))
            {
                try
                {
                    Directory.CreateDirectory(currentDirectory + "\\DOWNLOADS");
                }
                catch
                {
                    MessageBox.Show("Не удалось создать папку");
                    return;
                }
            }
            string url = Clipboard.GetText();
            if (!url.Contains("https://coub.com"))
            {
                string checkpath = this.currentDirectory + @"\DOWNLOADS";
                this.toolStripStatusLabel1.Content = "В ссылке не обнаружен coub.com! [" + url + "]";
                YoutubeDownloader downloader = new YoutubeDownloader();
                Task task1 = new Task((Action)(() => this.IsDownloadInProgress = true));
                Task<string> task2 = new Task<string>((Func<string>)(() => downloader.Download(url, this,
                    this.currentDirectory + @"\DOWNLOADS")));
                task2.ContinueWith((Action<Task<string>>)(td => this.UpdateLog(td.Result)));
                task2.Start();
            }
            else
            {
                CoubDownloader downloader = new CoubDownloader();
                Task task1 = new Task((Action)(() => this.IsDownloadInProgress = true));
                Task<string> task2 = new Task<string>((Func<string>)(() => downloader.Download(url, this, 
                    this.currentDirectory + @"\DOWNLOADS")));
                task2.ContinueWith((Action<Task<string>>)(td => this.UpdateLog(td.Result)));
                task2.Start();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.MyControl.SourceProvider.MediaPlayer.IsPlaying())
            {
                this.MyControl.SourceProvider.MediaPlayer.Pause();
            }
            else
            {
                this.MyControl.SourceProvider.MediaPlayer.Play();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try 
            { 
                this.MyControl.SourceProvider.MediaPlayer.Audio.Volume = Convert.ToInt32(Volume.Value);
            }
            catch
            {
                return;
            }
        }

        private void autoplay_Checked(object sender, RoutedEventArgs e)
        {
            this.autoplayEnabled = true;
            this.repeat.IsChecked = false;
        }

        private void autoplay_Unchecked(object sender, RoutedEventArgs e)
        {
            this.autoplayEnabled = false;
        }

        private void repeat_Checked(object sender, RoutedEventArgs e)
        {
            this.repeatEnabled = true;
            this.autoplay.IsChecked = false;
            this.playRandom.IsChecked = false;
        }

        private void repeat_Unchecked(object sender, RoutedEventArgs e)
        {
            this.repeatEnabled = false;
        }

        private void playRandom_Checked(object sender, RoutedEventArgs e)
        {
            this.randomEnabled = true;
            this.autoplay.IsChecked = true;
            this.repeat.IsChecked = false;
        }

        private void playRandom_Unchecked(object sender, RoutedEventArgs e)
        {
            this.randomEnabled = false;
        }

        private void vlcControl1_EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            this.UpdateLog("End reached\n");
            if (repeatEnabled)
            {
                new System.Threading.Tasks.Task(() =>
                {
                    MyControl.SourceProvider.MediaPlayer.Play(currentFiVid);
                }).Start();
            } 
            else if (autoplayEnabled)
            {
                VideoData nextVideo;
                if (!randomEnabled)
                {
                    var id = this.previewList.Items.IndexOf(Video);
                    if (id + 1 < this.previewList.Items.Count)
                    {
                        nextVideo = this.previewList.Items[id + 1] as VideoData;
                    }
                    else
                    {
                        nextVideo = this.previewList.Items[0] as VideoData;
                    }
                }
                else
                {
                    int id = new Random().Next(0, this.previewList.Items.Count - 1);
                    nextVideo = this.previewList.Items[id] as VideoData;
                    // TODO: проверка на меньше двух видео
                }
                this.currentVid = currentDirectory + "\\" + currentTreeDirectory + "\\" + nextVideo.Title;
                this.currentFiVid = new FileInfo(this.currentVid);
                new System.Threading.Tasks.Task(() =>
                {
                    MyControl.SourceProvider.MediaPlayer.Play(currentFiVid);
                }).Start();
            }
        }
    }
}
