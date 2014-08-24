using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfWcfServer.Models
{


    public class SnapshotDispatcherTimer
    {
        public event EventHandler<string> OnScanningCompleted;
        public event EventHandler<string> OnScanningBeginning;

        private DispatcherTimer dispatcherTimer;
        private string dir;
        public string DirName { get { return dir; } }

        public SnapshotDispatcherTimer(string dir)
        {
            this.dir = dir;

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(TimeSpan.TicksPerMinute);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
        }

        public void Start()
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                dispatcherTimer.Stop();
                if (OnScanningBeginning != null)
                {
                    this.OnScanningBeginning.BeginInvoke(this, string.Format("开始扫描目录{0}", dir), null, null);
                }

                return WcfServiceFileSystemWatcher.Models.ScannerHelper.ScanDirectory(dir);

            }).ContinueWith(async (json) =>
            {
                string filename = System.IO.Path.Combine(dir, DateTime.Today.ToShortDateString() + ".txt");
                System.IO.File.WriteAllText(filename, await json);
                if (this.OnScanningCompleted != null)
                {
                    this.OnScanningCompleted.BeginInvoke(this, string.Format("目录{0}的扫描结果存储在{1}", dir, filename), null, null);
                }
                dispatcherTimer.Start();
            });
        }

        public void Stop()
        {
            if (dispatcherTimer != null)
                dispatcherTimer.Stop();
        }
    }
}
