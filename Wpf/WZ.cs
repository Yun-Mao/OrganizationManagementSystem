using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    public class PropertyChangedBase2 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    class WZ: PropertyChangedBase2
    {
        private bool selected;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; Notify("Selected"); }
        }

        private string name;
        private int num;
        private int remain;
        private string detail;
        private string time;
        private string touser;
        private string state;
        private string gtime;
        public WZ(string name, int num, int remain, string detail)
        {
            this.name = name;
            this.num = num;
            this.remain = remain;
            this.detail = detail;
        }
        public WZ(string name, int num, string detail,string time,string touser,int state)
        {
            this.name = name;
            this.num = num;
            this.detail = detail;
            this.time = time;
            this.touser = touser;
            if (state == 1)
            {
                this.state = "借";
            }
            else
            {
                this.state = "已还";
            }
        }
        public string Name//get和set分别为只读和只写，这是绑定的正常写法，Fname为我们要进行绑定的一个属性
        {
            get { return name; }
            set { name = value; }
        }
        public int Num
        {
            get { return num; }
            set { num = value; }
        }
        public int Remain
        {
            get { return remain; }
            set { remain = value; }
        }
        public string Touser
        {
            get { return touser; }
            set { touser = value; }
        }
        public string State
        {
            get { return state; }
            set { state = value; }
        }
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        public string GTime
        {
            get { return gtime; }
            set { gtime = value; }
        }
        public string Detail
        {
            get { return detail; }
            set { detail = value; }
        }
    }
}
