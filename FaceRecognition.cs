using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Face;

public class FaceRecognition { 
    private EigenFaceRecognizer recognizer;
    private DetectFace detection;
    
    public FaceRecognition() {
        // Create the recognizer
        recognizer = EigenFaceRecognizer.Create();
        detection = new DetectFace(); 

        // Load the training data
        Mat[] images = LoadTrainingImages();
        int[] labels = LoadTrainingLabels();

        // Train the recognizer
        recognizer.Train(images, labels);
    }

    private Mat[] LoadTrainingImages() {
        // Set the path to the directory containing the training images
        string trainingDataPath = DataPath.TrainingRecognitionImages;

        // Get a list of all file paths in the directory
        string[] filePaths = Directory.GetFiles(trainingDataPath);

        // Load the images and return them as an array of Mat objects
        Mat[] images = new Mat[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            // Normalise images to greyscale and 100x100
            Mat image = new Mat(filePaths[i], ImreadModes.Color);
            image = ImageProcessing.NormaliseMat(image);
            images[i] = image;
        }
        return images;
    }


    // Each face from the same person should share the same label
    private int[] LoadTrainingLabels() { 
        int num = Directory.GetFiles(DataPath.TrainingRecognitionImages).Length;
        int[] labels = new int[num];
        for (int i = 0; i < num; i++) {
            labels[i] = 0;
        }
        return labels; 
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