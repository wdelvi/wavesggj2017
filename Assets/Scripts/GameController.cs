using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
	public GameObject player;

	public List<GameObject> followPlayer;	

	//All of our options for chunks
	public List<GameObject> levelChunkOptions;

	public Text meterCounter; 

	private LevelController levelController;
		
	// Use this for initialization
	void Start () 
	{
		this.levelController = new LevelController ( this.levelChunkOptions );
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.UpdateFollowObjects ();
		this.UpdateUI ();
		this.levelController.Update ();
	}

	void UpdateFollowObjects()
	{
		for (int i = 0; i < this.followPlayer.Count; i++)
		{
			Vector3 newPosition = this.followPlayer [i].transform.localPosition;
			newPosition.x = this.player.transform.localPosition.x;
			this.followPlayer [i].transform.localPosition = newPosition;
		}
	}

	void UpdateUI()
	{
		float inflatedXPosition = Mathf.Round( Camera.main.transform.localPosition.x * 10000 ) / 100;
		this.meterCounter.text = "" + inflatedXPosition + " M";  
	}
}
