using System;
using System.Diagnostics;

public class Program
{
    public static int Main(string[] Arguments)
    {
        if (Arguments.Length == 0)
        {
            Console.WriteLine(string.Format("Resource dat file not specified!"));
            return -1;
        }
        string resourceDatFile = Arguments[0];
        if (!resourceDatFile.EndsWith(".dat"))
        {
            Console.WriteLine("Resource data file should ends with .dat");
            return -1;
        }
        if (!File.Exists(resourceDatFile))
        {
            Console.WriteLine("Resource data file does not exist");
            return -1;
        }

        using (FileStream Stream = File.Open(resourceDatFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            //byte[] contentBytes = new byte[Stream.Length];
            BinaryReader Reader = new BinaryReader(Stream);
            int fileCount = Reader.ReadInt32();
            if (fileCount <= 0)
            {
                Console.WriteLine("Resoure data file, fileCount is 0");
                return -1;
            }
            int[] arrPos = new int[fileCount];
            for (int i = 0; i < fileCount; i++)
            {
                arrPos[i] = Reader.ReadInt32();
            }
            for (int i = 0; i < fileCount; i++)
            {
                Stream.Seek(arrPos[i], SeekOrigin.Begin);
                int resLen = Reader.ReadInt32();
                //String resName = Reader.ReadString();
                int strLen = Reader.ReadInt32();
                byte[] byteStr = Reader.ReadBytes(strLen);
                String resName = System.Text.Encoding.UTF8.GetString(byteStr);

                byte[] byteContent = Reader.ReadBytes(resLen);
                String dirDatFile = Path.GetDirectoryName(resourceDatFile);
                if (!Directory.Exists(dirDatFile))
                {
                    Directory.CreateDirectory(dirDatFile);
                }
                String targetFile = Path.Combine(dirDatFile, "Export/" + resName);
                try {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    using (FileStream OutStream = File.Open(targetFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        OutStream.Write(byteContent);
                    }
                }
                catch (IOException)
                {

                }
            }
        }

        return 0;
    }
}
