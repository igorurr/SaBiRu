using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayer : MonoBehaviour {

    public float PlayerSpeed;       // время за которое префаб игрока проходит от одной точки к другой

    QueuePlayerJumps m_QueuePlayerJumps;       // Точки на платформах в которые должен прыгать префаб игрока (то что юзер на экране натыкал)

    public PlayerJump CurrentJump;

    public Tuple<Platform, Platform> PairPlatformsLookAts { get { return CurrentJump.PairPlatformsLookAts; } }

    protected abstract List<Vector3> VerticalMove { get; }

    // Use this for initialization
    void Start ()
    {
        PlayerSpeed = GlobalSetings.Instance.StartSpeed;

        m_QueuePlayerJumps = new QueuePlayerJumps();

        CurrentJump = new PlayerJump( GlobalSetings.TS.PlayerDefaultOffset, RootPlatforms.Instance.StartPlatform);
        
        RenderCamera.Instance.OnClickGameZone += CreateJumpFromPointAndEnqueueFromQueueJump;
    }
	
	// Update is called once per frame
	void Update () {
        UpdatePlayerOnMoveBetweenPlatforms();
        UpdatePositionSpherePlayer();
    }

    private void OnDestroy()
    {
        RenderCamera.Instance.OnClickGameZone -= CreateJumpFromPointAndEnqueueFromQueueJump;
    }

    public event Action OnFinishedJump;

    void GoNextCurveJump()
    {
        // подразумевается что если m_QueuePlayerJumps.IsEmpty - то следующая платформа - текущая

        if ( m_QueuePlayerJumps.IsEmpty ) { // прыгаем на месте
            CurrentJump.NewPlayerJumpFromToCurrent();
        }
        else {  // прыгаем на след. платформу
            CurrentJump = m_QueuePlayerJumps.DequeueJump();
        }
    }

    void UpdatePlayerOnMoveBetweenPlatforms()
    {
        // по данной формуле смотреть: офигительные вычисления координат шарика
        CurrentJump.PositionBetweenPlatforms += Time.deltaTime * PlayerSpeed*PlayerSpeed / GlobalSetings.Instance.ConstMovingSpherePlayer;
        CurrentJump.PositionBetweenPlatforms = Mathf.Clamp01(CurrentJump.PositionBetweenPlatforms);
    }

    void CreateJumpFromPointAndEnqueueFromQueueJump(Ray ray)
    {
        /*
         Если юзер прыгнет в никуда, но в этом месте позже появится платформа - будет не очень,
         если юзер прыгнет на платформу а она уедет - тоже не очень.

        первая проблема решается с помощью введения корутин на ожидание появления платформы, ежели там ничего не появится - юзер летит в стандартную точку (ожидаемая позиция платформ на основе соседних наверн) и если там пусто - пролетается
        вторая проблема: храним луч прыжка юзера, если во время контакта с точкой куда он летит нет ничего - юзер пролетает.
         
         */
        RaycastHit rh;
        Physics.Raycast(ray, out rh);

        PlayerJump current = m_QueuePlayerJumps.IsEmpty ? CurrentJump : m_QueuePlayerJumps.GetLast;

        Vector3 toPoint = rh.point + GlobalSetings.TS.PlayerDefaultOffset;

        // временное костыльное решение
        Platform toPlatform = (rh.transform) ? rh.transform.GetComponent<Platform>() : null;

        m_QueuePlayerJumps.EnqueueJump( current.FromHereToThere( rh.point + GlobalSetings.TS.PlayerDefaultOffset, toPlatform ) );
    }

    // найти предполагаемую точку приземления юзера если там есть платформа по лучу
    void FindAssumeJumpEndPoind(Ray _Ray)
    {

    }

    void UpdatePositionSpherePlayer()
    {
        transform.position = CurrentJump.PositionOnMoveBetweenPlatforms + MultiPointLerp( VerticalMove, CurrentJump.PositionBetweenPlatforms );

        if ( CurrentJump.PositionBetweenPlatforms == 1f )
        {
            GoNextCurveJump();
            OnFinishedJump();
        }
    }

    // после добавлении анимации может пригодится - хз
    Vector3 MultiPointLerp(List<Vector3> points, float t)
    {
        float curpointf = t * (points.Count - 1);
        int curpoint = (int)curpointf;

        if (curpoint == points.Count - 1)
            return points[curpoint];

        return Vector3.Lerp(points[curpoint], points[curpoint + 1], curpointf - curpoint);
    }
}
