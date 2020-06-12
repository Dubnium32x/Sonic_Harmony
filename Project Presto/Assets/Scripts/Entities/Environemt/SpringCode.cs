using UnityEngine;
using System.Collections;
using System.Linq;

[AddComponentMenu("Freedom Engine/Objects/Spring")]
[RequireComponent(typeof(Collider))]
public class SpringCode : MonoBehaviour
{
	[Header("General Settings")]
	public float force;
	public Transform mesh;
	public AudioClip springSound;

	[Header("Player Snap Settings")]
	public bool snapPositionX;
	public bool snapPositionY;

	[Header("Control Lock Settings")]
	public bool lockControl;
	public float lockTime;

	[Header("Hide Settings")]
	public bool hidden;
	public float hideDistance;
	public float hideTime;

	private new AudioSource audio;
	private new Collider collider;

	private Vector3 meshStartPosition;
	private Vector3 hidePoint;

	private void Start()
	{
		InitializeCollider();
		InitializeAudioSource();
		InitializeSpring();
	}

	private void InitializeCollider()
	{
		var mycolliders = GetComponents<BoxCollider>();
		if (mycolliders == null || mycolliders.Length < 2)
		{
			collider = gameObject.AddComponent<BoxCollider>();
			collider.isTrigger = true;
		}
		else
		{
			//get designated trigger collider
			collider = mycolliders.First(Result => Result.isTrigger);
		}
	}

	private void InitializeAudioSource()
	{
		if (!TryGetComponent(out audio))
		{
			audio = gameObject.AddComponent<AudioSource>();
		}
	}

	private void InitializeSpring()
	{
		if (hidden)
		{
			meshStartPosition = mesh.position;
			hidePoint = mesh.position - transform.up * hideDistance;
			mesh.position = hidePoint;
		}
	}

	private void HandlePlayerSnaping(CharControlMotor player)
	{
		if (snapPositionX || snapPositionY)
		{
			var playerPosition = player.transform.position;

			if (snapPositionX)
			{
				playerPosition.x = transform.position.x;
			}

			if (snapPositionY)
			{
				playerPosition.y = transform.position.y;
			}

			player.transform.position = playerPosition;
		}
	}

	private IEnumerator ShowSpring()
	{
		mesh.transform.position = meshStartPosition;

		yield return new WaitForSeconds(2f);

		var elapsedTime = 0f;
		var initialPosition = mesh.position;

		while (elapsedTime < hideTime)
		{
			var alpha = elapsedTime / hideTime;
			mesh.position = Vector3.Lerp(initialPosition, hidePoint, alpha);
			elapsedTime += Time.deltaTime;

			yield return null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<CharControlMotor>(out var player))
		{
			//check if player is landing on spring
			var direction = (player.transform.position - transform.position).normalized;
			if (!(Vector3.Dot(transform.up, direction) > 0.7f) ) return;
			
			player.velocity = transform.up.normalized * force;
			player.UpdateDirection(player.velocity.x);

			if (lockControl)
			{
				player.input.LockHorizontalControl(lockTime);
			}

			HandlePlayerSnaping(player);

			if (transform.up.y > 0)
			{
				player.state.ChangeState<SpringState>();
			}

			if (hidden)
			{
				StopAllCoroutines();
				StartCoroutine(ShowSpring());
			}

			audio.PlayOneShot(springSound, 0.5f);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Debug.DrawRay(transform.position, transform.up * 2f);
	}
#endif
}