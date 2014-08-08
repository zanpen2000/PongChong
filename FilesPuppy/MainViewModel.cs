using ExtendPropertyLib;
using ExtendPropertyLib.WPF;
using FilesPuppy.Models;
using FilesPuppy.Views;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Threading;
using FilesPuppy.Layers;


namespace FilesPuppy
{
    [Export(typeof(IShell))]
    public class MainViewModel : ViewModelBase, IShell
    {
        [Import(typeof(IWindowManager))]
        public IWindowManager iw = null;

        /// <summary>
        /// WCF服务状态
        /// </summary>
        public static ExtendProperty StartedProperty = RegisterProperty<MainViewModel>(v => v.Started);
        public bool Started { set { SetValue(StartedProperty, value); } get { return (bool)GetValue(StartedProperty); } }

        /// <summary>
        /// 监视状态
        /// </summary>
        public static ExtendProperty WatchedProperty = RegisterProperty<MainViewModel>(v => v.Watched);
        public bool Watched { set { SetValue(WatchedProperty, value); } get { return (bool)GetValue(WatchedProperty); } }

        /// <summary>
        /// 窗体日志
        /// </summary>
        public static ExtendProperty LogLinesProperty = RegisterProperty<MainViewModel>(v => v.LogLines);
        public ObservableCollection<LogLineModel> LogLines { set { SetValue(LogLinesProperty, value); } get { return (ObservableCollection<LogLineModel>)GetValue(LogLinesProperty); } }

        /// <summary>
        /// WCF主机
        /// 
        /// </summary>
        public ServiceHost Host { get; private set; }

        public RelayCommand OnServiceCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand OnWatchStartCommand { get; set; }
        public RelayCommand OnWatchStopCommand { get; set; }

        public RelayCommand GetAllCommand { get; set; }

        /// <summary>
        /// 全目录扫描
        /// </summary>
        public RelayCommand StartScanCommand { get; set; }

        public override void OnDoCreate(ExtendPropertyLib.ExtendObject item, params object[] args)
        {
            base.OnDoCreate(item, args);
            LogLines = new ObservableCollection<LogLineModel>();

            OnServiceCommand = new RelayCommand(StartService, CanSetService);
            OnWatchStartCommand = new RelayCommand(SetWatchStart, CanSetWatchStart);
            OnWatchStopCommand = new RelayCommand(SetWatchStop, CanSetWatchStop);
            SettingCommand = new RelayCommand(ShowSettingView);
            ExitCommand = new RelayCommand(AppExit, CanAppExit);
            StartScanCommand = new RelayCommand(ScanAll);
            GetAllCommand = new RelayCommand(GetAllTesting);

            this.PropertyChanged += MainViewModel_PropertyChanged;

        }

        private void GetAllTesting()
        {
            ServiceCaller.ServiceExecute<WcfServiceFileSystemWatcher.IWatcher>((w) =>
            {
                w.ScanDirectory("G:\\");
            });
            iw.ShowMessage("done!");
        }

        private void ScanAll()
        {

            ServiceCaller.ServiceExecute<WcfServiceFileSystemWatcher.IWatcher>((w) =>
            {
                w.ScanDirectory("");
            });
        }

        private bool CanSetWatchStop()
        {
            return WatcherLocator.Watchers.Select(t => t.Watched == true).Count() > 0;
        }

        private bool CanSetWatchStart()
        {
            return WatcherLocator.Watchers.Select(t => t.Watched == false).Count() > 0;
        }

        private void SetWatchStop()
        {
            var viewModel = CreateView<DirSelectStartViewModel>(true);
            viewModel.OnOff = false;
            var result = (bool)iw.ShowDialog(viewModel);
            if (result)
            {
                if (WatcherLocator.Watchers.Count > 0)
                {
                    foreach (var item in WatcherLocator.Watchers)
                    {
                        if (item.Watched && item.Selected)
                        {
                            SetLog("停止监视 " + item.Path);
                            item.OnCreated -= Value_OnCreated;
                            item.OnDeleted -= Value_OnDeleted;
                            item.OnChanged -= Value_OnChanged;
                            item.OnRenamed -= Value_OnRenamed;
                            item.Stop();
                        }
                    }
                }
            }
        }

        private void SetWatchStart()
        {
            var viewModel = CreateView<DirSelectStartViewModel>(true);
            viewModel.OnOff = true;
            var result = (bool)iw.ShowDialog(viewModel);
            if (result)
            {
                if (WatcherLocator.Watchers.Count > 0)
                {
                    foreach (var item in WatcherLocator.Watchers)
                    {
                        if (!item.Watched && item.Selected)
                        {
                            SetLog("开始监视 " + item.Path);
                            item.OnCreated += Value_OnCreated;
                            item.OnDeleted += Value_OnDeleted;
                            item.OnChanged += Value_OnChanged;
                            item.OnRenamed += Value_OnRenamed;
                            item.Start();
                        }
                    }
                }
            }
        }


        private bool CanAppExit()
        {
            return true;
        }

        private void AppExit()
        {
            iw.Close(this);
        }

        private void ShowSettingView()
        {
            var viewModel = CreateView<SettingViewModel>(true);
            iw.ShowDialog(viewModel);
        }

        void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Started"))
            {
                this.OnServiceCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName.Equals("Watched"))
            {
                this.OnWatchStopCommand.RaiseCanExecuteChanged();
                this.OnWatchStartCommand.RaiseCanExecuteChanged();
            }
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

        private void StartService()
        {
            try
            {
                if (Started)
                {

                    Host.Close();
                }
                else
                {
                    Host = new ServiceHost(typeof(WcfServiceFileSystemWatcher.Watcher));
                    Host.Opened += host_Opened;
                    Host.Closed += host_Closed;
                    Host.UnknownMessageReceived += host_UnknownMessageReceived;
                    Host.Open();
                }
                Started = !Started;
            }
            catch (Exception ex)
            {
                SetLog(ex.Message);
            }
        }

        public Dispatcher ThreadDispatcher = Dispatcher.CurrentDispatcher;

        private void SetLog(string msg)
        {
            ThreadDispatcher.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                LogLines.Add(new LogLineModel() { Message = msg, Time = DateTime.Now.ToString() });
            }, null);
        }

        private void host_Closed(object sender, EventArgs e)
        {
            SetLog("传输服务已关闭");
        }

        private void host_Opened(object sender, EventArgs e)
        {
            SetLog("传输服务已启动");

        }

        void Value_OnRenamed(object sender, System.IO.RenamedEventArgs e)
        {
            SetLog("重命名文件 " + e.OldFullPath + " -> " + e.FullPath);
        }

        void Value_OnChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            SetLog("修改文件 " + e.FullPath);
        }

        void Value_OnDeleted(object sender, System.IO.FileSystemEventArgs e)
        {
            SetLog("删除文件 " + e.FullPath);
        }

        void Value_OnCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            SetLog("创建文件 " + e.FullPath);
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
