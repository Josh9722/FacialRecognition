using OpenCvSharp;

class GenerateTrainingImages {
    // ************************* PRIVATE CLASS VARIABLES *************************
    private static bool generatingImages = true;
    private static string OutputPath = DataPath.TrainingRecognitionImages;
    private CameraFeed cameraFeed; 
    private DetectFace faceDetection;
    private Mat lastUsedFrame;


    // ************************* CONSTRUCTOR METHODS *************************
    public GenerateTrainingImages()
    {
        cameraFeed = new CameraFeed();
        cameraFeed.FreezeTime = 0; 
        cameraFeed.WaitTime = 10; 
        cameraFeed.StartCameraFeed();

        faceDetection = new DetectFace(); 


        lastUsedFrame = cameraFeed.facesFrame; 
        QueryUser(); 
    }


    // ************************* PRIVATE METHODS *************************
    private void QueryUser() {
        bool detectionStarted = false;
        bool addingFaces = true;
        int personLabel = GetHighestLabel() + 1;
        
        while (addingFaces) {
            // Decision Query
            Console.WriteLine("Press enter to start training on a new face. Please ensure you are the only face in the frame.");
            Console.WriteLine("Press spacebar to exit out of the training program");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Spacebar)
            {
                addingFaces = false;
                generatingImages = false;
                cameraFeed.reading = false;
                return;
            }

            if (key.Key == ConsoleKey.Enter) {
                // Samples Query
                Console.WriteLine("Enter how many image samples you want taken for this person: ");
                if (!int.TryParse(Console.ReadLine(), out int numSamples))
                {
                    Console.WriteLine("Invalid input, defaulting to 10 samples");
                    numSamples = 10;
                }

                // Start taking samples
                if (!detectionStarted) {
                    detectionStarted = true;
                    cameraFeed.StartFaceDetection(faceDetection);
                } else {
                    cameraFeed.RestartFaceDetection(); 
                }

                Console.WriteLine("Starting training on face " + personLabel.ToString());
                GenerationLoop(personLabel, numSamples);
                personLabel++;

                cameraFeed.PauseFaceDetection();
            }
        }
    }
    
    // Loops through and saves numSamples amount face images for the given label 
    private void GenerationLoop(int personLabel = 0, int numSamples = 10)
    {
        Mat lastUsedSample = new Mat();  
        for (int sampleNumber = 0; sampleNumber < numSamples && cameraFeed.reading && generatingImages;) { 
            Mat currentSample = cameraFeed.facesFrame.Clone();
            if (cameraFeed.frameReady && currentSample != lastUsedSample)
            {
                int numFacesFound = 0; 
                TakeNewSample(currentSample, personLabel, out numFacesFound);
                lastUsedSample = currentSample;
                sampleNumber += numFacesFound;
            } 
        }
    }
    
    private void TakeNewSample(Mat frame, int personLabel, out int numFaces) {
        string path = OutputPath + "Person " + personLabel.ToString() + "/";
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        Utilities.SaveAllFaces(frame, out numFaces, path);
    }

    private int GetHighestLabel() { 
        int highestLabel = -1;
        foreach (string name in Directory.GetDirectories(OutputPath)) {
            string[] split = name.Split(' ');
            int label = int.Parse(split[split.Length - 1]);
            if (label > highestLabel) {
                highestLabel = label;
            }
        }
        return highestLabel;
    }
}