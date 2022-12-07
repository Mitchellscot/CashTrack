using System.IO;

namespace CashTrack.Common
{
    public static class FileUtilities
    {
        public static byte[] GetFileBytesAndDeleteFile(string filePath)
        {
            FileStream fileStream = System.IO.File.OpenRead(filePath);
            byte[] data = new byte[fileStream.Length];
            int buffer = fileStream.Read(data, 0, data.Length);
            if (buffer != fileStream.Length)
                throw new IOException(filePath);
            fileStream.Close();
            System.IO.File.Delete(filePath);
            return data;
        }
        public static byte[] GetFileBytes(string filePath)
        {
            FileStream fileStream = System.IO.File.OpenRead(filePath);
            byte[] data = new byte[fileStream.Length];
            int buffer = fileStream.Read(data, 0, data.Length);
            if (buffer != fileStream.Length)
                throw new IOException(filePath);
            fileStream.Close();
            return data;
        }
    }
}
