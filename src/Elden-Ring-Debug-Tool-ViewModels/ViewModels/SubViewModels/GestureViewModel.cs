﻿using Erd_Tools.Models.Game;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class GestureViewModel : ViewModelBase
    {
        private readonly Gesture _gesture;
        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetField(ref _enabled, value);
        }

        public int Id => _gesture.Id; 
        public string Name => _gesture.Name; 
        
        public GestureViewModel(Gesture gesture)
        {
            _gesture = gesture;
        }
    }
}