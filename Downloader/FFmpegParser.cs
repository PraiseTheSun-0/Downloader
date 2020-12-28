using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    public class FFmpegParser
    {
        private Process ffmpeg;

        public void exec(string input, string output, string prms)
        {
            this.ffmpeg = new Process();
            this.ffmpeg.StartInfo.Arguments = " -i " + input + (prms != null ? " " + prms : "") + " " + output;
            this.ffmpeg.StartInfo.FileName = "ffmpeg.exe";
            this.ffmpeg.StartInfo.UseShellExecute = false;
            this.ffmpeg.StartInfo.RedirectStandardOutput = true;
            this.ffmpeg.StartInfo.RedirectStandardError = true;
            this.ffmpeg.StartInfo.CreateNoWindow = true;
            this.ffmpeg.Start();
            this.ffmpeg.WaitForExit();
            this.ffmpeg.Close();
        }

        public void GetThumbnail(string video, string jpg, string resolution)
        {
            if (resolution == null)
                resolution = "640x480";
            this.exec(video, jpg, "-s " + resolution);
        }
    }
}
