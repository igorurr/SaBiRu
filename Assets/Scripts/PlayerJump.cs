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

    // ������ �� ��������� �� �������� �����
    public Vector3 PositionOnMoveBetweenPlatforms { get { return Vector3.Lerp( From, To, PositionBetweenPlatforms ); } }
    public Tuple<Platform, Platform> PairPlatformsLookAts { get { return new Tuple<Platform, Platform>(FromPlatform, ToPlatform); } }

    // �� ���� �� ������ ��� ���� � ������ ���� ������������ � �����������, �� ���� � ����������� �� ���������.
    // �� ���� ������ ��� ���� � ������ ��� ������������� ������ �� ���� ����������� �� ���������
    public PlayerJump() {}

    public PlayerJump( Vector3 _point, Platform _Platform )
    {
        From = _point;
        To = _point;
        PositionBetweenPlatforms = 0;

        FromPlatform = _Platform;
        ToPlatform = _Platform;
    }

    // ���������� ������ �� ����� ����� �������� ������ � ����� �� ������ �����
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

    // ������� �� ����� � ������� �����
    public void NewPlayerJumpFromToCurrent()
    {
        From = To;
        PositionBetweenPlatforms = 0;
        FromPlatform = ToPlatform;
    }
}