using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    public class PropertyChangedBase : INotifyPropertyChanged
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

    public partial class DataInfo : PropertyChangedBase
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

        /// <summary>
        ///  唯一标识 
        /// </summary>
        public string ID { get; set; }

    }
}
