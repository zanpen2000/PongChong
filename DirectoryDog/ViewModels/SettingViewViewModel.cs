using DirectoryDog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DirectoryDog.ViewModels
{
    public class SettingViewViewModel : ViewModelBase
    {
        private ObservableCollection<string> dirList;


        

        public ObservableCollection<string> DirList
        {
            get { return dirList; }
            set
            {
                dirList = value;
                this.OnPropertyChanged("DirList");
            }
        }

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand DelCommand { get; set; }

        public DelegateCommand ApplyCommand { get; set; }

        public DelegateCommand CloseCommand { get; set; }


        public SettingViewViewModel()
        {
            AddCommand = new DelegateCommand(Add);
            DelCommand = new DelegateCommand(Del, CanDel);
            ApplyCommand = new DelegateCommand(Apply,CanApply);
            CloseCommand = new DelegateCommand(Close);
        }

        private void Close(object obj)
        {
            
        }

        private bool CanApply(object obj)
        {
            return true;
        }

        private void Apply(object obj)
        {
           
        }

        private bool CanDel(object obj)
        {
            return true;
        }

        private void Del(object obj)
        {
            
        }

        private void Add(object obj)
        {
            
        }

        void LoadDirList()
        {
            DirList = (ObservableCollection<string>)DirListHelper.Load();
        }

        void SaveDirList()
        {
            DirListHelper.Save(DirList);
        }
    }
}
