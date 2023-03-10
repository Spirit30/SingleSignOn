using System;
using System.IO;

namespace SingleSignOn.Data.Storage
{
    public class AvatarStorage
    {
        /// <summary>
        /// Avatar bytes in Base 64 format.
        /// </summary>
        public static string LoadOrDefault(string displayName)
        {
            string path = DisplayNameToPath(displayName);

            if (File.Exists(path))
            {
                return LoadWithPath(path);
            }

            return null;
        }

        static string LoadWithPath(string path)
        {
            var bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }

        public static void Save(string displayName, string avatarBase64)
        {
            string path = DisplayNameToPath(displayName);
            byte[] avatarBytes = Convert.FromBase64String(avatarBase64);
            File.WriteAllBytes(path, avatarBytes);
        }

        static string DisplayNameToPath(string displayName)
        {
            Directory.CreateDirectory(Const.AVATAR_STORAGE_DIRECTORY);
            return $"{Const.AVATAR_STORAGE_DIRECTORY}/{displayName}.jpg";
        }
    }
}