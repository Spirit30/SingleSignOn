using System.IO;

namespace SingleSignOn.Data.Storage
{
    public class CustomDataStorage
    {
        /// <summary>
        /// Custom Data in Json format.
        /// </summary>
        public static string LoadOrDefault(string displayName)
        {
            string path = DisplayNameToPath(displayName);

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }

        public static void Save(string displayName, string json)
        {
            string path = DisplayNameToPath(displayName);
            File.WriteAllText(path, json);
        }

        static string DisplayNameToPath(string displayName)
        {
            Directory.CreateDirectory(Const.CUSTOM_DATA_STORAGE_DIRECTORY);
            return $"{Const.CUSTOM_DATA_STORAGE_DIRECTORY}/{displayName}.json";
        }
    }
}