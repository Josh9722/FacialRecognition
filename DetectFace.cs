using OpenCvSharp;


class DetectFace {


    public void RunTest()
    {
        // Load the cascades
        string[] cascadePaths = DataPath.AllCascades();
        CascadeClassifier[] allCascades = new CascadeClassifier[cascadePaths.Length];
        for(int i = 0; i < cascadePaths.Length; i++) {
            allCascades[i] = new CascadeClassifier(cascadePaths[i]);
        }

        // Detect faces
        Mat haarResult = FindFace(allCascades);
        Cv2.ImShow("Faces by Haar", haarResult);
        Cv2.WaitKey(0);
        Cv2.DestroyAllWindows();
    }

    private Rect[] GetFaces(params CascadeClassifier[] ListOfCascades)
    {
        List<Rect> faces = new List<Rect>();
        CascadeClassifier[] cascades = ListOfCascades;
        using Mat src = new Mat(DataPath.PeopleGrid, ImreadModes.Color);
        using Mat gray = new Mat();


        foreach (CascadeClassifier cascade in cascades)
        {
            Rect[] detectedFaces = cascade.DetectMultiScale(new Mat(DataPath.PeopleGrid, ImreadModes.Color), 1.08, 2, HaarDetectionTypes.ScaleImage);

            foreach (Rect detectedFace in detectedFaces) {
                if (!faces.Any(face => face.IntersectsWith(detectedFace) || face.Contains(detectedFace))) { 
                    faces.Add(detectedFace);
                }
            }
        }
        Console.WriteLine(faces.Count);

        return faces.ToArray();
    }


    private void SaveFacesInNewImages(params CascadeClassifier[] cascade) { 
        Rect[] faces = GetFaces(cascade);
        Console.WriteLine(faces.Length); // DEBUG
        using var src = new Mat(DataPath.PeopleGrid, ImreadModes.Color);
        string path = DataPath.ImagesOutput;
        
        int index = 0;
        foreach (Rect face in faces) {
            var faceImage = src.SubMat(face);
            string number = index.ToString(); 
            Cv2.ImWrite(path + number + ".jpg", faceImage);
            index++; 
        }
    }


    private Mat FindFace(params CascadeClassifier[] cascade)
    {
        Mat result;

        using (var src = new Mat(DataPath.PeopleGrid, ImreadModes.Color))
        using (var gray = new Mat())
        {
            result = src.Clone();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Detect faces
            Rect[] faces = GetFaces(cascade);

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
