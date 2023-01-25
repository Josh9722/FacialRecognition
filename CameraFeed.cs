using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using OpenCvSharp;

class CameraFeed
{
    // ************************* PUBLIC CLASS VARIABLES *************************
    // Configs
    public bool reading = false;
    public bool RecognitionRespectsTimer = false; // If true, recognition will only run after the timer has expired
    public bool SaveRecognitions = false;
    public int FreezeTime = 1000; // How long to freeze frame after a processed frame has been used
    public int WaitTime = 4000; // How long to wait before attempting to use a new processedframe

    // New Frames
    public Mat facesFrame;
    public Mat processedFrame; // Used for adding a modified frame to the camera feed
    public bool frameReady = false;


    // ************************* PRIVATE CLASS VARIABLES *************************
    // Status Markers
    private bool DetectingFaces = false; 
    private bool RecognisingFaces = false;
    
    // Objects 
    private VideoCapture capture;
    private Mat frame;
    private DetectFace DetectFace;
    private FaceRecognition FaceRecognition;

    // Timers 
    private int _FreezeDuration = 0; 
    private int _WaitDuration = 0;  
    
    // Threads
    private Thread FaceDetectionThread;
    private Thread FaceRecognitionThread; 
    private static Thread CameraFeedThread; 
    private Thread CountDown;


    // ************************* CONSTRUCTOR *************************
    public CameraFeed()
    {
        // Init Variables
        frame = new Mat(); // Realtime frame
        facesFrame = frame.Clone(); // Frame with identified faces
        processedFrame = frame.Clone(); // Frame with circled faces
        capture = new VideoCapture(0);

        // Start camera feed
        if (CameraFeedThread == null) {
            Console.WriteLine("Starting camera feed");
            CameraFeedThread = new Thread(StartFeed);
        }

        // Threads        
        FaceDetectionThread = new Thread(DetectFaceInBackground);
        FaceRecognitionThread = new Thread(RecogniseFaceInBackground);
        CountDown = new Thread(durationCountdown);
    }


    // ************************* PRIVATE METHODS *************************
    private void StartFeed()
    {
        reading = true;
        if (!CountDown.IsAlive) {
            CountDown.Start();
        }

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
            Console.WriteLine("Displaying new face detection...");
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
        if (RecognisingFaces) {
            if (RecognitionRespectsTimer)
            {
                Console.WriteLine("New recognition...");
                FaceRecognition.RecogniseAllFaces(facesFrame, save:SaveRecognitions);
            }
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
                FaceRecognition.RecogniseAllFaces(facesFrame, save: SaveRecognitions);
                lastRecognition = facesFrame;
            } 
        }
    }


    // ************************* PUBLIC METHODS *************************
    public void StartCameraFeed() { 
        if (CameraFeedThread.IsAlive) {
            Console.WriteLine("Already reading camera feed");
        } else {
            CameraFeedThread.Start();
        }
    }
    
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


    // ************************* HELPER METHODS *************************
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