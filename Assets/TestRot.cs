using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = newRot;
    }
}
