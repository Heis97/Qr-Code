using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Qr_Code
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "https://www.google.com/";
            textBox2.Text = "Qrcod1";
            textBox3.Text = "1.0";

            textBox4.Text = "3";

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            QRCodeWriter qrEncode = new QRCodeWriter(); //создание QR кода

            string str = textBox1.Text;  //строка на русском языке
            double k = Convert.ToDouble(textBox3.Text)/2;
            string name_dxf = textBox2.Text;
            int pix = Convert.ToInt32(textBox4.Text);
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();    //для колекции поведений
            hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");   //добавление в коллекцию кодировки utf-8
            BitMatrix qrMatrix = qrEncode.encode(   //создание матрицы QR
                str,                 //кодируемая строка
                BarcodeFormat.QR_CODE,  //формат кода, т.к. используется QRCodeWriter применяется QR_CODE
                25,                    //ширина
                25,                    //высота
                hints);                 //применение колекции поведений
            BarcodeWriter qrWrite = new BarcodeWriter();    //класс для кодирования QR в растровом файле
            Bitmap qrImage = qrWrite.Write(qrMatrix);   //создание изображения
            try
            {
                qrImage.Save("1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);//сохранение изображения
            }
            catch
            {

            }
            
            pictureBox1.Image = (Image)(new Bitmap((Image)qrImage, pictureBox1.Width, pictureBox1.Height));
            BarcodeReader qrDecode = new BarcodeReader(); //чтение QR кода
            Result text = qrDecode.Decode((Bitmap)Bitmap.FromFile("1.bmp")); //декодирование растрового изображения

            var dxf = new DxfMaker(name_dxf);
            for (int x = 0; x < qrImage.Width; x += pix)
            {
                for (int y = 0; y < qrImage.Height; y += pix)
                {
                    if (qrImage.GetPixel(x, y).R < 100)
                    {
                        dxf.addPoint(x, y);
                    }
                }
            }
            dxf.saveDxf(name_dxf+".dxf");
        }
    }
    public class DxfMaker
    {
        string name;
        string begintext;
        string middletext;
        string endtext;
        string text;
        string textAll;
        int count;
        public DxfMaker(string iName)
        {
            name = iName;
            textAll = "";
            count = 75;
        }

        public void addPoint(float x, float y)
        {
            text += "POINT\n5\n" + count.ToString() + "\n330\n1F\n100\nAcDbEntity\n8\n0\n6\nCONTINUOUS\n62\n7\n100\nAcDbPoint\n10\n" + x.ToString() + "\n20\n" + y.ToString() + "\n30\n0.0\n0\n";
            count++;
        }
        public void saveDxf(string fpath)
        {
            extractTexts();
            textAll = begintext+"\n"+ count.ToString() + "\n" + middletext + "\n" + text + endtext;
            try
            {
                System.IO.StreamWriter fio = new System.IO.StreamWriter(fpath);
                fio.Write(textAll);
                fio.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private string readFile(string path)
        {
            string file1="";
            using (StreamReader sr = new StreamReader(path))
            {
                file1 = sr.ReadToEnd();
            }
            return file1;
        }
        private void extractTexts()
        {
            begintext = readFile("Texts\\beginText.txt");
            middletext = readFile("Texts\\middleText.txt");
            endtext = readFile("Texts\\endText.txt");
        }


    }
    
}