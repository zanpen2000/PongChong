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

        public static ExtendProperty DirsViewProperty = RegisterProperty<DirSelectStartViewModel>(v => v.DirsView);
        public CollectionView DirsView
        {
            set { SetValue(DirsViewProperty, value); }
            get { return (CollectionView)GetValue(DirsViewProperty); }
        }

        public static ExtendProperty OnOffProperty = RegisterProperty<DirSelectStartViewModel>(v => v.OnOff);
        public bool OnOff
        {
            set { SetValue(OnOffProperty, value); }
            get { return (bool)GetValue(OnOffProperty); }
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
            FilesPuppy.Models.DirListHelper.Load().ToList().ForEach(v =>
            {
                WatcherLocator.AddWatcher(v);
            });

            DirsView = new CollectionView(WatcherLocator.Watchers);
            DirsView.Filter = new Predicate<object>(Contains);
            OKCommand = new RelayCommand(OKExecute, CanOKExecute);
            CancelCommand = new RelayCommand(CancelExecute);
        }

        private bool Contains(object obj)
        {
            var o = (obj as WatchPuppy);
            return o.Watched == OnOff;
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

            DirsView.Filter = (obj) => { return (obj as WatchPuppy).Selected == OnOff; };

            foreach (WatchPuppy watch in DirsView)
            {
                watch.Selected = OnOff;
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
