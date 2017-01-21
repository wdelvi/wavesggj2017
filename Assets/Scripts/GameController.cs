using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
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
		//Temporary function to move camera artifically
		Vector3 newCameraPosition = Camera.main.transform.localPosition;
		newCameraPosition.x += 0.1f;
		Camera.main.transform.localPosition = newCameraPosition; 

		this.UpdateUI ();
		this.levelController.Update ();
	}

	void UpdateUI()
	{
		this.meterCounter.text = "" + ( Mathf.Round(Camera.main.transform.localPosition.x * 100 ) + Mathf.Round ( Random.Range( 0.0f, 0.99f ) * 100 ) / 100 ) + " M";  
	}
}
