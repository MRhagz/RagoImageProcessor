using AForge.Video;
using Emgu.CV;
using ImageProcessor.Rago;
using RagoAlgo;
using RagoAlgo;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ImageProcessor
{
    public partial class Form1 : Form
    {
        private enum FilterMode
        {
            None,
            Greyscale,
            Inversion,
            Sepia,
            Laplascian,
            HorzVert,
            AllDirections,
            Lossy,
            Horizontal,
            Vertical,
        }

        private enum Mode
        {
            ImageProcessing,
            VideoProcessing
        }

        private Bitmap imageA;
        private Bitmap imageB;
        private Bitmap resultImage;

        private VideoCapture _capture;
        private Timer _timer;

        private FilterMode filter;
        private Mode mode;

        public Form1()
        {
            InitializeComponent();
            imageA = null;
            imageB = null;
            resultImage = null;
            ConfigureForTwo();
            filter = FilterMode.None;
            mode = Mode.ImageProcessing;

            _capture = new VideoCapture(0);
            _timer = null;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                using (Mat frame = _capture.QueryFrame())
                {
                    if (frame != null)
                    {
                        //Bitmap bitmap = frame.ToBitmap();

                        Bitmap filtered = frame.ToBitmap();
                        switch (filter)
                        {
                            case FilterMode.Greyscale:
                                PixelFilters.ApplyFiter(ref filtered, Rago.PixelFilters.Grayscale);
                                break;
                            case FilterMode.Inversion:
                                PixelFilters.ApplyFiter(ref filtered, Rago.PixelFilters.Invert);
                                break;
                            case FilterMode.Sepia:
                                PixelFilters.ApplyFiter(ref filtered, Rago.PixelFilters.Sepia);
                                break;
                            case FilterMode.Laplascian:
                                //AliacAlgo.AliacAlgo.LaplascianEmboss(filtered);
                                PixelFilters.ApplyConvolutionFilter(ref filtered, Rago.PixelFilters.Laplascian);
                                break;
                            case FilterMode.HorzVert:
                                PixelFilters.ApplyConvolutionFilter(ref filtered, Rago.PixelFilters.HorzVertEmboss);
                                break;
                            case FilterMode.AllDirections:
                                PixelFilters.ApplyConvolutionFilter(ref filtered, Rago.PixelFilters.AllDirectionsEmboss);
                                break;
                            case FilterMode.Lossy:
                                PixelFilters.ApplyConvolutionFilter(ref filtered, Rago.PixelFilters.LossyEmboss);
                                break;
                            case FilterMode.Horizontal:
                                PixelFilters.ApplyConvolutionFilter(ref filtered, Rago.PixelFilters.HorizontalEmboss);
                                break;
                            case FilterMode.Vertical:
                                PixelFilters.ApplyConvolutionFilter(ref filtered, Rago.PixelFilters.VerticalEmboss);
                                break;
                            default:
                                break;

                        }

                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = filtered;
                    }
                }
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null || removeBackgroundToolStripMenuItem.Text.Equals("Edit Image") && (pictureBox3 == null || pictureBox3.Image == null))
            {
                MessageBox.Show("There is no image to save.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg";
            DialogResult result = saveDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            if (pictureBox2.Image != null && !removeBackgroundToolStripMenuItem.Text.Equals("Edit Image"))
            {
                SaveImage(pictureBox2.Image, saveDialog.FileName);
            }
            else
            {
                SaveImage(pictureBox3.Image, saveDialog.FileName);
            }
        }

        private void SaveImage(Image image, string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();

            if (ext == ".jpg" || ext == ".jpeg")
            {
                image.Save(filename, ImageFormat.Jpeg);
            }
            else if (ext == ".png")
            {
                image.Save(filename, ImageFormat.Png);
            }
            else
            {
                // Default to PNG if extension is missing or unrecognized
                filename += ".png";
                image.Save(filename, ImageFormat.Png);
            }
        }


        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog1.FileName))
            {
                return;
            }
            if (imageA != null)
            {
                imageA.Dispose();
            }

            imageA = ResizeImage(Image.FromFile(openFileDialog1.FileName), pictureBox1.Width, pictureBox1.Height);

            pictureBox1.Image = imageA;

            imageB = new Bitmap(imageA.Width, imageA.Height);
            //pictureBox2.Image = imageB;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void basicCopyButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }
            else if (pictureBox1.Image == pictureBox2.Image)
            {
                MessageBox.Show("The image has already been copied.");
                return;
            }



            //for (int y = 0; y < imageA.Height; y++)
            //{
            //    for (int x = 0; x < imageA.Width; x++)
            //    {
            //        Color pixelColor = imageA.GetPixel(x, y);
            //        imageB.SetPixel(x, y, pixelColor);
            //    }
            //}

            imageB = (Bitmap)imageA.Clone();
            pictureBox2.Image = imageB;

            if (imageA == pictureBox1.Image)
            {
                MessageBox.Show("The image has been copied.");
            }
        }

        private void Form1_Close(object sender, EventArgs e)
        {
            imageA.Dispose();
            imageB.Dispose();

            _timer.Stop();
            _capture.Dispose();
            //base.OnFormClosing(e);
        }



        private void greyscaleButton_Click(object sender, EventArgs e)
        {
            if (mode == Mode.VideoProcessing)
            {
                filter = FilterMode.Greyscale;
                return;
            }

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }
            if (imageB == null)
            {
                imageB = new Bitmap(imageA.Width, imageA.Height);
            }

            for (int y = 0; y < imageA.Height; y++)
            {
                for (int x = 0; x < imageA.Width; x++)
                {
                    Color pixelColor = imageA.GetPixel(x, y);
                    int greyValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    Color grey = Color.FromArgb(greyValue, greyValue, greyValue);
                    imageB.SetPixel(x, y, grey);
                }
            }

            pictureBox2.Image = imageB;
            pictureBox2.Refresh();



            MessageBox.Show("The image has been converted to greyscale.");

        }

        private void colorInversionButton_Click(object sender, EventArgs e)
        {
            if (mode == Mode.VideoProcessing)
            {
                filter = FilterMode.Inversion;
                return;
            }

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }
            if (imageB == null)
            {
                imageB = new Bitmap(imageA.Width, imageA.Height);
            }
            for (int y = 0; y < imageA.Height; y++)
            {
                for (int x = 0; x < imageA.Width; x++)
                {
                    Color pixelColor = imageA.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);
                    imageB.SetPixel(x, y, invertedColor);
                }
            }
            pictureBox2.Image = imageB;
            pictureBox2.Refresh();
            MessageBox.Show("The image colors have been inverted.");
        }

        private void histogramButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }
            if (imageB == null)
            {
                imageB = new Bitmap(imageA.Width, imageA.Height);
            }
            int[] magnitude = new int[256];

            for (int y = 0; y < imageA.Height; y++)
            {
                for (int x = 0; x < imageA.Width; x++)
                {
                    Color pixelColor = imageA.GetPixel(x, y);
                    int greyValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    magnitude[greyValue]++;
                }
            }

            imageB = ResizeImage(RagoAlgo.Rago.histogram(imageA, magnitude), imageA.Width, imageA.Height);
            pictureBox2.Image = imageB;
            pictureBox2.Refresh();
            MessageBox.Show("The histogram has been generated.");
        }

        private void sepiaButton_Click(object sender, EventArgs e)
        {
            if (mode == Mode.VideoProcessing)
            {
                filter = FilterMode.Sepia;
                return;
            }
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }
            for (int y = 0; y < imageA.Height; y++)
            {
                for (int x = 0; x < imageA.Width; x++)
                {
                    Color pixelColor = imageA.GetPixel(x, y);
                    Color sepiaColor = convertToSepia(pixelColor);
                    imageB.SetPixel(x, y, sepiaColor);
                }
            }
            pictureBox2.Image = imageB;
            pictureBox2.Refresh();
            MessageBox.Show("The image has been converted to sepia.");
        }

        private Color convertToSepia(Color pixel)
        {
            int red = (int)(pixel.R * 0.393 + pixel.G * 0.769 + pixel.B * 0.189);
            red = Math.Min(255, red);
            int green = (int)(pixel.R * 0.349 + pixel.G * 0.686 + pixel.B * 0.168);
            green = Math.Min(255, green);
            int blue = (int)(pixel.R * 0.272 + pixel.G * 0.534 + pixel.B * 0.131);
            blue = Math.Min(255, blue);

            return Color.FromArgb(red, green, blue);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void configureForCamera()
        {
            Button overlayButton = new Button()
            {
                Name = "OverlayButton",
                Text = "Capture",
            };

            //overlayutton.FlatStyle = FlatStyle.Flat;
            overlayButton.BackColor = Color.FromArgb(180, Color.Black); // semi-transparent background
            overlayButton.ForeColor = Color.White;
            //overlayButton.Dock = DockStyle.Bottom;
            overlayButton.SetBounds(pictureBox1.Width - 200 / 2, pictureBox1.Height, 200, 60); // position inside PictureBox
            overlayButton.Click += new EventHandler(Capture);

            pictureBox1.Controls.Add(overlayButton);

            this.Width = 800;
            this.Height = 600;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 1);
            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 2));
            }
            ClearAll();
            tableLayoutPanel1.PerformLayout();
        }

        private void ConfigureForTwo()
        {
            editToolStripMenuItem.Enabled = true;
            this.Width = 800;
            this.Height = 600;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(pictureBox2, 1, 1);
            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 2));
            }
            ClearAll();
            TwoLabels();
            tableLayoutPanel1.PerformLayout();
        }

        private void ConfigureForThree()
        {
            editToolStripMenuItem.Enabled = false;
            this.Width = 1200;
            this.Height = 600;

            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.RowCount = 3; // Ensure at least two rows
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(pictureBox2, 1, 1);
            pictureBox3 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray // Makes it visible even without an image
            };
            tableLayoutPanel1.Controls.Add(pictureBox3, 2, 1);

            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));
            }


            // BUTTONS
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // For buttons
            Button[] buttons = CreateButtons();
            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                tableLayoutPanel1.Controls.Add(buttons[i], i, 2);
            }
            ClearAll();
            ThreeLabels();
            tableLayoutPanel1.PerformLayout();
        }

        private void ClearAll()
        {
            if (imageA != null)
            {
                imageA.Dispose();
                imageA = null;
            }
            if (imageB != null)
            {
                imageB.Dispose();
                imageB = null;
            }
            if (resultImage != null)
            {
                resultImage.Dispose();
                resultImage = null;
            }
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            if (pictureBox3 != null)
            {
                pictureBox3.Image = null;
            }
        }

        private void ThreeLabels()
        {
            LabelView label1 = new LabelView("Image A", "(Original Image)");
            LabelView label2 = new LabelView("Image B", "(Background Image)");
            LabelView label3 = new LabelView("Image C", "(Processed Image)");
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 1, 0);
            tableLayoutPanel1.Controls.Add(label3, 2, 0);


            tableLayoutPanel1.RowStyles[0] = (new RowStyle(SizeType.AutoSize));
        }

        private void TwoLabels()
        {
            LabelView label1 = new LabelView("Image A", "(Original Image)");
            LabelView label2 = new LabelView("Image B", "(Output Image)");
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 1, 0);
            tableLayoutPanel1.RowStyles[0] = (new RowStyle(SizeType.AutoSize));

        }

        private Button[] CreateButtons()
        {
            Button[] buttons = new Button[3];

            buttons[0] = new Button()
            {
                Name = "loadImageButton",
                Text = "Load Image",
                Dock = DockStyle.Fill,
            };

            buttons[1] = new Button()
            {
                Name = "LoadBackgroundButton",
                Text = "Load Background",
                Dock = DockStyle.Fill,
            };

            buttons[2] = new Button()
            {
                Name = "Subtract",
                Text = "Subtract",
                Dock = DockStyle.Fill
            };

            buttons[0].Click += new EventHandler(OpenImage);
            buttons[1].Click += new EventHandler(OpenImage);
            buttons[2].Click += new EventHandler(ProcessChromaKey);

            return buttons;
        }

        private void Form1_Click2(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Form1_Click1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenImage(object sender, EventArgs e)
        {
            // Only proceed if the user selected a file
            if (openFileDialog1.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog1.FileName))
            {
                return;
            }

            if (sender is Button btn)
            {
                if (btn.Name.Equals("loadImageButton"))
                {
                    if (imageA != null)
                    {
                        imageA.Dispose();
                    }
                    imageA = ResizeImage(Image.FromFile(openFileDialog1.FileName), pictureBox1.Width, pictureBox1.Height);
                    pictureBox1.Image = imageA;
                }
                else if (btn.Name.Equals("LoadBackgroundButton"))
                {
                    if (imageB != null)
                    {
                        imageB.Dispose();
                    }
                    imageB = ResizeImage(Image.FromFile(openFileDialog1.FileName), pictureBox2.Width, pictureBox2.Height);
                    pictureBox2.Image = imageB;
                }
                else
                {
                    MessageBox.Show("Unknown button clicked.");
                }
            }
        }


        private void ProcessChromaKey(object sender, EventArgs e)
        {
            if (imageA == null || imageB == null)
            {
                MessageBox.Show("Make sure Image A and B is not NULL");
                return;
            }

            if (resultImage == null)
            {
                resultImage = new Bitmap(imageA);
                using (Graphics g = Graphics.FromImage(resultImage)) {
                    g.Clear(Color.White);
                }
            }

            removeBackground(imageA, imageB);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigureForThree();
        }

        private void removeBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (removeBackgroundToolStripMenuItem.Text.Equals("Remove Background"))
            {
                ConfigureForThree();
                removeBackgroundToolStripMenuItem.Text = "Edit Image";
            }
            else
            {
                ConfigureForTwo();
                removeBackgroundToolStripMenuItem.Text = "Remove Background";
            }
        }

        private void ClearAll(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        // better background removal
        private void removeBackground(Bitmap a, Bitmap imageB)
        {

            resultImage = new Bitmap(a);
            Color chromaKey = Color.FromArgb(0, 255, 0);

            double threshold = 80;

            for (int y = 0; y < a.Height; y++)
            {
                for (int x = 0; x < a.Width; x++)
                {
                    Color fg = a.GetPixel(x, y);
                    Color bg = imageB.GetPixel(x, y);


                    double distance = ColorDistance(fg, chromaKey);

                    if (distance < threshold)
                    {

                        double alpha = distance / threshold;
                        int r = (int)(fg.R * alpha + bg.R * (1 - alpha));
                        int g = (int)(fg.G * alpha + bg.G * (1 - alpha));
                        int b = (int)(fg.B * alpha + bg.B * (1 - alpha));
                        resultImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                    else
                    {
                        resultImage.SetPixel(x, y, fg);
                    }



                }
            }

            pictureBox3.Image = resultImage;
            pictureBox3.Refresh();
        }

        private bool isGreen(Color p)

        {
            if (p.GetHue() >= 60 && p.GetHue() <= 130 && p.GetBrightness() >= 0.4 && p.GetBrightness() <= 0.3)
                return true;
            return false;
        }

        private double ColorDistance(Color c1, Color c2)
        {
            int rDiff = c1.R - c2.R;
            int gDiff = c1.G - c2.G;
            int bDiff = c1.B - c2.B;
            return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }

        private void useCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (useCameraToolStripMenuItem.Text.Equals("Use Camera"))
                StartCamera();
            else
            {
                closeCamera();
                pictureBox1.Controls.Clear();
            }

        }

        private void StartCamera()
        {
            configureForCamera();
            if (_capture == null)
            {
                _capture = new VideoCapture(0);
            }

            // Start the timer for frame processing
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Interval = 30; // ~30 FPS
                _timer.Tick += ProcessFrame;
            }

            mode = Mode.VideoProcessing;
            useCameraToolStripMenuItem.Text = "Close Camera";
            _timer.Start();
        }


        private void closeCamera()
        {
            ConfigureForTwo();
            if (_timer.Enabled)
            {
                _timer.Stop();
            }

            if (_capture != null)
            {
                _capture.Dispose();
                _capture = null;
            }

            mode = Mode.ImageProcessing;
            filter = FilterMode.None;

            ClearAll();

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;

            useCameraToolStripMenuItem.Text = "Use Camera";
        }

        private void smoothenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultImage?.Dispose();
            resultImage = imageA.Clone() as Bitmap;
            AliacAlgo.AliacAlgo.Smooth(resultImage, 1);
            pictureBox2.Image?.Dispose();
            pictureBox2.Image = resultImage;
        }

        private void guassianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultImage?.Dispose();
            resultImage = imageA.Clone() as Bitmap;
            AliacAlgo.AliacAlgo.GaussianBlur(resultImage, 4);
            pictureBox2.Image?.Dispose();
            pictureBox2.Image = resultImage;
        }

        private void sharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultImage?.Dispose();
            resultImage = imageA.Clone() as Bitmap;
            AliacAlgo.AliacAlgo.Sharpen(resultImage, 11);
            pictureBox2.Image?.Dispose();
            pictureBox2.Image = resultImage;
        }

        private void Emboss(object sender, EventArgs e)
        {
            var buttSender = (ToolStripMenuItem)sender;
            if (buttSender != null)
            {
                //    resultImage?.Dispose();
                resultImage = (Bitmap)imageA.Clone();

                switch (buttSender.Name)
                {
                    case "laplascian":
                        if (mode == Mode.VideoProcessing)
                        {
                            filter = FilterMode.Laplascian;
                            return;
                        }
                        else
                            AliacAlgo.AliacAlgo.LaplascianEmboss(resultImage);
                        break;
                    case "horzVert":
                        if (mode == Mode.VideoProcessing)
                        {
                            filter = FilterMode.HorzVert;
                            return;
                        }
                        else
                            AliacAlgo.AliacAlgo.HorzVertEmboss(resultImage);
                        break;
                    case "allDirections":
                        if (mode == Mode.VideoProcessing)
                        {
                            filter = FilterMode.AllDirections;
                            return;
                        }
                        else
                            AliacAlgo.AliacAlgo.AllDirectionsEmboss(resultImage);
                        break;
                    case "lossy":
                        if (mode == Mode.VideoProcessing)
                        {
                            filter = FilterMode.Lossy;
                            return;
                        }
                        else
                            AliacAlgo.AliacAlgo.LossyEmboss(resultImage);
                        break;
                    case "horizontal":
                        if (mode == Mode.VideoProcessing)
                        {
                            filter = FilterMode.Horizontal;
                            return;
                        }
                        else
                            AliacAlgo.AliacAlgo.HorizontalEmboss(resultImage);
                        break;
                    case "vertical":
                        if (mode == Mode.VideoProcessing) 
                        { 
                            filter = FilterMode.Vertical;
                            return;
                        }
                        else
                            AliacAlgo.AliacAlgo.VerticalEmboss(resultImage);
                        break;
                    default:
                        MessageBox.Show("Unknown emboss type.");
                        break;
                }


                pictureBox2.Image?.Dispose();
                pictureBox2.Image = resultImage;
            }
            else
            {
                MessageBox.Show("Unknown emboss type.");
            }
        }

        private void Capture(Object sender, EventArgs eventArgs)
        {
            if (mode == Mode.VideoProcessing)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg";
                DialogResult result = saveDialog.ShowDialog();

                if (result != DialogResult.OK)
                {
                    return;
                }

                SaveImage(pictureBox1.Image, saveDialog.FileName);
            }
        }

        private void meanRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultImage?.Dispose();
            resultImage = imageA.Clone() as Bitmap;
            AliacAlgo.AliacAlgo.MeanRemoval(resultImage, 9);
            pictureBox2.Image?.Dispose();
            pictureBox2.Image = resultImage;
        }
    }

   

    
    //test
}


