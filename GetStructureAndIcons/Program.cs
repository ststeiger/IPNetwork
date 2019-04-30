
using System.Windows.Media.Imaging;


namespace GetStructureAndIcons
{


    public class Ico2Png
    {

        public static byte[] GetIcon(System.IO.Stream iconStream)
        {
            byte[] iconBytes = null;

            IconBitmapDecoder decoder = new IconBitmapDecoder(
            iconStream,
            BitmapCreateOptions.PreservePixelFormat,
            BitmapCacheOption.None);

            // loop through images inside the file
            foreach (BitmapFrame frame in decoder.Frames)
            {
                // save file as PNG
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(frame);
                int size = frame.PixelHeight;

                // haven't tested the next lines - include them for bitdepth
                // See RenniePet's answer for details
                // var depth = frame.Thumbnail.Format.BitsPerPixel;
                // var path = outPath + fileName + size + depth +".png";

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    encoder.Save(ms);

                    iconBytes = ms.ToArray();
                }

            }

            return iconBytes;
        }


        public static byte[] GetIcon(string filepath)
        {
            byte[] iconBytes = null;

            using (System.IO.Stream iconStream = new System.IO.FileStream(filepath, System.IO.FileMode.Open))
            {
                iconBytes = GetIcon(iconStream);
            }

            return iconBytes;
        }


        public void Run(string inPath, string outPath)
        {
            if (!System.IO.Directory.Exists(inPath))
            {
                throw new System.Exception("In Path does not exist");
            }

            if (!System.IO.Directory.Exists(outPath))
            {
                System.IO.Directory.CreateDirectory(outPath);
            }


            string[] files = System.IO.Directory.GetFiles(inPath, "*.ico");
            foreach (string filepath in files)
            {
                System.IO.Stream iconStream = new System.IO.FileStream(filepath, System.IO.FileMode.Open);
                IconBitmapDecoder decoder = new IconBitmapDecoder(
                    iconStream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.None);

                string fileName = System.IO.Path.GetFileName(filepath);

                // loop through images inside the file
                foreach (BitmapFrame frame in decoder.Frames)
                {
                    // save file as PNG
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(frame);
                    int size = frame.PixelHeight;

                    // haven't tested the next lines - include them for bitdepth
                    // See RenniePet's answer for details
                    // var depth = frame.Thumbnail.Format.BitsPerPixel;
                    // var path = outPath + fileName + size + depth +".png";

                    string path = outPath + fileName + size + ".png";
                    using (System.IO.Stream saveStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                    {
                        encoder.Save(saveStream);
                    }
                }
            }
        }
    }


    public class FSI
    {
        public string FullName;
        public bool IsDirectory;
        public byte[] Icon;


        public FSI(string fullName, bool isDirectory, byte[] iconBytes)
        {
            this.FullName = fullName;
            this.IsDirectory = isDirectory;
            this.Icon = iconBytes;
        }


        public FSI(string fullName, byte[] iconBytes)
            : this(fullName, false, iconBytes)
        { }

        public FSI(string fullName, bool isDirectory)
            :this(fullName, isDirectory, null)
        { }

        public FSI()
        { }


        

    }



    class Program
    {

        static void Main(string[] args)
        {
            System.Collections.Generic.List<FSI> ls = new System.Collections.Generic.List<FSI>();

            string path = @"C:\Program Files\dotnet";
            // string[] entries = System.IO.Directory.GetFileSystemEntries(path);
            System.IO.FileSystemInfo[] entries = new System.IO.DirectoryInfo(path).GetFileSystemInfos("*.*", System.IO.SearchOption.AllDirectories);

            foreach (System.IO.FileSystemInfo fsi in entries)
            {
                if ((fsi.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                {
                    ls.Add(new FSI(fsi.FullName, true));
                    continue;
                }
                    

                System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fsi.FullName);

                //using (System.IO.FileStream fs = System.IO.File.OpenWrite(""))
                //{
                //    icon.Save(fs);
                //}
                using (System.IO.Stream fs = new System.IO.MemoryStream())
                {
                    icon.Save(fs);
                    fs.Position = 0;
                    byte[] iconBytes = Ico2Png.GetIcon(fs);

                    // System.IO.File.WriteAllBytes(@"d:\test.png", iconBytes);
                    ls.Add(new FSI(fsi.FullName, false, iconBytes));
                    continue;
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ls, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"d:\dotNetStructure.json", json, System.Text.Encoding.UTF8);
            System.Console.WriteLine(ls);
        } // End Sub Main 


    }


}
