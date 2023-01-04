using OpenCvSharp;

class GenerateTrainingImages { 
    // ************ PUBLIC CLASS VARIABLES ************
    public static bool generatingImages = true;


    // ************ PRIVATE CLASS VARIABLES ************
    private static string OutputPath = DataPath.TrainingRecognitionImages;
    private CameraFeed feed; 
    private Mat lastUsedFrame; 


    // ************ PUBLIC METHODS ************
    public GenerateTrainingImages()
    {
        feed = new CameraFeed();

        lastUsedFrame = feed.facesFrame; 
        QueryUser(); 
    }

    public void AddSample(Mat Image, int label) {
        Utilities.SaveAllFaces(Image, OutputPath);
    }


    // ************ PRIVATE METHODS ************
    private void QueryUser() { 
        bool detectionStarted = false;
        bool addingFaces = true;
        int personLabel = 0; 
        while (addingFaces) { 
            Console.WriteLine("Press enter to start training on a new face. Please ensure you are the only face in the frame.");
            Console.WriteLine("Press spacebar to exit out of the training program");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Spacebar)
            {
                addingFaces = false;
                generatingImages = false;
                feed.reading = false;
                return;
            }

            if (key.Key == ConsoleKey.Enter) { 
                if (!detectionStarted) {
                    detectionStarted = true;
                    feed.StartFaceDetection(new DetectFace());
                } else { 
                    feed.RestartFaceDetection(); 
                }

                Console.WriteLine("Starting training on face " + personLabel.ToString());
                GenerationLoop(personLabel, 50);
                personLabel++;

                feed.PauseFaceDetection();
            }
        }
    }
    
    private void GenerationLoop(int personLabel = 0, int numSamples = 10)
    {
        int currentSamples = 0;
        Mat lastUsedSample = new Mat();
        int counter = 0;  
        while (generatingImages && feed.reading && currentSamples < numSamples)
        {
            Mat current = feed.facesFrame.Clone();
            if (feed.frameReady)
            {
                if (lastUsedSample != current) { 
                    int numFacesFound = 0; 
                    TakeNewSample(current, personLabel, out numFacesFound);
                    lastUsedSample = current;
                    currentSamples+= numFacesFound;
                }
            } 
            counter++; 
        }
    }
    
    private void TakeNewSample(Mat frame, int personLabel, out int numFaces) {
        string path = OutputPath + "Person " + personLabel.ToString() + "/";
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        Utilities.SaveAllFaces(frame, out numFaces, path);
    }
}