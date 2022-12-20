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
    public static class ImageProcessing
    {
        public static void Main(string[] args)
        {
            DetectFace face = new DetectFace();
            face.RunTest();  
            
        }
        
    }
}
