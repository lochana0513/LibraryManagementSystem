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

    public class InfoCard : Control

    {
       
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(InfoCard), new PropertyMetadata(null));

        public static readonly DependencyProperty BackgroundCustomProperty =
        DependencyProperty.Register("BackgroundCustom", typeof(Brush), typeof(InfoCard), new PropertyMetadata(Brushes.Transparent));

      
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);

        }


      
        public string BackgroundCustom
        {
            get => (string)GetValue(BackgroundCustomProperty);
            set => SetValue(BackgroundCustomProperty, value);
        }

        static InfoCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoCard), new FrameworkPropertyMetadata(typeof(InfoCard)));
        }
    }
}
