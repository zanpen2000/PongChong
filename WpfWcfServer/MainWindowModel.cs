using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExtendPropertyLib;
using ExtendPropertyLib.WPF;
using System.ServiceModel;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using WpfWcfServer.Layers;
using System.Windows.Threading;
using WpfWcfServer.Models;
using System.ComponentModel;

namespace WpfWcfServer
{
    [Export(typeof(IShell))]
    public class MainWindowModel : ViewModelBase, IShell
    {
        [Import(typeof(IWindowManager))]
        public IWindowManager iw = null;

        /// <summary>
        /// WCF服务状态
        /// </summary>
        public static ExtendProperty StartedProperty = RegisterProperty<MainWindowModel>(v => v.Started);
        public bool Started { set { SetValue(StartedProperty, value); } get { return (bool)GetValue(StartedProperty); } }

        /// <summary>
        /// 窗体日志
        /// </summary>
        public static ExtendProperty LogLinesProperty = RegisterProperty<MainWindowModel>(v => v.LogLines);
        public ObservableCollection<LogLineModel> LogLines { set { SetValue(LogLinesProperty, value); } get { return (ObservableCollection<LogLineModel>)GetValue(LogLinesProperty); } }

        /// <summary>
        /// WCF主机
        /// 
        /// </summary>
        public MyServiceHost Host { get; private set; }
        public RelayCommand OnServiceCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }

        private DispatcherTimer dispatcherTimer;

        List<SnapshotDispatcherTimer> timerList;

        public override void OnDoCreate(ExtendPropertyLib.ExtendObject item, params object[] args)
        {
            base.OnDoCreate(item, args);

            LogLines = new ObservableCollection<LogLineModel>();

            OnServiceCommand = new RelayCommand(StartService, CanSetService);
            ExitCommand = new RelayCommand(AppExit, CanAppExit);
            SettingCommand = new RelayCommand(AppSetting, CanAppSetting);

            this.PropertyChanged += Model_PropertyChanged;

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(TimeSpan.TicksPerSecond); 
            dispatcherTimer.Tick += dispatcherTimer_Tick;


            timerList = new List<SnapshotDispatcherTimer>();
        }

        private void AppSetting()
        {
            var viewModel = CreateView<WpfWcfServer.Views.SettingViewModel>(true);
            iw.ShowDialog(viewModel);
        }

        private bool CanAppSetting()
        {
            return true;
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            var dirs = DirListHelper.Load();

            List<string> lst = (from n in timerList select n.DirName).ToList();

            var newdirs = dirs.Except(lst);

            foreach (var dir in newdirs)
            {
                SetLog(string.Format("添加目录:{0}", dir));
                var timer = new SnapshotDispatcherTimer(dir);
                timer.OnScanningBeginning += timer_OnScanningBeginning;
                timer.OnScanningCompleted += timer_OnScanningCompleted;
                timerList.Add(timer);
                timer.Start();
            }

            lst = (from n in timerList select n.DirName).ToList();

            var invalidTimers = lst.Except(dirs);

            foreach (var dirname in invalidTimers)
            {
                SetLog(string.Format("移除目录:{0}", dirname));
                var timer = timerList.Find(x => x.DirName == dirname);
                timer.Stop();
                timerList.Remove(timer);
            }

            dispatcherTimer.Start();
        }

        void timer_OnScanningBeginning(object sender, string e)
        {
            SetLog(e);
        }

        void timer_OnScanningCompleted(object sender, string e)
        {
            SetLog(e);
        }

        void Host_Faulted(object sender, EventArgs e)
        {
            SetLog("传输服务出现异常");
            Started = false;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Started"))
            {
                this.OnServiceCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanAppExit()
        {
            return true;
        }

        private void AppExit()
        {
            if (Host != null && Host.State == CommunicationState.Opened)
                Host.Close();
            System.Threading.Thread.Sleep(500);
            iw.Close(this);
        }

        private bool CanSetService()
        {
            if (Host == null) return true;

            if (Host.State == CommunicationState.Opened || Host.State == CommunicationState.Closed || Host.State == CommunicationState.Created)
            {
                return true;
            }
            else if (Host.State == CommunicationState.Faulted)
            {
                return false;
            }
            else
            {
                return false;
            }

        }

        private async void StartService()
        {
            await Task.Run(() =>
            {
                ThreadDispatcher.BeginInvoke((Action)delegate
                {
                    try
                    {
                        if (Started)
                        {
                            dispatcherTimer.Stop();
                            timerList.ForEach(t => t.Stop());
                            Host.Close();
                        }
                        else
                        {
                            dispatcherTimer.Start();
                            timerList.ForEach(t => t.Start());

                            Host = new MyServiceHost(typeof(WcfServiceFileSystemWatcher.Watcher));
                            Host.Opened += host_Opened;
                            Host.Closed += host_Closed;
                            Host.Faulted += Host_Faulted;
                            Host.UnknownMessageReceived += host_UnknownMessageReceived;
                            Host.Closing += Host_Closing;

                            Host.Open();
                        }

                    }
                    catch (Exception ex)
                    {
                        SetLog(ex.Message);
                    }
                });

            });
        }

        void Host_Closing(object sender, EventArgs e)
        {
            SetLog("传输服务正在关闭");
        }

        public Dispatcher ThreadDispatcher = Dispatcher.CurrentDispatcher;

        private void SetLog(string msg)
        {
            ThreadDispatcher.Invoke((Action)delegate
            {
                LogLines.Add(new LogLineModel() { Message = msg, Time = DateTime.Now.ToString() });
            }, null);
        }

        private void host_Closed(object sender, EventArgs e)
        {
            SetLog("传输服务已关闭");
            Started = false;
        }

        private void host_Opened(object sender, EventArgs e)
        {
            SetLog("传输服务已启动");
            Started = true;

        }

        private void host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            SetLog("接收到未知消息：" + e.Message);
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void Closed()
        {

            System.Windows.Application.Current.Shutdown();
        }

        public override string GetViewTitle()
        {
            return "目录监视";
        }
    }
}
