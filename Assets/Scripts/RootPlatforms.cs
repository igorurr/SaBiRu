using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RootPlatforms : MonoBehaviour {

    public static RootPlatforms Instance;

    public List<GameObject> m_PrefabsPlatforms;
    public List<Platform> m_Platforms;
    public int m_CurrentPlatform;

    public string ResourcesPlatforms { get { return string.Format("{0}Platforms/", GlobalSetings.Instance.ThemePathFromResources); } }
    public string AssetsPlatforms { get { return string.Format("{0}Platforms/", GlobalSetings.Instance.ThemePathFromAssets); } }

    public int FirstVisiblePlatform;
    public int LastVisiblePlatform;

    // использовать только в начале катки
    public Platform StartPlatform { get { return m_Platforms[0]; } }

    public Vector3 LastPointPosition;

    //public event System.Action OnEndFirstGenerationPlatforms;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        m_CurrentPlatform = 0;
        
        LoadPrefabsPlatforms();

        LastPointPosition = new Vector3(0, 0, 0);

        //CreateAndPushLastPlatformOnScene();
        FirstVisiblePlatform = LastVisiblePlatform = 0;

        UpdatePlatforms();
    }

    // Update is called once per frame
    void Update() {
        ReMathFirstLastVisiblePlatforms();
        UpdatePlatforms();
        //Debug.Log(m_Platforms[0].IsVisible);
    }

    void LoadPrefabsPlatforms()
    {
        string[] platformsPrefabs = Directory.GetFiles(AssetsPlatforms);
        foreach (string platformPrefab in platformsPrefabs)
        {
            string sufix = platformPrefab.Substring(platformPrefab.LastIndexOf("."));

            string localPath = ResourcesPlatforms + platformPrefab.Substring(platformPrefab.LastIndexOf("/") + 1);
            localPath = localPath.Substring(0, localPath.LastIndexOf("."));

            if (sufix == ".prefab")
                m_PrefabsPlatforms.Add(Resources.Load<GameObject>(localPath));
        }
    }

    void UpdatePlatforms()
    {
        // удаляем старые
        DeleteFirstPlatformOnScene( FirstVisiblePlatform - GlobalSetings.Instance.CountUnvisiblePlatformsBeforeVisibles );

        // добавляем новые
        CreateAndPushLastPlatformOnScene(
            GlobalSetings.Instance.CountUnvisiblePlatformsAfterVisibles - 
            ( m_Platforms.Count - LastVisiblePlatform - 1 )
        );
    }

    void ReMathFirstLastVisiblePlatforms()
    {
        if ( m_Platforms.Count == 0 )
            return;

        // ищем первую видимую платформу
        int i = 0;
        for (; i < m_Platforms.Count; i++)
            if (m_Platforms[i].IsVisible)
                break;
        FirstVisiblePlatform = i==m_Platforms.Count ? 0 : i;

        // ищем последнюю видимую платформу
        i = m_Platforms.Count-1;
        for (; i >= 0; i--)
            if (m_Platforms[i].IsVisible)
                break;
        LastVisiblePlatform = i==-1 ? m_Platforms.Count-1 : i;
    }

    void CreateAndPushLastPlatformOnScene(int _count=1)
    {
        for(int i=0; i<_count && m_Platforms.Count < GlobalSetings.Instance.MaxPlatformsOnScene; i++)
            m_Platforms.Add(GetRandomPlatform());
    }

    void DeleteFirstPlatformOnScene(int _count = 1)
    {
        for (int i = 0; i < _count && m_Platforms.Count > 0; i++)
        {
            Platform remove = m_Platforms[0];
            m_Platforms.RemoveAt(0);

            Destroy(remove.gameObject);

            FirstVisiblePlatform = (FirstVisiblePlatform-1)>0 ? FirstVisiblePlatform-1 : 0;
            LastVisiblePlatform = (LastVisiblePlatform-1)>0 ? LastVisiblePlatform-1 : 0;

            m_CurrentPlatform--;
        }
    }

    Platform GetRandomPlatform()
    {
        //GameObject platformPrefab = m_PrefabsPlatforms[(int)(Random.value * (m_PrefabsPlatforms.Count))];
        GameObject platformPrefab = m_PrefabsPlatforms[1];
        GameObject platformGO = Instantiate( platformPrefab, LastPointPosition, platformPrefab.transform.rotation, transform );

        LastPointPosition += GetForvardVectorRandomedDispersion();

        Platform p = platformGO.GetComponent<Platform>();

        if(m_Platforms.Count>0)
        {
            m_Platforms[m_Platforms.Count-1].NextPlatform = p;
            p.PrevPlatform = m_Platforms[m_Platforms.Count-1];
        }
        
        return p;
    }

    Vector3 GetForvardVectorRandomedDispersion()
    {
        return Quaternion.Euler(
            (2f * Random.value - 1) * GlobalSetings.Instance.disperionAngleMaxX,
            (2f * Random.value - 1) * GlobalSetings.Instance.disperionAngleMaxY,
            0
        )
        * GlobalSetings.Instance.ForwardVector;
    }

    /*public Platform GetPlatformAfterCurrent(int _AfterCount = 0)
    {
        int returnPlatform = m_CurrentPlatform + _AfterCount;
        return ( returnPlatform<m_Platforms.Count && returnPlatform>=0 ) ? m_Platforms[returnPlatform] : null;
    }*/

    public void NextPlatform()
    {
        m_CurrentPlatform++;
    }
}
