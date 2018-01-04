using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    class IndexEntry : IComparable//索引文件类
    {
        string term;//索引词
        string contentpointers;//位置二元组
        public IndexEntry()//构造函数
        {
            term = "";
            contentpointers = "";
        }
        public IndexEntry(string term,string contentpointers)//构造函数
        {
            this.term=term;
            this.contentpointers=contentpointers;
        }
        public string getTerm()// 获取词项
        {
            return term;
        }
        public void addContentField(string r) // 添加content域值
        {
            contentpointers += r;
        }
        public override string ToString()// 获得所有条目的字符串
        {
            string s = "";
            s += "#" + term + "$";
            s += "{" + contentpointers + "}";
            s += "\r\n";
            return s;
        }
        public int CompareTo(object obj)//排序
        {
            int res = 0;
            try
            {
                IndexEntry IE = (IndexEntry)obj;
                res = this.term.CompareTo(IE.term);
            }
            catch (Exception ex)
            {
                throw new Exception("比较异常", ex.InnerException);
            }
            return res;
        }
    }
}
