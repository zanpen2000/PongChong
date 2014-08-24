using ExtendPropertyLib.WPF;
using ExtendPropertyLib;
using System.Collections.ObjectModel;
using System.Linq;
using WpfWcfServer.Models;
using System.ComponentModel.Composition;

namespace WpfWcfServer.Views
{
    public class SettingViewModel : ViewModelBase
    {
        [Import(typeof(IWindowManager))]
        private IWindowManager iw = null;

        public static ExtendProperty DirListProperty = RegisterProperty<SettingViewModel>(v => v.DirList);
        public ObservableCollection<string> DirList { set { SetValue(DirListProperty, value); } get { return (ObservableCollection<string>)GetValue(DirListProperty); } }

        public static ExtendProperty DirListChangedStateProperty = RegisterProperty<SettingViewModel>(v => v.DirListChangedState);
        public bool DirListChangedState { set { SetValue(DirListChangedStateProperty, value); } get { return (bool)GetValue(DirListChangedStateProperty); } }

        public static ExtendProperty SelectedDirProperty = RegisterProperty<SettingViewModel>(v => v.SelectedDir);
        public string SelectedDir { set { SetValue(SelectedDirProperty, value); } get { return (string)GetValue(SelectedDirProperty); } }


        public RelayCommand AddCommand { get; set; }

        public RelayCommand DelCommand { get; set; }

        public RelayCommand ApplyCommand { get; set; }

        public RelayCommand CloseCommand { get; set; }


        private void Close()
        {
            iw.Close(this);
        }

        private bool CanApply()
        {
            return DirListChangedState;
        }

        private void Apply()
        {
            SaveDirList();
        }

        private bool CanDel()
        {
            return !string.IsNullOrEmpty(SelectedDir);
        }

        private void Del()
        {
            DirList.Remove(SelectedDir);
            DirListChangedState = true;
        }

        private void Add()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (DirList.IndexOf(dialog.SelectedPath) < 0)
                {
                    DirList.Add(dialog.SelectedPath);
                    DirListChangedState = true;
                }
            }
        }

        void LoadDirList()
        {
            var dirs = DirListHelper.Load();
            if (dirs != null)
            {
                dirs.ToList().ForEach(x => DirList.Add(x));
            }
            DirListChangedState = false;
        }

        void SaveDirList()
        {
            DirListHelper.Save(DirList);
            DirListChangedState = false;
        }

        public override void OnDoCreate(ExtendObject item, params object[] args)
        {
            base.OnDoCreate(item, args);
        }

        public SettingViewModel()
        {
            AddCommand = new RelayCommand(Add);
            DelCommand = new RelayCommand(Del, CanDel);
            ApplyCommand = new RelayCommand(Apply, CanApply);
            CloseCommand = new RelayCommand(Close);

            this.PropertyChanged += SettingViewModel_PropertyChanged;
        }

        void SettingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedDir"))
            {
                DelCommand.RaiseCanExecuteChanged();
            }
            if (e.PropertyName.Equals("DirListChangedState"))
            {
                ApplyCommand.RaiseCanExecuteChanged();
            }
        }

        public override void OnLoad()
        {
            base.OnLoad();
            DirList = new ObservableCollection<string>();
            LoadDirList();
            if (DirList == null)
            {
                DirList = new ObservableCollection<string>();
            }


        }
    }
}
