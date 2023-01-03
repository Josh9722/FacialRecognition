using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using OpenCvSharp;

class CameraFeed
{
    public bool reading = true; 
    private VideoCapture capture;
    private Mat frame;
    private int FreezeDuration; // How long to freeze frame
    private int WaitDuration; // How long to wait before taking regular input again 


    public Mat facesFrame; 
    public Mat processedFrame; // Used for adding a modified frame to the camera feed
    public bool frameReady = false;


    private Thread FaceDetection;
    private Thread CountDown; 
    private DetectFace DetectFace; 
    

    // ************ CONSTRUCTOR ************
    public CameraFeed()
    {
        // Init Variables
        frame = new Mat(); // Realtime frame
        facesFrame = frame.Clone(); // Frame with identified faces
        processedFrame = frame.Clone(); // Frame with circled faces
        capture = new VideoCapture(0);
        DetectFace = new DetectFace();

        // Threads        
        FaceDetection = new Thread(DetectFaceInBackground);
        FaceDetection.Start();
        CountDown = new Thread(durationCountdown);
        CountDown.Start();

        // Start camera feed
        Thread CameraFeed = new Thread(StartFeed);
        CameraFeed.Start();
    }
    

    // ************ PRIVATE METHODS ************
    private void StartFeed()
    {
        // Handle errors
        if (!capture.IsOpened())
        {
            Console.WriteLine("Error opening camera");
            return;
        }

        while (reading)
        {
            capture.Read(frame);

            if (frame.Empty() || frame == null)
            {
                Console.WriteLine("No captured frame");
                break;
            }
            if (Cv2.WaitKey(1) == 27) // esc key
            {
                reading = false; 
                break;
            }

            Display();
        }
    }

    private void Display()
    {
        if (FreezeDuration != 0)
        {
            return;
        }

        if (frameReady && WaitDuration == 0)
        {
            frameReady = false;
            Cv2.ImShow("Camera", processedFrame);
            FreezeDuration = 1000;
            WaitDuration = 2000;
        }
        else
        {
            Cv2.ImShow("Camera", frame);
        }
    }

    private void DetectFaceInBackground()
    {
        CascadeClassifier[] allCascades = DetectFace.GetAllCascades();

        while (reading)
        {
            Mat currentFrame = frame.Clone();
            processedFrame = DetectFace.FindFace(currentFrame, allCascades);
            if (processedFrame == currentFrame) { 
                frameReady = false;
            } else {
                facesFrame = currentFrame; 
                frameReady = true;
            }
        }
    }


    // ************ HELPER METHODS ************
    private void durationCountdown()
    {
        int decrement = 100; 
        int wait = 100;
        
        while (reading)
        {
            // Decrement durations
            if (FreezeDuration > 0)
            {
                FreezeDuration -= decrement;
            }
            if (WaitDuration > 0)
            {
                WaitDuration -= decrement;
            }
            
            // Fix for sub 0 durations
            if (FreezeDuration < 0)
            {
                FreezeDuration = 0;
            }
            if (WaitDuration < 0)
            {
                WaitDuration = 0;
            }

            Thread.Sleep(wait);
        }
    }
}