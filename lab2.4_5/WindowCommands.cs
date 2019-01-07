using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace lab2._4_5
{
    class WindowCommands
    {
        static WindowCommands()
        {
            SelectDarkThemeCommand = new RoutedCommand("SelectDarkThemeCommand", typeof(MainWindow));
            SelectLightThemeCommand = new RoutedCommand("SelectLightThemeCommand", typeof(MainWindow));
        }
        public static RoutedCommand SelectDarkThemeCommand { get; set; }
        public static RoutedCommand SelectLightThemeCommand { get; set; }

    }
}