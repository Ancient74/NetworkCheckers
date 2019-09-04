using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetworkCheckers
{
    public class FileUserNameProvider : IUserNameProvider
    {
        private readonly string fileName;
        public FileUserNameProvider(string fileName)
        {
            this.fileName = fileName;
        }
        public string GetName()
        {
            using(FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using(StreamReader sr = new StreamReader(fs))
                {
                    string res = sr.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(res))
                        return "guest";
                    return res;
                }
            }
        }

        public void SaveName(string name)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter sr = new StreamWriter(fs))
                {
                    sr.Write(name);
                }
            }
        }
    }
}
