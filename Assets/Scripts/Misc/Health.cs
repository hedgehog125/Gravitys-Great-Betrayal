using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Health : MonoBehaviour {
	[SerializeField] private int m_startHealth;
	[SerializeField] private int m_maxHealth;
	[SerializeField] private int m_invulnerabilityTime;
	[SerializeField] private int m_safeTime = 10;
	[SerializeField] private DeathMode m_onDeath;
	private enum DeathMode {
		DoNothing,
		Delete,
		Respawn
	}

	[HideInInspector] public int CurrentHealth { get; private set; }
	[HideInInspector] public int StartHealth { get => m_startHealth; }
	[HideInInspector] public int MaxHealth { get => m_maxHealth; }
	[HideInInspector] public int SafeTime { get => m_safeTime; }

	[HideInInspector] public bool IsAlive { get; private set; }
	[HideInInspector] public bool IsInvulnerable { get; private set; }
	[HideInInspector] public bool IsSafe { get; private set; } = true;
	[HideInInspector] public Vector3 RespawnLocation;
	[HideInInspector] public Vector3 SoftRespawnLocation;


	private Rigidbody rb;

	private int invulnerabilityTick = -1;
	private int safeTick;
	private Vector3[] safeRespawnHistory;
	private readonly UnityEvent deathEvent = new();
	private readonly UnityEvent softRespawnEvent = new();

	private void Awake() {
		CurrentHealth = m_startHealth;
		RespawnLocation = transform.position;
		rb = GetComponent<Rigidbody>();

		safeRespawnHistory = new Vector3[m_safeTime];
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

		if (IsSafe) {
			Util.InsertAtStartAndShift(SoftRespawnLocation, safeRespawnHistory);
		}
		else {
			if (safeTick == m_safeTime) {
				IsSafe = true;
				safeTick = 0;
			}
			else {
				safeTick++;
			}
		}
	}

	public void TakeDamage(int amount, bool softRespawn = false) {
		Heal(-amount, softRespawn);
	}
	public void Heal(int amount, bool softRespawn = false) {
		bool isDamage = amount < 0;
		if (isDamage) {
			IsSafe = false;
			safeTick = 0;
		}
		if (isDamage && IsInvulnerable) return;

		CurrentHealth += amount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, m_maxHealth);

		IsAlive = CurrentHealth != 0;
		if (isDamage) {
			if (IsAlive) {
				invulnerabilityTick = 0;
				IsInvulnerable = true;

				if (softRespawn) SoftRespawn();
			}
			else Die();
		}
	}

	public void SoftRespawn() {
		transform.position = safeRespawnHistory[^1];
		rb.velocity = Vector3.zero;

		softRespawnEvent.Invoke();
	}
	private void Die() {
		deathEvent.Invoke();
		if (m_onDeath == DeathMode.Respawn) {
			transform.position = RespawnLocation;
			rb.velocity = Vector3.zero;

			CurrentHealth = m_startHealth;
			IsSafe = true;
		}
		else if (m_onDeath == DeathMode.Delete) {
			Destroy(gameObject);
			return;
		}
	}

	public void ListenForDeath(UnityAction callback) {
		deathEvent.AddListener(callback);
	}
	public void ListenForSoftRespawn(UnityAction callback) {
		softRespawnEvent.AddListener(callback);
	}
}
