using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outEach : MonoBehaviour {

	private string val;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void setValue(string val) {
		this.val = val;
	}

	public string getValue() {
		return this.val;
	}
}
