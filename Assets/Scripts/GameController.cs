using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour 
{
	//All of our options for chunks
	public List<GameObject> levelChunkOptions;

	private LevelController levelController;
		
	// Use this for initialization
	void Start () 
	{
		this.levelController = new LevelController ( this.levelChunkOptions );
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.levelController.Update ();
	}
}
