using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    class workchart
    {
        string name;
        string ip;
        public workchart(string name,string ip)
        {
            this.name = name;
            this.ip = ip;
        }
        public string Name//get和set分别为只读和只写，这是绑定的正常写法，Fname为我们要进行绑定的一个属性
        {
            get { return name; }
            set { name = value; }
        }
        public string IP//get和set分别为只读和只写，这是绑定的正常写法，Fname为我们要进行绑定的一个属性
        {
            get { return ip; }
            set { ip = value; }
        }
    }
}
