using System;
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

namespace LibraryManagementSystem.Components
{

    public class NavButton : ListBoxItem
    {
        static NavButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
        }

        public Uri Navlink
        {
            get { return (Uri)GetValue(NavlinkProperty); }
            set { SetValue(NavlinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Navlink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavlinkProperty =
            DependencyProperty.Register("Navlink", typeof(Uri), typeof(NavButton), new PropertyMetadata(null));



        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(NavButton), new PropertyMetadata(null));

        public string Navitem
        {
            get { return (string)GetValue(NavitemProperty); }
            set { SetValue(NavitemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Navitem. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavitemProperty =
            DependencyProperty.Register("Navitem", typeof(string), typeof(NavButton), new PropertyMetadata(null));

    }
}
