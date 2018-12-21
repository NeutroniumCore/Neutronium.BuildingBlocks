﻿using System;
using Neutronium.MVVMComponents;

namespace Vm.Tools.Standard
{
    public interface IChooserCommand<out T> where T : IIODialog
    {
        bool CanBeExecuted { get; set; }

        T Picker { get; }

        IObservable<string> Results { get; }

        ICommandWithoutParameter Choose { get; }
    }
}