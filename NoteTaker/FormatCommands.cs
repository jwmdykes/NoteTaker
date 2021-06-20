using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NoteTaker
{
    public static class FormatCommands
    {
        static RoutedUICommand penColorPopup = new RoutedUICommand("Open color picker popup",
            "penColorPopup", typeof(FormatCommands));

        public static RoutedUICommand PenColorPopup { get { return penColorPopup; } }
    } 
}
