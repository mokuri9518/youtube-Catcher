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
        /// 版本 Version : 1.1
        /// <code>外觀修改</code>
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
                ProgressBar_Text.BackColor = Color.FromArgb(65,173,65);//progressBar1的顏色
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
            int chunkSize = 2 * 1024 * 1024; // 每個區塊 2MB
            int maxThreads = 8; // 最大並行下載執行緒數
            try
            {
                progressBar1.Visible = true;
                var video = await youtube.Videos.GetAsync(url);
                Update_PRO();
                ProgressBar_Text.Text = "取得影片資訊...";
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Update_PRO();
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                if (streamInfo == null)
                    throw new Exception("無法獲取音訊流");
                Update_PRO();
                long totalBytes = streamInfo.Size.Bytes;
                if (totalBytes == 0)
                    throw new Exception("無法取得檔案大小");

                string fileName = video.Title;
                string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalidChars)
                {
                    fileName = fileName.Replace(c.ToString(), "_"); // 移除不支援字元
                }
                Update_PRO();
                if (fileName.Length > 100) // 避免 Windows 路徑超過 260 字元
                {
                    fileName = fileName.Substring(0, 100);
                }
                string directory = Folder_Name ?? Environment.CurrentDirectory;
                string baseFilePath = Path.Combine(directory, $"{fileName}.mp3");
                Update_PRO();
                filePath = baseFilePath;
                int count = 1;
                while (File.Exists(filePath)) // 如果檔案已存在，加上編號
                {
                    filePath = Path.Combine(directory, $"{fileName} ({count}).mp3");
                    count++;
                }
                Update_PRO();
                string 檔案大小 = totalBytes / 1024 > 1024 ? (totalBytes / 1024 / 1024).ToString() + "MB" : (totalBytes / 1024).ToString() + "KB";
                now_loading.Invoke(new Action(() => now_loading.Text = video.Title + $"(大小:{檔案大小})"));
                // 分割檔案成多個區塊
                int totalChunks = (int)Math.Ceiling((double)totalBytes / chunkSize);
                Update_PRO();
                var httpClients = Enumerable.Range(0, maxThreads).Select(_ => new HttpClient()).ToArray(); // 建立多個 HttpClient
                var tasks = new List<Task<byte[]>>();
                Update_PRO();
                for (int i = 0; i < totalChunks; i++)
                {
                    int startByte = i * chunkSize;
                    int endByte = Math.Min(startByte + chunkSize - 1, (int)totalBytes - 1);
                    int threadIndex = i % maxThreads; // 分配 HttpClient
                    tasks.Add(DownloadChunk(httpClients[threadIndex], streamInfo.Url, startByte, endByte));
                }
                Update_PRO();
                var results = await Task.WhenAll(tasks); // 同時下載所有區塊
                // 寫入檔案
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 524288, true))
                {
                    foreach (var chunk in results)
                    {
                        await fileStream.WriteAsync(chunk, 0, chunk.Length);
                    }
                }
                progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
                Update_PRO();
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
            return "success";
        }

        /// <summary>
        /// 下載特定區塊的資料
        /// </summary>
        private async Task<byte[]> DownloadChunk(HttpClient client, string url, int startByte, int endByte)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startByte, endByte); // 設定 Range

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }



        private async void Download_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Folder_Name) || string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("請選擇儲存資料夾並輸入網址", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                ProgressBar_Text.Text = $"總用時: {totalTime:hh\\:mm\\:ss}";
                await Task.Run(() => { Task.Delay(10 * 1000); ProgressBar_Text.Invoke(new Action(() => { ProgressBar_Text.Text = ""; }));});
            }
            catch (Exception ex)
            {
                MessageBox.Show($"下載錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Direction.Text = $"儲存路徑: {Folder_Name}";
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
