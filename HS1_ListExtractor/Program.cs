using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AssetStudio;

namespace HS1_ListExtractor
{
    internal class Program
    {
        private static void Main()
        {
            // Declare shit
            var WriterFile = "HoneySelectMods.txt";
            var ErrorFile = "HoneySelectMods-errors.txt";
            var WorkerDir = "worker";

            var Writer = new StreamWriter(WriterFile, false, Encoding.UTF8);
            var ErrorWriter = new StreamWriter(ErrorFile, false, Encoding.UTF8);
            var testWriter = new StreamWriter("test.txt", false, Encoding.UTF8);
            Writer.AutoFlush = true;
            ErrorWriter.AutoFlush = true;
            testWriter.AutoFlush = true;
            var CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Writer.WriteLine("File;Source;Zip;Cat;ID;NewID;Name;NewName;Type;Remove?;Found IG?");

            foreach (var u3dFile in Directory.EnumerateDirectories(WorkerDir, "*list*", SearchOption.AllDirectories))
            {
                var assetsManager = new AssetsManager();
                assetsManager.LoadFolder(Path.GetDirectoryName(u3dFile));
                foreach (var assetFile in assetsManager.assetsFileList)
                foreach (var asset in assetFile.Objects.Select(x => x.Value))
                    if (u3dFile.Contains("list"))
                        switch (asset)
                        {
                            case TextAsset textAsset:
                                var WorkingString = textAsset.Dump();
                                testWriter.WriteLine(WorkingString);
                                Letssplit(WorkingString, assetFile.originalPath, "script", Writer, ErrorWriter);
                                break;
                            case AssetBundle ab:
                                break;
                        }

                assetsManager.Clear();
            }

            Writer.Close();
            ErrorWriter.Close();
            testWriter.Close();
        }

        public static void Letssplit(string input, string fileName, string type, StreamWriter Writer,
            StreamWriter Error)
        {
            var result = input.Split('\r', '\n');
            var m_Name = "";
            var filenameSplit = fileName.Split("\\");


            Console.WriteLine(filenameSplit[1]);

            foreach (var line in result)
            {
                if (line.Contains("m_Name"))
                    m_Name = line.Remove(line.Length - 1, 1).Remove(0, 18);
                if (line.Contains("m_Script") && type == "script" && m_Name != "ID TAG" && m_Name != "ID_Tag" &&
                    !line.Contains("ID_TA"))
                {
                    var m_Script = line.Remove(line.Length - 1, 1).Remove(0, 20);
                    var SplitScript = m_Script.Split("\t");
                    if (SplitScript.Length > 4 && fileName.Contains("list"))
                    {
                        try
                        {
                            string WorkingStringScript;

                            // Let's make the values more readable
                            var CurrentFile = filenameSplit[1];
                            var CurrentID = SplitScript[0];
                            var CurrentName = SplitScript[2];
                            var CurrentCategory = CurrentID.Substring(0,3);
                            var currentCategoryText = GetCategoryText(CurrentCategory);


                            if (SplitScript[3].ToLower().Contains("unity3d")) // Eye Shadows
                                WorkingStringScript =
                                    $"{SplitScript[3].Replace("/", "\\")};;{CurrentFile};{currentCategoryText};{CurrentID};;{CurrentName}";
                            else if (SplitScript[2].ToLower().Contains("unity3d")) // Face
                                WorkingStringScript =
                                    $"{SplitScript[2].Replace("/", "\\")};;{CurrentFile};{currentCategoryText};{CurrentID};;{CurrentName}";
                            else
                                WorkingStringScript =
                                    $"{SplitScript[4].Replace("/", "\\")};;{CurrentFile};{currentCategoryText};{CurrentID};;{CurrentName}";
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
                        foreach (var error in SplitScript) Error.WriteLine(error);
                        Error.WriteLine("-----------------------");
                    }
                }
            }
        }

        private static string GetCategoryText(string catNum)
        {
            // Category translations from HSResilveMoreSlotID
            string category;
            switch (catNum)
            {
                case "100":
                    return "Head Type (M)";
                case "101":
                    return "Hair (M)";
                case "102":
                    return "Normal Top (M)";
                case "103":
                    return "Shoes (M)";
                case "150":
                    return "Face Type (M)";
                case "151":
                    return "Eyebrow (M)";
                case "152":
                    return "Eye (M)";
                case "153":
                    return "Beard";
                case "154":
                    return "Tattoo Face (M)";
                case "155":
                    return "Face Wrinkle (M)";
                case "170":
                    return "Body Type (M)";
                case "171":
                    return "Tattoo Body (M)";
                case "172":
                    return "Body Detail (M)";
                case "200":
                    return "Head Type (F)";
                case "201":
                    return "Hair Back\\Sets";
                case "202":
                    return "Hair Front";
                case "203":
                    return "Hair Side";
                case "204":
                    return "Hair Optional";
                case "205":
                    return "Normal Top (F)";
                case "206":
                    return "Normal Bottom";
                case "207":
                    return "Bra";
                case "208":
                    return "Underwear";
                case "209":
                    return "Swimsuit";
                case "210":
                    return "Swimsuit Top";
                case "211":
                    return "Swimsuit Bottom";
                case "212":
                    return "Gloves";
                case "213":
                    return "Pantyhose";
                case "214":
                    return "Socks";
                case "215":
                    return "Shoes (F)";
                case "250":
                    return "Face Type (F)";
                case "251":
                    return "Eyebrow (F)";
                case "252":
                    return "Eyelash";
                case "253":
                    return "Eye Shadow";
                case "254":
                    return "Eye (F)";
                case "255":
                    return "Eye Highlight";
                case "256":
                    return "Cheek Color";
                case "257":
                    return "Lip Type";
                case "258":
                    return "Tattoo Face (F)";
                case "259":
                    return "Mole";
                case "260":
                    return "Face Wrinkle (F)";
                case "270":
                    return "Body Type (F)";
                case "271":
                    return "Tattoo Body (F)";
                case "272":
                    return "Nipple";
                case "273":
                    return "Pubes";
                case "274":
                    return "Tan (F)";
                case "275":
                    return "Body Detail (F)";
                case "350":
                    return "Head Accessory";
                case "351":
                    return "Ear Accessory";
                case "352":
                    return "Glasses Accessory";
                case "353":
                    return "Face Accessory";
                case "354":
                    return "Neck Accessory";
                case "355":
                    return "Shoulder Accessory";
                case "356":
                    return "Chest Accessory";
                case "357":
                    return "Waist Accessory";
                case "358":
                    return "Back Accessory";
                case "359":
                    return "Arm Accessory";
                case "360":
                    return "Hand Accessory";
                case "361":
                    return "Leg Accessory";
                default:
                    return catNum;
            }
            return string.Empty;
        }
    }
}