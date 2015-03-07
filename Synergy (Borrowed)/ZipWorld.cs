using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
namespace Synergy__Borrowed_.FCU
{
    public class ZipWorld
    {
        public World World;
        public ZipWorld(World world)
        {
            this.World = world;
        }
        public void StartZip()
        {
            Console.WriteLine("Zipping world");
            List<string> list = new List<string>();
            string[] directories = Directory.GetDirectories(Path.Combine(this.World.Location, "Segments"));
            for (int i = 0; i < directories.Length; i++)
            {
                string text = directories[i];
                string fileName = text + ".zip";
                string path = text;
                try
                {
                    using (ZipFile zipFile = new ZipFile())
                    {
                        string[] files = Directory.GetFiles(path);
                        string[] array = files;
                        for (int j = 0; j < array.Length; j++)
                        {
                            string fileName2 = array[j];
                            ZipEntry zipEntry = zipFile.AddFile(fileName2, "/");
                        }
                        zipFile.Save(fileName);
                    }
                }
                catch (Exception arg)
                {
                    Console.Error.WriteLine("exception: " + arg);
                }
                list.Add(text);
            }
            foreach (string text in list)
            {
                Directory.Delete(text, true);
            }
            Console.WriteLine("Done");
        }
    }
}
