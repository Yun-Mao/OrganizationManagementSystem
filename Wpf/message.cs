using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    class message
    {
        private string fromuser;
        private string time;
        private string text;
        private string state;
        public message(string fromuser,string time,string text,int state)
        {
            this.fromuser = fromuser;
            this.time = time;
            this.text = text;
            if (state == 1)
            {
                this.state = "未读";
            }
            else
            {
                this.state = "已读";
            }
        }
        public string Fromuser
        {
            get { return fromuser; }
            set { fromuser = value; }
        }
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public string State
        {
            get { return state; }
            set { state = value; }
        }
    }
}
