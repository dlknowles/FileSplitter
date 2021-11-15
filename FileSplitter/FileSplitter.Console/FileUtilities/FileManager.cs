using System;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

namespace FileSplitter.ConsoleApp.FileUtilities
{
    public class FileManager
    {
        private static FileManager instance;

        /// <summary>
        /// Instance of the FileManager. There can be only one.
        /// </summary>
        public static FileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// The file path to use as the working directory.
        /// Defaults to filePath setting in the configuration file.
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// The maximum file length (in bytes) to make each file when executing the SplitFiles method.
        /// Defaults to the maxFileLength setting in the configuration file.
        /// </summary>
        public long MaxFileLength { get; set; }

        /// <summary>
        /// Does stuff to files.
        /// ReadFiles()
        /// SplitFiles()
        /// </summary>
        public FileManager()
        {
            FilePath = Settings.GetFilePath();
            MaxFileLength = Settings.GetMaxFileLength();
        }

        /// <summary>
        /// Lists the files currently in the directory defined in the FilePath property.
        /// Displays file name, file length (in bytes).
        /// The line will start with "!" if the file exceeds the maximum file size.
        /// </summary>
        public void ReadFiles()
        {
            Console.WriteLine("Reading directory {0}", FilePath);
            FileInfo[] files = new DirectoryInfo(FilePath).GetFiles();

            foreach (var file in files)
            {
                var sizeAlert = (file.Length > MaxFileLength) ? "! " : "";

                Console.WriteLine("{0}{1}: {2}", sizeAlert, file.Name, file.Length);
            }
        }


        /// <summary>
        /// Iterates over each file in the working directory (set by FilePath property).
        /// If a file size (in bytes) is greater than the maximum file size (set by MaxFileLength property), 
        /// the SplitFiles() method will break the file into multiple files that are less than the maximum file size.
        /// </summary>
        /// <param name="includeHeaders">Do the files contain headers? If true, includes the first line in each new file. 
        /// Otherwise, only leaves the first line in the original file.</param>
        public void SplitFiles(bool includeHeaders = false)
        {
            Console.WriteLine("Splitting files in directory {0}", FilePath);
            Console.WriteLine((includeHeaders) ? "Treating first line as header" : "No headers");
            FileInfo[] files = new DirectoryInfo(FilePath).GetFiles();

            foreach (var file in files)
            {
                if (file.Length > MaxFileLength)
                {
                    SplitFile(file, includeHeaders, true);
                }                
            }
        }

        private void SplitFile(FileInfo file, bool includeHeaders, bool overwrite)
        {
            var originalFileLength = file.Length;
            var counter = 1;

            while (originalFileLength > MaxFileLength)
            {
                Console.WriteLine("Splitting {0}: {1}", file.Name, file.Length);

                // 1. create a new file with the last <MaxFileSize> bytes in the file. Append "-<#>" to the end of the file (e.g. "-01", "-02", etc.)
                var fileBytes = File.ReadAllBytes(file.FullName);
                var newFileName = GetNewFileName(file.FullName, counter++);

                while (!overwrite && File.Exists(newFileName))
                {
                    newFileName = GetNewFileName(file.FullName, counter++);
                }

                var newFile = File.Create(newFileName);
                Console.WriteLine("Creating new file: {0}", newFile.Name);

                newFile.Write(fileBytes);

                // 2. remove the last <MaxFileSize> bytes in the file.

                // 3. reevaluate the file length to see if it's small enough. If not, do the process again.
                originalFileLength = 0; // file.Length;
            }

        }

        /// <summary>
        /// Returns a new file name with the <paramref name="fileNumber"/> appended to the end.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileNumber"></param>
        /// <returns></returns>
        private string GetNewFileName(string fileName, int fileNumber)
        {
            var extension = fileName.Substring(fileName.LastIndexOf("."));
            var fileNameEnd = ((fileNumber < 10) ? "0" : "") + fileNumber.ToString();
            var val = fileName.Substring(0, fileName.Length - 4) + "-" + fileNameEnd + extension;

            return val;
        }

    }
}
