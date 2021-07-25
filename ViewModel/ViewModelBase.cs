using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DOS2_Handbook.ViewModel
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string pName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }

        [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
        public class CallerMemberNameAttribute : Attribute
        {

        }
    }
}
