namespace Wpf
{
    internal class txtfile
    {
        private string fullName;

        public txtfile(string fullName)
        {
            this.fullName = fullName;
        }
        public string FullName//get和set分别为只读和只写，这是绑定的正常写法，Email为我们要进行绑定的一个属性
        {
            get { return fullName; }
            set { fullName = value; }
        }
    }
}