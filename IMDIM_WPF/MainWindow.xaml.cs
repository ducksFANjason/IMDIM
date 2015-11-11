using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace IMDIM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string delimiter = ",";
        string fileName;
        string imageStats;
        string directoryPath;
        string outputPath;
        bool filePathCheck = false;
        bool fileExtCheck = false;
        bool orientationRadio = true;
        bool portraitFavored = true;
        string outputName = "overlay";
        bool errorFlag;
        public bool imagePathValid;
        public bool outputPathValid;

        // Runs the main program - starts processing the images.
        private void RUN_Click(object sender, RoutedEventArgs e)
        {
            errorFlag = false;
            imagePathValid = isPathValid(directoryPath);
            outputPathValid = isPathValid(outputPath);
            if (imagePathValid && outputPathValid)
            {
                string[] fileCount = Directory.GetFiles(directoryPath);
                progressBar1.Maximum = fileCount.Length;
                progressBar1.Value = 0;

                foreach (string filePath in Directory.GetFiles(directoryPath))
                {
                    try
                    {
                        fileName = filePath;
                        string line = SetFileName(fileName, filePath, directoryPath) + delimiter + ReportImage(filePath);
                        System.IO.File.AppendAllText(outputPath + outputName + ".txt", line + Environment.NewLine);
                        progressBar1.Dispatcher.Invoke((Action)(() => progressBar1.Value++), DispatcherPriority.Background);
                    }
                    catch
                    {
                        string error = filePath + ": There was a problem processing this file.";
                        System.IO.File.AppendAllText(outputPath + "exception.txt", error + Environment.NewLine);
                        errorFlag = true;
                        progressBar1.Dispatcher.Invoke((Action)(() => progressBar1.Value++), DispatcherPriority.Background);
                    }
                }
                if (errorFlag != true)
                {
                    System.Windows.MessageBox.Show("Output completed successfully!");
                }

                else
                {
                    System.Windows.MessageBox.Show("Output was completed with errors.\r\nAn exception text log has been generated at the following path:\r\n" + outputPath);
                }
            }

            else
            {
                System.Windows.MessageBox.Show("Please enter valid filepaths for the source images and the output file.");
            }
        }

        // Ingests an image and returns its filename and orientation (portrait/landscape).
        private string FileOrientation(string filePath)
        {
            if (portraitFavored == true)
            {
                System.Drawing.Image image = new Bitmap(filePath);
                if (image.Width > image.Height)
                {
                    image.Dispose();
                    return "Landscape";
                }

                else
                {
                    image.Dispose();
                    return "Portrait";
                }
            }

            else
            {
                System.Drawing.Image image = new Bitmap(filePath);
                if (image.Height > image.Width)
                {
                    image.Dispose();
                    return "Portrait";
                }

                else
                {
                    image.Dispose();
                    return "Landscape";
                }
            }
        }

        // Ingests an image and returns its filename and dimensions (height,width).
        private string ImageDimensions(string filePath)
        {
            System.Drawing.Image image = new Bitmap(filePath);
            string imageHeight = image.Height.ToString();
            string imageWidth = image.Width.ToString();
            image.Dispose();
            return imageHeight + delimiter + imageWidth;
        }

        // Removes the directory path from the file listing, leaving just the file name and extension.
        static string RemovePath(string fileName, string filePath, string directoryPath)
        {
            fileName = filePath.Remove(0, directoryPath.Length);
            return fileName;
        }

        // Removes the extension leaving just the proper file name.
        private string RemoveExt(string fileNameWithExt)
        {
            fileName = fileNameWithExt;
            string[] extensions = new string[] { ".tiff", ".jpeg", ".tif", ".jpg", ".png", ".gif", ".bmp" };
            for (int i = 0; i < extensions.Length; i++)
            {
                int extIndex = fileNameWithExt.IndexOf(extensions[i], StringComparison.OrdinalIgnoreCase);
                int extLength = extensions[i].Length;
                if (extIndex != -1)
                {
                    fileName = fileNameWithExt.Remove(extIndex, extLength);
                    break;
                }
            }
            return fileName;
        }

        // Changes the delimiter in output text file.
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            delimiter = textBox1.Text;
            if (textBox1.Text.Trim().Length == 0)
            {
                delimiter = ",";
            }
        }

        // Sets the directory path to location of images.
        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox2.Text.LastIndexOf("\\") != (textBox2.Text.Length - 1))
            {
                directoryPath = textBox2.Text + "\\";
            }
        }

        // Changes the filename based on user-chosen preferences. 
        private string SetFileName(string fileName, string filePath, string directoryPath)
        {
            if (filePathCheck == false)
            {
                fileName = RemovePath(fileName, filePath, directoryPath);
            }
            if (fileExtCheck == false)
            {
                fileName = RemoveExt(fileName);
            }
            return fileName;
        }

        // Changes how the image stats are returned (orientation vs. dimensions) based on user-chosen preferences.
        private string ReportImage(string filePath)
        {
            if (orientationRadio == true)
            {
                imageStats = FileOrientation(filePath);
            }

            else
            {
                imageStats = ImageDimensions(filePath);
            }
            return imageStats;
        }

        // Activates the option to leave the filepath in the output text file.
        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            filePathCheck = true;
        }

        // Deactivates the option to leave the filepath in the output text file.
        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            filePathCheck = false;
        }

        // Activates the option to leave the doc extension in the output text file.
        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            fileExtCheck = true;
        }

        // Deactivates the option to leave the doc extension in the output text file.
        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            fileExtCheck = false;
        }

        // Copies developer email address to the system clipboard.
        private void EMAIL_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText("jasonurbaniec@gmail.com");
        }

        // Sets the directory where the output text file should be saved.
        private void textBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox3.Text.LastIndexOf("\\") != (textBox3.Text.Length - 1))
            {
                outputPath = textBox3.Text + "\\";
            }
        }

        // Activates option to return results as dimensions (height,width).
        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            orientationRadio = false;
        }

        // Activates option to return results as orientation (portrait / landscape).
        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            orientationRadio = true;
        }

        // Deactivates the default setting of 'Portrait-favored' to set it to 'Landscape-favored'.
        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            portraitFavored = false;
        }

        // Reactivates the default setting of 'Portrait-favored' to set it to 'Landscape-favored'.
        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            portraitFavored = true;
        }

        // Changes name of output text file to user-entered string.
        private void textBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            outputName = textBox4.Text;
            if (textBox4.Text.Trim().Length == 0)
            {
                outputName = "overlay";
            }
        }

        // Opens browse dialogue box for directory path.
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            textBox2.Text = dialog.SelectedPath;
            dialog.Dispose();
        }

        // Opens browse dialgoue box for output path.
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            textBox3.Text = dialog.SelectedPath;
            dialog.Dispose();
        }

        // Checks if the given path is valid.
        private bool isPathValid(string directoryPath)
        {
            try
            {
                Directory.GetFiles(directoryPath);
                return true;
            }

            catch
            {
                return false;
            }
        }
    }
}


