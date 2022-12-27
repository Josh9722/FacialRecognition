using OpenCvSharp;


class DetectFace {
    int confidenceThreshold = 80;

    public void RunTest()
    {
        // Load the cascades
        string[] cascadePaths = DataPath.AllCascades();
        CascadeClassifier[] allCascades = new CascadeClassifier[cascadePaths.Length];
        for(int i = 0; i < cascadePaths.Length; i++) {
            allCascades[i] = new CascadeClassifier(cascadePaths[i]);
        }

    }

    private Rect[] GetFaces(Mat src, params CascadeClassifier[] ListOfCascades)
    {
        List<Rect> faces = new List<Rect>();
        CascadeClassifier[] cascades = ListOfCascades;
        using Mat gray = new Mat();


        foreach (CascadeClassifier cascade in cascades)
        {
            int[] rejectLevels;
            double[] levelWeights;
            Rect[] detectedFaces =  cascade.DetectMultiScale(src, out rejectLevels, out levelWeights, 1.08, 2, HaarDetectionTypes.ScaleImage, outputRejectLevels: true);

            foreach (Rect detectedFace in detectedFaces) {
                if (levelWeights[Array.IndexOf(detectedFaces, detectedFace)] < confidenceThreshold) {
                    continue;
                }

                if (!faces.Any(face => face.IntersectsWith(detectedFace) || face.Contains(detectedFace))) { 
                    faces.Add(detectedFace);
                }
            }
        }

        return faces.ToArray();
    }


    private void SaveFacesInNewImages(Mat src, params CascadeClassifier[] cascade) { 
        Rect[] faces = GetFaces(src, cascade);
        Console.WriteLine(faces.Length); // DEBUG
        string path = DataPath.ImagesOutput;
        
        int index = 0;
        foreach (Rect face in faces) {
            var faceImage = src.SubMat(face);
            string number = index.ToString(); 
            Cv2.ImWrite(path + number + ".jpg", faceImage);
            index++; 
        }
    }


    public Mat FindFace(Mat src, params CascadeClassifier[] cascade)
    {
        if (src == null || src.Empty())
        {
            Console.WriteLine("Error: Image is empty");
            return src; 
        }

        Mat result = src.Clone();
        Mat gray = new Mat();

        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        // Detect faces
        Rect[] faces = GetFaces(src, cascade);
        if (faces.Length == 0) {
            return null;
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
