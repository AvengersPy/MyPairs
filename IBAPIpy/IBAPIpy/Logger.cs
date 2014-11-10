using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// logger
using System.IO;
using System.Security;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MYLogger
{
    public class MyLogger
    {
        private static MyLogger instance;
        private StreamWriter logWriter;

        public static MyLogger Instance
        {
            get
            {
                if (instance == null)
                { 
                    instance = new MyLogger();
                }
                return instance;
            }
        }

        public void Open(string filePath, bool append)
        {
            if (logWriter != null)
            {
                throw new InvalidOperationException("Logger is already open");
            }
            logWriter = new StreamWriter(filePath, append);
            logWriter.AutoFlush = true;
        }

        public void Close()
        {
            if (logWriter != null)
            {
                logWriter.Close();
                logWriter = null;
            }
        }
        public void CreateEntry(string entry)
        {
            if (this.logWriter == null)
                throw new InvalidOperationException("Logger is not open");

            logWriter.WriteLine("{0} - {1}", DateTime.Now.ToString(), entry);
        }
    }
}

