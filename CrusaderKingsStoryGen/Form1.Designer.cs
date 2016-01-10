namespace CrusaderKingsStoryGen
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.modname = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.selectCK2Dir = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ck2dir = new System.Windows.Forms.TextBox();
            this.reset = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CultureStability = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.ReligionStability = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GovernmentStability = new System.Windows.Forms.NumericUpDown();
            this.exportButton = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.start = new System.Windows.Forms.Button();
            this.renderPanel = new CrusaderKingsStoryGen.RenderPanel();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReligionStability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GovernmentStability)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.modname);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.selectCK2Dir);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ck2dir);
            this.panel1.Controls.Add(this.reset);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Controls.Add(this.stop);
            this.panel1.Controls.Add(this.start);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(321, 755);
            this.panel1.TabIndex = 1;
            // 
            // modname
            // 
            this.modname.Location = new System.Drawing.Point(95, 80);
            this.modname.Name = "modname";
            this.modname.Size = new System.Drawing.Size(128, 26);
            this.modname.TabIndex = 9;
            this.modname.Text = "storygen";
            this.modname.TextChanged += new System.EventHandler(this.modname_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Mod Name:";
            // 
            // selectCK2Dir
            // 
            this.selectCK2Dir.Location = new System.Drawing.Point(230, 40);
            this.selectCK2Dir.Name = "selectCK2Dir";
            this.selectCK2Dir.Size = new System.Drawing.Size(73, 27);
            this.selectCK2Dir.TabIndex = 7;
            this.selectCK2Dir.Text = "...";
            this.selectCK2Dir.UseVisualStyleBackColor = true;
            this.selectCK2Dir.Click += new System.EventHandler(this.selectCK2Dir_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "CK2 Game Install Dir:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // ck2dir
            // 
            this.ck2dir.Location = new System.Drawing.Point(12, 40);
            this.ck2dir.Name = "ck2dir";
            this.ck2dir.ReadOnly = true;
            this.ck2dir.Size = new System.Drawing.Size(207, 26);
            this.ck2dir.TabIndex = 5;
            this.ck2dir.TextChanged += new System.EventHandler(this.ck2dir_TextChanged);
            this.ck2dir.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ck2dir_KeyPress);
            // 
            // reset
            // 
            this.reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reset.Location = new System.Drawing.Point(21, 467);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(118, 42);
            this.reset.TabIndex = 4;
            this.reset.Text = "Reset";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CultureStability);
            this.groupBox1.Controls.Add(this.numericUpDown3);
            this.groupBox1.Controls.Add(this.ReligionStability);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.GovernmentStability);
            this.groupBox1.Location = new System.Drawing.Point(12, 153);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 152);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stability";
            // 
            // CultureStability
            // 
            this.CultureStability.AutoSize = true;
            this.CultureStability.Location = new System.Drawing.Point(64, 107);
            this.CultureStability.Name = "CultureStability";
            this.CultureStability.Size = new System.Drawing.Size(64, 20);
            this.CultureStability.TabIndex = 5;
            this.CultureStability.Text = "Culture:";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(133, 104);
            this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(78, 26);
            this.numericUpDown3.TabIndex = 4;
            this.numericUpDown3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // ReligionStability
            // 
            this.ReligionStability.Location = new System.Drawing.Point(133, 72);
            this.ReligionStability.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ReligionStability.Name = "ReligionStability";
            this.ReligionStability.Size = new System.Drawing.Size(78, 26);
            this.ReligionStability.TabIndex = 3;
            this.ReligionStability.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ReligionStability.ValueChanged += new System.EventHandler(this.ReligionStability_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Religion:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Government:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // GovernmentStability
            // 
            this.GovernmentStability.Location = new System.Drawing.Point(133, 40);
            this.GovernmentStability.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GovernmentStability.Name = "GovernmentStability";
            this.GovernmentStability.Size = new System.Drawing.Size(78, 26);
            this.GovernmentStability.TabIndex = 0;
            this.GovernmentStability.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GovernmentStability.ValueChanged += new System.EventHandler(this.GovernmentStability_ValueChanged);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportButton.Location = new System.Drawing.Point(21, 701);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(118, 42);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // stop
            // 
            this.stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stop.Location = new System.Drawing.Point(21, 594);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(118, 42);
            this.stop.TabIndex = 1;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // start
            // 
            this.start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.start.Location = new System.Drawing.Point(21, 546);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(118, 42);
            this.start.TabIndex = 0;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // renderPanel
            // 
            this.renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel.Location = new System.Drawing.Point(321, 0);
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.Size = new System.Drawing.Size(1218, 755);
            this.renderPanel.TabIndex = 0;
            this.renderPanel.Click += new System.EventHandler(this.renderPanel_Click);
            this.renderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.renderPanel_Paint);
            this.renderPanel.Resize += new System.EventHandler(this.renderPanel_Resize);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1539, 755);
            this.Controls.Add(this.renderPanel);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Out of Africa: CK2 Alternate History Generator v0.2";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReligionStability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GovernmentStability)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RenderPanel renderPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown GovernmentStability;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown ReligionStability;
        private System.Windows.Forms.Label CultureStability;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button selectCK2Dir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ck2dir;
        public System.Windows.Forms.TextBox modname;

    }
}

