using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DOS2_Handbook.Service
{
    class FileServices
    {
        public static string ResourcePackagePath = "ornatechest.pak";

        public static bool ResourceFileExists()
        {
            return File.Exists(ResourcePackagePath);
        }

        /// <summary>
        /// 读取文本类型的文件，并返回其中的内容
        /// </summary>
        /// <param name="entryName"></param>
        /// <returns></returns>
        public static string GetTextFileContent(string entryName)
        {
            var archive = ZipFile.OpenRead(ResourcePackagePath);
            var entry = archive.GetEntry(entryName);
            if (entry != null)
            {
                return new StreamReader(entry.Open()).ReadToEnd();
            }
            return "";
        }


        /// <summary>
        /// 获取zip文件中的指定图片内容
        /// </summary>
        /// <param name="entryName"></param>
        /// <returns>BitmapImage类型的图片</returns>
        public static BitmapImage GetImageAsBitmapImage(string entryName)
        {
            var archive = ZipFile.OpenRead(ResourcePackagePath);
            var entry = archive.GetEntry(entryName);
            if(entry != null)
            {
                var bitmap = new BitmapImage();
                var ms = new MemoryStream();
                var stream = entry.Open();
                stream.CopyTo(ms);      //辣鸡机制，白白占用内存
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                stream.Dispose();
                ms.Dispose();
                return bitmap;
            }
            return null;
        }

    }
}
