using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed_h = 0.5f;	// horizontal speed

	[SerializeField]
	private float speed_wave = 2f;	// vertical speed of wave

	[SerializeField]
	private float speed_press = 3f;	// vertical speed of pressing

	[SerializeField]
	private bool is_dead;			// player is dead, disable input

	[SerializeField]
	private bool is_in_wave;		// player is inside of a wave

	[SerializeField]
	private bool is_on_ground;		// player is on ground

	private float speed_v;			// vertical speed
	private Rigidbody2D rb;

	private float starting_y;

	private bool is_down;

	// Use this for initialization
	void Start () {
		is_dead = false;
		is_down = false;
		is_in_wave = true;
		rb = GetComponent<Rigidbody2D> ();
		this.starting_y = this.gameObject.transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (!is_dead) {	// if player is alive, accept input
			// movement
			if (is_on_ground) {
				// 
			} else {
				
				is_down = Input.GetKey ("space") || Input.GetMouseButton(0);
				if (is_in_wave && is_down) {
					// force down
					speed_v = speed_wave - speed_press;
				} else if (is_in_wave) {
					speed_v = speed_wave;
				} else {
					speed_v = 0;
				}
				if (speed_v >= 0) {
					rb.AddForce (Vector2.up * speed_v);
				} else {
					rb.AddForce (Vector2.down * speed_v * -1);
				}
			}

			float final_speed_h = ( speed_h * ( this.gameObject.transform.localPosition.y - this.starting_y ) );

			Debug.Log (this.gameObject.transform.localPosition.y - this.starting_y);

			transform.Translate ( Vector2.right * final_speed_h * Time.deltaTime );
		}
	}

	public void SetDead (bool _is_dead) {
		is_dead = _is_dead;
	}

	public void SetInWave (bool _is_in_wave) {
		is_in_wave = _is_in_wave;
	}

	public void SetOnGround (bool _is_on_ground) {
		is_on_ground = _is_on_ground;
	}

	void OnColliderEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Obstacle") {
			Debug.Log ("Hit obstacle");
			//Not sure we'll kill him in either of these cases?
			//This may be a jump?
			//SetDead (true);
		} else if (other.gameObject.tag == "Ground") {
			Debug.Log ("Hit Ground");
			//Not sure we'll kill him in either of these cases?
			//This may bump him upwards? Don't know what this should do.
			//SetOnGround (true);
		} else if (other.gameObject.tag == "Wave") {
			Debug.Log ("Hit Wave");
			SetDead (true);
		}
	}

	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.tag == "Air") {
			Debug.Log ("Hit Air");
			SetInWave (false);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Air") {
			SetInWave (true);
		} else if (other.gameObject.tag == "Ground") {
			SetOnGround (false);
		}
	}
}
