using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	//Speed at which objects are moved
	public float gameSpeed;
	public float distance;
	public int distanceWhole;
	public float timeToBlockInputAfterDeath = 2;

	public GameObject player;

	[SerializeField]
	public PlayerController playerController;

	//The objects that get moved allong the x
	public List<GameObject> objectsToMove;	

	public Text distanceUI;
	public Text deathUI;

	public List<AudioClip> downSounds;
	public List<AudioClip> upSounds;
	public List<AudioClip> wipeoutSounds;
	public List<AudioClip> hitSounds;
	public List<AudioClip> airSounds;
	public List<AudioClip> groundSounds;

	[SerializeField]
	private LevelController levelController = new LevelController();
	private bool gameActive = true;
	private bool gamePaused = true;
	private bool lastInput = false;
	private AudioSource sound;
	private float inputBlockTimer;
		
	// Use this for initialization
	private void Start () 
	{
		this.distanceUI.text = "";
		distance = 0.0f;
		distanceWhole = 0;
		this.deathUI.gameObject.SetActive(false);
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		playerController.Setup();
		this.sound = this.GetComponent<AudioSource> ();
		this.levelController.Setup();
		this.inputBlockTimer = this.timeToBlockInputAfterDeath;
	}

	private void EndGame()
	{
		this.PauseGame ();
		this.gameActive = false;
		this.deathUI.gameObject.SetActive (true);
		this.deathUI.text = "You surfed " + distanceWhole + "m. Tap to restart.";
		this.inputBlockTimer = 0;
	}

	private void RestartGame()
	{
		SceneManager.LoadScene( "mainScene", LoadSceneMode.Single );
	}

	private void PauseGame()
	{
		this.gamePaused = true;
	}

	private void UnpauseGame()
	{
		this.gamePaused = false;
	}
	
	// Update is called once per frame
	private void Update ( ) 
	{
		bool inputDown = Input.GetKey ("space") || Input.GetMouseButton(0);
		this.playerController.is_down = inputDown;

		if ( this.gameActive && !this.gamePaused )
		{
			this.playerController.UpdateFrame ();
			this.UpdateFollowObjects ();
			this.UpdateUI ();
			this.UpdateSounds ( inputDown, lastInput );
			this.levelController.Update ();

			if ( this.playerController.is_dead )
			{
				this.EndGame ();
			}
		}
		else
		{
			this.inputBlockTimer += Time.deltaTime;

			if ( inputDown && inputBlockTimer >= this.timeToBlockInputAfterDeath )
			{
				this.InputPressed ();
			}
		}

		this.lastInput = inputDown;
	}

	private void UpdateFollowObjects()
	{
		distance += gameSpeed;
		for (int i = 0; i < this.objectsToMove.Count; i++)
		{
			Vector3 newPosition = this.objectsToMove [i].transform.localPosition;
			newPosition.x += gameSpeed;
			this.objectsToMove [i].transform.localPosition = newPosition;
		}
	}

	private void UpdateSounds( bool newInput, bool oldInput )
	{
		if (this.playerController.is_dead && this.gameActive)
		{
			this.PlayRandomSound ( this.wipeoutSounds, true );
		}
		else if (newInput == true && oldInput == false)
		{
			this.PlayRandomSound ( this.upSounds, true );	
		}
		else if (newInput == false && oldInput == true)
		{
			this.PlayRandomSound ( this.downSounds, true );
		}
		else if (this.playerController.is_collide_now)
		{
			this.PlayRandomSound ( this.hitSounds, true );
		}
		else if (this.playerController.is_in_air)
		{
			this.PlayRandomSound ( this.airSounds, false );
		}
		else if (this.playerController.is_on_ground)
		{
			this.PlayRandomSound ( this.groundSounds, false );
		}
	}

	private void PlayRandomSound( List<AudioClip> soundClips, bool interuptSounds )
	{
		if ( soundClips.Count > 0)
		{
			if (!this.sound.isPlaying || interuptSounds)
			{
				AudioClip clipToPlay = soundClips [Random.Range (0, soundClips.Count - 1)]; 
				this.sound.clip = clipToPlay;
				this.sound.Play ();
			}
		}
	}

	private void UpdateUI()
	{
		distanceWhole = (int)(Mathf.Round(distance));
		this.distanceUI.text = distanceWhole + " m";  
	}

	private void InputPressed()
	{
		if ( this.gameActive && this.playerController.is_dead )
		{
			this.EndGame ();
		}
		else if ( this.gamePaused )
		{
			if (this.playerController.is_dead)
			{
				this.RestartGame ();
			}
			else
			{
				this.UnpauseGame ();
			}
		}
	}
}
