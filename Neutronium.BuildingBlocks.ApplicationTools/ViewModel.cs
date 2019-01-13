using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Neutronium.BuildingBlocks
{
    /// <summary>
    /// Base class for ViewModel providing implementation of <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanging"/>
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Update property value and call corresponding <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanging"/> if needed
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="property">Property to be updated</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">Property name (filled by compiler when left null)</param>
        /// <returns>
        /// True if property value has changed and events have been fired
        /// </returns>
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
