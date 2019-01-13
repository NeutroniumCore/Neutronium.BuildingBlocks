using System;
using Neutronium.MVVMComponents;

namespace Neutronium.BuildingBlocks.ApplicationTools
{
    /// <summary>
    /// Helper to create a command from <see cref="IFilePicker"/> or <see cref="IFolderPicker"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChooserCommand<out T> where T : IIODialog
    {
        /// <summary>
        /// CanBeExecuted
        /// </summary>
        bool CanBeExecuted { get; set; }

        /// <summary>
        /// File or Folder picker instance
        /// </summary>
        T Picker { get; }

        /// <summary>
        /// Results as observable
        /// </summary>
        IObservable<string> Results { get; }

        /// <summary>
        /// Picker exposed as command.
        /// </summary>
        ICommandWithoutParameter Choose { get; }
    }
}
