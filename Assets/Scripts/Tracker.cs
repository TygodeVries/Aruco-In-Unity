using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker
{

    TrackerScanner trackerScanner;
    internal Tracker(int id, TrackerScanner trackerScanner)
    {
        this.trackerScanner = trackerScanner;
        this.Id = id;
    }

    public bool IsTracked()
    {
        double timeSinceLastUpdate = DateTime.Now.Subtract(LastUpdateTime).TotalMilliseconds;
        return timeSinceLastUpdate < TrackerTimeOut;
    }

    /// <summary>
    /// The ID of the tracker
    /// </summary>
    public int Id;

    /// <summary>
    /// The last time the tracker was updated.
    /// </summary>
    public DateTime LastUpdateTime;

    /// <summary>
    /// How long it should take since the last update for the tracker to be invalid.
    /// </summary>
    public double TrackerTimeOut = 200;

    /// <summary>
    /// The cornors of the trackers last known position
    /// </summary>
    public Point2f[] Cornors;

    public bool IsOld()
    {
        double timeSinceLastUpdate = DateTime.Now.Subtract(LastUpdateTime).TotalMilliseconds;
        return timeSinceLastUpdate > 10 * 1000;   
    }


    /// <summary>
    /// Returns the location of the cornor mapped into the world
    /// Returns from 0 to worldscale
    /// </summary>
    /// <param name="i"></param>
    /// <param name="worldScale">The scale of the world</param>
    /// <returns></returns>
    public Vector2 GetCornorInWorld(int i, float worldScale)
    {
        if(!IsTracked())
        {
            Debug.LogError("Attempting to get tracking data from a non tracked tracker.");
            return new Vector2(0, 0);
        }

        Point2f cornor = Cornors[i];
        Vector2 screen = trackerScanner.screenResolution;

        float a = cornor.X / screen.x;
        if(!trackerScanner.isInverted)
        {
            a = (-a + 1);
        }

        return new Vector2(a * worldScale, cornor.Y / screen.x * worldScale);
    }
}