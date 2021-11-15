using System;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using FileSplitter.ConsoleApp.FileUtilities;

namespace FileSplitter.ConsoleApp
{
    class Program
    {
       

        static void Main(string[] args)
        {
            FileManager fileManager = FileManager.Instance;
            fileManager.SplitFiles();
        }
    }
}
