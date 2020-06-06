using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp {
    public partial class MyForm : Form {
        private bool CounterRunning = false;
        private int MaxValue = 50;
        private int MinValue = 1;
        private List<int> CheatValues = new List<int>();
        private string Data;

        public MyForm() {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //忽略线程访问安全机制，允许另一个线程访问其它线程创建的控件而不抛出异常。（影响不大）

            LoadData();
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
                        textBoxMinValue.Text = MinValue.ToString();
                        if (file.EndOfStream) {
                            return;
                        }
                        int.TryParse(file.ReadLine(), out MaxValue);
                        textBoxMaxValue.Text = MaxValue.ToString();
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
            } catch (UnauthorizedAccessException) {
                //为了更像原版，不作处理
            }
        }

        private void SaveData() {
            try {
                Data = MinValue.ToString() + System.Environment.NewLine + MaxValue.ToString();
                using (FileStream fs = new FileStream("./data.dat", FileMode.Truncate, FileAccess.ReadWrite)) {
                    using (StreamWriter sw = new StreamWriter(fs)) {
                        sw.WriteLine(Data);
                        foreach (var i in CheatValues) {
                            sw.WriteLine(i.ToString());
                        }
                        File.SetAttributes("./data.dat", FileAttributes.Hidden);
                    }
                }
            } catch (UnauthorizedAccessException) {
                //为了更像原版，不作处理
            }
        }

        private void RandNum() {
            int min = MinValue;
            int max = MaxValue;
            int num = 0;
            if (MinValue >= MaxValue) {
                MinValue = 1;
                MaxValue = 50;
                textBoxMinValue.Text = "1";
                textBoxMaxValue.Text = "50";
                CounterRunning = false;
                ButtonStart.Enabled = true;
                ButtonStop.Enabled = false;
            }
            Random random = new Random(num);
            while (CounterRunning) {
                num = random.Next(min, max + 1);
                while (CheatValues.Contains(num))
                    num = random.Next(min, max + 1);
                label.Text = num.ToString();
                Thread.Sleep(10);
            }
        }

        private void IndexValueChange(object sender, EventArgs e) {
            try {
                MinValue = Convert.ToInt32(textBoxMinValue.Text);
                MaxValue = Convert.ToInt32(textBoxMaxValue.Text);
            } catch (FormatException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonStart_Click(object sender, EventArgs e) {
            CounterRunning = true;
            ButtonStart.Enabled = false;
            ButtonStop.Enabled = true;
            Task task = new Task(RandNum);
            task.Start();
        }

        private void ButtonStop_Click(object sender, EventArgs e) {
            CounterRunning = false;
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
            CounterRunning = false;
            SaveData();
            Application.Exit();
        }

        private void MyForm_MouseEnter(object sender, EventArgs e) {
            Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void MyForm_MouseLeave(object sender, EventArgs e) {
            Cursor = Cursors.Default;
        }

        private void ButtonStart_MouseEnter(object sender, EventArgs e) {
            Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void ButtonStop_MouseEnter(object sender, EventArgs e) {
            Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void label_MouseEnter(object sender, EventArgs e) {
            Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void labelIndex_MouseEnter(object sender, EventArgs e) {
            Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e) {
            CounterRunning = false;
            SaveData();
            Application.Exit();
        }

        //限制只能输入数字
        private void textBoxMinValue_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e) {
            FormInformation form = new FormInformation();
            form.ShowDialog();
        }
    }
}
