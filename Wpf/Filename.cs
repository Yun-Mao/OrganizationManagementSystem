namespace main
{
    internal class Filename//搜索结果类
    {
        private string fname;
        private string line;
        private string ch;
        private string name;

        public Filename(string fname, string line, string ch, string name)
        {
            this.fname = fname;
            this.line = line;
            this.ch = ch;
            this.name = name;
        }
        public string Fname//get和set分别为只读和只写，这是绑定的正常写法，Fname为我们要进行绑定的一个属性
        {
            get { return fname; }
            set { fname = value; }
        }
        public string Line
        {
            get { return line; }
            set { line = value; }
        }
        public string Ch
        {
            get { return ch; }
            set { ch = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}