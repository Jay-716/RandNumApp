using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp
{
    public partial class MyForm : Form
    {
        public bool CounterRunning { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int CheatValue { get; set; }
        public string Data { get; set; }

        public MyForm()
        {
            InitializeComponent();

            this.CounterRunning = false;
            this.ButtonStop.Enabled = false;//默认打开软件，停止按钮不可用
            this.labelIndex.Visible = false;
            this.textBoxMinValue.Visible = false;
            this.textBoxMaxValue.Visible = false;
            this.MaxValue = 50;
            this.MinValue = 1;
            this.CheatValue = 0;
            LoadData();
        }

        private void LoadData()
        {
            if(File.Exists("./data.dat"))
            {
                StreamReader file = new StreamReader("./data.dat");
                MinValue = Convert.ToInt32(file.ReadLine());
                MaxValue = Convert.ToInt32(file.ReadLine());
                CheatValue = Convert.ToInt32(file.ReadLine());
                file.Close();
            }
            else
            {
                FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("1\n50\n31");
                sw.Close();
            }
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            CounterRunning = true;
            this.ButtonStart.Enabled = false;
            this.ButtonStop.Enabled = true;
            Task task = new Task(new Action(RandNum));
            task.Start();
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            CounterRunning = false;
            this.ButtonStart.Enabled = true;
            this.ButtonStop.Enabled = false;
        }

        private void RandNum()
        {
            int min = MinValue;
            int max = MaxValue;
            int num;
            Random random = new Random(Convert.ToInt32(DateTime.Now.ToString().Substring(13, 2)));
            while (CounterRunning)
            {
                num = random.Next(min, max + 1);
                while (num == CheatValue)
                    num = random.Next(min, max + 1);
                this.label.Text = num.ToString();
                System.Threading.Thread.Sleep(10);
            }
        }

        private void ToolStripMenuItemShowIndex_Click(object sender, EventArgs e)
        {
            this.textBoxMinValue.Text = MinValue.ToString();
            this.textBoxMaxValue.Text = MaxValue.ToString();
            this.labelIndex.Visible = true;
            this.textBoxMinValue.Visible = true;
            this.textBoxMaxValue.Visible = true;
            this.textBoxMaxValue.Leave += IndexValueChange;
        }

        private void ToolStripMenuItemHideIndex_Click(object sender, EventArgs e)
        {
            this.labelIndex.Visible = false;
            this.textBoxMinValue.Visible = false;
            this.textBoxMaxValue.Visible = false;
        }

        private void IndexValueChange(object sender, EventArgs e)
        {
            this.MaxValue = Convert.ToInt32(this.textBoxMaxValue.Text);
            this.MinValue = Convert.ToInt32(this.textBoxMinValue.Text);
            this.Data = MinValue.ToString() + '\n' + MaxValue.ToString() + '\n' + CheatValue.ToString();
            FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(Data);
            sw.Close();
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Data = MinValue.ToString() + '\n' + MaxValue.ToString() + '\n' + CheatValue.ToString();
            FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(Data);
            sw.Close();
            Application.Exit();
        }
    }
}
