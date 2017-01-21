using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed_h = 1.5f;	// horizontal speed
	public float speed_h_in_wave = 1.25f;
	public float speed_h_in_air = 0.125f;
	public float speed_h_on_ground = 0.125f;

	public float speed_wave = 5f;	// vertical speed of wave

	public float speed_press = 10f;	// vertical speed of pressing

	public bool is_dead;			// player is dead, disable input

	public bool is_in_air;		// player at top
	public bool is_in_wave;		// player in middle
	public bool is_on_ground;		// player is on ground

	private float speed_v;			// vertical speed
	private Rigidbody2D rb;

	private float starting_y;

	public bool is_down;

	// Use this for initialization
	public void Setup() {
		is_dead = false;
		is_down = false;
		is_in_wave = true;
		is_in_air = false;
		is_on_ground = false;
		rb = GetComponent<Rigidbody2D> ();
		this.starting_y = this.gameObject.transform.localPosition.y;
	}

	private void UpdateSpeed() {
		if (is_in_air)
		{
			speed_h = speed_h_in_air;
		}
		else if (is_on_ground)
		{
			speed_h = speed_h_on_ground;
		}
		else if (is_in_wave)
		{
			speed_h = speed_h_in_wave;
		}
	}
	
	// Update is called once per frame
	public void UpdateFrame() {
		is_down = Input.GetKey ("space") || Input.GetMouseButton(0);
		if (!is_dead) {	// if player is alive, accept input
			UpdateSpeed();
			// movement
			if (is_on_ground) {
				// 
			} else {
				
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

			// Debug.Log (this.gameObject.transform.localPosition.y - this.starting_y);

			transform.Translate ( Vector2.right * final_speed_h * Time.deltaTime );
		}
	}

	public void SetDead (bool _is_dead) {
		is_dead = _is_dead;
	}

	public void SetInAir (bool _is_in_air) {
		is_in_air = _is_in_air;
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
			SetOnGround (true);
		} else if (other.gameObject.tag == "Wave") {
			Debug.Log ("Hit Wave");
			SetDead (true);
		}
	}

	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.tag == "Air") {
			Debug.Log ("Hit Air");
			SetInAir (true);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Air") {
			SetInAir (false);
		} else if (other.gameObject.tag == "Ground") {
			SetOnGround (false);
		}
	}
}
