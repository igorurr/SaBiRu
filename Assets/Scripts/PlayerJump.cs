using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump
{
    BasePlayer      m_Player;
    Vector3         m_From;
    Vector3         m_To;
    float           m_PercentWayStartPosition;
    float           m_PercentWayPosition;    // [0;1]
    Ray             m_ScreenRay;
    bool            m_FinishPointWasCrossed;
    Vector3         m_AfterCrossedForwardVector;
    float           m_DistanceJump; // изменять только в начале

    public Vector3  PlayerCurrentPosition { get; private set; }
    public bool     NextPlatformIsCurentPlatform { get; private set; }
    public float    PercentWayPosition { get { return m_FinishPointWasCrossed ? 1 : m_PercentWayPosition; } }
    public float    DistanceJump { get { return Mathf.Lerp( 0, m_DistanceJump, PercentWayPosition ); } }


    // си шарп не думает что если у класса есть контструктор с аргументами, то есть и конструктор по умолчанию.
    // си шарп думает что если у класса нет конструкторов вообще то есть конструктор по умолчанию
    //public PlayerJump() {}

    PlayerJump(
        BasePlayer _Player,
        Vector3 _From,
        Vector3 _To,
        bool _NextPlatformIsCurentPlatform
    )
    {
        m_Player = _Player;
        m_From = _From;
        m_To = _To;
        m_PercentWayStartPosition = 0;
        m_FinishPointWasCrossed = false;
        m_PercentWayPosition = 0;
        m_DistanceJump = (m_From - m_To).magnitude;

        NextPlatformIsCurentPlatform = _NextPlatformIsCurentPlatform;
    }

    public PlayerJump(
        BasePlayer _Player,
        Vector3 _FromTo
    )
    : this( _Player, _FromTo, _FromTo, true )
    { }

    public PlayerJump(
        BasePlayer _Player,
        Ray _ScreenRay
    )
    : this(_Player, new Vector3(), new Vector3(), false )
    {
        CreateRayCast( _ScreenRay );
    }

    void CreateRayCast(Ray _ScreenRay)
    {
        m_ScreenRay = _ScreenRay;

        RaycastHit rh;
        if ( CheckAndGetRayCast( out rh ) )
            m_To = rh.point;
        else
        {
            // документация: Нахождение EndPoint в случае изначального промаха игрока

            List<Platform> aroundNext =  m_Player.CurrentPlatform.NextPlatform.GetPlatformsAroundCurrent(1);

            float yR = 0;

            for ( int i = 0; i < aroundNext.Count; i++ )
                yR += aroundNext[i].StartPosition.y;
            yR /= aroundNext.Count;

            float t = (yR - m_ScreenRay.origin.y) / m_ScreenRay.direction.y;

            m_To = m_ScreenRay.GetPoint(t);
        }
    }

    public void CreateStartPoint(Vector3 _From)
    {
        m_From = _From;
        m_DistanceJump = (m_From - m_To).magnitude;
        m_PercentWayStartPosition = 0;
        m_FinishPointWasCrossed = false;
        m_PercentWayPosition = 0;
    }

    // прыгать на месте в текущей точке
    public PlayerJump NewPlayerJumpFromToCurrent()
    {
        return new PlayerJump( m_Player, PlayerCurrentPosition );
    }




    public bool CheckAndGetRayCast( out RaycastHit _Rh )
    {
        return (
            Physics.Raycast( m_ScreenRay, out _Rh ) &&
            _Rh.transform == m_Player.CurrentPlatform.NextPlatform.transform
        );
    }

    public void UpdateRayCast()
    {
        RaycastHit rh;
        if (
            !NextPlatformIsCurentPlatform &&
            !m_FinishPointWasCrossed &&
            CheckAndGetRayCast( out rh )
        )
        {
            m_To = rh.point;
            UpdateEndPoint();
        }
    }

    public void UpdatePlayerPosition()
    {
        m_PercentWayPosition += Time.deltaTime * m_Player.PlayerSpeed / GlobalSetings.Instance.ConstMovingSpherePlayer;
        m_PercentWayPosition = Mathf.Clamp01(m_PercentWayPosition);

        if ( m_FinishPointWasCrossed )
        {
            PlayerCurrentPosition = m_AfterCrossedForwardVector * m_PercentWayPosition;

            if ( m_PercentWayPosition == 1 )
                m_Player.FailedJump();
        }
        else
        {
            float percentSubWayPosition = ( m_PercentWayPosition - m_PercentWayStartPosition ) / ( 1 - m_PercentWayStartPosition );

            PlayerCurrentPosition = new Vector3(
                Mathf.Lerp( m_From.x, m_To.x, percentSubWayPosition ),
                Mathf.Lerp( m_From.y, m_To.y, percentSubWayPosition ),
                Mathf.Lerp( m_From.z, m_To.z, percentSubWayPosition )
            );

            if ( m_PercentWayPosition == 1 )
            {
                m_PercentWayPosition = 0;
                m_FinishPointWasCrossed = true;

                // constants
                float percentAfterCrossedForwardVectorForStartPoint = 0.95f;
                float multiplieAfterCrossedForwardVectorr = 200;

                float subPercentAfterCrossedForwardVectorForStartPoint = (percentAfterCrossedForwardVectorForStartPoint - m_PercentWayStartPosition) / (1 - m_PercentWayStartPosition);
                Vector3 AfterCrossedForwardVectorStartPoint = new Vector3(
                    Mathf.Lerp( m_From.x, m_To.x, subPercentAfterCrossedForwardVectorForStartPoint ),
                    Mathf.Lerp( m_From.y, m_To.y, subPercentAfterCrossedForwardVectorForStartPoint ),
                    Mathf.Lerp( m_From.z, m_To.z, subPercentAfterCrossedForwardVectorForStartPoint )
                ) + BasePlayer.MultiPointLerp( m_Player.VerticalMove, percentAfterCrossedForwardVectorForStartPoint );

                m_AfterCrossedForwardVector = ( AfterCrossedForwardVectorStartPoint - m_To ).normalized * multiplieAfterCrossedForwardVectorr;
            }
        }

        m_Player.UpdatePosition();
    }

    void UpdateEndPoint()
    {
        m_From = PlayerCurrentPosition;
        m_PercentWayStartPosition = m_PercentWayPosition;
    }

    public void CheckCollision( Platform _Platform )
    {
        if( 
            ( NextPlatformIsCurentPlatform && _Platform == m_Player.CurrentPlatform ) ||
            ( !NextPlatformIsCurentPlatform && _Platform == m_Player.CurrentPlatform.NextPlatform )
        )
            m_Player.SuccesJump();
    }
}