namespace mp3_catch
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Download = new Button();
            progressBar1 = new ProgressBar();
            now_loading = new Label();
            Direction = new Button();
            Cancel_Button = new Button();
            ProgressBar_Text = new Label();
            groupBox1 = new GroupBox();
            Format_Selection = new ComboBox();
            Open_UrlForm = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // Download
            // 
            Download.BackColor = SystemColors.Control;
            Download.FlatStyle = FlatStyle.System;
            Download.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            Download.ForeColor = Color.Black;
            Download.Location = new Point(372, 182);
            Download.Margin = new Padding(4);
            Download.Name = "Download";
            Download.Size = new Size(127, 74);
            Download.TabIndex = 1;
            Download.Text = "下載";
            Download.UseVisualStyleBackColor = false;
            Download.Click += Download_Click;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.FromArgb(0, 0, 64);
            progressBar1.Location = new Point(15, 219);
            progressBar1.Margin = new Padding(4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(243, 29);
            progressBar1.TabIndex = 2;
            progressBar1.Visible = false;
            // 
            // now_loading
            // 
            now_loading.AutoSize = true;
            now_loading.Font = new Font("Microsoft JhengHei UI", 8F);
            now_loading.ForeColor = Color.Black;
            now_loading.Location = new Point(22, 182);
            now_loading.Margin = new Padding(4, 0, 4, 0);
            now_loading.Name = "now_loading";
            now_loading.Size = new Size(0, 18);
            now_loading.TabIndex = 3;
            // 
            // Direction
            // 
            Direction.BackColor = SystemColors.Control;
            Direction.FlatStyle = FlatStyle.System;
            Direction.Font = new Font("微軟正黑體", 12F);
            Direction.ForeColor = Color.Black;
            Direction.Location = new Point(15, 70);
            Direction.Margin = new Padding(4);
            Direction.Name = "Direction";
            Direction.Size = new Size(484, 34);
            Direction.TabIndex = 4;
            Direction.Text = "檔案路徑";
            Direction.UseVisualStyleBackColor = false;
            Direction.Click += Direction_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.BackColor = SystemColors.Control;
            Cancel_Button.Enabled = false;
            Cancel_Button.FlatStyle = FlatStyle.System;
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(266, 219);
            Cancel_Button.Margin = new Padding(4);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.Size = new Size(96, 29);
            Cancel_Button.TabIndex = 5;
            Cancel_Button.Text = "取消";
            Cancel_Button.UseVisualStyleBackColor = false;
            Cancel_Button.Visible = false;
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // ProgressBar_Text
            // 
            ProgressBar_Text.AutoSize = true;
            ProgressBar_Text.BackColor = Color.FromArgb(224, 224, 224);
            ProgressBar_Text.ForeColor = SystemColors.ControlText;
            ProgressBar_Text.Location = new Point(24, 224);
            ProgressBar_Text.Name = "ProgressBar_Text";
            ProgressBar_Text.Size = new Size(0, 19);
            ProgressBar_Text.TabIndex = 6;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Control;
            groupBox1.Controls.Add(Format_Selection);
            groupBox1.Location = new Point(15, 111);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(243, 69);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "下載格式";
            // 
            // Format_Selection
            // 
            Format_Selection.FormattingEnabled = true;
            Format_Selection.Items.AddRange(new object[] { "mp3", "mp4" });
            Format_Selection.Location = new Point(9, 26);
            Format_Selection.Name = "Format_Selection";
            Format_Selection.Size = new Size(151, 27);
            Format_Selection.TabIndex = 9;
            Format_Selection.Text = "檔案格式選項";
            // 
            // Open_UrlForm
            // 
            Open_UrlForm.BackColor = SystemColors.Control;
            Open_UrlForm.FlatStyle = FlatStyle.System;
            Open_UrlForm.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            Open_UrlForm.ForeColor = Color.Black;
            Open_UrlForm.Location = new Point(13, 13);
            Open_UrlForm.Margin = new Padding(4);
            Open_UrlForm.Name = "Open_UrlForm";
            Open_UrlForm.Size = new Size(486, 49);
            Open_UrlForm.TabIndex = 8;
            Open_UrlForm.Text = "網址連結";
            Open_UrlForm.UseVisualStyleBackColor = false;
            Open_UrlForm.Click += Open_UrlForm_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(512, 283);
            Controls.Add(Open_UrlForm);
            Controls.Add(groupBox1);
            Controls.Add(ProgressBar_Text);
            Controls.Add(Cancel_Button);
            Controls.Add(Direction);
            Controls.Add(now_loading);
            Controls.Add(progressBar1);
            Controls.Add(Download);
            Margin = new Padding(4);
            MaximumSize = new Size(530, 330);
            MinimumSize = new Size(530, 330);
            Name = "MainForm";
            Text = "mp3 Catcher ";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button Download;
        private ProgressBar progressBar1;
        private Label now_loading;
        private Button Direction;
        private Button Cancel_Button;
        private Label ProgressBar_Text;
        private GroupBox groupBox1;
        private Button Open_UrlForm;
        private ComboBox Format_Selection;
    }
}
