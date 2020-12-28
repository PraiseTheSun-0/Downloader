using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    public static class DirUtil
    {
        public static IEnumerable<DirectoryInfo> EnumerateDir(string path)
        {
            try
            {
                return new DirectoryInfo(path).EnumerateDirectories();
            }
            catch (Exception ex)
            {
                Console.WriteLine((object)ex);
                return (IEnumerable<DirectoryInfo>)null;
            }
        }

        public static IEnumerable<FileInfo> EnumerateFiles(string path)
        {
            try
            {
                return new DirectoryInfo(path).EnumerateFiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine((object)ex);
                return (IEnumerable<FileInfo>)null;
            }
        }
    }
}
