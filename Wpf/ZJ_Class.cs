using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    public partial class ZJ_Class
    {
        private float money;
        private string type;
        private string people;
        private string detail;
        private string time;
        public ZJ_Class(float money, string type, string people, string time,string detail)
        {
            this.money = money;
            this.type = type;
            this.people = people;
            this.time = time;
            this.detail = detail;
        }
        public float Money//get和set分别为只读和只写，这是绑定的正常写法，Fname为我们要进行绑定的一个属性
        {
            get { return money; }
            set { money = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string People
        {
            get { return people; }
            set { people = value; }
        }
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        public string Detail
        {
            get { return detail; }
            set { detail = value; }
        }
    }
}
