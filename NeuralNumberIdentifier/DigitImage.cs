using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNumberIdentifier
{
    class DigitImage
    {
        public int width { get; set; }
        public int height { get; set; }
        public byte[][] pixels { get; set; }
        public byte label { get; set; }

        public DigitImage(int width, int height, byte[][] pixels, byte label)
        {
            this.width = width; this.height = height;
            this.pixels = new byte[height][];
            for (int i = 0; i < this.pixels.Length; ++i)
                this.pixels[i] = new byte[width];
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    this.pixels[i][j] = pixels[i][j];
            this.label = label;
        }

        public static DigitImage[] LoadData(string pixelFile, string labelFile, int numImages)
        {
            DigitImage[] result = new DigitImage[numImages];
            byte[][] pixels = new byte[28][];
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = new byte[28];
            FileStream ifsPixels = new FileStream(pixelFile, FileMode.Open);
            FileStream ifsLabels = new FileStream(labelFile, FileMode.Open);
            BinaryReader brImages = new BinaryReader(ifsPixels);
            BinaryReader brLabels = new BinaryReader(ifsLabels);
            int magic1 = brImages.ReadInt32();
            magic1 = ReverseBytes(magic1);
            int imageCount = brImages.ReadInt32();
            imageCount = ReverseBytes(imageCount);
            int numRows = brImages.ReadInt32();
            numRows = ReverseBytes(numRows);
            int numCols = brImages.ReadInt32();
            numCols = ReverseBytes(numCols);
            int magic2 = brLabels.ReadInt32();
            magic2 = ReverseBytes(magic2);
            int numLabels = brLabels.ReadInt32();
            numLabels = ReverseBytes(numLabels);
            for (int di = 0; di < numImages; ++di)
            {
                for (int i = 0; i < 28; ++i)
                {
                    for (int j = 0; j < 28; ++j)
                    {
                        byte b = brImages.ReadByte();
                        pixels[i][j] = b;
                    }
                }
                byte lbl = brLabels.ReadByte();
                DigitImage dImage = new DigitImage(28, 28, pixels, lbl);
                result[di] = dImage;
            }
            ifsPixels.Close(); brImages.Close();
            ifsLabels.Close(); brLabels.Close();
            return result;
        }

        public static int ReverseBytes(int v)
        {
            byte[] intAsBytes = BitConverter.GetBytes(v);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }

        public static Bitmap GetBitmap(DigitImage dImage, int mag)
        {
            int width = dImage.width * mag;
            int height = dImage.height * mag;
            Bitmap result = new Bitmap(width, height);
            Graphics gr = Graphics.FromImage(result);
            for (int i = 0; i < dImage.height; i++)
            {
                for (int j = 0; j < dImage.width; j++)
                {
                    int pixelColor = dImage.pixels[i][j];
                    Color c = Color.FromArgb(pixelColor, pixelColor, pixelColor);
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * mag, i * mag, mag, mag);
                }
            }
            return result;
        }

        public static double[] GetInput(DigitImage image)
        {
            double[] result = new double[image.width * image.height];
            int k = 0;
            for (int i = 0; i < image.width; i++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    result[k] = ((double)image.pixels[i][j]) / 255.0;
                    k++;
                }
            }
            return result;
        }
    }
}
