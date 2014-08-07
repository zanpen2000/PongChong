using ExtendPropertyLib;
using System;
using System.Linq;

namespace FilesPuppy.Views
{
    using System.ComponentModel.Composition;
    using ExtendPropertyLib.WPF;
    using System.Collections.ObjectModel;
    using FilesPuppy.Models;
    using System.Windows.Data;
    using System.Collections.Generic;
    using ExtendPropertyLib.WPF.Helper;

    public class DirSelectStartViewModel : ViewModelBase
    {
        [Import(typeof(IWindowManager))]
        public IWindowManager Window = null;

        //public static ExtendProperty DirsProperty = RegisterProperty<DirSelectStartViewModel>(v => v.Dirs);
        //public ObservableCollection<DirWithStatus> Dirs
        //{
        //    set { SetValue(DirsProperty, value); }
        //    get { return (ObservableCollection<DirWithStatus>)GetValue(DirsProperty); }
        //}

        public static ExtendProperty DirsViewProperty = RegisterProperty<DirSelectStartViewModel>(v => v.DirsView);
        public CollectionView DirsView
        {
            set { SetValue(DirsViewProperty, value); }
            get { return (CollectionView)GetValue(DirsViewProperty); }
        }


        ///<summary>
        ///构造方法
        ///</summary>
        public DirSelectStartViewModel()
        {
            init();
        }

        public RelayCommand OKCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        private void init()
        {
            List<DirWithStatus> dirs = new List<DirWithStatus>();
            FilesPuppy.Models.DirListHelper.Load().ToList().ForEach(v =>
            {
                dirs.Add(new DirWithStatus() { Fullpath = v, Selected = false });
            });

            DirsView = new CollectionView(dirs);
            DirsView.Filter = new Predicate<object>(Contains);
            OKCommand = new RelayCommand(OKExecute, CanOKExecute);
            CancelCommand = new RelayCommand(CancelExecute);
        }

        private bool Contains(object obj)
        {
            return (obj as DirWithStatus).Selected == false;
        }

        private void CancelExecute()
        {
            this.TryCancel();
        }

        private bool CanOKExecute()
        {
            return DirsView.Count > 0;
        }

        private void OKExecute()
        {
            DirsView.Filter = null;

            DirsView.Filter = (obj) => { return (obj as DirWithStatus).Selected == true; };

            foreach (DirWithStatus item in DirsView)
            {
                WatchPuppy w = new WatchPuppy(item.Fullpath,"*");
                WatcherLocator.AddWatcher(w);
            }

            this.TryOK();
        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        public override void OnLoad()
        {
            base.OnLoad();
        }
    }
}
