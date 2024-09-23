using OpenCvSharp;
using OpenCvSharp.Aruco;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator
{
    // Start is called before the first frame update
    public static void Generate(int id, string outpath)
    {
        Dictionary dir = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_250);
        Mat marketImage = new Mat();
        dir.GenerateImageMarker(id, 100, marketImage);
        marketImage.SaveImage(outpath);
    }
}
