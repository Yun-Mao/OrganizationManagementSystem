using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wpf
{

    class IndexList
    {
        int Indexedfile = 0;
        List<IndexEntry> MyIndexList = new List<IndexEntry>();
        public void setIndexedFile(int indexedfile)
        {
            Indexedfile = indexedfile;
        }
        public int isContain(string term)
        {
            int res = -1;
            for (int i = 0; i < MyIndexList.Count; i++)
            {
                if (MyIndexList[i].getTerm() == term)
                {
                    res = i;
                    break;
                }
            }
            return res;
        }
        public void addContents(int fileNumber, string s,int line)//暂时提供单分词的
        {
            string RsingleCH = @"[\u4e00-\u9fa5]+";
            Regex r = new Regex(RsingleCH);
            int count = 0;
            MatchCollection mc = r.Matches(s, count);
            for (int i = 0; i < mc.Count; i++)
            {
                  addContent(fileNumber,line, i+1, mc[i].Value);
            }
        }
        public void addContent(int fileNumber, int line,int location, string term)
        {
            //关系三元组(fileNumber,location)
            string R = "(" + fileNumber.ToString()+","+line + "," + location.ToString() + ")";
            int INDEX;
            //已经存在
            if ((INDEX = isContain(term)) != -1)
            {
                MyIndexList[INDEX].addContentField(R);
            }
            //还没有出现过
            else
            {
                MyIndexList.Add(new IndexEntry(term, R));
            }
        }
        public string ToString(int START, int END)
        {
            string outstring = "";
            for (int i = START; i < MyIndexList.Count && i <= END; i++)
            {
                outstring += MyIndexList[i].ToString();
            }
            return outstring;
        }
        public void writeIndexFile(string dirpath)
        {
            FileStream fs = new FileStream(dirpath+"/Index.txt", FileMode.Create);
            string outstring = "@" + Indexedfile.ToString() + "@" + ToString(0, MyIndexList.Count);
            byte[] outbyte = System.Text.Encoding.Default.GetBytes(outstring);
            fs.Write(outbyte, 0, outbyte.Length);;
            fs.Close();
        }
        /// 排序
        public void Sort()
        {
            MyIndexList.Sort();
        }
    }
}
