using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSetings : MonoBehaviour {

    public static GlobalSetings Instance;
    public static BaseThemeSettings TS;
    
    public string ThemePathFromAssets { get { return string.Format("./Assets/Resources/{0}/", LocalStorage.Instance.ThemeCurent); } }
    public string ThemePathFromResources { get { return string.Format("{0}/", LocalStorage.Instance.ThemeCurent); } }

    public Vector3 ForwardVector;
    public float disperionAngleMaxX;
    public float disperionAngleMaxY;

    public int MaxPlatformsOnScene;
    public int CountUnvisiblePlatformsBeforeVisibles;
    public int CountUnvisiblePlatformsAfterVisibles;

    public float ConstMovingSpherePlayer;
    public float StartSpeed;

    public Vector3 CameraHorisontalPositionAbovePlatform;
    public Vector3 CameraVerticalPositionAbovePlatform;
    // координата y должна быть равна 0
    public Vector3 DefaultCameraRotation;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        ReLoadThemeSettings();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void ReLoadThemeSettings()
    {
        if( TS != null )
        {
            Destroy(TS.gameObject);
            TS = null;
        }

        GameObject go = Instantiate(
            Resources.Load<GameObject>(ThemePathFromResources + "ThemeSettings"),
            transform
        );
        TS = go.GetComponent<BaseThemeSettings>();
    }
}
