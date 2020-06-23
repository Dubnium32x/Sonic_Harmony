using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/OneUp")]
public partial class OneUpBox : ItemBox
{
	protected override void OnCollect(CharControlMotor player)
	{
		player.AddLife();
	}
}
