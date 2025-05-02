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
    // �M�ץD����
    public partial class MainForm : Form
    {
        public readonly string VERSION = "1.1"; // ������T
        Random random = new Random(); // �H���ƥͦ����A�Ω�i�ױ���s
        private string? Folder_Name = null; // �x�s���|
        private CancellationTokenSource _cancellationTokenSource; // �Ω�����U��
        private YoutubeClient youtube; // YoutubeExplode �Ȥ��
        private HttpClient httpClient; // HTTP �ШD�Τ��
        private string Url_List = string.Empty; // �x�s���}��J

        // ��s�i�ױ��~�[
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

        // �غc�l�G��l�Ƴ]�w
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
                ProgressBar_Text.Text = "���o�v����T...";

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Update_PRO();

                // ���o�u�]�t���T�� MP4 ��y
                var streamInfo = streamManifest
                    .GetVideoStreams()
                    .GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("�L�k���o���T�� MP4 ��y");

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
                    now_loading.Text = video.Title + $" (����:{video.Duration?.ToString(@"hh\:mm\:ss")})"));

                Update_PRO();
                ProgressBar_Text.Text = "�U���v����...";
                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

                progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
                ProgressBar_Text.Invoke(new Action(() => ProgressBar_Text.Text = "�U������"));

                return "success";
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("�U���w����", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (File.Exists(filePath)) File.Delete(filePath);
                _cancellationTokenSource.TryReset();
                ResetUI();
                return "cancel";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"���~: {ex.Message}", "�U������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                now_loading.Invoke(new Action(() => now_loading.Text = ""));
            }

            return "fail";
        }

        // �U�����T�]MP3�榡�^
        private async Task<string> DownloadAudioMp3(string url)
        {
            string filePath = string.Empty;
            try
            {
                progressBar1.Visible = true;
                var video = await youtube.Videos.GetAsync(url);
                Update_PRO();
                ProgressBar_Text.Text = "���o�v����T...";

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Update_PRO();

                // ���o�u�]�t���T�� MP3 ��y
                var streamInfo = streamManifest
                    .GetAudioStreams()
                    .GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("�L�k���o���T�� MP3 ��y");

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
                    now_loading.Text = video.Title + $" (����:{video.Duration?.ToString(@"hh\:mm\:ss")})"));

                Update_PRO();
                ProgressBar_Text.Text = "�U�����T��...";
                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

                progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
                ProgressBar_Text.Invoke(new Action(() => ProgressBar_Text.Text = "�U������"));

                return "success";
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("�U���w����", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (File.Exists(filePath)) File.Delete(filePath);
                _cancellationTokenSource.TryReset();
                ResetUI();
                return "cancel";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"���~: {ex.Message}", "�U������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                now_loading.Invoke(new Action(() => now_loading.Text = ""));
            }

            return "fail";
        }

        // �}�l�U�����s
        private async void Download_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Url_List) || Url_List == "")
            {
                MessageBox.Show("�п�J���}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        // �U���v���]MP4�榡�^
                        await DownloadVideoMp4(url.Trim());
                    }
                    else if (Format_Selection.SelectedIndex == 0)
                    {
                        // �U�����T�]MP3�榡�^
                        await DownloadAudioMp3(url.Trim());
                    }
                }

                TimeSpan totalTime = DateTime.Now - startTime;
                ProgressBar_Text.Text = $"�`�ή�: {totalTime:hh\\:mm\\:ss}";
                await Task.Run(() => { Task.Delay(10 * 1000); ProgressBar_Text.Invoke(() => ProgressBar_Text.Text = ""); });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�U�����~: {ex.Message}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Direction.Text = $"�x�s���|: {Folder_Name}";
                }
            }
        }

        // �������s
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            ResetUI();
        }

        // UI ���m
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
        // �}�Һ��}��J���
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
                // �]�w�����l�j�p�P�˦�
                this.Text = "��J���}";
                this.Size = new Size(760, 461);
                this.BackColor = SystemColors.Control;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Icon = new MainForm().Icon;
                // �إ߼���
                Label label = new Label
                {
                    Text = "�п�J YouTube ���ּv�����}�G",
                    AutoSize = true,
                    Font = new Font("Microsoft JhengHei UI", 12),
                    Location = new Point(20, 10)
                };

                // �إ߿�J��
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
                    Text = "�۰ʱƪ�",
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
                // �إ� OK ���s
                Button okBtn = new Button
                {
                    Text = "�T�{",
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
                        MessageBox.Show("�п�J���Ī����}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // �إ� Cancel ���s
                Button cancelBtn = new Button
                {
                    Text = "����",
                    Size = new Size(128, 50),
                    Font = new Font("Microsoft JhengHei UI", 12),
                    Location = new Point(420, 333)
                };
                cancelBtn.Click += (s, e) =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                };

                // �[�J���
                this.Controls.Add(label);
                this.Controls.Add(textBox);
                this.Controls.Add(okBtn);
                this.Controls.Add(cancelBtn);
                this.Controls.Add(FormatBtn);
                // �q�\ Resize �ƥ�
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
