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
        public static bool RequiresTraining = false; 
        private static FaceRecognition? faceRecognition; 

        public static void Main(string[] args)
        {
            faceRecognition = new FaceRecognition();

            if (RequiresTraining) {
                new GenerateTrainingImages();
            } 

            faceRecognition.RecogniseAllFaces(new Mat(DataPath.a, ImreadModes.Color), "b", true);
            
        }
        
    }
}
