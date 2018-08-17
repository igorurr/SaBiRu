using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump
{
    static public float MedianeLengthOneJump;

    static public List<Vector3> VerticalMove;

    public Vector3 From;
    public Vector3 To;

    public Platform FromPlatform;
    public Platform ToPlatform;

    public float PositionBetweenPlatforms;    // [0;1]

    // прямая от начальной до конечной точки
    public Vector3 PositionOnMoveBetweenPlatforms { get { return Vector3.Lerp( From, To, PositionBetweenPlatforms ); } }
    public Tuple<Platform, Platform> PairPlatformsLookAts { get { return new Tuple<Platform, Platform>(FromPlatform, ToPlatform); } }

    // си шарп не думает что если у класса есть контструктор с аргументами, то есть и конструктор по умолчанию.
    // си шарп думает что если у класса нет конструкторов вообще то есть конструктор по умолчанию
    public PlayerJump() {}

    public PlayerJump( Vector3 _point, Platform _Platform )
    {
        From = _point;
        To = _point;
        PositionBetweenPlatforms = 0;

        FromPlatform = _Platform;
        ToPlatform = _Platform;
    }

    // Загенерить прыжок из конца точки текущего прыжка в какую то другую точку
    public PlayerJump FromHereToThere( Vector3 _to, Platform _PlatformTo )
    {
        PlayerJump ret = new PlayerJump();

        ret.From = To;
        ret.To = _to;
        ret.PositionBetweenPlatforms = 0;

        ret.FromPlatform = ToPlatform;
        ret.ToPlatform = _PlatformTo;

        return ret;
    }

    // прыгать на месте в текущей точке
    public void NewPlayerJumpFromToCurrent()
    {
        From = To;
        PositionBetweenPlatforms = 0;
        FromPlatform = ToPlatform;
    }
}