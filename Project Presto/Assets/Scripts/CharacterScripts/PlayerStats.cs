using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Freedom Engine/Player Stats", order = 0)]
public class PlayerStats : ScriptableObject
{
	[Header("General")]
	public float minAngleToRotate;
	public float minSpeedToSlide;
	public float minAngleToSlide;
	public float minAngleToFall;
	public float pushBack;
	public float diePushUp;
	public float invincibleTime;
	public float controlLockTime;
	public float topSpeed;
	public float maxSpeed;

	[Header("Ring")]
	public float maxLostRingCount;
	public float ringScatterForce;

	[Header("Ground")]
	public float acceleration;
	public float deceleration;
	public float friction;
	public float slope;
	public float minSpeedToBrake;
	public float turnSpeed;

	[Header("Air")]
	public float airAcceleration;
	public float gravity;

	[Header("Jump")]
	public float minJumpHeight;
	public float maxJumpHeight;

	[Header("Roll")]
	public float minSpeedToRoll;
	public float minSpeedToUnroll;
	public float rollDeceleration;
	public float rollFriction;
	public float slopeRollUp;
	public float slopeRollDown;
	public float AutoActivateRollSpeed;
	[Header("Spindash")]
	public float SpinchargePower;
	public float SpinmaxChargePower;
	public float SpinminReleasePower;
	public float SpinpowerLoss;
	
	[Header("Peel")]
	public float PeelchargePower;
	public float PeelmaxChargePower;
	public float PeelminReleasePower;
	public float PeelpowerLoss;
}