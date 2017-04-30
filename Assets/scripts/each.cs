using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class each : MonoBehaviour {

	private string value; 

	Operations op;


	int isUp = 0;  // 0未上移，1已上移

	static int myTurn = 1;  
	/* 判断是否轮到出牌；0-未轮到；1-轮到
	 * 若轮到出牌，第一次点击，牌上移；第二次点击，出牌
	 * 若未轮到，则第一次点击上移，第二次，无变化
	 * 点击其他牌，上一次的牌复位
	*/
	static GameObject lastObject;

	// Use this for initialization
	void Start () {
		op = GameObject.Find("MainCamera").GetComponent<Operations>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown () {
		Vector2 pos = (Vector2) transform.position;

		if (myTurn == 0) {  // not my turn
//			if (pos.y < -3.2) {
			if (this.isUp == 0) {
				transform.position = pos + new Vector2 (0, 0.8f);
				if (lastObject != null) {
					lastObject.transform.position = (Vector2)lastObject.transform.position - new Vector2 (0, 0.8f);
					lastObject.GetComponent<each> ().setIsUp (0);
				}
				lastObject = gameObject;
				this.isUp = 1;
			} 
		} else {  // my turn
//			if (pos.y > -3.9) {  // 出牌
			if (this.isUp == 1) {  // 出牌

				op.play (this.value);
				gameObject.SetActive(false);

			} else {
				transform.position = pos + new Vector2 (0, 0.8f);

				if (lastObject != null) {
					lastObject.transform.position = (Vector2)lastObject.transform.position - new Vector2 (0, 0.8f);
					lastObject.GetComponent<each> ().setIsUp (0);
				}
				lastObject = gameObject;
				this.isUp = 1;

				op.hightLight (this.value);
			}
		}
	}

	public void setValue(string value) {
		this.value = value;
	}

	public string getValue() {
		return this.value;
	}

	public void setIsUp(int isUp) {
		this.isUp = isUp;
	}

	// 0未轮到我；1轮到我
	public static void setMyTurn(int turn) {
		each.myTurn = turn;
	}

	public static int getMyTurn() {
		return each.myTurn;
	}
}
