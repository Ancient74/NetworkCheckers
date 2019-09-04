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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkCheckers
{
    /// <summary>
    /// Логика взаимодействия для BackgroundControl.xaml
    /// </summary>
    public partial class BackgroundControl : UserControl
    {
        public BackgroundControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Resources["backgroundAnimation"] is Storyboard storyboard)
                storyboard.Begin();
        }
    }
}
