using System;
using System.Collections.Generic;
using System.IO;

namespace RandNumApp.Configurator {
    class Program {
        static void Main(string[] args) {
            Initialize();
            Help();
            ConsoleKeyInfo keyInfo;
            Configurator configurator = new Configurator();
            do {
                Console.Write(">>> ");
                keyInfo = Console.ReadKey();
                Console.ReadLine();
                switch (keyInfo.KeyChar) {
                    case 'k':
                        configurator.GenerateKey();
                        break;
                    case 'a':
                        configurator.AddAvoid();
                        break;
                    case 'm':
                        configurator.AddMore();
                        break;
                    case 'r':
                        configurator.ChangeRate();
                        break;
                    case 'w':
                        configurator.WriteConfiguration();
                        break;
                    case 'q':
                        Console.WriteLine("Quit");
                        break;
                    case 'h':
                        Help();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } while (keyInfo.KeyChar != 'q');
        }

        private static void Initialize() {
            string DataFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/RandNumApp/";
            Console.WriteLine("Start initialization");
            if (Directory.Exists(DataFilePath)) {
                Console.WriteLine("Find " + DataFilePath);
            } else {
                Console.WriteLine("Can not find " + DataFilePath);
                Directory.CreateDirectory(DataFilePath);
                Console.WriteLine("Create " + DataFilePath);
            }

            if (File.Exists(DataFilePath + "key.conf")) {
                Console.WriteLine("Find " + DataFilePath + "key.conf");
            } else {
                Console.WriteLine("Can not find " + DataFilePath + "key.conf");
                File.Create(DataFilePath + "key.conf");
                Console.WriteLine("Create " + DataFilePath + "key.conf");
                using (FileStream fsw = new FileStream(DataFilePath + "key.conf", FileMode.Create, FileAccess.Write)) {
                    using (StreamWriter sw = new StreamWriter(fsw)) {
                        sw.WriteLine("B404A74B");
                    }
                }
                Console.WriteLine("Generate original key");
            }
            if (File.Exists(DataFilePath + "range.conf")) {
                Console.WriteLine("Find " + DataFilePath + "range.conf");
            } else {
                Console.WriteLine("Can not find " + DataFilePath + "range.conf");
                File.Create(DataFilePath + "range.conf");
                Console.WriteLine("Create" + DataFilePath + "range.conf");
                using (FileStream fsw = new FileStream(DataFilePath + "range.conf", FileMode.Create, FileAccess.Write)) {
                    using (StreamWriter sw = new StreamWriter(fsw)) {
                        sw.WriteLine("1");
                        sw.WriteLine("50");
                    }
                }
                Console.WriteLine("Generate original range");
            }
        }

        private static void Help() {
            Console.WriteLine("\n\n\nRandNumApp.Configurator");
            Console.WriteLine("k : Generate a new key");
            Console.WriteLine("a : Add a new avoid value");
            Console.WriteLine("m : Add a new more value");
            Console.WriteLine("r : Change the more rate");
            Console.WriteLine("w : Write configuration");
            Console.WriteLine("q : Quit the configurator");
            Console.WriteLine("h : Display this help information");
        }
    }

    class Configurator {
        private readonly string DataFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/RandNumApp/";

        private bool RangeChange = false;
        private int NewMinValue = 1;
        private int NewMaxValue = 50;
        private bool KeyChange = false;
        private string NewKey = "";
        private bool AvoidChange = false;
        private List<int> NewAvoidValues = new List<int>();
        private bool MoreChange = false;
        private List<int> NewMoreValues = new List<int>();
        private bool MoreRateChange = false;
        private int NewMoreRate = 30;

        public Configurator() {
            FileStream KeyFileStream = new FileStream(DataFilePath + "key.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileStream RangeFileStream = new FileStream(DataFilePath + "range.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileStream AvoidFileStream = new FileStream(DataFilePath + "avoid.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileStream MoreFileStream = new FileStream(DataFilePath + "more.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            string currentKey = "";
            using (StreamReader currentKeyReader = new StreamReader(KeyFileStream)) {
                currentKey = currentKeyReader.ReadLine();
                NewKey = currentKey;
            }

            using (StreamReader currentRangeReader = new StreamReader(RangeFileStream)) {
                List<string> fileStrLines = new List<string>();
                while (!currentRangeReader.EndOfStream) {
                    fileStrLines.Add(currentRangeReader.ReadLine());
                }

                //判断范围文件为空
                if (fileStrLines.Count == 0) {
                    using (FileStream fs = new FileStream(DataFilePath + "range.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        using (StreamWriter sw = new StreamWriter(fs)) {
                            sw.WriteLine("1");
                            sw.WriteLine("50");
                        }
                    }
                } else if (fileStrLines.Count == 2) {
                    bool convertSuccessful =
                    int.TryParse(fileStrLines[0], out NewMinValue) &&
                    int.TryParse(fileStrLines[1], out NewMaxValue);
                    if (!convertSuccessful || NewMinValue >= NewMaxValue) {
                        NewMinValue = 1;
                        NewMaxValue = 50;
                        RangeChange = true;
                    }
                } else {
                    RangeChange = true;
                }
            }

            using (StreamReader currentAvoidReader = new StreamReader(AvoidFileStream)) {
                List<string> fileStrs = new List<string>();
                while (!currentAvoidReader.EndOfStream) {
                    fileStrs.Add(currentAvoidReader.ReadLine());
                }
                foreach (string str in fileStrs) {
                    if (int.TryParse(DataManage.Decrypt(str.ToCharArray(), currentKey), out int num)) {
                        NewAvoidValues.Add(num);
                    }
                }
            }

            using (StreamReader currentMoreReader = new StreamReader(MoreFileStream)) {
                List<string> fileStrs = new List<string>();
                while (!currentMoreReader.EndOfStream) {
                    fileStrs.Add(currentMoreReader.ReadLine());
                }

                if (fileStrs.Count <= 1) {
                    MoreChange = false;
                } else {
                    if (int.TryParse(DataManage.Decrypt(fileStrs[0].ToCharArray(), currentKey), out int rate)) {
                        NewMoreRate = rate;
                    }
                    for (int i = 1; i < fileStrs.Count; i++) {
                        if (int.TryParse(DataManage.Decrypt(fileStrs[i].ToCharArray(), currentKey), out int num)) {
                            NewMoreValues.Add(num);
                        }
                    }
                }
            }
        }

        ~Configurator() {
            if (NewAvoidValues.Count == 0) {
                File.Delete(DataFilePath + "avoid.conf");
            }
            if (NewMoreValues.Count == 0) {
                File.Delete(DataFilePath + "more.conf");
            }
        }

        public void GenerateKey() {
            Random keyGenerator = new Random();
            string _key = "";
            for (int i = 0; i < 8; i++) {
                _key += Convert.ToChar(keyGenerator.Next(49, 123));
            }
            NewKey = _key;
            KeyChange = true;
        }

        public void AddAvoid() {
            string s = Console.ReadLine();
            if (int.TryParse(s, out int num)) {
                NewAvoidValues.Add(num);
                AvoidChange = true;
            } else {
                Console.WriteLine("Invalid value");
            }
        }

        public void AddMore() {
            string s = Console.ReadLine();
            if (int.TryParse(s, out int num)) {
                NewMoreValues.Add(num);
                MoreChange = true;
            } else {
                Console.WriteLine("Invalid value");
            }
        }

        public void ChangeRate() {
            string s = Console.ReadLine();
            if (int.TryParse(s, out int num)) {
                NewMoreRate = num;
                MoreRateChange = true;
            } else {
                Console.WriteLine("Invalid value");
            }
        }

        public void WriteConfiguration() {
            FileStream KeyFileStream = new FileStream(DataFilePath + "key.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileStream RangeFileStream = new FileStream(DataFilePath + "range.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileStream AvoidFileStream = new FileStream(DataFilePath + "avoid.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileStream MoreFileStream = new FileStream(DataFilePath + "more.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter Key = new StreamWriter(KeyFileStream);
            StreamWriter Range = new StreamWriter(RangeFileStream);
            StreamWriter Avoid = new StreamWriter(AvoidFileStream);
            StreamWriter More = new StreamWriter(MoreFileStream);

            if (RangeChange) {
                Range.WriteLine(DataManage.Encrypt(NewMinValue.ToString(), NewKey));
                Range.WriteLine(DataManage.Encrypt(NewMaxValue.ToString(), NewKey));
                RangeChange = false;
                Range.Close();
            }
            if (AvoidChange) {
                foreach (int i in NewAvoidValues) {
                    Avoid.WriteLine(DataManage.Encrypt(i.ToString(), NewKey));
                }
                AvoidChange = false;
                Avoid.Close();
            }
            if (MoreChange) {
                More.WriteLine(DataManage.Encrypt(NewMoreRate.ToString(), NewKey));
                foreach (int i in NewMoreValues) {
                    More.WriteLine(DataManage.Encrypt(i.ToString(), NewKey));
                }
                MoreChange = false;
                More.Close();
            }
            if (KeyChange) {
                Key.WriteLine(NewKey);
                KeyChange = false;
                Key.Close();
            }
        }
    }

    public class DataManage {
        public static char[] Encrypt(string content, string secretKey) {
            char[] data = content.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++) {
                data[i] ^= key[i % key.Length];
            }

            return data;
        }

        public static string Decrypt(char[] data, string secretKey) {
            char[] key = secretKey.ToCharArray();

            for (int i = 0; i < data.Length; i++) {
                data[i] ^= key[i % key.Length];
            }

            return new string(data);
        }
    }
}
