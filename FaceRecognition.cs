using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Face;

public class FaceRecognition { 
    // ************************* PRIVATE CLASS VARIABLES *************************
    private EigenFaceRecognizer recognizer;
    private DetectFace detection;
    private string InputPath = DataPath.TrainingRecognitionImages;
    

    // ************************* CONSTRUCTOR *************************
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


    // ************************* PUBLIC METHODS *************************
    public void RecogniseAllFaces(Mat src, string origin = "", bool save = false)
    {
        Rect[] faces = detection.GetFaces(src);
        foreach (Rect face in faces)
        {
            Mat faceImage = src.SubMat(face);
            ImageProcessing.NormaliseMat(faceImage);
            RecogniseFace(faceImage, origin, save);
        }
    }


    // ************************* PRIVATE METHODS *************************
    // Returns images sorted by label 
    private void LoadTrainingData(out Mat[] images, out int[] labels) {
        int NumberOfFiles = Directory.GetFiles(InputPath, "*.jpg", SearchOption.AllDirectories).Length;
        images = new Mat[NumberOfFiles];
        labels = new int[NumberOfFiles];

        int labelNumber = 0; 
        int index = 0; 
        foreach (string folder in Directory.EnumerateDirectories(InputPath)) {
            Console.WriteLine("Loading training data from " + Path.GetFileName(folder));
            Console.WriteLine("Label number: " + labelNumber.ToString());

            Console.WriteLine("Loading File: ");
            string[] filePaths = Directory.GetFiles(folder); 
            for (int i = 0; i < filePaths.Length; i++) {
                Console.Write(Path.GetFileName(filePaths[i]) + " "); 

                // Retrieve image 
                Mat image = new Mat(filePaths[i], ImreadModes.Color);
                image = ImageProcessing.NormaliseMat(image);
                
                // Add image and label
                images[index] = image;                
                labels[index] = labelNumber;
                
                index++; 
            }
            Console.WriteLine();

            labelNumber++; 
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
            string predictedLabelString = "Predicted Label " + predictedLabel.ToString() + " ";
            string confidenceString = "Confidence " + ((int)confidence).ToString();
            string FileName = origin + predictedLabelString + confidenceString + ".jpg";
            Cv2.ImWrite(OutputPath + FileName, testImage);
        }  
    }
}