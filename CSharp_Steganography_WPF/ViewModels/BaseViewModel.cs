using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Steganography_WPF.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        protected void OnPropertyChanged<T>(ref T store, T value, [CallerMemberName] string name = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            if (store is not null && store.Equals(value))
                return;
            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
