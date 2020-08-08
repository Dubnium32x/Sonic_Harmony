using System;
using UnityEngine;
using UnityEditor;
public class DebugState : PlayerState
{///Simple Dimple Debug System SDDS by Birb64
	private int ObjectId;
	private bool DebugOn;
	private Transform Player; //Read-only
	[Header("Objects Needing testing")]
	public GameObject RedSpring;
	public GameObject YellowSpring;
	public GameObject Spikes;
	public GameObject Ring;
	public GameObject RingMonitor;
	public GameObject ShieldMonitor;
	public GameObject SpeedMonitor;
	public GameObject InvinciMonitor;
	public GameObject Checkpoint;
	public GameObject MovingPlatform;
	public GameObject FallingPlatform;
	public GameObject MotoBug;
	public GameObject FunSonic;
	public GameObject FunRing;
	public GameObject OneUp;
	public override void Enter(CharControlMotor player)
	{
		player.EnableCollision(false);
	}
	
    public override void Step(CharControlMotor player, float deltaTime)
    {
		Player = player.PlayerObject.transform;
			DebugOn = true;
			if (Input.GetKeyDown(KeyCode.Keypad6))
			{
				ObjectId = ObjectId + 1;
			}
			if (Input.GetKeyDown(KeyCode.Keypad4))
			{
				ObjectId = Mathf.Abs(ObjectId - 1);
			}
			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				if (ObjectId == 1)
				{
				Instantiate(RedSpring, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 2)
				{
				Instantiate(YellowSpring, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 3)
				{
				Instantiate(Spikes, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 4)
				{
				Instantiate(Ring, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 5)
				{
				Instantiate(RingMonitor, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 6)
				{
				Instantiate(ShieldMonitor, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 7)
				{
				Instantiate(SpeedMonitor, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 8)
				{
				Instantiate(InvinciMonitor, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 9)
				{
				Instantiate(Checkpoint, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 10)
				{
				Instantiate(MovingPlatform, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 11)
				{
				Instantiate(FallingPlatform, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 12)
				{
				Instantiate(MotoBug, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 13)
				{
				Instantiate(FunSonic, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 14)
				{
				Instantiate(FunRing, player.PlayerObject.transform.position, Quaternion.identity);
				}
				if (ObjectId == 15)
				{
				Instantiate(OneUp, player.PlayerObject.transform.position, Quaternion.identity);
				}
			}
		

        player.HandleAcceleration(deltaTime);
		
			player.velocity = Vector3.zero;
			
			if(player.input.right)
			{
				player.velocity = Vector3.right * 20;
			}
			if(player.input.left)
			{
				player.velocity = Vector3.left * 20;
			}
			if(player.input.up)
			{
				player.velocity = Vector3.up * 20;
			}
			if(player.input.down)
			{
				player.velocity = Vector3.down * 20;
			}
    }
	public override void Exit(CharControlMotor player)
	{
        DebugOn = false;
		player.EnableCollision(true);
	}
  void OnGUI() 
 {
	 if (DebugOn)
	 {
     GUI.Label(new Rect(1800, 100, 100, 1000), "Object Value: " + ObjectId.ToString());
	 
	 GUI.Label(new Rect(1800, 150, 100, 1000), "Controls");
	 GUI.Label(new Rect(1800, 200, 100, 1000), "Keypad2: Leave Debug Mode");
	 GUI.Label(new Rect(1800, 250, 100, 1000), "Keypad4: scroll through Object Id negative");
	 GUI.Label(new Rect(1800, 300, 100, 1000), "Keypad6: scroll through Object Id positive");
	 GUI.Label(new Rect(1800, 350, 100, 1000), "Keypad5: Place Object");
	 }
	
 }
}
