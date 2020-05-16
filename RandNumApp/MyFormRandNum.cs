using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace WindowsFormsApp {
    public partial class MyForm : Form {
        public bool CounterRunning { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public List<int> CheatValues { get; set; }
        public string Data { get; set; }

        public MyForm() {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //忽略线程访问安全机制，允许另一个线程访问其它线程创建的控件而不抛出异常。（影响不大）

            this.CounterRunning = false;
            this.MaxValue = 50;
            this.MinValue = 1;
            this.CheatValues = new List<int>();
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
                    StreamReader file = new StreamReader("./data.dat");
                    MinValue = Convert.ToInt32(file.ReadLine());
                    if (file.EndOfStream) throw new FormatException();//当data.dat只有一行时，抛出格式错误（与Convert的异常处理混用）
                    MaxValue = Convert.ToInt32(file.ReadLine());
                    while (!file.EndOfStream) {
                        string num = file.ReadLine();
                        if(!num.Equals("\n"))
                            CheatValues.Add(Convert.ToInt32(num));
                    }
                    file.Close();
                    File.SetAttributes("./data.dat", FileAttributes.Hidden);
                } else {
                    FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("1\n50");
                    sw.Close();
                    File.SetAttributes("./data.dat", FileAttributes.Hidden);
                }
            } catch (UnauthorizedAccessException) {
            } catch (FormatException) {
                this.MaxValue = 50;
                this.MinValue = 1;
                this.textBoxMinValue.Text = MinValue.ToString();
                this.textBoxMaxValue.Text = MaxValue.ToString();
            }
        }

        private void ButtonStart_Click(object sender, EventArgs e) {
            CounterRunning = true;
            this.ButtonStart.Enabled = false;
            this.ButtonStop.Enabled = true;
            Task task = new Task(RandNum);
            task.Start();
        }

        private void ButtonStop_Click(object sender, EventArgs e) {
            CounterRunning = false;
            this.ButtonStart.Enabled = true;
            this.ButtonStop.Enabled = false;
        }

        private void RandNum() {
            int min = MinValue;
            int max = MaxValue;
            int num = 0;
            Random random = new Random(num);
            while (CounterRunning) {
                num = random.Next(min, max + 1);
                while (CheatValues.Contains(num))
                    num = random.Next(min, max + 1);
                this.label.Text = num.ToString();
                System.Threading.Thread.Sleep(10);
            }
        }

        private void ToolStripMenuItemShowIndex_Click(object sender, EventArgs e) {
            this.textBoxMinValue.Text = MinValue.ToString();
            this.textBoxMaxValue.Text = MaxValue.ToString();
            this.labelIndex.Visible = true;
            this.textBoxMinValue.Visible = true;
            this.textBoxMaxValue.Visible = true;
            this.textBoxMinValue.Enabled = true;
            this.textBoxMaxValue.Enabled = true;
            this.textBoxMaxValue.Leave += IndexValueChange;
            this.ToolStripMenuItemHideIndex.Enabled = true;
            this.ToolStripMenuItemShowIndex.Enabled = false;
        }

        private void ToolStripMenuItemHideIndex_Click(object sender, EventArgs e) {
            this.labelIndex.Visible = false;
            this.textBoxMinValue.Visible = false;
            this.textBoxMaxValue.Visible = false;
            this.textBoxMinValue.Enabled = false;
            this.textBoxMaxValue.Enabled = false;
            this.ToolStripMenuItemHideIndex.Enabled = false;
            this.ToolStripMenuItemShowIndex.Enabled = true;
        }

        private void IndexValueChange(object sender, EventArgs e) {
            try {
                this.MaxValue = Convert.ToInt32(this.textBoxMaxValue.Text);
                this.MinValue = Convert.ToInt32(this.textBoxMinValue.Text);
            } catch (FormatException ex) {
                this.MaxValue = 50;
                this.MinValue = 1;
                this.textBoxMinValue.Text = MinValue.ToString();
                this.textBoxMaxValue.Text = MaxValue.ToString();
                MessageBox.Show("请输入合法的数字。\n" + ex.ToString());
            }
            SaveData();
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e) {
            SaveData();
            Application.Exit();
        }

        private void SaveData() {
            try {
                this.Data = MinValue.ToString() + '\n' + MaxValue.ToString() + '\n';
                foreach (var i in CheatValues) {
                    if (!(CheatValues.IndexOf(i) == CheatValues.Count - 1))//避免在文件末尾加入换行符造成读取时抛出FormatException
                        this.Data += i.ToString() + '\n';
                    else
                        this.Data += i.ToString();
                }
                FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Data);
                sw.Close();
                File.SetAttributes("./data.dat", FileAttributes.Hidden);
            } catch (UnauthorizedAccessException) {
            } catch (FormatException ex) {
                this.MaxValue = 50;
                this.MinValue = 1;
                this.textBoxMinValue.Text = MinValue.ToString();
                this.textBoxMaxValue.Text = MaxValue.ToString();
                MessageBox.Show("请输入合法的数字。\n" + ex.ToString());
            }
        }

        private void MyForm_MouseEnter(object sender, EventArgs e) {
            this.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void MyForm_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
        }

        private void ButtonStart_MouseEnter(object sender, EventArgs e) {
            this.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void ButtonStop_MouseEnter(object sender, EventArgs e) {
            this.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void label_MouseEnter(object sender, EventArgs e) {
            this.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }

        private void labelIndex_MouseEnter(object sender, EventArgs e) {
            this.Cursor = new Cursor(Properties.Resources.icon.GetHicon());
        }
    }
}
