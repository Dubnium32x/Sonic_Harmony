using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Ring")]
[RequireComponent(typeof(SphereCollider))]
public class Ring : FreedomObject
{
	[Header("Settings")]
	public bool lost = false;
	public bool collectable = true;
	private bool collected = false;
	public LayerMask solidLayer;

	[Header("Movement Parameter")]
	public float gravity;
	public float bounceFactor;
	public float lifeTime;
	public float uncollectibleTime;
	public float collectDelay = 1f;

	[Header("Components")]
	public AudioClip collectSound;

	private new SphereCollider collider;
	private new AudioSource audio;
	private Animator anim;

	private RaycastHit hit;
	private float uncollectibleTimer;
	private float lifeTimer;
	private float collectTimer;

	[HideInInspector]
	public Vector3 velocity;

	private void Start()
	{
		if (!TryGetComponent(out collider))
		{
			collider = gameObject.AddComponent<SphereCollider>();
		}

		if (!TryGetComponent(out audio))
		{
			audio = gameObject.AddComponent<AudioSource>();
		}

		anim = GetComponent<Animator>();

		collider.isTrigger = true;
	}

	private void Update()
	{
		if (this.gameObject.activeSelf)
		{
			var deltaTime = Time.smoothDeltaTime;

			HandleCollectibleStatus(deltaTime);
			HandleCollision(deltaTime);
			HandleLifeTime(deltaTime);
			HandleCollectTime(deltaTime);
		}
	}

	public override void OnRespawn()
	{
		Disable();
		Enable();
	}

	private void HandleCollectibleStatus(float deltaTime)
	{
		if (!collectable)
		{
			uncollectibleTimer += deltaTime;

			if (uncollectibleTimer >= uncollectibleTime)
			{
				collectable = true;
				uncollectibleTimer = 0;
			}
		}
	}

	private void HandleLifeTime(float deltaTime)
	{
		if (lost)
		{
			lifeTimer += deltaTime;

			if (lifeTimer >= lifeTime)
			{
				Disable();
				lifeTimer = 0;
			}
		}
	}

	private void HandleCollectTime (float deltaTime)
	{
		if(collected)
        {
			collectTimer += deltaTime;

			if(collectTimer > collectDelay)
            {
				Disable();
				collectTimer = 0;
            }
        }
	}

	private void HandleCollision(float deltaTime)
	{
		if (lost && !collected)
		{
			velocity.y -= gravity * deltaTime;
			transform.position += velocity * deltaTime;

			if (Physics.Raycast(transform.position, velocity.normalized, out hit, collider.radius, solidLayer))
			{
				velocity = Vector3.Reflect(velocity, hit.normal) * bounceFactor;
				transform.position = hit.point + hit.normal * collider.radius;
			}
		}
	}

	public void Disable()
	{
		collectable = false;
		this.gameObject.SetActive(false);
	}

	public void Enable()
	{
		collectable = !lost;
		collected = false;
		lifeTimer = 0;
		uncollectibleTimer = 0;
		collectTimer = 0;
		if(anim == null)
			anim = GetComponent<Animator>();
		anim.SetBool("Collected", false);
		this.gameObject.SetActive(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (collected || !collectable || !other.CompareTag("Player")) return;
		var player = other.gameObject.GetComponent<CharControlMotor>();
		ScoreManager.Instance.Rings++;
		player.jumpSource.PlayOneShot(player.audios.ring_ding);
		anim.SetBool("Collected", true);
		collected = true;
	}
}
