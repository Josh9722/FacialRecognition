using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using OpenCvSharp;

class CameraFeed
{
    public bool reading = true;
    public bool RecognitionRespectsTimer = false; // If true, recognition will only run after the timer has expired
    private bool DetectingFaces = false; 
    private bool RecognisingFaces = false;
    private VideoCapture capture;
    private Mat frame;

    private DetectFace DetectFace;
    private FaceRecognition FaceRecognition;

    // Frame Timings
    private int _FreezeDuration = 0; // How long to freeze frame after a processed frame has been used
    private int _WaitDuration = 0; // How long to wait before attempting to use processedframe 
    public int FreezeTime = 1000; 
    public int WaitTime = 4000;

    // New Frames
    public Mat facesFrame; 
    public Mat processedFrame; // Used for adding a modified frame to the camera feed
    public bool frameReady = false;

    // Threads
    private Thread FaceDetectionThread;
    private Thread FaceRecognitionThread; 
    private static Thread CameraFeedThread; 
    private Thread CountDown; 
    

    // ************ CONSTRUCTOR ************
    public CameraFeed()
    {
        // Init Variables
        frame = new Mat(); // Realtime frame
        facesFrame = frame.Clone(); // Frame with identified faces
        processedFrame = frame.Clone(); // Frame with circled faces
        capture = new VideoCapture(0);

        // Threads        
        FaceDetectionThread = new Thread(DetectFaceInBackground);
        FaceRecognitionThread = new Thread(RecogniseFaceInBackground);
        CountDown = new Thread(durationCountdown);
        CountDown.Start();

        // Start camera feed
        if (CameraFeedThread == null) {
            CameraFeedThread = new Thread(StartFeed);
            CameraFeedThread.Start();
        }
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
            PerformTimedActions();

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

    private void PerformTimedActions() { 
        if (RecognitionRespectsTimer) {
            Console.WriteLine("New recognition...");
            FaceRecognition.RecogniseAllFaces(facesFrame, "", true);
            Console.WriteLine("Recognition complete");
        }
    }

    private void DetectFaceInBackground()
    {
        while (reading)
        {
            while (DetectingFaces) {
                Mat currentFrame = frame.Clone();
                processedFrame = DetectFace.FindFace(currentFrame);
                if (processedFrame == currentFrame)
                { // No face detected if frames are the same  
                    frameReady = false;
                }
                else
                {
                    facesFrame = currentFrame;
                    frameReady = true;
                }
            }
            
        }
    }

    private void RecogniseFaceInBackground() { 
        // Create new array of type Mat to store last faces recognised
        Mat lastRecognition = facesFrame;
        while (reading && RecognisingFaces) {
            if (RecognitionRespectsTimer) {
                continue; 
            }
            if (facesFrame != lastRecognition) { 
                FaceRecognition.RecogniseAllFaces(facesFrame);
                lastRecognition = facesFrame;
            } 
        }
    }


    // ************ PUBLIC METHODS ************
    public void StartFaceDetection(DetectFace detection) { // parms for detectface class
        if (FaceDetectionThread.IsAlive) {
            Console.WriteLine("Already detecting faces");
        } else {
            DetectingFaces = true;
            DetectFace = detection; 
            FaceDetectionThread.Start();
        }
    }

    public void PauseFaceDetection() { 
        if (DetectingFaces) {
            DetectingFaces = false;
        } else {
            Console.WriteLine("Already not detecting faces");
        }
    }

    public void RestartFaceDetection() { 
        if (DetectingFaces) {
            Console.WriteLine("Already detecting faces");
        } else {
            DetectingFaces = true;
        }
    }

    public void StartFaceRecognition(FaceRecognition recognition) { 
        if (RecognisingFaces) {
            Console.WriteLine("Already recognising faces");
        } else {
            RecognisingFaces = true;

            FaceRecognition = recognition;
            FaceRecognitionThread.Start(); 
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