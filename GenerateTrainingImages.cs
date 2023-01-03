using OpenCvSharp;

class GenerateTrainingImages { 
    // ************ PUBLIC CLASS VARIABLES ************
    public static bool generatingImages = true;


    // ************ PRIVATE CLASS VARIABLES ************
    private static string OutputPath = DataPath.TrainingRecognitionImages;
    private CameraFeed feed; 
    private Mat lastFrame; 


    // ************ PUBLIC METHODS ************
    public GenerateTrainingImages()
    {
        feed = new CameraFeed();
        lastFrame = feed.facesFrame; 
        GenerationLoop();
    }

    public void AddSample(Mat Image) {
        Utilities.SaveAllFaces(Image, OutputPath);
    }


    // ************ PRIVATE METHODS ************
    private void GenerationLoop()
    {
        while (generatingImages && feed.reading)
        {
            TakeNewSample();
        }
    }
    
    private void TakeNewSample() {
        if (feed.frameReady && feed.facesFrame != lastFrame) { 
            Utilities.SaveAllFaces(feed.facesFrame, DataPath.ImagesOutput);
        }
    }
}