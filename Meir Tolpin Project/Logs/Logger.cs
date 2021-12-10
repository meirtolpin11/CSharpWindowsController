using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Meir_Tolpin_Project.Logs
{
    static class Logger
    {
        static string fileName;
        static DateTime date;
        static FileStream fs;

        public static void init(string _fileName)
        {
            fileName = _fileName;
            int i = 0;
            while (true)
            {
                try
                {
                    fs = File.Open(i + fileName, FileMode.Create);
                    fileName = i + fileName;
                    break;
                }
                catch
                {
                    i++;
                }
            }
            
        }

        public static void write(string data)
        {
            date = DateTime.Now;
            Byte[] _data = new UTF8Encoding(true).GetBytes(date.ToString() + " ->> " + data + "\n");
            // Add some information to the file.
            fs.Write(_data, 0, _data.Length);
            fs.Close();
            fs = File.Open(fileName, FileMode.Append);
            
        }


    }
}
