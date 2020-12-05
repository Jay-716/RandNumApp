using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp {
    public partial class MyForm : Form {
        private bool CounterIsRunning = false;
        private int MinValue = 1;
        private int MaxValue = 50;
        private bool HasAvoid = false;
        private bool HasMore = false;
        private string Key = "B404A74B";//初始密钥 - RandNumApp V1.3 的 CRC-32
        private readonly List<int> AvoidValues = new List<int>();
        private readonly List<int> MoreValues = new List<int>();
        private int MoreRate = 30;//抽到MoreValues中的数的概率
        private bool LastMoreResult = false;//表示上一次是否抽到MoreValues中的数
        private string DataFilePath = "";
        private readonly Logger Logger = new Logger();

        public MyForm() {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //忽略线程访问安全机制，允许另一个线程访问其它线程创建的控件而不抛出异常。（影响不大）

            Logger.Write("Start", Logger.LogLevel.Information);
            DataFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/RandNumApp/";
            LoadData();

            Cursor cursor = new Cursor(Properties.Resources.icon.GetHicon());
            this.Cursor = cursor;
            ButtonStart.Cursor = cursor;
            ButtonStop.Cursor = cursor;
            labelRange.Cursor = cursor;
            label.Cursor = cursor;
        }

        /// <summary>
        /// 读取数据文件
        /// </summary>
        private void LoadData() {
            try {
                Logger.Write("Loading data from " + DataFilePath, Logger.LogLevel.Information);
                if (!Directory.Exists(DataFilePath)) {
                    Directory.CreateDirectory(DataFilePath);
                    Logger.Write("Create directory " + DataFilePath, Logger.LogLevel.Information);
                }

                //读取范围
                if (File.Exists(DataFilePath + "range.conf")) {
                    using (StreamReader file = new StreamReader(DataFilePath + "range.conf")) {
                        List<string> fileStrLines = new List<string>();
                        while (!file.EndOfStream) {
                            fileStrLines.Add(file.ReadLine());
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
                            int.TryParse(fileStrLines[0], out MinValue) &&
                            int.TryParse(fileStrLines[1], out MaxValue);
                            if (!convertSuccessful || MinValue >= MaxValue) {
                                MinValue = 1;
                                MaxValue = 50;
                            }
                        } else {
                            Logger.Write("Range file syntax error", Logger.LogLevel.Warning);
                            return;
                        }

                        Logger.Write(string.Format("Read range {0}-{1}", MinValue, MaxValue), Logger.LogLevel.Information);
                    }
                } else {
                    using (FileStream fs = new FileStream(DataFilePath + "range.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        using (StreamWriter sw = new StreamWriter(fs)) {
                            sw.WriteLine("1");
                            sw.WriteLine("50");
                        }
                    }
                    Logger.Write("Create range.conf", Logger.LogLevel.Information);
                }

                //读取密钥
                if (File.Exists(DataFilePath + "key.conf")) {
                    bool regenerate = false;
                    using (StreamReader fs = new StreamReader(DataFilePath + "key.conf")) {
                        if (!fs.EndOfStream) {
                            string key = fs.ReadLine();
                            if (key == Key) {
                                regenerate = true;
                            } else {
                                Key = key;
                            }
                        } else {
                            regenerate = true;
                        }
                    }
                    if (regenerate) {
                        using (StreamWriter sw = new StreamWriter(new FileStream(DataFilePath + "key.conf", FileMode.Truncate, FileAccess.ReadWrite))) {
                            Random keyGenerator = new Random();
                            string key = "";
                            for (int i = 0; i < 8; i++) {
                                key += Convert.ToChar(keyGenerator.Next(49, 123));
                            }
                            sw.WriteLine(key);
                        }
                        Logger.Write("Regenerate key", Logger.LogLevel.Information);
                    }
                    Logger.Write("Load key", Logger.LogLevel.Information);
                } else {
                    using (FileStream fsw = new FileStream(DataFilePath + "key.conf", FileMode.Create, FileAccess.Write)) {
                        string key = "";
                        using (StreamWriter sw = new StreamWriter(fsw)) {
                            Random keyGenerator = new Random();
                            for (int i = 0; i < 8; i++) {
                                key += Convert.ToChar(keyGenerator.Next(48, 123));
                            }
                            sw.WriteLine(key);
                            Key = key;
                        }
                    }
                    Logger.Write("Create key.conf", Logger.LogLevel.Information);
                }

                //读取后门文件 - avoid
                if (File.Exists(DataFilePath + "avoid.conf")) {
                    HasAvoid = true;
                    using (StreamReader fs = new StreamReader(DataFilePath + "avoid.conf")) {
                        List<string> fileStrs = new List<string>();
                        while (!fs.EndOfStream) {
                            fileStrs.Add(fs.ReadLine());
                        }

                        if (fileStrs.Count == 0) {
                            HasAvoid = false;
                            Logger.Write("No avoid values", Logger.LogLevel.Information);
                        } else {
                            string logAvoid = "";
                            foreach (string str in fileStrs) {
                                if (int.TryParse(DataManage.Decrypt(str.ToCharArray(), Key), out int num)) {
                                    AvoidValues.Add(num);
                                    logAvoid += num.ToString() + " ";
                                }
                            }
                            Logger.Write("Load avoid values - " + logAvoid, Logger.LogLevel.Information);
                        }
                    }
                }

                //读取后门文件 - more
                if (File.Exists(DataFilePath + "more.conf")) {
                    HasMore = true;
                    using (StreamReader fs = new StreamReader(DataFilePath + "more.conf")) {
                        List<string> fileStrs = new List<string>();
                        while (!fs.EndOfStream) {
                            fileStrs.Add(fs.ReadLine());
                        }

                        if (fileStrs.Count <= 1) {
                            HasMore = false;
                            Logger.Write("No more values", Logger.LogLevel.Information);
                        } else {
                            string logMore = "";
                            if (int.TryParse(DataManage.Decrypt(fileStrs[0].ToCharArray(), Key), out int rate)) {
                                MoreRate = rate;
                            }
                            for (int i = 1; i < fileStrs.Count; i++) {
                                if (int.TryParse(DataManage.Decrypt(fileStrs[i].ToCharArray(), Key), out int num)) {
                                    MoreValues.Add(num);
                                    logMore += num.ToString();
                                }
                            }
                            Logger.Write("Load more values - " + logMore, Logger.LogLevel.Information);
                        }
                    }
                }

                if (!HasAvoid) {
                    File.Delete(DataFilePath + "avoid.conf");
                }
                if (!HasMore) {
                    File.Delete(DataFilePath + "more.conf");
                }
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message, "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("UnauthorizedAccessException in LoadData()", Logger.LogLevel.Exception);
                Logger.Write(ex.ToString(), Logger.LogLevel.Exception);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "UnHandledException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("UnhandledException in LoadData()", Logger.LogLevel.Exception);
                Logger.Write(ex.ToString(), Logger.LogLevel.Exception);
            }
        }

        private void SaveData() {
            try {
                using (FileStream fs = new FileStream(DataFilePath + "range.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                    using (StreamWriter sw = new StreamWriter(fs)) {
                        sw.WriteLine(MinValue.ToString());
                        sw.WriteLine(MaxValue.ToString());
                        Logger.Write("Save range", Logger.LogLevel.Information);
                    }
                }
                if (HasAvoid) {
                    using (FileStream fs = new FileStream(DataFilePath + "avoid.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        using (StreamWriter sw = new StreamWriter(fs)) {
                            foreach (int i in AvoidValues) {
                                sw.WriteLine(DataManage.Encrypt(i.ToString(), Key));
                            }
                        }
                    }
                    Logger.Write("Save avoid values", Logger.LogLevel.Information);
                }
                if (HasMore) {
                    using (FileStream fs = new FileStream(DataFilePath + "more.conf", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        using (StreamWriter sw = new StreamWriter(fs)) {
                            sw.WriteLine(DataManage.Encrypt(MoreRate.ToString(), Key));
                            for (int i = 0; i < MoreValues.Count; i++) {
                                sw.WriteLine(DataManage.Encrypt(MoreValues[i].ToString(), Key));
                            }
                        }
                    }
                    Logger.Write("Save more values", Logger.LogLevel.Information);
                }
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message + "\nFile IO Exception!", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("UnauthorizedAccessException in SaveData()", Logger.LogLevel.Exception);
                Logger.Write(ex.ToString(), Logger.LogLevel.Exception);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "UnHandledException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("UnhandledException in SaveData()", Logger.LogLevel.Exception);
                Logger.Write(ex.ToString(), Logger.LogLevel.Exception);
            }
        }

        private void RandNum() {
            int min = MinValue;
            int max = MaxValue;
            int result;
            if (MinValue >= MaxValue) {
                MinValue = 1;
                MaxValue = 50;
                textBoxMinValue.Text = "1";
                textBoxMaxValue.Text = "50";
                CounterIsRunning = false;
                ButtonStart.Enabled = true;
                ButtonStop.Enabled = false;
            }

            //在未按下停止按钮之前就确定最终结果
            Random realRandom = new Random(DateTime.UtcNow.Millisecond + DateTime.Now.Second);
            if (HasMore && (!LastMoreResult) && MoreValues.Count != 0 && realRandom.Next(1, 101) <= MoreRate) {
                result = MoreValues[realRandom.Next(0, MoreValues.Count)];
                if (result < min || result > max) {
                    result = realRandom.Next(min, max + 1);
                }
                LastMoreResult = true;
                Logger.Write("Hit more values - " + result.ToString(), Logger.LogLevel.Information);
            } else {
                result = realRandom.Next(min, max + 1);
                LastMoreResult = false;
            }
            while (HasAvoid && AvoidValues.Contains(result)) {
                Logger.Write("Avoid value - " + result.ToString(), Logger.LogLevel.Information);
                result = realRandom.Next(min, max + 1);
            }
            Logger.Write("Last result - " + result.ToString(), Logger.LogLevel.Information);

            Random random = new Random();
            while (CounterIsRunning) {
                label.Text = random.Next(min, max + 1).ToString();
                Thread.Sleep(8);
            }

            label.Text = result.ToString();
        }

        private void TextBox_ValueChanged(object sender, EventArgs e) {
            MinValue = Convert.ToInt32(textBoxMinValue.Text);
            MaxValue = Convert.ToInt32(textBoxMaxValue.Text);
            Logger.Write(string.Format("Range changed to {0}-{1}", MinValue, MaxValue), Logger.LogLevel.Information);
        }

        private void ButtonStart_Click(object sender, EventArgs e) {
            CounterIsRunning = true;
            ButtonStart.Enabled = false;
            ButtonStop.Enabled = true;
            (new Task(RandNum)).Start();
            Logger.Write("Start button clicked", Logger.LogLevel.Information);
        }

        private void ButtonStop_Click(object sender, EventArgs e) {
            CounterIsRunning = false;
            ButtonStart.Enabled = true;
            ButtonStop.Enabled = false;
            Logger.Write("Stop button clicked", Logger.LogLevel.Information);
        }

        private void ToolStripMenuItemShowIndex_Click(object sender, EventArgs e) {
            textBoxMinValue.Text = MinValue.ToString();
            textBoxMaxValue.Text = MaxValue.ToString();
            labelRange.Visible = true;
            textBoxMinValue.Visible = true;
            textBoxMaxValue.Visible = true;
            textBoxMinValue.Enabled = true;
            textBoxMaxValue.Enabled = true;
            ToolStripMenuItemHideIndex.Enabled = true;
            ToolStripMenuItemShowIndex.Enabled = false;
        }

        private void ToolStripMenuItemHideIndex_Click(object sender, EventArgs e) {
            labelRange.Visible = false;
            textBoxMinValue.Visible = false;
            textBoxMaxValue.Visible = false;
            textBoxMinValue.Enabled = false;
            textBoxMaxValue.Enabled = false;
            ToolStripMenuItemHideIndex.Enabled = false;
            ToolStripMenuItemShowIndex.Enabled = true;
            SaveData();
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e) {
            //Application.Exit()方法会关闭窗体，自动调用MyForm_FormClosed
            Application.Exit();
        }

        //限制只能输入数字
        private void TextBoxMinValue_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void ToolStripMenuItemAboutInfo_Click(object sender, EventArgs e) {
            FormInformation form = new FormInformation();
            form.ShowDialog();
        }

        private void MyForm_FormClosed(object sender, FormClosedEventArgs e) {
            CounterIsRunning = false;
            this.Hide();
            Logger.Write("Exit", Logger.LogLevel.Information);
            Logger.Flush();
            SaveData();
            Thread.Sleep(3000);
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

    public class Logger {
        private string LogFilePath = "";
        private readonly List<string> Buffer = new List<string>();

        public Logger() {
            LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/RandNumApp/logs/";
            if (!Directory.Exists(LogFilePath)) {
                Directory.CreateDirectory(LogFilePath);
            }

            LogFilePath += string.Format("Log-{0}.log", DateTime.Now.ToString()).Replace('/', '.').Replace(' ', '.').Replace(':', '.');
        }

        public enum LogLevel {
            Information,
            Warning,
            Exception,
        }

        public void Write(string message, LogLevel logLevel) {
            switch (logLevel) {
                case LogLevel.Information:
                    Buffer.Add(DateTime.Now.ToString() + " Information: " + message);
                    break;
                case LogLevel.Warning:
                    Buffer.Add(DateTime.Now.ToString() + " Warning: " + message);
                    break;
                case LogLevel.Exception:
                    Buffer.Add(DateTime.Now.ToString() + " Exception: " + message);
                    break;
                default:
                    Buffer.Add(DateTime.Now.ToString() + " Exception: LogEnumError");
                    break;
            }
        }

        public void Flush() {
            using (FileStream fs = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    foreach (string str in Buffer) {
                        sw.WriteLine(str);
                    }
                }
            }
            Buffer.Clear();
        }
    }
}
