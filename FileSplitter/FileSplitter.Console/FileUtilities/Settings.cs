using System.Configuration;

namespace FileSplitter.ConsoleApp.FileUtilities
{
    public static class Settings
    {
        public static string GetFilePath()
        {
            return ConfigurationManager.AppSettings["filePath"];
        }

        public static long GetMaxFileLength()
        {
            return long.Parse(ConfigurationManager.AppSettings["maxFileSize"]);
        }
    }
}
