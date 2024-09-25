using detection_gui.inference;
using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.WPF;
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
using System.Windows.Threading;

namespace detection_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config config = new Config();
        private string[] images;
        private int imageCount = 0;
        private bool showImages = false;
        private DispatcherTimer timer;
        private InferenceFunctions infFunctions;
        private bool runInference = false;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += timer_Tick;
            
            infFunctions = new InferenceFunctions();
            infFunctions.Config = config;
        }

        private void ChooseInput_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                // Set options here
            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                config.OutputFolder = folderName;
            }
        }

        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChooseModel_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                // Set options here
            };

            if (fileDialog.ShowDialog() == true)
            {
                config.ModelFileLocation = fileDialog.FileName;
            }
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                // Set options here
            };

            if (folderDialog.ShowDialog() == true)
            {
                config.InputFolder = folderDialog.FolderName;
            }
        }

        private void ShowImages_Click(object sender, RoutedEventArgs e)
        {
            if(!showImages)
            {
                images = Directory.GetFiles(config.InputFolder, "*.jpg");
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if(imageCount == images.Length)
            {
                imageCount = 0;
            }
            AnimationImage.Source = new BitmapImage(new Uri(images[imageCount]));
            imageCount++; 
        }

        public void UpdateImage(object sender, EventArgs e)
        {

        }

        private async void Inference_Click(object sender, RoutedEventArgs e)
        {
            runInference = true;
            infFunctions.SetupDetection();
            ModelLabel.Content = "Model: " + infFunctions.Yolo.OnnxModel.ModelType.ToString();
            var imageLocations = Directory.GetFiles(config.InputFolder, "*.jpg");
            foreach (var imageLocation in imageLocations)
            {
                if(!runInference)
                {
                    break;
                }
                var resultImage = infFunctions.inference(imageLocation);
                SKData encoded = resultImage.Encode(SKEncodedImageFormat.Jpeg, 80);
                DetectionImage.Source = WPFExtensions.ToWriteableBitmap(resultImage);
                await Task.Delay(100);
            }
        }

        private void StopInference_Click(object sender, RoutedEventArgs e)
        {
            runInference= false;
        }
    }
}