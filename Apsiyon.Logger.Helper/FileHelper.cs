using Newtonsoft.Json;

namespace Apsiyon.Logger.Helper
{
    public class FileHelper
    {
        public static string CreateLogFile(string path, LogQueueHelper.LogDto log)
        {
            path = path + "\\" + Helper.PathHelper.PathParser(Helper.ConfigurationDto.Config.File.LogName);

            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(log));

            return path;
        }
    }
}