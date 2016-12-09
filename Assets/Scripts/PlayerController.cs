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
			
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 9.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

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
		Debug.Log ("X rotation: " + transform.eulerAngles.x + " Y rotation: " + transform.eulerAngles.y + " Z rotation: " + transform.eulerAngles.z);
		transform.Rotate(-transform.eulerAngles.x * 0.9f, 0, -transform.eulerAngles.z * 0.9f);	
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
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 15;

		// Spawn the bullet on the Clients
		NetworkServer.Spawn(bullet);

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);        
	}

}