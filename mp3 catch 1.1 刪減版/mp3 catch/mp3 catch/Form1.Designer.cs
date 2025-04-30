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
            richTextBox1 = new RichTextBox();
            Download = new Button();
            progressBar1 = new ProgressBar();
            now_loading = new Label();
            Direction = new Button();
            Cancel_Button = new Button();
            ProgressBar_Text = new Label();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.White;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Font = new Font("Microsoft JhengHei UI", 8F);
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.Location = new Point(8, 77);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            richTextBox1.Size = new Size(641, 183);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // Download
            // 
            Download.BackColor = SystemColors.Control;
            Download.FlatStyle = FlatStyle.System;
            Download.Font = new Font("Microsoft JhengHei UI", 12F);
            Download.ForeColor = Color.Black;
            Download.Location = new Point(8, 10);
            Download.Name = "Download";
            Download.Size = new Size(310, 39);
            Download.TabIndex = 1;
            Download.Text = "下載";
            Download.UseVisualStyleBackColor = false;
            Download.Click += Download_Click;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.FromArgb(0, 0, 64);
            progressBar1.Location = new Point(12, 290);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(189, 23);
            progressBar1.TabIndex = 2;
            progressBar1.Visible = false;
            // 
            // now_loading
            // 
            now_loading.AutoSize = true;
            now_loading.Font = new Font("Microsoft JhengHei UI", 8F);
            now_loading.ForeColor = Color.Black;
            now_loading.Location = new Point(12, 263);
            now_loading.Name = "now_loading";
            now_loading.Size = new Size(0, 14);
            now_loading.TabIndex = 3;
            // 
            // Direction
            // 
            Direction.BackColor = SystemColors.Control;
            Direction.FlatStyle = FlatStyle.System;
            Direction.Font = new Font("微軟正黑體", 12F);
            Direction.ForeColor = Color.Black;
            Direction.Location = new Point(324, 10);
            Direction.Name = "Direction";
            Direction.Size = new Size(315, 39);
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
            Cancel_Button.Location = new Point(207, 290);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.Size = new Size(75, 23);
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
            ProgressBar_Text.Location = new Point(19, 294);
            ProgressBar_Text.Margin = new Padding(2, 0, 2, 0);
            ProgressBar_Text.Name = "ProgressBar_Text";
            ProgressBar_Text.Size = new Size(0, 15);
            ProgressBar_Text.TabIndex = 6;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(651, 335);
            Controls.Add(ProgressBar_Text);
            Controls.Add(Cancel_Button);
            Controls.Add(Direction);
            Controls.Add(now_loading);
            Controls.Add(progressBar1);
            Controls.Add(Download);
            Controls.Add(richTextBox1);
            MaximumSize = new Size(667, 374);
            MinimumSize = new Size(667, 374);
            Name = "MainForm";
            Text = "mp3 Catcher ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button Download;
        private ProgressBar progressBar1;
        private Label now_loading;
        private Button Direction;
        private Button Cancel_Button;
        private Label ProgressBar_Text;
    }
}
