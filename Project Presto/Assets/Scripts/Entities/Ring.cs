using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Ring")]
[RequireComponent(typeof(SphereCollider))]
public class Ring : FreedomObject
{
	[Header("Settings")]
	public bool lost = false;
	public bool collectable = true;
	public LayerMask solidLayer;

	[Header("Movement Parameter")]
	public float gravity;
	public float bounceFactor;
	public float lifeTime;
	public float uncollectibleTime;

	[Header("Components")]
    public ParticleSystem collectParticle;
	public AudioSource collectSFX;

	private new SphereCollider collider;

    private RaycastHit hit;
	private float uncollectibleTimer;
	private float lifeTimer;

	[HideInInspector]
	public Vector3 velocity;

	private void Start()
	{
		if (!TryGetComponent(out collider))
		{
			collider = gameObject.AddComponent<SphereCollider>();
		}
        collider.isTrigger = true;
        var myPhys = gameObject.AddComponent<SphereCollider>();
        myPhys.isTrigger = false;
        myPhys.radius = 0.2f;
    }

	private void Update()
	{
		if (gameObject.activeSelf)
		{
			var deltaTime = Time.smoothDeltaTime;

			HandleCollectibleStatus(deltaTime);
			HandleCollision(deltaTime);
			HandleLifeTime(deltaTime);
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

	private void HandleCollision(float deltaTime)
	{
		if (lost)
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

		// Disable specified components so that audio still plays
		gameObject.GetComponent<SpriteRenderer>().enabled = false; // Disable sprite to no longer be shown
		gameObject.GetComponent<SphereCollider>().enabled = false; // Disable collider to no longer have collision checks
		enabled = false; // Disable script to no longer run code
		//gameObject.SetActive(false);
	}

	public void Enable()
	{
		collectable = !lost;
		lifeTimer = 0;
		uncollectibleTimer = 0;
		gameObject.SetActive(true);
		enabled = true;
		gameObject.GetComponent<SpriteRenderer>().enabled = true; // Disable sprite to no longer be shown
	}

	private void OnTriggerEnter(Collider other)
	{
		if (collectable && other.CompareTag("Player"))
		{
            other.gameObject.GetComponent<CharControlMotor>().RingGot();
            collectParticle.Play();
			collectSFX.Play();
			Disable();
		}
	}
}
