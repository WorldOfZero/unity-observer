using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour {

    public Vector3 testOne = Vector3.zero;
    public Vector3 testTwo
    {
        get { return testOne; }
        set { testOne = value; }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
