using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gemini
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GeminiTextRequest geminiTextRequest = new GeminiTextRequest();
        private bool isSave;
        private bool isClose;
        private string prevInput = "";

        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Closing += MainWindow_Closing;
            StateChanged += MainWindow_StateChanged; //MetroWindow_StateChanged;
            SizeChanged += MetroWindow_SizeChanged;

            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;

            cmBoxTalks.Items.Add("");
            cmBoxTalks.Items.Add("Lütfen");
            cmBoxTalks.Items.Add("Affedersin ");
            cmBoxTalks.Items.Add("Lütfen bana asp.net 7'de söyleyin ");
            cmBoxTalks.Items.Add("WPF'de ");
            cmBoxTalks.Items.Add("Yukarıdaki konunun devamı olarak lütfen sorunuz ");
            cmBoxTalks.Items.Add("Lütfen bana abp vnext'te söyleyin ");
            cmBoxTalks.SelectedIndex = 0;

            panelUser.IsEnabled = true;
            chkTrans.IsChecked = false;
        }


        private bool checkInput()
        {
            panelUser.IsEnabled = false;

            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                panelUser.IsEnabled = true;
                return false;
            }

            txtResult.Text += "\r\n\r\n";
            for (int i = 0; i < 100; i++)
            {
                txtResult.Text += "*";
            }
            txtResult.Text += $"\r\n\r\n问： {txtInput.Text}\r\n\r\n";
            prevInput = txtInput.Text;
            txtResult.CaretIndex = txtResult.Text.Length;
            txtResult.ScrollToEnd();

            return true;
        }
        private async Task SubmitMsgAsync()
        {

            if (!checkInput()) return;

            string queryStr = txtInput.Text + "(Please find relevant materials in English)";

            if ((bool)chkTrans.IsChecked)
            {
                queryStr += ",Please answer in English and Chinese.";
            }

            queryStr += ",thank you！";

            GeminiTextResponse geminiTextResponse = await geminiTextRequest.SendMsg(txtInput.Text);
            if (geminiTextResponse != null)
            {
                txtResult.Text += $"答： {geminiTextResponse.candidates[0].content.parts[0].text}\r\n\r\n";
            }

            if (isSave && !string.IsNullOrWhiteSpace(txtResult.Text))
            {
                try
                {
                    string dir = $"D:\\Projects\\2023\\OpenAI\\docs\\{DateTime.Now.ToString("yyyyMMdd")}";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    string path = $"{dir}\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{geminiTextResponse.candidates[0].content.parts[0].text
            .Replace("Bu konuşmanın özetini lütfen bir Çince cümlede belirtin:\n\n*", "").Replace("\n", "").Replace("*", "")}.md";

                    File.WriteAllText(path, txtResult.Text);

                    isSave = false;

                    if (isClose && File.Exists(path))
                        Application.Current.Shutdown();

                    txtResult.Text = "";

                }
                catch (Exception err)
                {
                    txtResult.Text += err.Message + "\r\n\r\n";
                }
            }

            txtInput.Text = "";
            panelUser.IsEnabled = true;

        }

        #region Controls Event

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Text = cmBoxTalks.Text + txtInput.Text;
            await SubmitMsgAsync();
        }

        private async void txtInput_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    txtInput.Text += "\r\n";
                    txtInput.CaretIndex = txtInput.Text.Length;
                }
                else
                {
                    txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 2);
                    txtInput.Text = cmBoxTalks.Text + txtInput.Text;
                    txtInput.CaretIndex = txtInput.Text.Length;
                    await SubmitMsgAsync();

                }
            }
            else if (e.Key == Key.Up)
            {
                if (!string.IsNullOrWhiteSpace(prevInput))
                {
                    txtInput.Text = string.IsNullOrWhiteSpace(cmBoxTalks.Text) ? prevInput : prevInput.Replace(cmBoxTalks.Text, "");
                    txtInput.CaretIndex = txtInput.Text.Length;
                    //await SubmitMsgAsync();
                }
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtResult.Text))
            {
                txtInput.Text = "Please summarize this converstation in one sentence in Chinese.";
                isSave = true;
                isClose = false;
                chkTrans.IsChecked = false;
                await SubmitMsgAsync();
                //chkTrans.IsChecked = true;

            }
        }

        private async void btnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtResult.Text))
            {
                txtInput.Text = "Please summarize this converstation in one sentence in Chinese.";
                isSave = true;
                isClose = true;
                chkTrans.IsChecked = false;

                await SubmitMsgAsync();
                //chkTrans.IsChecked = true;

                //Visibility = Visibility.Hidden;
            }
        }
        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Width = SystemParameters.WorkArea.Width;
                Height = SystemParameters.WorkArea.Height;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                Width = 810;
            }
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            txtInput.Width = Width - panelBtn.Width - cmBoxTalks.Width;
            txtResult.Height = Height - panelUser.Height - 50;
        }

        private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtResult.Text))
            {
                var result = MessageBox.Show("Çıkış öncesinde kaydetmek ister misiniz? Evet seçeneği ile kaydedip çıkabilir, Hayır seçeneği ile direkt çıkabilirsiniz.", "Uyarı", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);


                if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = true;

                    txtInput.Text = "Please summarize this converstation in one sentence in Chinese.";
                    isSave = true;
                    isClose = true;

                    await SubmitMsgAsync();

                    //Visibility = Visibility.Hidden;
                }
            }
        }

        private async void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtResult.Text))
            {
                //txtInput.Text = "Please summarize this converstation in one sentence in Chinese.";               
                //await SubmitMsgAsync();                
                //isClose = false;

                if (MessageBox.Show("Emin misiniz? İptal etmek için İptal'i, devam etmek için Tamam'ı seçin.", "Uyarı", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)

                    txtResult.Text = "";
            }
        }

        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCreateImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbmAssistants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion






    }
}