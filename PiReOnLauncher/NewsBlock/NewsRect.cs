using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiReOnLauncher.NewsBlock
{
    public class NewsRect
    {
        public NewsRect(string Header, string Content, double Date)
        {
            this.Header = Header;
            this.Content = Content;
            this.Date = FromUnix(Date);
        }
        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } //DateTime ?_?
        private DateTime FromUnix(double time)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(time).ToLocalTime();
            return dtDateTime;
        }
    }
}
