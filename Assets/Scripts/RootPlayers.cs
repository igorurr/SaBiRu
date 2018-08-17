using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootPlayers : MonoBehaviour {

    public static BasePlayer MainPlayer;

	// Use this for initialization
	void Start () {
        LoadMainPlayer();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadMainPlayer()
    {
        GameObject go = Instantiate(
            Resources.Load<GameObject>( GlobalSetings.Instance.ThemePathFromResources + "Player/Player" ),
            transform
        );
        MainPlayer = go.GetComponent<BasePlayer>();
    }
}
