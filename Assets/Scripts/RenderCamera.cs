using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCamera : MonoBehaviour {

    public static RenderCamera Instance;

    Plane[]     m_CameraPlanes;
    Camera      m_Camera;

    Platform    m_CurrentPlatform; // у юзера текущая платформа - следующая для текущей платформы камеры, исключения: старт и падение юзера в пропасть
    Vector3     m_FromLookAt;
    Vector3     m_ToLookAt;
    Vector3     m_FromPosition;
    Vector3     m_ToPosition;

    // Use this for initialization
    void Start () {
        Instance = this;

        m_Camera = GetComponent<Camera>();
        m_CameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_Camera);

        RootPlayers.MainPlayer.OnFinishedJump += BasePlayer_OnFinishedJump;

        m_CurrentPlatform = RootPlatforms.Instance.StartPlatform;
        GetNewLookAt( RootPlatforms.Instance.StartPlatform );
        GetNewPosition( RootPlatforms.Instance.StartPlatform );
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
            Vector3.Lerp( m_FromPosition, m_ToPosition, RootPlayers.MainPlayer.PercentWayPositionInJamp );
        
        transform.LookAt( Vector3.Lerp( m_FromLookAt, m_ToLookAt, RootPlayers.MainPlayer.PercentWayPositionInJamp ) );
        transform.rotation *= Quaternion.Euler( GlobalSetings.Instance.DefaultCameraRotation );
    }

    void GetNewLookAt( Platform _ToPlatformView )
    {
        List<Platform> p1 = m_CurrentPlatform.NextPlatform.GetPlatformsAroundCurrent(1);
        List<Platform> p2 = _ToPlatformView.NextPlatform.GetPlatformsAroundCurrent(1);

        Vector3 sum1 = new Vector3();
        foreach (Platform v in p1)
            sum1 += v.StartPosition;

        Vector3 sum2 = new Vector3();
        foreach (Platform v in p2)
            sum2 += v.StartPosition;

        m_FromLookAt = sum1 / p1.Count;
        m_ToLookAt = sum2 / p2.Count;
    }

    void GetNewPosition( Platform _ToPlatformView )
    {
        // дефолтные платформы будут использоваться только на старте, когда до первой платформы платформ нет
        List<Platform> p1 = m_CurrentPlatform.GetPlatformsAroundCurrent( 3 );
        List<Platform> p2 = _ToPlatformView.GetPlatformsAroundCurrent( 3 );

        //Vector3 minusFirstPlatformPosition

        Vector3 sum1 = new Vector3();
        foreach (Platform v in p1)
            sum1 += v.StartPosition;

        Vector3 sum2 = new Vector3();
        foreach (Platform v in p2)
            sum2 += v.StartPosition;

        m_FromPosition = sum1 / p1.Count;
        m_ToPosition = sum2 / p2.Count;
    }

    void BasePlayer_OnFinishedJump( bool _JumpSucces ) {
        if ( 
            ( _JumpSucces && m_CurrentPlatform.NextPlatform.NextPlatform != RootPlayers.MainPlayer.CurrentPlatform ) || // тек. плат. камеры отстаёт на 1 платформу от тек. для юзера, затем происходит у юзера GoNextJump, а тек. плат. камеры ещё не апдейтнулась
            ( !_JumpSucces && m_CurrentPlatform != RootPlayers.MainPlayer.CurrentPlatform )         // Юзер зафейлил прыжок и текущая платформа камеры пока не пришла в текущую для юзера
        )
            m_CurrentPlatform = m_CurrentPlatform.NextPlatform;

        GetNewLookAt( RootPlayers.MainPlayer.CurrentPlatform );
        GetNewPosition( RootPlayers.MainPlayer.CurrentPlatform );
    }
}
