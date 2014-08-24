using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemoPuppy
{
    using AppLayer.Layers;
    using WcfServiceFileSystemWatcher;

    class Program
    {
       
        static void Main(string[] args)
        {
            string localfile = DateTime.Today.ToShortDateString() + ".txt";

            string filepath = System.IO.Path.Combine(@"g:\\sourcecode", localfile);

            var down = (new Program()).downloadFile(filepath, localfile).Result;

            Console.WriteLine("开始下载文件" + (down ? "Success" : "Failed"));

            Console.ReadKey();
        }



        public async Task<bool> downloadFile(string filepath, string localfile)
        {
            return await Task.Factory.StartNew<bool>(() =>
            {
                ServiceCaller.ServiceExecute<IWatcher>((w) =>
                {
                    System.IO.Stream stream = w.DownloadFile(filepath);

                    if (stream != null && stream.CanRead)
                    {
                        using (System.IO.FileStream fs = new System.IO.FileStream(localfile, System.IO.FileMode.Create))
                        {
                            const int bufferLength = 4096;
                            byte[] buffer = new byte[bufferLength];

                            int count;

                            while ((count = stream.Read(buffer, 0, bufferLength)) > 0)
                            {
                                fs.Write(buffer, 0, count);
                            }
                        }
                        stream.Close();
                    }
                });
                return System.IO.File.Exists(localfile);
            });
        }
    }
}
