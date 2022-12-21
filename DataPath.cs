public static class DataPath { 
    // Images
    public const string ImagesCheck = "Data/Images/ImagesCheck/"; // Check all images in folder
    public const string ImagesOutput = "Data/Images/ImagesOutput/"; // Output folder for images
    public const string PeopleGrid = "Data/Images/PeopleGrid.jpg";
    

    // Cascades
    public const string CascadeFolder = "Data/Cascades/"; // Folder with all cascades
    public const string HaarCascade = "Data/Cascades/haarcascade_frontalface_default.xml"; 
    public const string HaarCascadeAlt = "Data/Cascades/haarcascade_frontalface_alt.xml";

    public static string[] AllCascades() { 
        return Directory.GetFiles(CascadeFolder, "*.xml", SearchOption.AllDirectories);
    }
}