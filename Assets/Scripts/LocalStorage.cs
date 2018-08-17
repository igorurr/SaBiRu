using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalStorage : MonoBehaviour {

    public static LocalStorage Instance;

    public string ThemeCurent;

    // Use this for initialization
    void Awake()
    {
        Instance = this;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
