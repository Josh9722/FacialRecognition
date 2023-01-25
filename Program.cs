using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;




namespace FacialRecognition
{
    public static class Program
    {
        // ************************* PRIVATE CLASS VARIABLES *************************
        private static bool RequiresTraining = false;


        // ************************* OBJECT INSTANTIATIONS *************************
        private static DetectFace detectFace = new DetectFace();
        private static FaceRecognition faceRecognition = new FaceRecognition();
        private static CameraFeed cameraFeed = new CameraFeed(); 


        public static void Main(string[] args)
        {
            if (RequiresTraining)
            {
                new GenerateTrainingImages();
                return; 
            }
            
            SampleDetection(DataPath.PeopleGrid); // Finds and circles faces in the image at the given path
            SampleRecognition(DataPath.RecognitionTestImage); // Console outputs prediction for each face in the image at the given path 
            
            RunMainProgram(); 
        }


        // The main program displays back the users camera feed
        // Every duration run face detection -> finds and circles all faces
        // Every (setable) duration run face recognition on those detected faces
        private static void RunMainProgram() {
            // Config
            cameraFeed.FreezeTime = 1000;
            cameraFeed.WaitTime = 4000;
            cameraFeed.RecognitionRespectsTimer = true;
            cameraFeed.SaveRecognitions = false;
            

            // Start camera processes 
            cameraFeed.StartCameraFeed();
            cameraFeed.StartFaceDetection(detectFace);
            cameraFeed.StartFaceRecognition(faceRecognition);
        }


        // Displays image with all detected faces circled 
        private static void SampleDetection(string path) { 
            Console.WriteLine("\n --- Sample Detection Started --- ");
            Mat image = new Mat(path, ImreadModes.Color); // New material of the image at path
            Mat result = detectFace.FindFace(image); // Material of the image with the faces circled
            
            bool running = true;
            while (running) { 
                Cv2.ImShow("Detection Example", result);
                if (Cv2.WaitKey(1) == 27) {
                    Cv2.DestroyWindow("Detection Example");
                    running = false; 
                }
            }
            Console.WriteLine("--- Sample Detection Ended --- \n");
        }


        // Outputs in console log -> prediction of who the person is (label) with a confidence value
        private static void SampleRecognition(string path)
        {
            Console.WriteLine("\n --- Sample Recognition Started --- ");
            Mat image = new Mat(path, ImreadModes.Color); 
            faceRecognition.RecogniseAllFaces(image, save:true); // Default save location is Images/ImagesOutput
            Console.WriteLine(" --- Sample Recognition Ended --- \n");
        }
    }
}
