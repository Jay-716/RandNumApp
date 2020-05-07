namespace WindowsFormsApp
{
    partial class MyForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.范围选项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemShowIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemHideIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.textBoxMinValue = new System.Windows.Forms.TextBox();
            this.labelIndex = new System.Windows.Forms.Label();
            this.textBoxMaxValue = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.menuStrip.AutoSize = false;
            this.menuStrip.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSettings,
            this.ToolStripMenuItemHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(332, 20);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // toolStripMenuItemSettings
            // 
            this.toolStripMenuItemSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.范围选项ToolStripMenuItem,
            this.ToolStripMenuItemExit});
            this.toolStripMenuItemSettings.Margin = new System.Windows.Forms.Padding(0, -2, 0, 0);
            this.toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            this.toolStripMenuItemSettings.Size = new System.Drawing.Size(42, 18);
            this.toolStripMenuItemSettings.Text = "设置";
            // 
            // 范围选项ToolStripMenuItem
            // 
            this.范围选项ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemShowIndex,
            this.ToolStripMenuItemHideIndex});
            this.范围选项ToolStripMenuItem.Name = "范围选项ToolStripMenuItem";
            this.范围选项ToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.范围选项ToolStripMenuItem.Text = "范围选项";
            // 
            // ToolStripMenuItemShowIndex
            // 
            this.ToolStripMenuItemShowIndex.Name = "ToolStripMenuItemShowIndex";
            this.ToolStripMenuItemShowIndex.Size = new System.Drawing.Size(98, 22);
            this.ToolStripMenuItemShowIndex.Text = "显示";
            this.ToolStripMenuItemShowIndex.Click += new System.EventHandler(this.ToolStripMenuItemShowIndex_Click);
            // 
            // ToolStripMenuItemHideIndex
            // 
            this.ToolStripMenuItemHideIndex.Enabled = false;
            this.ToolStripMenuItemHideIndex.Name = "ToolStripMenuItemHideIndex";
            this.ToolStripMenuItemHideIndex.Size = new System.Drawing.Size(98, 22);
            this.ToolStripMenuItemHideIndex.Text = "隐藏";
            this.ToolStripMenuItemHideIndex.Click += new System.EventHandler(this.ToolStripMenuItemHideIndex_Click);
            // 
            // ToolStripMenuItemExit
            // 
            this.ToolStripMenuItemExit.Name = "ToolStripMenuItemExit";
            this.ToolStripMenuItemExit.Size = new System.Drawing.Size(120, 22);
            this.ToolStripMenuItemExit.Text = "退出";
            this.ToolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // ToolStripMenuItemHelp
            // 
            this.ToolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem});
            this.ToolStripMenuItemHelp.Margin = new System.Windows.Forms.Padding(0, -2, 0, 0);
            this.ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp";
            this.ToolStripMenuItemHelp.Size = new System.Drawing.Size(42, 18);
            this.ToolStripMenuItemHelp.Text = "帮助";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.关于ToolStripMenuItem.Text = "关于";
            // 
            // ButtonStart
            // 
            this.ButtonStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("ButtonStart.Image")));
            this.ButtonStart.Location = new System.Drawing.Point(55, 65);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(65, 57);
            this.ButtonStart.TabIndex = 1;
            this.ButtonStart.UseVisualStyleBackColor = true;
            this.ButtonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            this.ButtonStart.MouseEnter += new System.EventHandler(this.ButtonStart_MouseEnter);
            // 
            // ButtonStop
            // 
            this.ButtonStop.Enabled = false;
            this.ButtonStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("ButtonStop.Image")));
            this.ButtonStop.Location = new System.Drawing.Point(215, 65);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(66, 57);
            this.ButtonStop.TabIndex = 2;
            this.ButtonStop.UseVisualStyleBackColor = true;
            this.ButtonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            this.ButtonStop.MouseEnter += new System.EventHandler(this.ButtonStop_MouseEnter);
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.Font = new System.Drawing.Font("宋体", 52F);
            this.label.Location = new System.Drawing.Point(78, 322);
            this.label.MaximumSize = new System.Drawing.Size(100, 80);
            this.label.MinimumSize = new System.Drawing.Size(100, 80);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(100, 80);
            this.label.TabIndex = 3;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            // 
            // textBoxMinValue
            // 
            this.textBoxMinValue.Enabled = false;
            this.textBoxMinValue.Location = new System.Drawing.Point(71, 28);
            this.textBoxMinValue.Name = "textBoxMinValue";
            this.textBoxMinValue.Size = new System.Drawing.Size(38, 21);
            this.textBoxMinValue.TabIndex = 4;
            this.textBoxMinValue.Visible = false;
            // 
            // labelIndex
            // 
            this.labelIndex.AutoSize = true;
            this.labelIndex.BackColor = System.Drawing.Color.Transparent;
            this.labelIndex.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelIndex.Location = new System.Drawing.Point(115, 33);
            this.labelIndex.Name = "labelIndex";
            this.labelIndex.Size = new System.Drawing.Size(28, 29);
            this.labelIndex.TabIndex = 5;
            this.labelIndex.Text = "~";
            this.labelIndex.Visible = false;
            // 
            // textBoxMaxValue
            // 
            this.textBoxMaxValue.Enabled = false;
            this.textBoxMaxValue.Location = new System.Drawing.Point(149, 28);
            this.textBoxMaxValue.Name = "textBoxMaxValue";
            this.textBoxMaxValue.Size = new System.Drawing.Size(39, 21);
            this.textBoxMaxValue.TabIndex = 6;
            this.textBoxMaxValue.Visible = false;
            // 
            // MyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(332, 421);
            this.Controls.Add(this.textBoxMaxValue);
            this.Controls.Add(this.labelIndex);
            this.Controls.Add(this.textBoxMinValue);
            this.Controls.Add(this.label);
            this.Controls.Add(this.ButtonStop);
            this.Controls.Add(this.ButtonStart);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(348, 460);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(348, 460);
            this.Name = "MyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "魔法学号";
            this.MouseEnter += new System.EventHandler(this.MyForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.MyForm_MouseLeave);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSettings;
        private System.Windows.Forms.ToolStripMenuItem 范围选项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemShowIndex;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemHideIndex;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.Button ButtonStop;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TextBox textBoxMinValue;
        private System.Windows.Forms.Label labelIndex;
        private System.Windows.Forms.TextBox textBoxMaxValue;
    }
}

