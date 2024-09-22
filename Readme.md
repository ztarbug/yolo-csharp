# .Net Yolo Examples
This repository contains a number of examples, to show how to use Yolo model inferencing with C#.

# Examples

## Command Line Object Detector

### How to execute
On the commandline use this to build & run program:
```shell
    cd detection-example
    dotnet run -m yolov8n.onnx -o out -i path/to/image.jpg
```
__Parameters__
Following list shows parameters, that you can use.
```shell
  -m, --model     Required. ONNX model file
  -i, --input     Required. Input image to inference
  -o, --output    Required. Output folder where detection result is placed
  -g, --gpu       Use GPU
```

# License
All software in this repo is under the AGPLv3 license and agreement can be found [here](LICENSE).