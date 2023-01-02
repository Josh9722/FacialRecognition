public static class DataPath {

    // Face Recognition
    public const string TrainingRecognitionImages = "Data/Images/FaceRecognitionTraining/"; // Folder with all images to train recognition algorithm
    public const string RecognitionTestImage = "Data/Images/RecognitionTestImage.jpg"; // Test image for recognition algorithm
    public const string a = "Data/Images/ImagesCheck/a.jpg";
    public const string b = "Data/Images/ImagesCheck/b.jpg";


    // Images Folders
    public const string ImagesCheck = "Data/Images/ImagesCheck/"; // Check all images in folder
    public const string ImagesOutput = "Data/Images/ImagesOutput/"; // Output folder for images
    public const string PeopleGrid = "Data/Images/PeopleGrid.jpg"; // Sample image for face detection
    

    // Cascades (for Face Detections)
    public const string CascadeFolder = "Data/Cascades/"; // Folder with all cascades
    public const string HaarCascade = "Data/Cascades/haarcascade_frontalface_default.xml"; 
    public const string HaarCascadeAlt = "Data/Cascades/haarcascade_frontalface_alt.xml";

    public static string[] AllCascades() { 
        return Directory.GetFiles(CascadeFolder, "*.xml", SearchOption.AllDirectories);
    }
}