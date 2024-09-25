using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet.Extensions;
using SkiaSharp;
using System.IO;

namespace detection_gui.inference
{
    internal class InferenceFunctions
    {
        private Config config;
        public Config Config { set { config = value; }  get { return config; } }

        private Yolo yolo;
        public Yolo Yolo { get { return yolo; } }

        public void SetupDetection()
        {
            yolo = new Yolo(new YoloOptions
            {
                OnnxModel = config.ModelFileLocation,
                ModelType = ModelType.ObjectDetection,  // Model type
                Cuda = false,                           // Use CPU or CUDA for GPU accelerated inference. Default = true
                GpuId = 0,                               // Select Gpu by id. Default = 0
                PrimeGpu = false,                       // Pre-allocate GPU before first. Default = false
            });
        }

        public SKImage inference(string imageLocation)
        {
            var image = SKImage.FromEncodedData(imageLocation);
            var results = yolo.RunObjectDetection(image, confidence: 0.25, iou: 0.7);
            var resultsImage = image.Draw(results);
            return resultsImage;
        }
    }
}
