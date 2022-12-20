using OpenCvSharp;


class DetectFace {


    public void RunTest()
    {
        // Load the cascades
        using var haarCascade = new CascadeClassifier(DataPath.HaarCascade);

        // Detect faces
        Mat haarResult = FindFace(haarCascade);
        
        Cv2.ImShow("Faces by Haar", haarResult);
        Cv2.WaitKey(0);
        Cv2.DestroyAllWindows();
    }


    private Mat FindFace(CascadeClassifier cascade)
    {
        Mat result;

        using (var src = new Mat(DataPath.Yalta, ImreadModes.Color))
        using (var gray = new Mat())
        {
            result = src.Clone();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Detect faces
            Rect[] faces = cascade.DetectMultiScale(
                gray, 1.08, 2, HaarDetectionTypes.ScaleImage, new Size(30, 30));

            // Render all detected faces
            foreach (Rect face in faces)
            {
                var center = new Point
                {
                    X = (int)(face.X + face.Width * 0.5),
                    Y = (int)(face.Y + face.Height * 0.5)
                };
                var axes = new Size
                {
                    Width = (int)(face.Width * 0.5),
                    Height = (int)(face.Height * 0.5)
                };
                Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
            }
        }
        return result;
    }
}
