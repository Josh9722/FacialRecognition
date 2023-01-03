using OpenCvSharp;


class DetectFace {

    public CascadeClassifier[] GetAllCascades()
    {
        string[] cascadePaths = DataPath.AllCascades();
        CascadeClassifier[] allCascades = new CascadeClassifier[cascadePaths.Length];
        for(int i = 0; i < cascadePaths.Length; i++) {
            allCascades[i] = new CascadeClassifier(cascadePaths[i]);
        }
        return allCascades; 
    }

    // Returns a list of faces in an image
    public Rect[] GetFaces(Mat src, params CascadeClassifier[] ListOfCascades)
    {
        CascadeClassifier[] cascades; 
        if (ListOfCascades.Length == 0) {
            // Use all cascades
            string[] cascadePaths = DataPath.AllCascades();
            CascadeClassifier[] allCascades = new CascadeClassifier[cascadePaths.Length];
            for (int i = 0; i < cascadePaths.Length; i++)
            {
                allCascades[i] = new CascadeClassifier(cascadePaths[i]);
            }
            cascades = allCascades;
        } else {
            cascades = ListOfCascades;
        }
        List<Rect> faces = new List<Rect>();
        
        
        using Mat gray = new Mat();

        // Using multiple cascades to increase accuracy
        foreach (CascadeClassifier cascade in cascades)
        {
            int[] rejectLevels;
            double[] levelWeights;
            Rect[] detectedFaces =  cascade.DetectMultiScale(src, out rejectLevels, out levelWeights, 1.08, 2, HaarDetectionTypes.ScaleImage, outputRejectLevels: true);

            foreach (Rect detectedFace in detectedFaces) {
                // Reject faces with low confidence
                double l = levelWeights[Array.IndexOf(detectedFaces, detectedFace)]; 
                float r = rejectLevels[Array.IndexOf(detectedFaces, detectedFace)];
                if (l <= 5) { 
                    continue; 
                }

                // Reject faces that are already in the list (from previous cascades)
                if (!faces.Any(face => face.IntersectsWith(detectedFace) || face.Contains(detectedFace))) { 
                    faces.Add(detectedFace);
                }
            }
        }

        return faces.ToArray();
    }


    // Circle found faces
    public Mat FindFace(Mat src, params CascadeClassifier[] cascade)
    {
        if (src == null || src.Empty())
        {
            return src; 
        }

        Mat result = src.Clone();
        Mat gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        // Detect faces
        Rect[] faces = GetFaces(src, cascade);
        if (faces.Length == 0) {
            return src;
        }

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

        return result;
    }
}
