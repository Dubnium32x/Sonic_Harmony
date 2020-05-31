using UnityEngine;
using System.Collections;

[AddComponentMenu("Freedom Engine/Objects/Player/Player Skin")]
public class PlayerSkin : MonoBehaviour
{
	public enum Mouth { Center, Dead, Left, Right }

	[Header("Skin Animator")]
	public Animator animator;

	[Header("Transforms")]
	public Transform root;

	[Header("Game Objects")]
	public GameObject skin;

	[Header("Renderers")]
	public SpriteRenderer skinRenederer;

	private Transform currentActivatedBody;

	private IEnumerator blinkCoroutine;

	private void Start()
	{
		InitializeBody();
	}

	private void LateUpdate()
	{
	}

	private void InitializeBody()
	{
		skin.transform.localScale = Vector3.one;
		currentActivatedBody = skin.transform;
	}

	public void SetEulerY(float angle)
	{
		var euler = root.eulerAngles;
		euler.y = angle;
		root.eulerAngles = euler;
	}

	public void SetEulerZ(float angle)
	{
		var euler = root.eulerAngles;
		euler.z = angle;
		root.eulerAngles = euler;
	}
}