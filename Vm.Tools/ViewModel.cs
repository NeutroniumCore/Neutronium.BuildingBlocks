﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vm.Tools
{
    public abstract class ViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected bool Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
                return false;

            PropertyIsChanging(propertyName);
            property = value;
            PropertyHasChanged(propertyName);
            return true;
        }

        protected void PropertyHasChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void PropertyIsChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
