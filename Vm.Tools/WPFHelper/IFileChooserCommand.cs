using System;
using Neutronium.MVVMComponents;

namespace Vm.Tools.WPFHelper
{
    public interface IFileChooserCommand: ISimpleCommand
    {
        Action<string> Setter { get; set; }

        IFilePicker FilePicker { get; }
    }
}
