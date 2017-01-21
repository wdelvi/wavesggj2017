using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour 
{
	private LevelController levelController;
		
	// Use this for initialization
	void Start () 
	{
		levelController = new LevelController ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.levelController.Update ();
	}
}
