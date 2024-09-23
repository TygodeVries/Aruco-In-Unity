using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using TMPro;
using Unity.VisualScripting;
using System;
public class TrackerScanner : MonoBehaviour
{
    [Header("Debug")]
    public TMP_Text debugText;
    public Material renderMaterial;

    [Header("Tracking Settings")]
    public bool isInverted = false; // If the camera input is inverted
    public PredefinedDictionaryName trackerFormat; // The format to read
    public string webCamName = "automatic"; // The name of the camera, If set to "automatic" then the program will find a camera itself. 

    public Vector2 screenResolution { get; private set; }

    WebCamTexture webCamTexture;
    Dictionary dictionary;
    DetectorParameters detectorParameters;

    List<Tracker> trackers;

    /// <summary>
    /// Returns the tracker.
    /// This function always returns a Tracker object, even if it is not being tracked.
    /// Check the state with Tracker.IsTracked()
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Tracker GetTracker(int id)
    {
        for (int i = 0; i < trackers.Count; i++)
        {
            Tracker tracker = trackers[i];
            if (tracker.Id == id)
            {
                return tracker;
            }
        }

        trackers.Add(new Tracker(id, this));
        return GetTracker(id);
    }

    // Start is called before the first frame update
    void Start()
    {
        trackers = new List<Tracker>();
        StartWebcam();

        // Set detector paramaters
        detectorParameters = new DetectorParameters();
    }


    // Material to render the webcamTexture to
    public void StartWebcam()
    {
        Debug.Log("Starting webcam...");
        
        if(webCamName == "automatic")
        webCamName = WebCamTexture.devices[0].name;

        webCamTexture = new WebCamTexture(webCamName, 500, 500);
        webCamTexture.Play();

        // Update the material texture
        if (renderMaterial != null)
            renderMaterial.SetTexture("_MainTex", webCamTexture);

        screenResolution = new Vector2(webCamTexture.width, webCamTexture.height);
    }

    public void Clean()
    {
        for(int i = 0; i < trackers.Count; i++)
        {
            Tracker tracker = (Tracker) trackers[i];
            if(tracker.IsOld())
            {
                trackers.RemoveAt(i);
                i--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!webCamTexture.didUpdateThisFrame)
        {
            return;
        }

        dictionary = CvAruco.GetPredefinedDictionary(trackerFormat);

        Mat image = UnityTextureToOpenCVMat(webCamTexture);

        Point2f[][] corners;
        Point2f[][] rejected;
        int[] ids;
        CvAruco.DetectMarkers(image, dictionary, out corners, out ids, detectorParameters, out rejected);

        if (ids == null)
        {
            return;
        }

        for (int i = 0; i < ids.Length; i++)
        {
            int id = ids[i];
            Tracker tracker = GetTracker(id);

            tracker.LastUpdateTime = DateTime.Now;
            tracker.Cornors = corners[i];
        }


        DrawDebugText();
    }

    public void DrawDebugText()
    {
        debugText.text = $"Total Trackers: {trackers.Count}"; 

        foreach(Tracker tracker in trackers)
        {
            debugText.text += $"\n- Id={tracker.Id} Tracked={tracker.IsTracked()}";
        }
    }

    Mat UnityTextureToOpenCVMat(WebCamTexture webCamTexture)
    {
        Color32[] pixels = webCamTexture.GetPixels32();

        Mat mat = new Mat(webCamTexture.width, webCamTexture.height, MatType.CV_8UC3);

        for(int y = 0; y < webCamTexture.height; y++)
        {
            for(int x = 0; x < webCamTexture.width; x++)
            {
                Color32 color = pixels[y * webCamTexture.width + (webCamTexture.width - x - 1)];
                if (isInverted)
                    color = pixels[y * webCamTexture.width + x];

                mat.Set(y, x, new Vec3b(color.b, color.g, color.r)); // OpenCV is BGR not RGB
            }
        }

        return mat;
    }
}
