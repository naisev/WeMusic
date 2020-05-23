using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.ViewModel
{
    class CreateListDialogViewModel : BindableBase
    {
        private string _listName;
        public string ListName
        {
            get { return _listName; }
            set
            {
                _listName = value;
                this.RaisePropertyChanged("ListName");
            }
        }

    }
}
