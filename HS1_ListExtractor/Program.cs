using System;
using System.IO;
using AssetStudio;
using System.Linq;
using System.Reflection;
using System.Text;
using MessagePack;

namespace HS1_ListExtractor
{
    class Program
    {
        static void Main()
        {
            // Declare shit
            string WriterFile = "HoneySelectMods.txt";
            string ErrorFile = "HoneySelectMods-errors.txt";
            string WorkerDir = "worker";

            var Writer = new StreamWriter(WriterFile, false, Encoding.UTF8);
            var ErrorWriter = new StreamWriter(ErrorFile, false, Encoding.UTF8);
            Writer.AutoFlush = true;
            ErrorWriter.AutoFlush = true;
            var CurrentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Writer.WriteLine("File;Path;Source;Zip;Cat;ID;NewID;Name;NewName;Type;Remove?;Found IG?");
                
            foreach (var u3dFile in Directory.EnumerateDirectories(WorkerDir,"*list*", SearchOption.AllDirectories))
            {
                AssetsManager assetsManager = new AssetsManager();
                assetsManager.LoadFolder(Path.GetDirectoryName(u3dFile));
                foreach (var assetFile in assetsManager.assetsFileList)
                {
                    foreach (var asset in assetFile.Objects.Select(x => x.Value))
                    {
                        if (u3dFile.Contains("list"))
                        {
                            switch (asset)
                            {
                                case TextAsset textAsset:
                                    var WorkingString = textAsset.Dump();
                                    Letssplit(WorkingString, assetFile.originalPath, "script", Writer, ErrorWriter);
                                    break;
                                case AssetBundle ab:
                                    break;
                            }
                        }
                    }
                }
                assetsManager.Clear();
            }
            Writer.Close();
            ErrorWriter.Close();
        }

        public static void Letssplit(string input, string fileName, string type, StreamWriter Writer, StreamWriter Error)
        {
            var result = input.Split(new[] { '\r', '\n' });
            string m_Name = "";
            string[] filenameSplit = fileName.Split("\\");



            Console.WriteLine(filenameSplit[1]);

            foreach (var line in result)
            {
                var linetry = "asd";
                if(line.Length > 8 && line != "TextAsset Base" && line != "")
                    linetry = line.Remove(3, line.Length - 1).Remove(0, 1);
                if (line.Contains("m_Name"))
                    m_Name = line.Remove(line.Length - 1, 1).Remove(0,18);
                if (line.Contains("m_Script") && type == "script" && m_Name != "ID TAG" && m_Name != "ID_Tag" && !line.Contains("ID_TA"))
                {
                    var m_Script = line.Remove(line.Length - 1, 1).Remove(0, 20);
                    string[] SplitScript = m_Script.Split("\t");
                    if (SplitScript.Length > 4 && fileName.Contains("list"))
                    {
                        try
                        {
                            string WorkingStringScript;
                            if (SplitScript[3].ToLower().Contains("unity3d")) // Eye Shadows
                                WorkingStringScript = $"{SplitScript[3].Replace("/", "\\")};{fileName};;{filenameSplit[1]};;{SplitScript[0]};;{SplitScript[2]}";
                            else if(SplitScript[2].ToLower().Contains("unity3d")) // Face
                                WorkingStringScript = $"{SplitScript[2].Replace("/", "\\")};{fileName};;{filenameSplit[1]};;{SplitScript[0]};;{SplitScript[2]}";
                            else
                                WorkingStringScript = $"{SplitScript[4].Replace("/", "\\")};{fileName};;{filenameSplit[1]};{SplitScript[1]};{SplitScript[0]};;{SplitScript[2]}";
                            Writer.WriteLine(WorkingStringScript);
                            //File.Delete(fileName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(fileName + " Failed!");
                            Error.WriteLine(fileName + " Failed with an array of " + SplitScript.Length + "!");
                            Console.WriteLine(e);
                            Console.WriteLine(SplitScript.Length);
                        }
                    }
                    else
                    {
                        Console.WriteLine(fileName + " has an array of " + SplitScript.Length + "!");
                        Error.WriteLine(fileName + " has an array of " + SplitScript.Length + "!");
                        foreach (var error in SplitScript)
                        {
                            Error.WriteLine(error);
                        }
                        Error.WriteLine("-----------------------");
                    }
                }
            }
            
        }
    }
}
