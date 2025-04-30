using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace mp3_catch
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// ���� Version : 1.1
        /// <code>�~�[�ק�</code>
        /// </summary>
        public readonly string VERSION = "1.1";
        Random random = new Random();
        private string? Folder_Name = null;
        private CancellationTokenSource _cancellationTokenSource;
        private YoutubeClient youtube;
        private HttpClient httpClient;
        private void Update_PRO()
        {
            int range = random.Next(3, 13);
            progressBar1.Value += (progressBar1.Value+range)>99?0:range;
            if (progressBar1.Value > 40)
            {
                ProgressBar_Text.BackColor = Color.FromArgb(65,173,65);//progressBar1���C��
            }
            else if(progressBar1.Value <= 40)
            {
                ProgressBar_Text.BackColor = Color.Transparent;
            }
        }
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

        private async Task<string> DownloadVideo(string url)
        {
            string filePath = string.Empty;
            int chunkSize = 2 * 1024 * 1024; // �C�Ӱ϶� 2MB
            int maxThreads = 8; // �̤j�æ�U���������
            try
            {
                progressBar1.Visible = true;
                var video = await youtube.Videos.GetAsync(url);
                Update_PRO();
                ProgressBar_Text.Text = "���o�v����T...";
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Update_PRO();
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                if (streamInfo == null)
                    throw new Exception("�L�k������T�y");
                Update_PRO();
                long totalBytes = streamInfo.Size.Bytes;
                if (totalBytes == 0)
                    throw new Exception("�L�k���o�ɮפj�p");

                string fileName = video.Title;
                string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalidChars)
                {
                    fileName = fileName.Replace(c.ToString(), "_"); // �������䴩�r��
                }
                Update_PRO();
                if (fileName.Length > 100) // �קK Windows ���|�W�L 260 �r��
                {
                    fileName = fileName.Substring(0, 100);
                }
                string directory = Folder_Name ?? Environment.CurrentDirectory;
                string baseFilePath = Path.Combine(directory, $"{fileName}.mp3");
                Update_PRO();
                filePath = baseFilePath;
                int count = 1;
                while (File.Exists(filePath)) // �p�G�ɮפw�s�b�A�[�W�s��
                {
                    filePath = Path.Combine(directory, $"{fileName} ({count}).mp3");
                    count++;
                }
                Update_PRO();
                string �ɮפj�p = totalBytes / 1024 > 1024 ? (totalBytes / 1024 / 1024).ToString() + "MB" : (totalBytes / 1024).ToString() + "KB";
                now_loading.Invoke(new Action(() => now_loading.Text = video.Title + $"(�j�p:{�ɮפj�p})"));
                // �����ɮצ��h�Ӱ϶�
                int totalChunks = (int)Math.Ceiling((double)totalBytes / chunkSize);
                Update_PRO();
                var httpClients = Enumerable.Range(0, maxThreads).Select(_ => new HttpClient()).ToArray(); // �إߦh�� HttpClient
                var tasks = new List<Task<byte[]>>();
                Update_PRO();
                for (int i = 0; i < totalChunks; i++)
                {
                    int startByte = i * chunkSize;
                    int endByte = Math.Min(startByte + chunkSize - 1, (int)totalBytes - 1);
                    int threadIndex = i % maxThreads; // ���t HttpClient
                    tasks.Add(DownloadChunk(httpClients[threadIndex], streamInfo.Url, startByte, endByte));
                }
                Update_PRO();
                var results = await Task.WhenAll(tasks); // �P�ɤU���Ҧ��϶�
                // �g�J�ɮ�
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 524288, true))
                {
                    foreach (var chunk in results)
                    {
                        await fileStream.WriteAsync(chunk, 0, chunk.Length);
                    }
                }
                progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
                Update_PRO();
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
            return "success";
        }

        /// <summary>
        /// �U���S�w�϶������
        /// </summary>
        private async Task<byte[]> DownloadChunk(HttpClient client, string url, int startByte, int endByte)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startByte, endByte); // �]�w Range

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }



        private async void Download_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Folder_Name) || string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("�п���x�s��Ƨ��ÿ�J���}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cancel_Button.Enabled = true;
            Cancel_Button.Visible = true;
            progressBar1.Visible = true;  
            DateTime startTime = DateTime.Now;

            try
            {
                Download.Enabled = false;
                string[] urls = richTextBox1.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string url in urls)
                {
                    string result = await DownloadVideo(url.Trim());
                    if (result == "cancel") break;
                }

                TimeSpan totalTime = DateTime.Now - startTime;
                ProgressBar_Text.Text = $"�`�ή�: {totalTime:hh\\:mm\\:ss}";
                await Task.Run(() => { Task.Delay(10 * 1000); ProgressBar_Text.Invoke(new Action(() => { ProgressBar_Text.Text = ""; }));});
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�U�����~: {ex.Message}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Download.Enabled=true;
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

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            ResetUI();
        }

        private void ResetUI()
        {
            Cancel_Button.Enabled = false;
            Cancel_Button.Visible = false;
            now_loading.Text = "";
            progressBar1.Value = 0;
            Update_PRO();
            progressBar1.Visible = false;
        }
    }
}
