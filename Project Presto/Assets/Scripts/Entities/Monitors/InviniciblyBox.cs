using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/OneUp")]
public class InviniciblyBox : ItemBox
{
	protected override void OnCollect(CharControlMotor player)
	{
		player.AddLife();
	}
}
