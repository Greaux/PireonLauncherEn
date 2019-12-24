using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PiReOnLauncher
{
    public class PireonClass
    {
        #region Gradient Color Background
        public static readonly DependencyProperty FirstColor = DependencyProperty.RegisterAttached("FirstColor", typeof(string), typeof(PireonClass), new UIPropertyMetadata(null));

        public static string GetFirstColor(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(FirstColor);
        }

        public static void SetFirstColor(DependencyObject obj, string value)
        {
            obj.SetValue(FirstColor, value);
        }
        public static readonly DependencyProperty SecondColor = DependencyProperty.RegisterAttached("SecondColor", typeof(string), typeof(PireonClass), new UIPropertyMetadata(null));

        public static string GetSecondColor(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(SecondColor);
        }

        public static void SetSecondColor(DependencyObject obj, string value)
        {
            obj.SetValue(SecondColor, value);
        }
        #endregion
        #region Gradient Color Brush
        public static readonly DependencyProperty FirstColorBrush = DependencyProperty.RegisterAttached("FirstColorBrush", typeof(string), typeof(PireonClass), new UIPropertyMetadata(null));

        public static string GetFirstColorBrush(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(FirstColorBrush);
        }

        public static void SetFirstColorBrush(DependencyObject obj, string value)
        {
            obj.SetValue(FirstColorBrush, value);
        }
        public static readonly DependencyProperty SecondColorBrush = DependencyProperty.RegisterAttached("SecondColorBrush", typeof(string), typeof(PireonClass), new UIPropertyMetadata(null));

        public static string GetSecondColorBrush(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(SecondColorBrush);
        }

        public static void SetSecondColorBrush(DependencyObject obj, string value)
        {
            obj.SetValue(SecondColorBrush, value);
        }
        #endregion
        #region Placeholder
        public static readonly DependencyProperty Placeholder = DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(PireonClass), new UIPropertyMetadata(null));

        public static string GetPlaceholder(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(Placeholder);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(Placeholder, value);
        }
        #endregion
    }
}
