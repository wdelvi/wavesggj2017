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
		Vector3 newCameraPosition = Camera.main.transform.localPosition;
		newCameraPosition.x += 0.1f;
		Camera.main.transform.localPosition = newCameraPosition; 
		this.levelController.Update ();
	}
}
