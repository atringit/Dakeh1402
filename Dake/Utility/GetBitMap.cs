using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml.Presentation;

namespace Dake.Utility
{
    public class GetBitMap
    {
        public static Bitmap GetThumbnail(string video, string thumbnail)
        {
            var cmd = "ffmpeg  -itsoffset -1  -i " + '"' + video + '"' +
                      " -vcodec mjpeg -vframes 1 -an -f rawvideo -s 320x240 " + '"' + thumbnail + '"';

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + cmd
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit(10000);

            return LoadImage(thumbnail);
        }

        static Bitmap LoadImage(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path));
            return (Bitmap) Image.FromStream(ms);
        }
    }
}