using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using ZstdSharp.Unsafe;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands.GestureView
{
    public class SetGestureCommand : CommandBase
    {
        private GestureViewViewModel _gestureViewViewModel;
        
        public SetGestureCommand(GestureViewViewModel gestureViewViewModel)
        {
            _gestureViewViewModel = gestureViewViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return _gestureViewViewModel.Hook.Setup && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (!(parameter is GestureViewModel gesture))
                throw new ArgumentNullException(nameof(parameter), "parameter was null. parameter must be an int");

            // We use the state in the view model, because the button changes it.  
            _gestureViewViewModel.Hook.GestureGameData.SetGesture(gesture.Id, gesture.Enabled);
        }
    }
}