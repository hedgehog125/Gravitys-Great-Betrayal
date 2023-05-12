using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
	[SerializeField] private int m_startHealth;
	[SerializeField] private int m_maxHealth;
	[SerializeField] private int m_invulnerabilityTime;
	[SerializeField] private DeathMode m_onDeath;
	private enum DeathMode {
		DoNothing,
		Delete,
		Respawn
	}

	[HideInInspector] public int CurrentHealth { get; private set; }
	[HideInInspector] public int StartHealth { get => m_startHealth; }
	[HideInInspector] public int MaxHealth { get => m_maxHealth; }
	[HideInInspector] public bool IsAlive { get; private set; }
	[HideInInspector] public bool IsInvulnerable { get; private set; }
	[HideInInspector] public Vector3 RespawnLocation;


	private Rigidbody rb;

	private int invulnerabilityTick = -1;
	private readonly UnityEvent deathEvent = new();

	private void Awake() {
		CurrentHealth = m_startHealth;
		RespawnLocation = transform.position;
		rb = GetComponent<Rigidbody>();
	}
	private void FixedUpdate() {
		if (IsInvulnerable) {
			if (invulnerabilityTick == m_invulnerabilityTime) {
				IsInvulnerable = false;
				invulnerabilityTick = -1;
			}
			else {
				invulnerabilityTick++;
			}
		}
	}

	public void TakeDamage(int amount) {
		Heal(-amount);
	}
	public void Heal(int amount) {
		bool isDamage = amount < 0;
		if (isDamage && IsInvulnerable) return;

		CurrentHealth += amount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, m_maxHealth);

		IsAlive = CurrentHealth != 0;
		if (IsAlive) {
			invulnerabilityTick = 0;
			IsInvulnerable = true;
		}
		else Die();
	}
	private void Die() {
		deathEvent.Invoke();
		if (m_onDeath == DeathMode.Respawn) {
			transform.position = RespawnLocation;
			rb.velocity = Vector3.zero;
			CurrentHealth = m_startHealth;
		}
		else if (m_onDeath == DeathMode.Delete) {
			Destroy(gameObject);
			return;
		}
	}

	public void ListenForDeath(UnityAction callback) {
		deathEvent.AddListener(callback);
	}
}
