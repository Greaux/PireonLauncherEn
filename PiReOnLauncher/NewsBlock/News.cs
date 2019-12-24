using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PiReOnLauncher.NewsBlock
{
    public class News
    {
        #region *Model*
        public static readonly DependencyProperty Header = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(News), new UIPropertyMetadata(null));

        public static string GetHeader(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(Header);
        }

        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(Header, value);
        }
        public static readonly DependencyProperty NewsContent = DependencyProperty.RegisterAttached("NewsContent", typeof(string), typeof(News), new UIPropertyMetadata(null));

        public static string GetNewsContent(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(NewsContent);
        }

        public static void SetNewsContent(DependencyObject obj, string value)
        {
            obj.SetValue(NewsContent, value);
        }
        public static readonly DependencyProperty Date = DependencyProperty.RegisterAttached("Date", typeof(string), typeof(News), new UIPropertyMetadata(null));

        public static string GetDate(DependencyObject obj, string value)
        {
            return (string)obj.GetValue(Date);
        }

        public static void SetDate(DependencyObject obj, string value)
        {
            obj.SetValue(Date, value);
        }
        #endregion
    }
}
