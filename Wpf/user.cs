using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wpf
{
    class user
    {
        private string name;
        private string power;
        private BitmapImage bitmap;
        private string password;
        public user(string name, string power, BitmapImage bitmap, string password)
        {
            this.name = name;
            this.power = power;
            this.bitmap = bitmap;
            this.password = password;
        }
        public user(string name)
        {
            this.name = name;
        }
        public string Name//get和set分别为只读和只写，这是绑定的正常写法，Fname为我们要进行绑定的一个属性
        {
            get { return name; }
            set { name = value; }
        }

        public string Power
        {
            get { return power; }
            set { power = value; }
        }
        public BitmapImage Bitmap
        {
            get { return bitmap; }
            set { bitmap = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
