using System;
using System.IO;

namespace Apsiyon.Logger.Helper
{
    public class PathHelper
    {
        public static string PathParser(string path)
        {
            path = path.Replace("[machinename]", Environment.MachineName);
            path = path.Replace("[today]", DateTime.UtcNow.ToString("yyyyMMdd"));
            path = path.Replace("[ticks]", DateTime.UtcNow.Ticks.ToString());

            return path;
        }

        public static string CreatePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var pathArr = Helper.PathHelper.PathParser(Helper.ConfigurationDto.Config.File.Path).Split('/');

            foreach (var path in pathArr)
            {
                currentDirectory = currentDirectory + "\\" + path;

                if (!Directory.Exists(currentDirectory))
                {
                    Directory.CreateDirectory(currentDirectory);
                }
            }

            return currentDirectory;
        }
    }
}