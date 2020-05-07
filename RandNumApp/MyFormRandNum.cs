using System;
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
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //忽略线程访问安全机制，允许另一个线程访问其它线程创建的控件而不抛出异常。（影响不大）

            this.CounterRunning = false;
            this.MaxValue = 50;
            this.MinValue = 1;
            this.CheatValue = 0;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists("./data.dat"))
                {
                    StreamReader file = new StreamReader("./data.dat");
                    MinValue = Convert.ToInt32(file.ReadLine());
                    MaxValue = Convert.ToInt32(file.ReadLine());
                    CheatValue = Convert.ToInt32(file.ReadLine());
                    file.Close();
                    File.SetAttributes("./data.dat", FileAttributes.Hidden);
                }
                else
                {
                    FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("1\n50\n31");
                    sw.Close();
                    File.SetAttributes("./data.dat", FileAttributes.Hidden);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("权限错误。无法修改数据文件。\n请将软件移到普通文件夹内运行或给予管理员权限。\n" + ex.ToString());
            }
            catch(FormatException ex)
            {
                this.MaxValue = 50;
                this.MinValue = 1;
                this.CheatValue = 0;
                this.textBoxMinValue.Text = MinValue.ToString();
                this.textBoxMaxValue.Text = MaxValue.ToString();
                MessageBox.Show("Data.dat错误\n" + ex.ToString());
            }
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            CounterRunning = true;
            this.ButtonStart.Enabled = false;
            this.ButtonStop.Enabled = true;
            Task task = new Task(RandNum);
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
            int num = 0;
            Random random = new Random(num);
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
            this.textBoxMinValue.Enabled = true;
            this.textBoxMaxValue.Enabled = true;
            this.textBoxMaxValue.Leave += IndexValueChange;
            this.ToolStripMenuItemHideIndex.Enabled = true;
            this.ToolStripMenuItemShowIndex.Enabled = false;
        }

        private void ToolStripMenuItemHideIndex_Click(object sender, EventArgs e)
        {
            this.labelIndex.Visible = false;
            this.textBoxMinValue.Visible = false;
            this.textBoxMaxValue.Visible = false;
            this.textBoxMinValue.Enabled = false;
            this.textBoxMaxValue.Enabled = false;
            this.ToolStripMenuItemHideIndex.Enabled = false;
            this.ToolStripMenuItemShowIndex.Enabled = true;
        }

        private void IndexValueChange(object sender, EventArgs e)
        {
            try
            {
                this.MaxValue = Convert.ToInt32(this.textBoxMaxValue.Text);
                this.MinValue = Convert.ToInt32(this.textBoxMinValue.Text);
                this.Data = MinValue.ToString() + '\n' + MaxValue.ToString() + '\n' + CheatValue.ToString();
                FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Data);
                sw.Close();
                File.SetAttributes("./data.dat", FileAttributes.Hidden);
            }
            catch (UnauthorizedAccessException ex)
            {
                this.MaxValue = 50;
                this.MinValue = 1;
                this.CheatValue = 0;
                MessageBox.Show("权限错误。无法修改数据文件。\n请将软件移到普通文件夹内运行或给予管理员权限。\n" + ex.ToString());
            }
            catch(FormatException ex)
            {
                this.MaxValue = 50;
                this.MinValue = 1;
                this.CheatValue = 0;
                this.textBoxMinValue.Text = MinValue.ToString();
                this.textBoxMaxValue.Text = MaxValue.ToString();
                MessageBox.Show("请输入合法的数字。\n" + ex.ToString());
            }
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Data = MinValue.ToString() + '\n' + MaxValue.ToString() + '\n' + CheatValue.ToString();
                FileStream fs = new FileStream("./data.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Data);
                sw.Close();
                File.SetAttributes("./data.dat", FileAttributes.Hidden);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("权限错误。无法修改数据文件。\n请将软件移到普通文件夹内运行或给予管理员权限。\n" + ex.ToString());
                Application.Exit();
            }
            Application.Exit();
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
    }
}
