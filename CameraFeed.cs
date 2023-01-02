using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using OpenCvSharp;

class CameraFeed
{
    public bool reading = true; // Can use this from other classes to stop loop if needed
    private VideoCapture capture;
    private Mat frame;
    private Thread faceDetection;
    private Mat processedFrame;
    private bool frameReady = false; // Used to check if frame is ready to be processed
    private DetectFace detectFace = new DetectFace(); 
    
    private int FreezeDuration; // How long to freeze frame on the identified face
    private int WaitDuration; // How long to wait before attempting to detect a face again 



    public CameraFeed()
    {
        // Init
        frame = new Mat();
        capture = new VideoCapture(0);
        Console.WriteLine("Using: " + capture.GetBackendName());
        processedFrame = frame.Clone(); 

        // Start duration countdown thread
        Thread CountDown = new Thread(durationCountdown); 
        CountDown.Start();

        // Start face detection thread
        faceDetection = new Thread(DetectFaceInBackground);
        faceDetection.Start();

        // Start camera feed
        StartFeed();

    }

    private void StartFeed()
    {
        // Handle errors
        if (!capture.IsOpened())
        {
            Console.WriteLine("Error opening camera");
            return;
        }


        // Camera output loop 
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

    private void DetectFaceInBackground()
    {
        // Use all cascades
        string[] cascadePaths = DataPath.AllCascades();
        CascadeClassifier[] allCascades = new CascadeClassifier[cascadePaths.Length];
        for (int i = 0; i < cascadePaths.Length; i++)
        {
            allCascades[i] = new CascadeClassifier(cascadePaths[i]);
        }

        // Detect faces
        while (reading) {
            processedFrame = detectFace.FindFace(frame, allCascades);
            //detectFace.SaveFacesInNewImages(frame, allCascades);
            if (processedFrame != null) {
                frameReady = true;
            } else { 
                frameReady = false;
            }
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
        } else { 
            Cv2.ImShow("Camera", frame);
        }
    }

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