using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/OneUp")]
public class OneUpBox : ItemBox
{
	protected override void OnCollect(CharControlMotor player)
	{
		ScoreManager.Instance.Lifes++;
	}
}
