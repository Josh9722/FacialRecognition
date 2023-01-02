using OpenCvSharp;


public static class Utilities { 
    public static Mat Normalise(Mat src) { 
        Mat result = src.Clone();
        Mat gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.ImShow("Gray", gray);
        Cv2.WaitKey(3000); 
        Cv2.EqualizeHist(gray, gray);
        Cv2.ImShow("Equalized", gray);
        Cv2.WaitKey(3000);
        return gray;
    }
}