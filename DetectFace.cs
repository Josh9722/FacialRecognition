using OpenCvSharp;

/* 
This class is used for detecting face(s) in an image
This is accomplished through the approach of cascade classifiers (Haar and Lbp)
 - This approach is less accurate but much faster than using alternative machine learning models
 - The issue of lower accuracy in the approach is addressed by the following:
    -> Consistently using different frames from the camera, 
    -> Using multiple cascades
    -> Rejecting faces with a low 'confidence' score
*/ 

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
        if (ListOfCascades.Length == 0) {
            ListOfCascades = GetAllCascades();
        } 

        List<Rect> faces = new List<Rect>();
        
        
        using Mat gray = new Mat();

        // Using multiple cascades to increase accuracy
        foreach (CascadeClassifier cascade in ListOfCascades)
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


    // Draws a circle around the all found faces
    public Mat FindFace(Mat src, params CascadeClassifier[] cascade)
    {
        if (src == null || src.Empty())
        {
            return src; 
        }

        Mat result = src.Clone();
        Mat gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        // Face detection
        Rect[] faces = GetFaces(src, cascade);
        if (faces.Length == 0) {
            return src;
        }

        // Circle all detected faces
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
