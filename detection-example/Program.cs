using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet.Extensions;
using SkiaSharp;

using Microsoft.Extensions.Logging;
using CommandLine;

namespace detectionexample
{
    class Program
    {
        static ILogger logger;

        private string? modelFileLocation;
        private string? inputFile;
        private string? outputFolder;
        private bool useGPU = false;

        static void Main(string[] args)
        {
            Console.WriteLine(" ");
            var p = new Program(args);
            p.SetupDetection();
        }

        public Program(string[] args) 
        {
            logger = SetupLogging();
            logger.LogInformation("Parse CLI params");

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

            // check if output folder exists
            if(!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }            
        }

        public void SetupDetection()
        {
            using var yolo = new Yolo(new YoloOptions
            {
                OnnxModel = modelFileLocation,
                ModelType = ModelType.ObjectDetection,  // Model type
                Cuda = useGPU,                           // Use CPU or CUDA for GPU accelerated inference. Default = true
                GpuId = 0,                               // Select Gpu by id. Default = 0
                PrimeGpu = false,                       // Pre-allocate GPU before first. Default = false
            });

            using var image = SKImage.FromEncodedData(inputFile);
            var results = yolo.RunObjectDetection(image, confidence: 0.25, iou: 0.7);
            using var resultsImage = image.Draw(results);

            string outputFile = outputFolder + Path.DirectorySeparatorChar + "detection_result.jpg";
            resultsImage.Save(outputFile, SKEncodedImageFormat.Jpeg, 80);
        }

        private void RunOptions(Options opts)
        {
            inputFile = opts.InputFile;
            outputFolder = opts.OutputFolder;
            modelFileLocation = opts.ModelFileLocation;
            useGPU = opts.UseGPU;
        }

        private void HandleParseError(IEnumerable<Error> errs)
        {
            logger.LogError("Can't parse CLI options.");
            Environment.Exit(1);
        }

        private static ILogger SetupLogging()
        {
            var factory = LoggerFactory.Create(builder => builder
                .AddSimpleConsole((options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss.fff ";                                   
                }))
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug));
            return factory.CreateLogger("Program");                                    
        }        
    }

    public class Options
    {
        [Option('m', "model", Required = true, HelpText = "ONNX model file")]
        public string? ModelFileLocation { get; set; }

        [Option('i', "input", Required = true, HelpText = "Input image to inference")]
        public string? InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output folder where detection result is placed")]
        public string? OutputFolder { get; set; }

        [Option('g', "gpu", Required = false, HelpText = "Use GPU")]
        public bool UseGPU { get; set; }        
    }    
}