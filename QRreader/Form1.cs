using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing.Aztec;
using EO.WebBrowser;


namespace QRreader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private FilterInfoCollection CaptureDevice;
       private  VideoCaptureDevice FinalFrame;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);

            }
            FinalFrame = new VideoCaptureDevice();
           
        }

       
        private void startbtn_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();

        }
        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
           // pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();

            // Mevcut çerçeveden QR kodunu çözün
            BarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode((Bitmap)eventArgs.Frame.Clone());
            if (result != null)
            {
                if (Uri.IsWellFormedUriString(result.Text, UriKind.Absolute))
                {
                    // Open URL in browser/Eo.WebBrowser (replace with desired method)
                    webView1.LoadUrl(result.Text);
                }
                else
                {
                    textBox2.Text = "Geçersiz URL";
                }
            }


        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame.IsRunning==true)
            {
                FinalFrame.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*
            BarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode((Bitmap)pictureBox1.Image);
            try
            {
                if (result.ToString().Trim() == "")
                {
                    return;
                }
                textBox2.Text = result.ToString().Trim();
            }
            catch (Exception ex)
            { 
            }*/
            try
            {
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode((Bitmap)pictureBox1.Image);
                // textBox2.Text = result.Text;

                if (result != null)
                {
                    /*
                    if (Uri.IsWellFormedUriString(result.Text, UriKind.Absolute))
                    {
                        // URL'yi tarayıcıda açın
                        webBrowser1.Navigate(result.Text);
                    }*/
                    if (Uri.IsWellFormedUriString(result.Text, UriKind.Absolute))
                    {
                        // URL'yi Eo.WebBrowser'da açın
                        webView1.LoadUrl(result.Text);
                    }
                    else
                    {
                        // Geçersiz URL mesajı göster
                        textBox2.Text = "Geçersiz URL";
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                // Geçersiz görüntü hatası
                textBox2.Text = "Hata: Geçersiz Görüntü";
                Console.WriteLine("Hata (ArgumentNullException): " + ex.Message);
            }
            catch (Exception ex)
            {
                // Genel hata yakalama
                textBox2.Text = "Hata: QR kodunu çözümlemede hata oluştu";
                Console.WriteLine("Genel Hata: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Resim Dosyaları";
            saveFile.Filter = "PNG dosyası|*.png";
            if (saveFile.ShowDialog()== DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFile.FileName);
                MessageBox.Show("kayıt işlemi başarılı");
            }
        }
    }
}
