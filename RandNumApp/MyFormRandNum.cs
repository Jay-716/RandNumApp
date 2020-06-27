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
        private readonly List<int> CheatValues = new List<int>();

        public MyForm() {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //忽略线程访问安全机制，允许另一个线程访问其它线程创建的控件而不抛出异常。（影响不大）

            LoadData();

            this.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
            ButtonStart.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
            ButtonStop.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
            labelIndex.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
            label.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        /// <summary>
        /// 读取数据文件
        /// 前两行为范围
        /// 其余为不会被抽到的数
        /// </summary>
        private void LoadData() {
            try {
                if (File.Exists("./data.dat")) {
                    using (StreamReader file = new StreamReader("./data.dat")) {
                        int.TryParse(file.ReadLine(), out MinValue);
                        if (file.EndOfStream) {
                            MinValue = 1;
                            return;
                        }
                        int.TryParse(file.ReadLine(), out MaxValue);
                        if (MaxValue <= MinValue) {
                            MinValue = 1;
                            MaxValue = 50;
                            return;
                        }
                        while (!file.EndOfStream) {
                            string strnum = file.ReadLine();
                            if (int.TryParse(strnum, out int num) && num >= MinValue && num <= MaxValue)
                                CheatValues.Add(num);
                        }
                        textBoxMinValue.Text = MinValue.ToString();
                        textBoxMaxValue.Text = MaxValue.ToString();
                        File.SetAttributes("./data.dat", FileAttributes.Hidden);
                    }
                } else {
                    using (FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        using (StreamWriter sw = new StreamWriter(fs)) {
                            sw.WriteLine('1');
                            sw.WriteLine("50");
                            File.SetAttributes("./data.dat", FileAttributes.Hidden);
                        }
                    }
                }
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message, "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveData() {
            try {
                using (FileStream fs = new FileStream("./data.dat", FileMode.Truncate, FileAccess.ReadWrite)) {
                    using (StreamWriter sw = new StreamWriter(fs)) {
                        sw.WriteLine(MinValue.ToString() + System.Environment.NewLine + MaxValue.ToString());
                        foreach (var i in CheatValues) {
                            sw.WriteLine(i.ToString());
                        }
                        File.SetAttributes("./data.dat", FileAttributes.Hidden);
                    }
                }
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message + "\nFile IO Exception!", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RandNum() {
            int min = MinValue;
            int max = MaxValue;
            int currentNum = 0;
            if (MinValue >= MaxValue) {
                MinValue = 1;
                MaxValue = 50;
                textBoxMinValue.Text = "1";
                textBoxMaxValue.Text = "50";
                CounterIsRunning = false;
                ButtonStart.Enabled = true;
                ButtonStop.Enabled = false;
            }
            Random random = new Random(currentNum);
            while (CounterIsRunning) {
                currentNum = random.Next(min, max + 1);
                label.Text = currentNum.ToString();
                Thread.Sleep(8);
            }
            random = new Random(DateTime.UtcNow.Millisecond + DateTime.Now.Second);
            while (CheatValues.Contains(currentNum)) {
                currentNum = random.Next(min, max + 1);
            }
            label.Text = currentNum.ToString();
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
            labelIndex.Visible = true;
            textBoxMinValue.Visible = true;
            textBoxMaxValue.Visible = true;
            textBoxMinValue.Enabled = true;
            textBoxMaxValue.Enabled = true;
            ToolStripMenuItemHideIndex.Enabled = true;
            ToolStripMenuItemShowIndex.Enabled = false;
        }

        private void ToolStripMenuItemHideIndex_Click(object sender, EventArgs e) {
            labelIndex.Visible = false;
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
