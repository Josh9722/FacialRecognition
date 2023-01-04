using OpenCvSharp;

static class ImageProcessing { 
    // Pre Processing to increase accuracy of confidence values

    public static List<Mat> GetAllVersions(Mat src) { 
        List<Mat> alternatives = new List<Mat>();
        alternatives.Add(src);
        alternatives.Add(Flipped(src));
        return alternatives; 
    }
    
    public static Mat NormaliseMat(Mat src) { // Should be run on Mat's containing face only
        if (src == null || src.Empty())
        {
            Console.WriteLine("Mat is empty");
            return new Mat();
        }
        
        Mat grey = src.Clone();
        Cv2.CvtColor(src, grey, ColorConversionCodes.BGR2GRAY);
        Mat result = grey.Resize(new Size(200, 200));
        return result;
    }

    public static Mat Flipped(Mat src) { 
        Mat result = src.Clone();
        Cv2.Flip(src, result, FlipMode.X);
        return result;
    }

}