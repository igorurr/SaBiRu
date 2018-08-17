using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCamera : MonoBehaviour {

    public static RenderCamera Instance;

    Plane[] m_CameraPlanes;
    Camera m_Camera;

    Vector3 m_FromLookAt;
    Vector3 m_ToLookAt;

    Vector3 m_FromPosition;
    Vector3 m_ToPosition;

    // Use this for initialization
    void Start () {
        Instance = this;

        m_Camera = GetComponent<Camera>();
        m_CameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_Camera);

        RootPlayers.MainPlayer.OnFinishedJump += BasePlayer_OnFinishedJump;

        GetNewLookAt( new Tuple<Platform, Platform> ( RootPlatforms.Instance.StartPlatform, RootPlatforms.Instance.StartPlatform ) );
        GetNewPosition( new Tuple<Platform, Platform> ( RootPlatforms.Instance.StartPlatform, RootPlatforms.Instance.StartPlatform ) );
    }
	
	// Update is called once per frame
	void Update () {
        ProcessPositionCamera();

    }

    private void OnDestroy()
    {
        RootPlayers.MainPlayer.OnFinishedJump -= BasePlayer_OnFinishedJump;
    }

    public bool PlaneIsVisible( Platform p )
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(m_Camera), p.Collider.bounds);
    }

    public event Action<Ray> OnClickGameZone;

    public void PlatformsClickedZoneClick()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        OnClickGameZone(ray);
    }

    void ProcessPositionCamera()
    {
        transform.position =
            // тут должно быть условие проверки на горизонтальную или вертикальную камеру
            GlobalSetings.Instance.CameraHorisontalPositionAbovePlatform +
            Vector3.Lerp( m_FromPosition, m_ToPosition, RootPlayers.MainPlayer.CurrentJump.PositionBetweenPlatforms );


        /*transform.rotation = Quaternion.Euler(
            //GlobalSetings.Instance.DefaultCameraRotation + 
            new Vector3( 0, 0, 0 )
        );*/
        
        transform.LookAt( Vector3.Lerp( m_FromLookAt, m_ToLookAt, RootPlayers.MainPlayer.CurrentJump.PositionBetweenPlatforms ) );
        transform.rotation *= Quaternion.Euler( GlobalSetings.Instance.DefaultCameraRotation );
    }

    void GetNewLookAt(Tuple<Platform, Platform> _Platforms)
    {
        List<Platform> p1 = _Platforms.Item1.GetPlatformAfterCurrent(0,2);
        List<Platform> p2 = _Platforms.Item2.GetPlatformAfterCurrent(0,2);

        Vector3 sum1 = new Vector3();
        foreach (Platform v in p1)
            sum1 += v.transform.position;

        Vector3 sum2 = new Vector3();
        foreach (Platform v in p2)
            sum2 += v.transform.position;

        m_FromLookAt = sum1 / p1.Count;
        m_ToLookAt = sum2 / p2.Count;
    }

    void GetNewPosition(Tuple<Platform, Platform> _Platforms)
    {
        List<Platform> p1 = _Platforms.Item1.GetPlatformBeforeCurrent(-3,-1);
        p1.AddRange(_Platforms.Item1.GetPlatformAfterCurrent(0,3));

        List<Platform> p2 = _Platforms.Item2.GetPlatformBeforeCurrent(-3,-1);
        p2.AddRange(_Platforms.Item2.GetPlatformAfterCurrent(0,3));

        Vector3 DefaultV = -p1[1].gameObject.transform.position;

        Vector3 sum1 = new Vector3();
        foreach (Platform v in p1)
            sum1 += v.transform.position;
        for (int i = p1.Count; i < 7; i++)
            sum1 += DefaultV;

        Vector3 sum2 = new Vector3();
        foreach (Platform v in p2)
            sum2 += v.transform.position;
        for (int i = p2.Count; i < 7; i++)
            sum2 += DefaultV;

        m_FromPosition = sum1 / 7;
        m_ToPosition = sum2 / 7;
    }

    void BasePlayer_OnFinishedJump() {
        GetNewLookAt(RootPlayers.MainPlayer.PairPlatformsLookAts);
        GetNewPosition(RootPlayers.MainPlayer.PairPlatformsLookAts);
    }
}
