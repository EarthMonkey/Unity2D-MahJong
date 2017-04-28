using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMyMajiang : MonoBehaviour {

	Text text;
	int start;  // 倒计时，-1时不到
	float totalTime; // 每帧总时长
	int DURATION;  // 倒计时时长
	int TWICE;  // 两次倒计时

	Operations op;

	// Use this for initialization
	void Start () {
		DURATION = 8;  // 8秒倒计时
		TWICE = 1;
		op = new Operations ();

		text = GameObject.Find ("Canvas").GetComponent<Text> ();

		op.setMaJiangs ();
		op.setPaiDuo ();

		sortPos_set ();

		start = -1;
		totalTime = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (start > -1) {
			Timer ();
		}
	}

	// 延迟执行
	void sortPos_set () {
		Invoke ("sortPosInvoke", 1f);  // 1秒后重新排序
	}

	void sortPosInvoke() {
		op.sortPos ();

		moPai_Set ();
	}

	public void moPai_Set() {
		Invoke ("moPaiInvoke", 0.5f);  // 0.5秒后执行摸牌，并开始倒计时
	}

	void moPaiInvoke() {
		op.moPai ();
		text.text = "0" + DURATION + "\n";
		initTime ();
	}

	void Timer() {

		totalTime += Time.deltaTime;
		if (totalTime >= 1) {//每过1秒执行一次
			start--;
			text.text = "0" + start + "\n";
			if (TWICE == 0) {
				text.material.color = Color.red;
			} else {
				text.material.color = new Color(0.9f, 0.505f, 0, 1);
//				text.color = new Color(0.9f, 0.505f, 0, 1);
			}
			totalTime = 0;
		}

		if (start == 0 && TWICE == 0) {
			start = -1;
			// 规定时间内没有出牌则自动出最后一张牌
			GameObject[] pais = GameObject.FindGameObjectsWithTag ("pai");
			GameObject lastObj = pais [pais.Length - 1];
			string val = lastObj.GetComponent<each> ().getValue ();
			op.play (val);
			lastObj.SetActive (false);
//			op.adjustPos ();

			TWICE = 1;
			start = DURATION + 1;
		} else if (start == 0 && TWICE == 1) {
			start = DURATION + 1;
			TWICE = 0;
		}
	}

	// 开始倒计时
	public void initTime () {
		TWICE = 1;
		this.start = DURATION;
	}

	// 停止倒计时
	public void stopTime() {
		this.start = -1;
		text.text = "";
	}
}