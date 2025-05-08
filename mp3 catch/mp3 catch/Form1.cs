using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace mp3_catch
{
    // 專案主視窗
    public partial class MainForm : Form
    {
        public readonly string VERSION = "1.1"; // 版本資訊
        Random random = new Random(); // 隨機數生成器，用於進度條更新
        private string? Folder_Name = null; // 儲存路徑
        private CancellationTokenSource _cancellationTokenSource; // 用於取消下載
        private YoutubeClient youtube; // YoutubeExplode 客戶端
        private HttpClient httpClient; // HTTP 請求用戶端
        private string Url_List = string.Empty; // 儲存網址輸入

        // 更新進度條外觀
        private void Update_PRO()
        {
            int range = random.Next(3, 13);
            progressBar1.Value += (progressBar1.Value + range) > 99 ? 0 : range;
            if (progressBar1.Value > 40)
            {
                ProgressBar_Text.BackColor = Color.FromArgb(65, 173, 65);
            }
            else if (progressBar1.Value <= 40)
            {
                ProgressBar_Text.BackColor = Color.Transparent;
            }
        }

        // 建構子：初始化設定
        public MainForm()
        {
            InitializeComponent();
            this.Text += VERSION;
            this.Icon = new Icon("AppData\\catch pixel.ico");
            progressBar1.Maximum = 100;

            _cancellationTokenSource = new CancellationTokenSource();
            youtube = new YoutubeClient();
            httpClient = new HttpClient();
            ResetUI();
        }

        private async Task<string> DownloadVideoMp4(string url)
        {
            string filePath = string.Empty;
            try
            {
                progressBar1.Visible = true;
                var video = await youtube.Videos.GetAsync(url);
                Update_PRO();
                ProgressBar_Text.Text = "取得影片資訊...";

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Update_PRO();

                // 取得只包含視訊的 MP4 串流
                var streamInfo = streamManifest
                    .GetVideoStreams()
                    .GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("無法取得視訊的 MP4 串流");

                string fileName = video.Title;
                string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalidChars)
                    fileName = fileName.Replace(c.ToString(), "_");

                if (fileName.Length > 100)
                    fileName = fileName.Substring(0, 100);

                string directory = Folder_Name ?? Environment.CurrentDirectory;
                filePath = Path.Combine(directory, $"{fileName}.mp4");

                int count = 1;
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(directory, $"{fileName} ({count}).mp4");
                    count++;
                }

                now_loading.Invoke(new Action(() =>
                    now_loading.Text = video.Title + $" (長度:{video.Duration?.ToString(@"hh\:mm\:ss")})"));

                Update_PRO();
                ProgressBar_Text.Text = "下載影片中...";
                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

                progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
                ProgressBar_Text.Invoke(new Action(() => ProgressBar_Text.Text = "下載完成"));

                return "success";
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("下載已取消", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (File.Exists(filePath)) File.Delete(filePath);
                _cancellationTokenSource.TryReset();
                ResetUI();
                return "cancel";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"錯誤: {ex.Message}", "下載失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                now_loading.Invoke(new Action(() => now_loading.Text = ""));
            }

            return "fail";
        }

        // 下載音訊（MP3格式）
        private async Task<string> DownloadAudioMp3(string url)
        {
            string filePath = string.Empty;
            try
            {
                progressBar1.Visible = true;
                var video = await youtube.Videos.GetAsync(url);
                Update_PRO();
                ProgressBar_Text.Text = "取得影片資訊...";

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Update_PRO();

                // 取得只包含音訊的 MP3 串流
                var streamInfo = streamManifest
                    .GetAudioStreams()
                    .GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("無法取得音訊的 MP3 串流");

                string fileName = video.Title;
                string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalidChars)
                    fileName = fileName.Replace(c.ToString(), "_");

                if (fileName.Length > 100)
                    fileName = fileName.Substring(0, 100);

                string directory = Folder_Name ?? Environment.CurrentDirectory;
                filePath = Path.Combine(directory, $"{fileName}.mp3");

                int count = 1;
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(directory, $"{fileName} ({count}).mp3");
                    count++;
                }

                now_loading.Invoke(new Action(() =>
                    now_loading.Text = video.Title + $" (長度:{video.Duration?.ToString(@"hh\:mm\:ss")})"));

                Update_PRO();
                ProgressBar_Text.Text = "下載音訊中...";
                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

                progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
                ProgressBar_Text.Invoke(new Action(() => ProgressBar_Text.Text = "下載完成"));

                return "success";
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("下載已取消", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (File.Exists(filePath)) File.Delete(filePath);
                _cancellationTokenSource.TryReset();
                ResetUI();
                return "cancel";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"錯誤: {ex.Message}", "下載失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                now_loading.Invoke(new Action(() => now_loading.Text = ""));
            }

            return "fail";
        }

        // 開始下載按鈕
        private async void Download_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Url_List) || Url_List == "")
            {
                MessageBox.Show("請輸入網址", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cancel_Button.Enabled = true;
            Cancel_Button.Visible = true;
            progressBar1.Visible = true;
            DateTime startTime = DateTime.Now;

            try
            {
                Download.Enabled = false;
                string[] urls = Url_List.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string url in urls)
                {
                    if (Format_Selection.SelectedIndex == 1)
                    {
                        // 下載影片（MP4格式）
                        await DownloadVideoMp4(url.Trim());
                    }
                    else if (Format_Selection.SelectedIndex == 0)
                    {
                        // 下載音訊（MP3格式）
                        await DownloadAudioMp3(url.Trim());
                    }
                }

                TimeSpan totalTime = DateTime.Now - startTime;
                ProgressBar_Text.Text = $"總用時: {totalTime:hh\\:mm\\:ss}";
                await Task.Run(() => { Task.Delay(10 * 1000); ProgressBar_Text.Invoke(() => ProgressBar_Text.Text = ""); });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"下載錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Download.Enabled = true;
                ResetUI();
            }
        }
   
        private void Direction_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Folder_Name = dialog.SelectedPath;
                    Direction.Text = $"儲存路徑: {Folder_Name}";
                }
            }
        }

        // 取消按鈕
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            ResetUI();
        }

        // UI 重置
        private void ResetUI()
        {
            Cancel_Button.Enabled = false;
            Cancel_Button.Visible = false;
            now_loading.Text = "";
            progressBar1.Value = 0;
            Update_PRO();
            progressBar1.Visible = false;
            Format_Selection.SelectedIndex = 0;
        }
        // 開啟網址輸入表單
        private void Open_UrlForm_Click(object sender, EventArgs e)
        {
            var urlForm = new UrlForm(Url_List);
            if (urlForm.ShowDialog() == DialogResult.OK)
            {
                Url_List = urlForm.EnteredUrl ?? "";
            }
        }
        public partial class UrlForm : Form
        {
            private Dictionary<Control, (float X, float Y, float Width, float Height)> controlLayout = new Dictionary<Control, (float X, float Y, float Width, float Height)>();
            private int initialFormWidth;
            private int initialFormHeight;

            public string? EnteredUrl { get; private set; }

            public UrlForm(string DefaultUrl)
            {
                // 設定窗體初始大小與樣式
                this.Text = "輸入網址";
                this.Size = new Size(760, 461);
                this.BackColor = SystemColors.Control;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Icon = new MainForm().Icon;
                // 建立標籤
                Label label = new Label
                {
                    Text = "請輸入 YouTube 音樂影片網址：",
                    AutoSize = true,
                    Font = new Font("Microsoft JhengHei UI", 12),
                    Location = new Point(20, 10)
                };

                // 建立輸入框
                RichTextBox textBox = new RichTextBox
                {
                    Size = new Size(710, 250),
                    Location = new Point(20, 40),
                    Font = new Font("Microsoft JhengHei UI", 10),
                    ScrollBars = RichTextBoxScrollBars.ForcedBoth,
                    BorderStyle = BorderStyle.None,
                    Text = DefaultUrl
                };
                Button FormatBtn = new Button
                {
                    Text = "自動排版",
                    Size = new Size(128, 50),
                    Font = new Font("Microsoft JhengHei UI", 12),
                    Location = new Point(20, 333)
                };
                FormatBtn.Click += (s, e) =>
                {
                    textBox.Text = string.Join
                    ("", textBox.Text.Replace("https:", "\nhttps:")
                                    .Skip(1)
                                    .ToArray()
                    ).Replace("\n\n", "\n");
                };
                // 建立 OK 按鈕
                Button okBtn = new Button
                {
                    Text = "確認",
                    Size = new Size(128, 50),
                    Font = new Font("Microsoft JhengHei UI", 12),
                    Location = new Point(220, 333)
                };
                okBtn.Click += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        EnteredUrl = textBox.Text;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("請輸入有效的網址", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // 建立 Cancel 按鈕
                Button cancelBtn = new Button
                {
                    Text = "取消",
                    Size = new Size(128, 50),
                    Font = new Font("Microsoft JhengHei UI", 12),
                    Location = new Point(420, 333)
                };
                cancelBtn.Click += (s, e) =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                };

                // 加入控制項
                this.Controls.Add(label);
                this.Controls.Add(textBox);
                this.Controls.Add(okBtn);
                this.Controls.Add(cancelBtn);
                this.Controls.Add(FormatBtn);
                // 訂閱 Resize 事件
                this.Resize += Form_Resize;
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                initialFormWidth = this.Width;
                initialFormHeight = this.Height;
                SaveInitialControlLayout(this);
            }

            private void SaveInitialControlLayout(Control parent)
            {
                foreach (Control control in parent.Controls)
                {
                    float x = (float)control.Left / initialFormWidth;
                    float y = (float)control.Top / initialFormHeight;
                    float width = (float)control.Width / initialFormWidth;
                    float height = (float)control.Height / initialFormHeight;
                    controlLayout[control] = (x, y, width, height);

                    if (control.HasChildren)
                        SaveInitialControlLayout(control);
                }
            }

            private void Form_Resize(object? sender, EventArgs e)
            {
                ResizeControls(this);
            }

            private void ResizeControls(Control parent)
            {
                foreach (Control control in parent.Controls)
                {
                    if (controlLayout.TryGetValue(control, out var layout))
                    {
                        control.Left = (int)(layout.X * this.Width);
                        control.Top = (int)(layout.Y * this.Height);
                        control.Width = (int)(layout.Width * this.Width);
                        control.Height = (int)(layout.Height * this.Height);
                    }

                    if (control.HasChildren)
                        ResizeControls(control);
                }
            }
        }
    }
}
