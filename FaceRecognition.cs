using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Face;

public class FaceRecognition { 
    private EigenFaceRecognizer recognizer;
    private DetectFace detection;
    private string InputPath = DataPath.TrainingRecognitionImages;
    
    public FaceRecognition() {
        // Create the recognizer
        recognizer = EigenFaceRecognizer.Create();
        detection = new DetectFace(); 

        // Load the training data
        Mat[] images; 
        int[] labels;
        LoadTrainingData(out images, out labels);

        // Train the recognizer
        recognizer.Train(images, labels);
    }

    private void LoadTrainingData(out Mat[] images, out int[] labels) {
        int NumberOfFiles = Directory.GetFiles(InputPath, "*.jpg", SearchOption.AllDirectories).Length;
        Console.WriteLine(NumberOfFiles + " ---");
        images = new Mat[NumberOfFiles];
        labels = new int[NumberOfFiles];

        int labelNumber = 0; 
        int index = 0; 
        foreach (string folder in Directory.EnumerateDirectories(InputPath)) {
            Console.WriteLine("Loading training data from " + Path.GetFileName(folder));
            Console.WriteLine("Label number: " + labelNumber.ToString());
            string[] filePaths = Directory.GetFiles(folder);
            for (int i = 0; i < filePaths.Length; i++) {
                Console.WriteLine("Loading File: " + Path.GetFileName(filePaths[i]) ); 

                Mat image = new Mat(filePaths[i], ImreadModes.Color);
                image = ImageProcessing.NormaliseMat(image);
                images[index] = image;
                
                labels[index] = labelNumber;
                
                index++; 
            }
            labelNumber++; 
        }
    }

    public void RecogniseAllFaces(Mat src, string origin = "", bool save = false) {
        Rect[] faces = detection.GetFaces(src);
        foreach (Rect face in faces) {
            Mat faceImage = src.SubMat(face);
            ImageProcessing.NormaliseMat(faceImage);
            RecogniseFace(faceImage, origin, save);
        }
    }

    private void RecogniseFace(Mat src, string origin, bool save) {
        if (src == null || src.Empty()) { 
            return; 
        }

        // Load the test image
        Mat testImage = src;
        testImage = ImageProcessing.NormaliseMat(testImage);

        // Predict the label of the test image
        int predictedLabel; 
        double confidence; // Low = good match  
        recognizer.Predict(testImage, out predictedLabel, out confidence);


        // Print the predicted label
        Console.WriteLine("Predicted label: " + predictedLabel + " (confidence: " + confidence + ")");
        
        // Save image with file origin name and confidence
        if (save) {
            if (origin != "")
            {
                origin = "From " + origin + "  ";
            }
            string OutputPath = DataPath.ImagesOutput;
            string predictedLabelString = "Predicted " + predictedLabel.ToString();
            string confidenceString = "Confidence " + ((int)confidence).ToString();
            string FileName = origin + predictedLabelString + confidenceString + ".jpg";
            Cv2.ImWrite(OutputPath + FileName, testImage);
        }  
    }
}