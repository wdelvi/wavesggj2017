using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
	//Speed at which objects are moved
	public float gameSpeed;

	public GameObject player;

	//The objects that get moved allong the x
	public List<GameObject> objectsToMove;	

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
		for (int i = 0; i < this.objectsToMove.Count; i++)
		{
			Vector3 newPosition = this.objectsToMove [i].transform.localPosition;
			newPosition.x += gameSpeed;
			this.objectsToMove [i].transform.localPosition = newPosition;
		}
	}

	void UpdateUI()
	{
		float inflatedXPosition = Mathf.Round( Camera.main.transform.localPosition.x * 10000 ) / 100;
		this.meterCounter.text = "" + inflatedXPosition + " M";  
	}
}
