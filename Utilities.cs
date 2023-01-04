using OpenCvSharp;


public static class Utilities
{
    // ************ PRIVATE CLASS VARIABLES ************
    private static int imgnum = 0;


    // ************ PUBLIC METHODS ************
    public static void SaveAllFaces(Mat src, string path = "") { 
        int numFaces = 0;
        SaveAllFaces(src, out numFaces, path);
    }
    
    public static void SaveAllFaces(Mat src, out int numFaces, string path = "") // Save all faces in a mat to jpgs at path
    {
        numFaces = 0;
        if (src == null || src.Empty())
        {
            return;
        }

        Rect[] faces = new DetectFace().GetFaces(src);
        foreach (Rect face in faces)
        {
            numFaces++;
            SaveFace(src, face, path);
        }
    }
    
    public static void SaveFace(Mat parent, Rect face, string path = "") // Save face as jpg from dimentions in parent mat. 
    {
        if (face == Rect.Empty || face.Size == Size.Zero) {
            return; 
        } 
        if (parent == null || parent.Empty())
        {
            return;
        }

        if (path == "")
        {
            path = DataPath.ImagesOutput; // Default output if none given
        } 

        Mat faceImage = parent.SubMat(face);
        faceImage = ImageProcessing.NormaliseMat(faceImage);
        Cv2.ImWrite(path + imgnum.ToString() + ".jpg", faceImage);
        imgnum++;
    }

}