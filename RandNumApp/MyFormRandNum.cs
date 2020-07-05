using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp {
    public partial class MyForm : Form {
        private bool CounterIsRunning = false;
        private int MaxValue = 50;
        private int MinValue = 1;
        private readonly List<int> AvoidValues = new List<int>();
        private readonly List<int> MoreValues = new List<int>();
        private int MoreRate = 20;
        private bool LastMoreResult = false;

        public MyForm() {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //忽略线程访问安全机制，允许另一个线程访问其它线程创建的控件而不抛出异常。（影响不大）

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
        /// 文件中必须有"Range:", "Avoid:", "More:"三个标识符，顺序不限
        /// "Range:"后跟两个数表示范围
        /// "Avoid:"后跟任意个数的数表示不会被抽到的数
        /// "More:"后跟概率(1~100)与要抽到更多的数
        /// </summary>
        private void LoadData() {
            try {
                if (File.Exists("./Data.dat") && File.OpenRead("./Data.dat").Length <= 256) {
                    using (StreamReader file = new StreamReader("./Data.dat")) {
                        List<string> fileStrLines = new List<string>();
                        while (!file.EndOfStream) {
                            fileStrLines.Add(file.ReadLine());
                        }

                        int indexOfRange = fileStrLines.IndexOf("Range:");
                        int indexOfAvoid = fileStrLines.IndexOf("Avoid:");
                        int indexOfMore = fileStrLines.IndexOf("More:");

                        //判断是否存在三个标识符
                        if (indexOfRange == -1 || indexOfAvoid == -1 || indexOfMore == -1) {
                            MessageBox.Show("Data.dat Syntax Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        //读取Range
                        if (!(int.TryParse(fileStrLines[indexOfRange + 1], out MinValue) && int.TryParse(fileStrLines[indexOfRange + 2], out MaxValue))) {
                            MinValue = 1;
                            MaxValue = 50;
                            MessageBox.Show("Data.dat Syntax Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (MinValue >= MaxValue) {
                            MinValue = 1;
                            MaxValue = 50;
                            MessageBox.Show("Data.dat Value Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        //读取Avoid
                        for (int i = indexOfAvoid + 1; i != indexOfRange && i != indexOfMore && i < fileStrLines.Count; i++) {
                            bool convertIsSuccessful = int.TryParse(fileStrLines[i], out int num);
                            if (num >= MinValue && num <= MaxValue && convertIsSuccessful) {
                                AvoidValues.Add(num);
                            }
                        }
                        //读取More
                        //读取概率
                        if (indexOfMore + 1 != indexOfRange && indexOfMore + 1 != indexOfAvoid && int.TryParse(fileStrLines[indexOfMore + 1], out int rate) && rate > 0 && rate <= 100) {
                            MoreRate = rate;
                        } else {
                            return;
                        }
                        for (int i = indexOfMore + 2; i != indexOfRange && i != indexOfAvoid && i < fileStrLines.Count; i++) {
                            bool convertIsSuccessful = int.TryParse(fileStrLines[i], out int num);
                            if (num >= MinValue && num <= MaxValue && convertIsSuccessful) {
                                MoreValues.Add(num);
                            }
                        }

                        File.SetAttributes("./Data.dat", FileAttributes.Hidden);
                    }
                } else {
                    using (FileStream fs = new FileStream("./Data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        using (StreamWriter sw = new StreamWriter(fs)) {
                            sw.WriteLine("Range:");
                            sw.WriteLine('1');
                            sw.WriteLine("50");
                            sw.WriteLine("Avoid:");
                            sw.WriteLine("More:");
                            File.SetAttributes("./Data.dat", FileAttributes.Hidden);
                        }
                    }
                }
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message, "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveData() {
            try {
                using (FileStream fs = new FileStream("./Data.dat", FileMode.Truncate, FileAccess.ReadWrite)) {
                    using (StreamWriter sw = new StreamWriter(fs)) {
                        sw.WriteLine("Range:");
                        sw.WriteLine(MinValue.ToString());
                        sw.WriteLine(MaxValue.ToString());
                        sw.WriteLine("Avoid:");
                        foreach (var i in AvoidValues) {
                            sw.WriteLine(i.ToString());
                        }
                        sw.WriteLine("More:");
                        sw.WriteLine(MoreRate.ToString());
                        foreach (var i in MoreValues) {
                            sw.WriteLine(i.ToString());
                        }
                        File.SetAttributes("./Data.dat", FileAttributes.Hidden);
                    }
                }
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message + "\nFile IO Exception!", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RandNum() {
            int min = MinValue;
            int max = MaxValue;
            int currentNum;
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
            if ((!LastMoreResult) && MoreValues.Count != 0 && realRandom.Next(1, 101) <= MoreRate) {
                result = MoreValues[realRandom.Next(0, MoreValues.Count)];
                if (result < min || result > max) {
                    result = realRandom.Next(min, max + 1);
                }
                LastMoreResult = true;
            } else {
                result = realRandom.Next(min, max + 1);
                LastMoreResult = false;
            }
            while (AvoidValues.Contains(result)) {
                result = realRandom.Next(min, max + 1);
            }

            Random random = new Random();
            while (CounterIsRunning) {
                currentNum = random.Next(min, max + 1);
                label.Text = currentNum.ToString();
                Thread.Sleep(8);
            }

            label.Text = result.ToString();
        }

        private void IndexValueChange(object sender, EventArgs e) {
            MinValue = Convert.ToInt32(textBoxMinValue.Text);
            MaxValue = Convert.ToInt32(textBoxMaxValue.Text);
        }

        private void ButtonStart_Click(object sender, EventArgs e) {
            CounterIsRunning = true;
            ButtonStart.Enabled = false;
            ButtonStop.Enabled = true;
            Task task = new Task(RandNum);
            task.Start();
        }

        private void ButtonStop_Click(object sender, EventArgs e) {
            CounterIsRunning = false;
            ButtonStart.Enabled = true;
            ButtonStop.Enabled = false;
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
            CounterIsRunning = false;
            SaveData();
            Application.Exit();
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e) {
            CounterIsRunning = false;
            SaveData();
            Application.Exit();
        }

        //限制只能输入数字
        private void textBoxMinValue_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void ToolStripMenuItemAboutInfo_Click(object sender, EventArgs e) {
            FormInformation form = new FormInformation();
            form.ShowDialog();
        }
    }
}
