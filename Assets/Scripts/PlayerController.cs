using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

	public GameObject bulletPrefab;
	public Transform bulletSpawn;
			
	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
			
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f; // rotate
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 15.0f;    // backwards-forwards
		var s = Input.GetAxis ("Strafe") * Time.deltaTime * 15.0f;     // strafe left-right

		transform.Rotate(0, x, 0);
		transform.Translate(s, 0, z);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdFire();
		}
	}

	void FixedUpdate() {
		StabilizePlayer ();
	}

	// Rotate player back towards vertical
	void StabilizePlayer() {
		Debug.Log ("X rotation: " + transform.eulerAngles.x.ToString ("0.##") + " Y rotation: " + transform.eulerAngles.y.ToString ("0.##") + " Z rotation: " + transform.eulerAngles.z.ToString ("0.##"));

		// We want to rotate back up to vertical
		Vector3 newEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

		float speed = 0.2F;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newEulerAngles), Time.time * speed);
	}

	public override void OnStartLocalPlayer()
	{
		GetComponent<MeshRenderer>().material.color = Color.blue;
		Camera.main.GetComponent<CameraController>().setTarget(gameObject.transform);
	}

	[Command]
	void CmdFire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 25;

		// Spawn the bullet on the Clients
		NetworkServer.Spawn(bullet);

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);        
	}

}