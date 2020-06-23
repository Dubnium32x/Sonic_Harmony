using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/RingBox")]
public class RingBox : ItemBox
{
	public int ringAmount;

	protected override void OnCollect(CharControlMotor player)
	{
		for (var i = 0; i < ringAmount; i++)
		{
			player.RingGot();
		}
	}
}
