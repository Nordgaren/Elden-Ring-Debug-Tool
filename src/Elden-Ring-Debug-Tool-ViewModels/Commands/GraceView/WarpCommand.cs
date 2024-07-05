﻿using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Warp Command")]
    public class WarpCommand : CommandBase
    {
        private GraceViewViewModel _graceViewViewModel { get; }

        public WarpCommand(GraceViewViewModel graceViewViewModel)
        {
            _graceViewViewModel = graceViewViewModel;
            _graceViewViewModel.PropertyChanged += _debugViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _graceViewViewModel.Hook.Setup && _graceViewViewModel.Hook.Loaded && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (!(parameter is int entityID))
                throw new ArgumentNullException(nameof(parameter), "parameter was null. parameter must be an int");

            _graceViewViewModel.Hook.Warp(entityID);
        }

        private void _debugViewViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(GraceViewViewModel.Setup)
                or nameof(GraceViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
