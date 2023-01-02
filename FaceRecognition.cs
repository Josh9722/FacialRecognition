using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Face;

public class FaceRecognition { 
    public FaceRecognition() {
        // Create the recognizer
        var recognizer = EigenFaceRecognizer.Create();

        // Load the training data
        Mat[] images = LoadTrainingImages();
        int[] labels = LoadTrainingLabels();

        // Train the recognizer
        recognizer.Train(images, labels);

        // Load the test image
        Mat testImage = LoadTestImage();

        // Predict the label of the test image
        int predictedLabel = recognizer.Predict(testImage);

        // Print the predicted label
        Console.WriteLine("Predicted label: " + predictedLabel);
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
            Mat image = new Mat(filePaths[i], ImreadModes.Grayscale);
            Cv2.Resize(image, image, new Size(100, 100));
            images[i] = image;
        }
        return images;
    }


    // Each face from the same person should share the same label
    private int[] LoadTrainingLabels() { 
        int num = Directory.GetFiles(DataPath.TrainingRecognitionImages).Length;
        int[] labels = new int[num];
        for (int i = 0; i < num; i++) {
            labels[i] = 1;
        }
        return labels; 
    }


    private Mat LoadTestImage() { 
        // Load image
        string testImagePath = DataPath.RecognitionTestImage;
        Mat testImage = new Mat(testImagePath, ImreadModes.Grayscale);

        // Find face(s) in image
        Rect[] faces = new DetectFace().GetFaces(testImage);

        // Temp: Take first face found
        Rect face = faces[0];

        // Convert face to mat 
        testImage = testImage.SubMat(face);

        // Convert to greyscale and 100x100
        Cv2.CvtColor(testImage, testImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Resize(testImage, testImage, new Size(100, 100));
        return testImage;
    }
}