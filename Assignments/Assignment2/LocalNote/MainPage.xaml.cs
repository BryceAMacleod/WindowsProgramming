﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LocalNote
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ViewModels.LocalNoteViewModel LNViewModel { get; set; } 

        public static AppBarButton EditAppBarButton { get; set;}
        public static TextBox EditContentTextbox { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.LNViewModel = new ViewModels.LocalNoteViewModel();
            EditAppBarButton = EditButton;
            EditContentTextbox = ContentTextbox;
            
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NoteList.SelectedIndex = -1;
        }
    }
}
