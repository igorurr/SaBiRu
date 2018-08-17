using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public Collider Collider;

    public Platform PrevPlatform;
    public Platform NextPlatform;

    public bool IsVisible { get { return RenderCamera.Instance.PlaneIsVisible(this); } }

	// Use this for initialization
	void Start() {
        Collider = GetComponent<Collider>();

    }
	
	// Update is called once per frame
	void Update () {
    }

    public List<Platform> GetPlatformAfterCurrent(int _AfterCurentFirst, int _AfterCurentLast)
    {
        List<Platform> l = new List<Platform>();
        Platform cur = this;

        for (int i = 0; i <= _AfterCurentLast; i++)
        {
            if (i >= _AfterCurentFirst)
                l.Add(cur);

            if ( cur.NextPlatform == null )
                break;
            cur = cur.NextPlatform;
        }

        return l;
    }

    public List<Platform> GetPlatformBeforeCurrent(int _AfterCurentFirst, int _AfterCurentLast)
    {
        List<Platform> l = new List<Platform>();
        Platform cur = this;

        for (int i = 0; i >= _AfterCurentFirst; i--)
        {
            if (i <= _AfterCurentLast)
                l.Insert(0,cur);

            if( cur.PrevPlatform == null )
                break;
            cur = cur.PrevPlatform;
        }

        return l;
    }
}
