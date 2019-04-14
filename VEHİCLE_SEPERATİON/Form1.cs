using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;                 //Serial port haberleşmesi için kütüphane
using System.Drawing.Imaging;

using AForge;                          //Görüntü işleme için temel kütüphane
using AForge.Video;                    //Kamera açmak için kullanılan kütüphane
using AForge.Video.DirectShow;         //Kameradan görüntü almak için kullanılan kütüphane
using AForge.Imaging;                  //Resim işlemek için temel kütüphane
using AForge.Imaging.Filters;          //Resim filtreleme kütüphane
using AForge.Imaging.Formats;          //Resim pixellere ayırma kütüphane







namespace VEHİCLE_SEPERATİON
{
    public partial class Form1 : Form
    {
        
        private FilterInfoCollection videoDevices;         //Video aygıtları için değişken atama
        private VideoCaptureDevice videoSource;            //Videodan alınan görüntü için değişken atama

        
        string[] portlar = SerialPort.GetPortNames();   //Usb den okunan seri portları listelemek için dizi atama
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice); //okunan video aygıtlarını yukarıdaki değişkenimize atadık.
            

            foreach (FilterInfo device in videoDevices)
            {
                comboBox3.Items.Add(device.Name);  //Okunan video aygıtlarını combobox'da listeledik
            }

            videoSource = new VideoCaptureDevice();  //Yakalanan görüntüyü yukarıda tanımladığımız "videoSource" değişkenine atadık

            foreach (string port in portlar)
            {
                comboBox1.Items.Add(port);    // combobox da okunan seri portları listeledik
                comboBox1.SelectedIndex = 0;  // Ve  başlangıçta portları ilk sırada olanı seçilsin
            }

            comboBox2.Items.Add("4800");      // ayrı bir combobox'da baudrate(haberleşme hızları) listeledik
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("115200");
            comboBox2.Items.Add("250000");
            comboBox2.SelectedIndex = 1;      //başlangıçta combobox'da ikincisi seçili olsun dedik "9600" olan
        }

        Blob[] blobs;            //pixel sayma işleminde saydırdığımız pixelleri atayacağımız diziyi tanımladık


        Bitmap kaynakResim;      //kaynakResim adlı değişkeni resim değişkeni yani Bitmap olarak tanımladık
        Bitmap siyahBeyazResim;  // yukarıdaki ile aynısı şekilde.

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                
                  
                    kaynakResim = (Bitmap)pictureBox1.Image.Clone();  //pixturebox1 den alı


                    OtsuThreshold otsuFiltre = new OtsuThreshold(); //Resmi otomatik olarak belirli bir eşik değerine göre siyah beyaz yapıyor


                    siyahBeyazResim = otsuFiltre.Apply(kaynakResim.PixelFormat != PixelFormat.Format8bppIndexed ? Grayscale.CommonAlgorithms.BT709.Apply(kaynakResim) : kaynakResim);
                    //siyah beyaz yaptıktan sonra değişkenimize atıyoruz

                    BlobCounterBase bc = new BlobCounter(); //pixel sayma işlemi için sayaç tanımlıyoruz "bc"
                    bc.FilterBlobs = true;

                    //sayaçta pixelleri saydıktan sonra minimum ve maximum yükseklik-genişlik değerlerini belirliyoruz

                    bc.MinHeight = 90; 
                    bc.MinWidth = 200;
                    bc.MaxHeight = 130;
                    bc.MaxWidth = 300;
                    bc.ProcessImage(siyahBeyazResim);      //siyah beyaz resimi işle
                    blobs = bc.GetObjectsInformation();    //pixeller saydırılıyor
                    pictureBox2.Image = siyahBeyazResim;   //siyahbeyaz resimi picturebox2 de gösteriyoruz
                    if (bc.ObjectsCount == 1)              //Eğer belirlediğimiz pixel kısıtlamalarına uygunsa if içerisindeki şartı uygular
                    {
                        label3.Text = "OTOMOBİL";          //label3 te OTOMOBİL yazsın
                        serialPort1.Write("Y");            // Arduino ya Y karakteri yollasın
                           
                    }
                    else if (bc.ObjectsCount == 0)            //pixel boyutları uyuşmuyorsa hiçbirşey yazmasın
                    {
                        label3.Text = " ";

                        //sayaçta pixelleri saydıktan sonra minimum ve maximum yükseklik-genişlik değerlerini belirliyoruz
                        bc.MinHeight = 80;
                        bc.MinWidth = 230;
                        bc.MaxHeight = 130;
                        bc.MaxWidth = 400;
                        bc.ProcessImage(siyahBeyazResim);     //siyah beyaz resimi işle
                        blobs = bc.GetObjectsInformation();   //pixeller saydırılıyor
                        pictureBox2.Image = siyahBeyazResim;  //siyahbeyaz resimi picturebox2 de gösteriyoruz
                        if (bc.ObjectsCount == 1)             //Eğer belirlediğimiz pixel kısıtlamalarına uygunsa if içerisindeki şartı uygular
                        {
                            label3.Text = "OTOBÜS";           //label3 te OTOBÜS yazsın
                            serialPort1.Write("K");           // Arduino ya Y karakteri yollasın
                        }
                        else if (bc.ObjectsCount == 0)       //pixel boyutları uyuşmuyorsa hiçbirşey yazmasın
                        {
                            label3.Text = " ";

                            //sayaçta pixelleri saydıktan sonra minimum ve maximum yükseklik-genişlik değerlerini belirliyoruz
                            bc.MinHeight = 80;
                            bc.MinWidth = 430;
                            bc.MaxHeight = 130;
                            bc.MaxWidth = 1200;
                            bc.ProcessImage(siyahBeyazResim);       //siyah beyaz resimi işle
                            blobs = bc.GetObjectsInformation();     //pixeller saydırılıyor
                            pictureBox2.Image = siyahBeyazResim;    //siyahbeyaz resimi picturebox2 de gösteriyoruz
                            if (bc.ObjectsCount == 1)               //Eğer belirlediğimiz pixel kısıtlamalarına uygunsa if içerisindeki şartı uygular
                            {
                                label3.Text = "TIR";               //label3 te TIR yazsın
                                serialPort1.Write("B");            // Arduino ya Y karakteri yollasın
                            }
                            else if (bc.ObjectsCount == 0)          //pixel boyutları uyuşmuyorsa hiçbirşey yazmasın
                            {
                                label3.Text = " ";

                            }
                        }

                    
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                timer1.Stop();
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();                      //Buton1 e basıldığında timer döngüsü başlasın
            if (serialPort1.IsOpen == false)    
            {
                if(comboBox1.Text=="")        //comboboxda birşey  seçili değilse seçilene kadar döngüyü tekrarlasın
                return;
                serialPort1.PortName = comboBox1.Text;     // seçtiğimiz portu değişkene atasın
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);    //seçtiğimiz haberleşme hızını atıyoruz
                try
                {
                    serialPort1.Open();       //Serial porta bağlı olan aygıtı aktif eder
                    label2.Text = "CONNECT";  //bağlıysa CONNECT yazsın
                    pictureBox3.BackColor = Color.Green;   //Aynı zamanda yeşil renk ile bildirim görünsün
                    

                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata:" + hata.Message);
                    
                }
            }

                else
                {
                    label2.Text="BAĞLANTI KURULDU";

                }

            }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();                       //timer durdur
            if (serialPort1.IsOpen == true)      //Seri port bağlıysa kapatır
            {
                serialPort1.Close();
                label2.Text="DİSCONNECT";         //bağlantı kesilincce DİSCONNECT yazsın
                pictureBox3.BackColor = Color.Red;  //Aynı zamanda kırmızı renk ile bildirim gelsin

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen == true)    //programı kapattığımızda seri port eğer bağlıysa bağlantıyı keser
            {
                serialPort1.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (videoSource.IsRunning)                 //Eğer çalışan bir kamera varsa kapatır
            {
                videoSource.Stop();
                pictureBox1.Image = null;
                pictureBox1.Invalidate();
            }
            else
            {
                videoSource = new VideoCaptureDevice(videoDevices[comboBox3.SelectedIndex].MonikerString); // combobox tan seçtiğmiz kamerayı çalıştırır
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);  //görüntüyü değişkene atar
                videoSource.Start();  
            }
        }
        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap İmage = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = İmage;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) //programı kapatttığımızda eğer kamera çalışıyorsa kapatır
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
            }
        }


        
        }
    }


