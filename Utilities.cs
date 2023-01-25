using OpenCvSharp;


public static class Utilities
{
    // ************ PRIVATE CLASS VARIABLES ************
    private static int imgnum = 0;


    // ************ PUBLIC METHODS ************
    // Saves all faces to path, returns number of faces saved
    public static void SaveAllFaces(Mat src, out int numFaces, string path = "") 
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