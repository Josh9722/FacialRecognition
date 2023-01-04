using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using OpenCvSharp;

class CameraFeed
{
    public bool reading = true; 
    private bool DetectingFaces = false; 
    private bool RecognisingFaces = false;
    private VideoCapture capture;
    private Mat frame;
    private DetectFace DetectFace;

    // Frame Timings
    private int _FreezeDuration = 0; // How long to freeze frame after a processed frame has been used
    private int _WaitDuration = 0; // How long to wait before attempting to use processedframe 
    public int FreezeTime = 1000; 
    public int WaitTime = 2000;

    // New Frames
    public Mat facesFrame; 
    public Mat processedFrame; // Used for adding a modified frame to the camera feed
    public bool frameReady = false;

    // Threads
    private Thread FaceDetection;
    private Thread FaceRecognition; 
    private Thread CountDown; 
    

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
        FaceRecognition = new Thread(RecogniseFaceInBackground);
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
        if (_FreezeDuration != 0)
        {
            return;
        }

        if (frameReady && _WaitDuration == 0)
        {
            frameReady = false;
            Cv2.ImShow("Camera", processedFrame);
            _FreezeDuration = FreezeTime;
            _WaitDuration = WaitTime;
        }
        else
        {
            Cv2.ImShow("Camera", frame);
        }
    }

    private void DetectFaceInBackground()
    {
        while (reading)
        {
            Mat currentFrame = frame.Clone();
            processedFrame = DetectFace.FindFace(currentFrame); 
            if (processedFrame == currentFrame) { // No face detected if frames are the same  
                frameReady = false;
            } else {
                facesFrame = currentFrame; 
                frameReady = true;
            }
        }
    }

    private void RecogniseFaceInBackground() { 
        while (reading) {

        }
    }


    // ************ PUBLIC METHODS ************
    public void StartFaceDetection() { // parms for detectface class
        if (DetectingFaces) {
            Console.WriteLine("Already detecting faces");
        } else {
            DetectingFaces = true;
            FaceDetection.Start();
        }
    }

    public void StartFaceRecognition(FaceRecognition recognition) { 
        if (RecognisingFaces) {
            Console.WriteLine("Already recognising faces");
        } else {
            RecognisingFaces = true;
            FaceRecognition.Start(); 
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
            if (_FreezeDuration > 0)
            {
                _FreezeDuration -= decrement;
            }
            if (_WaitDuration > 0)
            {
                _WaitDuration -= decrement;
            }
            
            // Fix for sub 0 durations
            if (_FreezeDuration < 0)
            {
                _FreezeDuration = 0;
            }
            if (_WaitDuration < 0)
            {
                _WaitDuration = 0;
            }

            Thread.Sleep(wait);
        }
    }
}