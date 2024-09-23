using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappedTracker : MonoBehaviour
{
    public int id;
    public TrackerScanner scanner;

  

    void Update()
    {
        Tracker tracker = scanner.GetTracker(id);

        if (tracker.IsTracked())
        {
            Vector2 pos = tracker.GetCornorInWorld(0, 16);
            transform.position = pos;
        }
    }
}
