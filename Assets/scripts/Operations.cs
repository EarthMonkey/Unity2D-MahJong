using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Operations : MonoBehaviour
{

	/*** 用来标记轮到谁出牌
	 * 东（庄家）：0
	 * 南：1
	 * 西：2
	 * 北：3
	 * 
	 * 区别对应的摸牌、出牌
	 * 当不是自己出牌时需要设置
	 */
	static int whoseTurn;  

	Stack<GameObject> paisStack; // 牌垛

	List<GameObject> leftPaiList;  // 左边牌
	List<GameObject> rightPaiList; // 右边牌
	List<GameObject> oppoPaiList;  // 对家牌

	List<GameObject> sortList;  // 重新排序后的牌list；始终维持最新变化

	GameObject primeMaJiang;
	GameObject primeOutPai;
	GameObject leftOutPai;
	GameObject rightOutPai;
	GameObject oppoOutPai;
	GameObject primePaiDuo;
	GameObject leftPai;
	GameObject rightPai;
	GameObject oppoPai;

	float paiWidth = 1.0f;   // 牌宽度
	float outpaiWidth = 0.55f;  // 已出牌宽度
	float outpaiHeight = 0.65f;
	float paiduoWidht = 0.69f;  // 牌垛牌宽度
	float paiduoHeight = 0.24f;  // 牌垛牌高度
	float leftPaiHeight = 0.56f; // 左右侧牌高度 
	float oppoWidth = 0.68f; // 对家牌高度
	string[] types =  {"wan_", "tong_", "tiao_"};

	SetMyMajiang setMy;

	void Start ()
	{
		whoseTurn = 0;

		paisStack = new Stack<GameObject> ();
		leftPaiList = new List<GameObject> ();
		rightPaiList = new List<GameObject> ();
		oppoPaiList = new List<GameObject> ();

		primeMaJiang = Resources.Load<GameObject> ("Prefabs/paiprefab");
		primeOutPai = Resources.Load<GameObject> ("Prefabs/outprefab");
		primePaiDuo = Resources.Load<GameObject> ("Prefabs/paiduo");
		leftPai = Resources.Load<GameObject> ("Prefabs/leftpai");
		rightPai = Resources.Load<GameObject> ("Prefabs/rightpai");
		oppoPai = Resources.Load<GameObject> ("Prefabs/oppopai");
		leftOutPai = Resources.Load<GameObject> ("Prefabs/outprefab_left");
		rightOutPai = Resources.Load<GameObject> ("Prefabs/outprefab_right");
		oppoOutPai = Resources.Load<GameObject> ("Prefabs/outprefab_oppo");

		setMy = GameObject.Find ("MainCamera").GetComponent<SetMyMajiang> ();
	}

	void Update() {

	}

	// 初始化麻将牌
	public void setMaJiangs() {

		/**我的牌*/
		sortList = new List<GameObject>();  // 用来排序的list
		primeMaJiang.transform.position = new Vector2(-0.2f - 13 * paiWidth/ 2, -4f);  // center layout
		for (int i = 0; i < 13; i++) {
			GameObject maj = (GameObject) GameObject.Instantiate (primeMaJiang);

			SpriteRenderer spr = (SpriteRenderer)maj.GetComponent ("SpriteRenderer");

			System.Random rd = new System.Random ();
			string srcVal = types[rd.Next(0, 3)] + rd.Next(1, 10);

			Texture2D texture2d = (Texture2D)Resources.Load("pai/" + srcVal);
			spr.sprite = Sprite.Create(texture2d,spr.sprite.textureRect,new Vector2(0.5f,0.5f));
			spr.sortingOrder = 2;

			maj.transform.position = (Vector2)primeMaJiang.transform.position + new Vector2 ( paiWidth * i, 0);

			maj.GetComponent<each>().setValue(srcVal);

			addSortList (maj, srcVal);
		}

		/**左右牌*/
		for (int j = 0; j < 13; j++) {
			GameObject lpai = (GameObject)GameObject.Instantiate (leftPai);
			lpai.transform.position = new Vector2 (-7.9f, -2.7f + leftPaiHeight * j);
			lpai.GetComponent<SpriteRenderer> ().sortingOrder -= j;
			leftPaiList.Add (lpai);

			GameObject rpai = (GameObject)GameObject.Instantiate (rightPai);
			rpai.transform.position = new Vector2 (7.9f, -2.7f + leftPaiHeight * j - leftPaiHeight - 0.5f);
			rpai.GetComponent<SpriteRenderer> ().sortingOrder -= j;
			rightPaiList.Add (rpai);
		}

		/**对家牌*/
		float oppoX = -13 * oppoWidth / 2 + 0.5f;
		for (int k = 0; k < 13; k++) {
			GameObject opai = (GameObject)GameObject.Instantiate (oppoPai);
			opai.transform.position = new Vector2 (oppoX + k * oppoWidth, 4.2f);
			oppoPaiList.Add (opai);
		}
	}

	// 初始化牌垛
	public void setPaiDuo() {
		 
		for (int i = 0; i < 20; i++) {
			GameObject paiduo = (GameObject)GameObject.Instantiate (primePaiDuo);

			float x = -3.7f + (i % 10) * paiduoWidht + (i / 10) * 0.06f;
			float y = -2.7f + (i / 10) * paiduoHeight;
			paiduo.transform.position = new Vector2 (x, y);

			paisStack.Push (paiduo);
		}
	}

	// 摸牌
	public void moPai() {

		switch (whoseTurn) {
			
		case 0:  // 东（庄家、我）
			{
				GameObject mopai = (GameObject)GameObject.Instantiate (primeMaJiang);

				SpriteRenderer spr = (SpriteRenderer)mopai.GetComponent ("SpriteRenderer");

				System.Random rd = new System.Random (); 
				string srcVal = types [rd.Next (0, 3)] + rd.Next (1, 10); // 随机数；=> 开始时传来随机序列，按顺序依次取牌

				Texture2D t2d = (Texture2D)Resources.Load ("pai/" + srcVal);
				spr.sprite = Sprite.Create (t2d, spr.sprite.textureRect, new Vector2 (0.5f, 0.5f));
				spr.sortingOrder = 2;

				float x = GameObject.FindGameObjectsWithTag ("pai").Length * paiWidth / 2 - 0.5f;
				mopai.transform.position = new Vector2 (x, -4f);

				mopai.GetComponent<each> ().setValue (srcVal);

				addSortList (mopai, srcVal);  // 添加到维持list

				GameObject paiduo_out = paisStack.Pop ();
				paiduo_out.SetActive (false);

				break;
			}
		case 1: // 南
			{
				int rCount = GameObject.FindGameObjectsWithTag ("rpai").Length;
				GameObject rpai = (GameObject)GameObject.Instantiate (rightPai);
				rpai.transform.position = new Vector2 (7.9f, -2.7f + leftPaiHeight * rCount - leftPaiHeight + 0.1f);
				rightPaiList.Add (rpai);

				break;
			}
		case 2: // 西
			{
				int oCount = GameObject.FindGameObjectsWithTag ("opai").Length;
				float oppoX = -oCount * oppoWidth / 2 + 0.5f;
				GameObject opai = (GameObject)GameObject.Instantiate (oppoPai);
				opai.transform.position = new Vector2 (oppoX - 0.3f - oppoWidth, 4.2f);
				oppoPaiList.Add (opai);

				break;
			}
		case 3: // 北
			{
//				int lCount = GameObject.FindGameObjectsWithTag ("lpai").Length;
				GameObject lpai = (GameObject)GameObject.Instantiate (leftPai);
				lpai.transform.position = new Vector2 (-7.9f, -2.7f - leftPaiHeight - 0.6f);
				leftPaiList.Add (lpai);
			
				break;
			}
		}
	}

	// 出牌
	public void play (string val) {

		switch (whoseTurn) {
		case 0:
			{
				GameObject outpai = (GameObject) GameObject.Instantiate(primeOutPai);
				SpriteRenderer spr = (SpriteRenderer)outpai.GetComponent ("SpriteRenderer");
				Texture2D t2d = (Texture2D)Resources.Load ("paiout/" + val + "_out");
				spr.sprite = Sprite.Create (t2d,spr.sprite.textureRect,new Vector2(0.5f,0.5f));

				int outCnts = GameObject.FindGameObjectsWithTag ("outpai").Length - 1;
				spr.sortingOrder += outCnts / 6;

				float x = -1f + outpaiWidth * (outCnts % 6);
				float y = -0.45f - outpaiHeight * (outCnts / 6);
				outpai.transform.position = new Vector2 (x, y);

				outpai.GetComponent<outEach> ().setValue (val);

				Operations.whoseTurn = 1;
				each.setMyTurn (0);
				break;
			}
		case 1:
			{
				GameObject outpai = (GameObject) GameObject.Instantiate(rightOutPai);
				SpriteRenderer spr = (SpriteRenderer)outpai.GetComponent ("SpriteRenderer");
				Texture2D t2d = (Texture2D)Resources.Load ("paiout_right/" + val + "_out");
				spr.sprite = Sprite.Create (t2d,spr.sprite.textureRect,new Vector2(0.5f,0.5f));

				int outCnts = GameObject.FindGameObjectsWithTag ("outpai_right").Length - 1;
				spr.sortingOrder -= outCnts;

				float x = 2f + outpaiHeight * (outCnts / 6);
				float y = 0.25f + outpaiWidth * (outCnts % 6);
				outpai.transform.position = new Vector2 (x, y);

				outpai.GetComponent<outEach> ().setValue (val);

				Operations.whoseTurn = 2;
				break;
			}
		case 2:
			{
				GameObject outpai = (GameObject) GameObject.Instantiate(oppoOutPai);
				SpriteRenderer spr = (SpriteRenderer)outpai.GetComponent ("SpriteRenderer");
				Texture2D t2d = (Texture2D)Resources.Load ("paiout_oppo/" + val + "_out");
				spr.sprite = Sprite.Create (t2d,spr.sprite.textureRect,new Vector2(0.5f,0.5f));

				int outCnts = GameObject.FindGameObjectsWithTag ("outpai_oppo").Length - 1;
				spr.sortingOrder += outCnts / 6;

				float x = 1.05f - outpaiWidth * (outCnts % 6);
				float y = 2.45f + outpaiHeight * (outCnts / 6);
				outpai.transform.position = new Vector2 (x, y);

				outpai.GetComponent<outEach> ().setValue (val);

				Operations.whoseTurn = 3;
				break;
			}
		case 3:
			{
				GameObject outpai = (GameObject) GameObject.Instantiate(leftOutPai);
				SpriteRenderer spr = (SpriteRenderer)outpai.GetComponent ("SpriteRenderer");
				Texture2D t2d = (Texture2D)Resources.Load ("paiout_left/" + val + "_out");
				spr.sprite = Sprite.Create (t2d,spr.sprite.textureRect,new Vector2(0.5f,0.5f));

				int outCnts = GameObject.FindGameObjectsWithTag ("outpai_left").Length - 1;
				spr.sortingOrder += outCnts / 6;

				float x = -1.8f - outpaiHeight * (outCnts / 6);
				float y = 1.6f - outpaiHeight * (outCnts % 6);
				outpai.transform.position = new Vector2 (x, y);

				outpai.GetComponent<outEach> ().setValue (val);

				Operations.whoseTurn = 0;
				each.setMyTurn (1);  // 恢复轮到我出牌
				break;
			}
		}
			
		deleteSortList (val);  // 从维持list中删除已出的牌，并重新排序

		// 出牌后，停止计时器，延迟2秒摸牌，并重新计时
		setMy.stopTime ();
		setMy.moPai_Set ();

		if (whoseTurn != 0) {
			Invoke ("autoPlay", 1.5f);
		}
	}

	// 根据排好序的的list，调整牌位置；每次出牌后调整牌位置
	public void sortPos() {

		for (int i = 0; i < sortList.Count; i++) {
			sortList [i].GetComponent<each> ().setIsUp (0);
			sortList [i].transform.position = new Vector2 (-0.2f - 13 * paiWidth/ 2 + paiWidth * i, -4f);
		}
	}

	// 摸牌添加到sortlist
	void addSortList(GameObject obj, string srcVal) {
		// 排序
		if (sortList.Count == 0) {
			sortList.Add (obj);
		} else {
			int isEnd = 0;  // 用来判断是否最大，若为0则插在末尾
			for (int j = 0; j < sortList.Count; j++) {
				string sortVal = sortList [j].GetComponent<each> ().getValue ();
				if (string.Compare (srcVal, sortVal) <= 0) {
					sortList.Insert (j, obj);
					isEnd = 1;
					break;
				} 
			}
			if (isEnd == 0) {
				sortList.Add (obj);
			}
		}
	}
		
	public void deleteSortList(string val) {

		for (int i = 0; i < sortList.Count; i++) {
			if (sortList [i].GetComponent<each> ().getValue ().Equals (val)) {
				sortList.Remove (sortList [i]);
				break;
			}
		}
		sortPos ();
	}

	// 高亮显示牌
	public void hightLight(string val) {

		GameObject[] objs = GameObject.FindGameObjectsWithTag ("outpai");
		foreach (GameObject ob in objs) {
			if (ob.GetComponent<outEach> ().getValue () == val) {
				((Renderer)ob.GetComponent<Renderer> ()).material.color = Color.green;
			} else {
				((Renderer)ob.GetComponent<Renderer> ()).material.color = Color.white;
			}
		}
	}

	// 自动出牌
	public void autoPlay() {
		// 规定时间内没有出牌则自动出最后一张牌
		String lastTag = "";
		switch (whoseTurn) {
		case 0:
			{
				lastTag = "pai";
				break;
			}
		case 1: 
			{
				lastTag = "rpai";
				break;
			}
		case 2:
			{
				lastTag = "opai";
				break;
			}
		case 3:
			{
				lastTag = "lpai";
				break;
			}
		}
			
		GameObject[] pais = GameObject.FindGameObjectsWithTag (lastTag);
		GameObject lastObj = pais [pais.Length - 1];

		if (whoseTurn == 0) {
			string val = lastObj.GetComponent<each> ().getValue ();
			play (val);
		} else {  // 暂时写死
			play ("wan_1");
		}

		lastObj.SetActive (false);
	}
}

