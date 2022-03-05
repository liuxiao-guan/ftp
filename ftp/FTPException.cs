using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ftp
{
    public class FTPException : Exception
    {
        public readonly int status;

        public int Status
        {
            get => status;
        }

        public FTPException(int status, string line) : base(line)
        {
            this.status = status;
        }
    }
}
