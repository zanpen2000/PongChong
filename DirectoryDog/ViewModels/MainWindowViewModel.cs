using Castle.DynamicProxy;
using DirectoryDog.AopInterceptor;
using DirectoryDog.Models;
using DirectoryDog.Views;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace DirectoryDog.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public const string Title = "目录监视";


        private bool status;

        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                this.OnPropertyChanged("Status");
            }
        }

        private ObservableCollection<FileModel> filelist;

        public ObservableCollection<FileModel> FileList
        {
            get { return filelist; }
            set
            {
                filelist = value;
                this.OnPropertyChanged("FileList");
            }
        }

        private WatchPuppy Puppy { get; set; }


        public DelegateCommand StartCommand { get; private set; }

        public DelegateCommand StopCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand SettingCommand { get; private set; }

        public Dispatcher ThreadDispatcher = Dispatcher.CurrentDispatcher;

        public MainWindowViewModel()
        {
            this.FileList = new ObservableCollection<FileModel>();


            this.PropertyChanged += MainWindowViewModel_PropertyChanged;

            StartCommand = new DelegateCommand(Start, CanStart);
            StopCommand = new DelegateCommand(Stop, CanStop);
            ExitCommand = new DelegateCommand(Exit, CanExit);
            SettingCommand = new DelegateCommand(Setting);

            this.Puppy = new WatchPuppy(@"d:\temp", "*");
            this.Puppy.OnCreated += Puppy_OnCreated;
            this.Puppy.OnDeleted += Puppy_OnDeleted;
            this.Puppy.OnRenamed += Puppy_OnRenamed;
            this.Puppy.OnChanged += Puppy_OnChanged;
        }

        private void Setting(object obj)
        {
            
        }

        void MainWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Status"))
            {
                this.StartCommand.InvalidateCanExecute();
                this.StopCommand.InvalidateCanExecute();
                this.ExitCommand.InvalidateCanExecute();
            }
        }

        private bool CanExit(object obj)
        {
            return !Status;

        }

        private void Exit(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        void Puppy_OnChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            ThreadDispatcher.InvokeAsync(() =>
            {
                this.FileList.Add(new FileModel() { FileName = e.FullPath, Time = DateTime.Now.ToString(), OldFileName = "", OperStr = "Changed" });
            });



        }

        void Puppy_OnRenamed(object sender, System.IO.RenamedEventArgs e)
        {

            ThreadDispatcher.InvokeAsync(() =>
            {
                this.FileList.Add(new FileModel() { FileName = e.FullPath, Time = DateTime.Now.ToString(), OldFileName = e.OldName, OperStr = "Renamed" });
            });


        }

        void Puppy_OnDeleted(object sender, System.IO.FileSystemEventArgs e)
        {
            ThreadDispatcher.InvokeAsync(() =>
            {
                this.FileList.Add(new FileModel() { FileName = e.FullPath, Time = DateTime.Now.ToString(), OldFileName = "", OperStr = "Deleted" });
            });

        }

        void Puppy_OnCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            //该类型的 CollectionView 不支持从调度程序线程以外的线程对其 SourceCollection 进行的更改。
            ThreadDispatcher.InvokeAsync(() =>
            {
                this.FileList.Add(new FileModel() { FileName = e.FullPath, Time = DateTime.Now.ToString(), OldFileName = "", OperStr = "Created" });
            });

        }

        private void Stop(object obj)
        {
            this.Puppy.Stop();
            Status = false;
        }

        private bool CanStop(object obj)
        {
            return Status;
        }

        private void Start(object obj)
        {

            this.Puppy.Start();
            Status = true;
        }

        private bool CanStart(object obj)
        {
            return !Status;
        }
    }
}
