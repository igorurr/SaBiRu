using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public Collider Collider;

    public Platform PrevPlatform;
    public Platform NextPlatform;

    public Vector3 StartPosition;

    public bool StopedMooving { get; private set; }

    public bool IsVisible { get { return RenderCamera.Instance.PlaneIsVisible(this); } }

    public GameObject IndicatorNextPlatform;
    public bool IndicatorNextPlatformIsActive { get; private set; }
    
	void Start() {
        Collider = GetComponent<Collider>();
        StopedMooving = false;

        HideIndicatorNextPlatform();

        StartPosition = transform.position;

        DoStart();
    }
	void Update () {
        DoUpdate();
    }
    protected virtual void DoStart() { }
    protected virtual void DoUpdate() { }

    public List<Platform> GetPlatformsAroundCurrent( int _Count )
    {
        List<Platform> l = new List<Platform> { this };

        Platform prev = PrevPlatform;
        Platform next = NextPlatform;

        for (int i = 0; i >= _Count; i--)
        {
            if( next != null )
            {
                l.Add( next );
                next = next.NextPlatform;
            }

            if ( prev != null )
            {
                l.Insert( 0, prev );
                prev = prev.PrevPlatform;
            }
        }

        return l;
    }

    public void StartMove()
    {
        StopedMooving = false;
    }

    public void StopMove()
    {
        StopedMooving = true;
    }


    public void ShowIndicatorNextPlatform()
    {
        IndicatorNextPlatformIsActive = true;
        IndicatorNextPlatform.SetActive( IndicatorNextPlatformIsActive );
    }

    public void HideIndicatorNextPlatform()
    {
        IndicatorNextPlatformIsActive = false;
        IndicatorNextPlatform.SetActive( IndicatorNextPlatformIsActive );
    }
}