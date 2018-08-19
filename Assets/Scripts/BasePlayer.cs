using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayer : MonoBehaviour {

    public float PlayerSpeed;       // время за которое префаб игрока проходит от одной точки к другой
    public Platform CurrentPlatform { get; private set; }
    public int NumberPlatform;
    public float HowManyPlayerPassedWay;

    QueuePlayerJumps m_QueuePlayerJumps;       // Точки на платформах в которые должен прыгать префаб игрока (то что юзер на экране натыкал)

    PlayerJump CurrentJump;

    public float PercentWayPositionInJamp { get { return CurrentJump.PercentWayPosition; } }
    public bool NextPlatformIsCurentPlatform { get { return CurrentJump.NextPlatformIsCurentPlatform; } }

    public abstract List<Vector3> VerticalMove { get; } // позиции относительно текущей точки игрока

    // Use this for initialization
    void Start ()
    {
        HowManyPlayerPassedWay = 0;
        NumberPlatform = 1;

        PlayerSpeed = GlobalSetings.Instance.StartSpeed;

        m_QueuePlayerJumps = new QueuePlayerJumps();

        //GlobalSetings.TS.PlayerDefaultOffset
        CurrentJump = new PlayerJump( this, GlobalSetings.TS.PlayerDefaultOffset);
        CurrentPlatform = RootPlatforms.Instance.StartPlatform;


        RenderCamera.Instance.OnClickGameZone += CreateJumpFromPointAndEnqueueFromQueueJump;
    }
	
	// Update is called once per frame
	void Update () {
        CurrentJump.UpdateRayCast();
        CurrentJump.UpdatePlayerPosition();
    }

    private void OnDestroy()
    {
        RenderCamera.Instance.OnClickGameZone -= CreateJumpFromPointAndEnqueueFromQueueJump;
    }

    private void OnCollisionEnter( Collision collision )
    {
        Platform p = collision.gameObject.GetComponent<Platform>();

        if ( p != null )
            CurrentJump.CheckCollision(p);
    }

    public event Action<bool> OnFinishedJump;

    void GoNextJump()
    {
        // подразумевается что если m_QueuePlayerJumps.IsEmpty - то следующая платформа - текущая

        if ( m_QueuePlayerJumps.IsEmpty ) { // прыгаем на месте
            CurrentJump = CurrentJump.NewPlayerJumpFromToCurrent();
        }
        else
        {  // прыгаем на след. платформу
            
            HowManyPlayerPassedWay += CurrentJump.DistanceJump;
            CurrentPlatform = CurrentPlatform.NextPlatform;
            NumberPlatform++;

            PlayerJump opj = CurrentJump;
            CurrentJump = m_QueuePlayerJumps.DequeueJump();
            CurrentJump.CreateStartPoint( opj.PlayerCurrentPosition );
        }
    }

    void GoZanovoJump()
    {
        CurrentPlatform.StopMove();

        HowManyPlayerPassedWay += CurrentJump.DistanceJump; // так и задумано, не баг а фича

        CurrentJump.CreateStartPoint( CurrentPlatform.transform.position );
    }

    void CreateJumpFromPointAndEnqueueFromQueueJump( Ray _Ray )
    {
        m_QueuePlayerJumps.EnqueueJump( new PlayerJump( this, _Ray ) );
    }

    public void UpdatePosition()
    {
        transform.position = CurrentJump.PlayerCurrentPosition + MultiPointLerp( VerticalMove, CurrentJump.PercentWayPosition );
    }

    public virtual void FailedJump()
    {
        Debug.Log("FailedJump");

        GoZanovoJump();
        OnFinishedJump( false );
    }

    public virtual void SuccesJump()
    {
        Debug.Log("SuccesJump");

        GoNextJump();
        OnFinishedJump( true );
    }

    // после добавлении анимации может пригодится - хз
    public static Vector3 MultiPointLerp(List<Vector3> points, float t)
    {
        float curpointf = t * (points.Count - 1);
        int curpoint = (int)curpointf;

        if (curpoint == points.Count - 1)
            return points[curpoint];

        return Vector3.Lerp(points[curpoint], points[curpoint + 1], curpointf - curpoint);
    }
}
