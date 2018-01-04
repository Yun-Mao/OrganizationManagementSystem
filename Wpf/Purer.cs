using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wpf
{
    class Purer
    {
        //提取中文
        string Rchinese = @"[\u4e00-\u9fa5]+";
        public void getTitleAnchorContent(string filepath, ref string content)
        {
            //打开文件
            FileStream fs = new FileStream(filepath, FileMode.Open);
            //读取文件内容
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //将文件内容存取到字符串s中
            string s = sr.ReadToEnd();
            //返回content
            content = s;
            fs.Close();
        }

        /// 提取所有中文
        public string findChinese(string s)
        {
            Regex r = new Regex(Rchinese);//正则表达式
            MatchCollection mcchinese = r.Matches(s);
            string schinese = "";
            for (int i = 0; i < mcchinese.Count; i++)
            {
                schinese += mcchinese[i].Value;
            }
            return schinese;
        }
    }
}
