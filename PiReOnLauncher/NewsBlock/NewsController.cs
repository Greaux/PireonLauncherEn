using AuthLib.JSON;
using Newtonsoft.Json.Linq;
using PiReOnLauncher.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PiReOnLauncher.NewsBlock
{
    public class NewsController
    {
        public NewsController()
        {
            
        }
        public Button Generate(NewsRect rect)
        {
            Button btn = new Button();
            btn.SetResourceReference(Button.StyleProperty, "NewsBtn");
            btn.SetValue(News.Header, rect.Header);
            btn.SetValue(News.NewsContent, rect.Content);
            btn.SetValue(News.Date, rect.Date.ToLongDateString());
            return btn;
        }
        public void Add(List<NewsRect> rect)
        {
            foreach (var r in rect) 
                LDispatcher.InvokeIt(() => PiReOn.INSTANCE.News.Children.Add(Generate(r)));
        }
        public void AddNews()
        {
            JObject obj = UJson.GetJSONByURL($"http://pireon.ru/index.php?act=newsapi&key=123321&n=4");
            Add(ParseJson(obj));
        }
        public JObject GetObj(int i)
        {
            return UJson.GetJSONByURL($"http://pireon.ru/index.php?act=newsapi&key=123321&n={i}");
        }
        public List<NewsRect> ParseJson(JObject obj)
        {
            List<NewsRect> Newsss = new List<NewsRect>();
            JToken token = obj.GetNamespace("DATA");
            foreach (var @new in token) {
                try
                {
                    string title = @new["title"].ToString();
                    string content = @new["text"].ToString();
                    double time = double.Parse(@new["time"].ToString());
                    Newsss.Add(new NewsRect(title, content, time));
                }
                catch { }
            }
            return Newsss;
        }
    }
}
