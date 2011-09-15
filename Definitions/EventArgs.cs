using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Proxomo
{
    public class ItemCompletedEventArgs<T>
    {
        public Exception Error { get; set; }
        public T Result { get; set; }
    }
}
