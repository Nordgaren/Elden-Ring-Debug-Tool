﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for GraceView.xaml
    /// </summary>
    public partial class GraceView : UserControl
    {
        public GraceView()
        {
            InitializeComponent();
        }

        private GraceViewViewModel _graceViewViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is GraceViewViewModel vm)
            {
                _graceViewViewModel = vm;
            }
        }
    }
}
