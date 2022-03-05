using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ftp
{
    class FileInfor
    {
        public FileInfor(string line)
        {
            int nameIndex = 8;
            int columnCount = 9;
            string[] s = new string[columnCount];

            int i = 0;
            int flag = 1;
            int p = 0;
            for (p = 0; p < line.Length; p++)
                if (line[p] != ' ')
                    break;

            for (; p < line.Length; p++)
            {
                // 文件名部分无视空格判断
                if (i == nameIndex)
                {
                    s[i] += line[p];
                    continue;
                }

                if (flag == 0)
                {
                    if (line[p] == ' ') continue;

                    flag = 1;
                    i++;
                    s[i] = "" + line[p];
                    continue;
                }

                if (flag == 1)
                {
                    if (line[p] != ' ')
                    {
                        s[i] += line[p];
                        continue;
                    }

                    flag = 0;
                    continue;
                }
            }                      
          this.ModifiedAt = new FileModifiedAt(s[5], s[6], s[7]);
                         
        }
        public FileModifiedAt ModifiedAt { get; }
        public class FileModifiedAt
        {
            private readonly DateTime _time;

            private readonly DateTime _now = DateTime.Now;


            public FileModifiedAt( string s, string s1, string s2)
            { 
                this._time = s2.Contains(":")
                    ? DateTime.Parse("" + this._now.Year + s + ' ' + s1 + ' ' + s2 + ':' + "00")
                    : DateTime.Parse(s + ' ' + s1 + ' ' + s2);
                this._time = this._time.ToLocalTime();
            }

           

            public override String ToString()
            {
                return this._time.Year == this._now.Year
                    ? this._time.ToString("MMMM dd HH:mm")
                    : this._time.ToString("yyyy-M-d dddd");
            }
        }
    }
}
