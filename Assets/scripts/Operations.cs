using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Operations:MonoBehaviour
{

	Stack<GameObject> paisStack; // 牌垛

	static List<GameObject> sortList;  // 重新排序后的牌list；始终维持最新变化

	float paiWidth = 1.0f;   // 牌宽度
	float outpaiWidth = 0.55f;  // 已出牌宽度
	float outpaiHeight = 0.65f;
	float paiduoWidht = 0.85f;  // 牌垛牌宽度
	float paiduoHeight = 0.3f; // 牌垛牌高度
	string[] types =  {"wan_", "tong_", "tiao_"};

	GameObject primeMaJiang;
	GameObject primeOutPai;
	GameObject primePaiDuo;

	GameObject canvas;

	public Operations ()
	{
		paisStack = new Stack<GameObject> ();

		primeMaJiang = Resources.Load<GameObject> ("Prefabs/paiprefab");
		primeOutPai = Resources.Load<GameObject> ("Prefabs/outprefab");
		primePaiDuo = Resources.Load<GameObject> ("Prefabs/paiduo");
	}

	// 初始化我的牌
	public void setMaJiangs() {

		Operations.sortList = new List<GameObject>();  // 用来排序的list

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
		GameObject mopai = (GameObject)GameObject.Instantiate (primeMaJiang);

		SpriteRenderer spr = (SpriteRenderer)mopai.GetComponent ("SpriteRenderer");

		System.Random rd = new System.Random(); 
		string srcVal = types[rd.Next(0, 3)] + rd.Next(1, 10); // 随机数；=> 开始时传来随机序列，按顺序依次取牌

		Texture2D t2d = (Texture2D)Resources.Load ("pai/" + srcVal);
		spr.sprite = Sprite.Create (t2d, spr.sprite.textureRect, new Vector2 (0.5f, 0.5f));
		spr.sortingOrder = 2;

		float x = GameObject.FindGameObjectsWithTag ("pai").Length * paiWidth / 2 - 0.5f;
		mopai.transform.position = new Vector2 (x, -4f);

		mopai.GetComponent<each> ().setValue (srcVal);

		addSortList (mopai, srcVal);  // 添加到维持list

		GameObject paiduo_out = (GameObject) paisStack.Pop();
		paiduo_out.SetActive (false);
	}

	// 出牌
	public void play (string val) {

		GameObject outpai = (GameObject) GameObject.Instantiate(primeOutPai);
		SpriteRenderer spr = (SpriteRenderer)outpai.GetComponent ("SpriteRenderer");
		Texture2D t2d = (Texture2D)Resources.Load ("paiout/" + val + "_out");
		spr.sprite = Sprite.Create (t2d,spr.sprite.textureRect,new Vector2(0.5f,0.5f));

		GameObject[] objs = GameObject.FindGameObjectsWithTag ("outpai");
		int outCnts = objs.Length - 1;

		spr.sortingOrder += outCnts / 6;

		float x = -1f + outpaiWidth * (outCnts % 6);
		float y = -0.45f - outpaiHeight * (outCnts / 6);
		outpai.transform.position = new Vector2 (x, y);

		outpai.GetComponent<outEach> ().setValue (val);

		deleteSortList (val);  // 从维持list中删除已出的牌，并重新排序

		// 出牌后，停止计时器，延迟2秒摸牌，并重新计时
		GameObject camera = GameObject.Find("MainCamera");
		camera.GetComponent<SetMyMajiang>().stopTime ();
		camera.GetComponent<SetMyMajiang>().moPai_Set ();
	}

	// 根据排好序的的list，调整牌位置；每次出牌后调整牌位置
	public void sortPos() {

		for (int i = 0; i < Operations.sortList.Count; i++) {
			Operations.sortList [i].transform.position = new Vector2 (-0.2f - 13 * paiWidth/ 2 + paiWidth * i, -4f);
			Operations.sortList [i].GetComponent<each> ().setIsUp (0);
		}
	}

	// 摸牌添加到sortlist
	void addSortList(GameObject obj, string srcVal) {
		// 排序
		if (Operations.sortList.Count == 0) {
			Operations.sortList.Add (obj);
		} else {
			int isEnd = 0;  // 用来判断是否最大，若为0则插在末尾
			for (int j = 0; j < Operations.sortList.Count; j++) {
				string sortVal = Operations.sortList [j].GetComponent<each> ().getValue ();
				if (string.Compare (srcVal, sortVal) <= 0) {
					Operations.sortList.Insert (j, obj);
					isEnd = 1;
					break;
				} 
			}
			if (isEnd == 0) {
				Operations.sortList.Add (obj);
			}
		}
	}
		
	public void deleteSortList(string val) {

		for (int i = 0; i < Operations.sortList.Count; i++) {
			if (Operations.sortList [i].GetComponent<each> ().getValue ().Equals (val)) {
				Operations.sortList.Remove (Operations.sortList [i]);
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
}

