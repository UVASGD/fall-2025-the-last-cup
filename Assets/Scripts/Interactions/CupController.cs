using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CupController : MonoBehaviour {
	[SerializeField]
	AudioSource sheathSound;

	[SerializeField]
	AudioSource unsheathSound;

	[SerializeField]
	AudioSource swingSound;

	[Header("Cup Components")]
	public SkinnedMeshRenderer cupBodyRenderer;

	public ThirdPersonController movementController;
	public EquipmentManager equipmentManager;
	public Material defaultMaterial;
	public Transform spawnPoint;
	public GameObject dirtCupPrefab;

	[Header("Cup Properties")]
	public float cooldownDuration = 0.2f;

	[Header("Equipment State")]
	[SerializeField] private bool _hasStraw = false;

	[Header("AnimationManager")]
	public AnimationManager animationManager;

	// Cup state properties
	public bool IsFull { get; private set; }
	public bool IsInCooldown => cooldownTimer > 0f;
	public bool HasStraw => _hasStraw;

	// Private fields
	private ScoopableObject.ScoopType heldType;
	private Material heldMaterial;
	private GameObject heldObject;
	private float cooldownTimer = 0f;

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	//public event

	// public event Action<ScoopableObject.ScoopType, GameObject> OnScoop;





	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	protected virtual void Update() {
		// Handle pause menu
		if (Time.timeScale == 0f) return;
		if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

		// Update cooldown
		if (IsInCooldown) cooldownTimer -= Time.deltaTime;
		//this.animationManager.Walk(this.movementController.CurrentVelocity.magnitude);
		// Handle base interactions
		if (Input.GetMouseButtonDown(0) && IsFull && !IsInCooldown)
			TryDescoop();
		else if (Input.GetKeyDown(KeyCode.R)) {
			swingSound.Play();
			animationManager.Spin();
		}
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void SetStrawEquipped(bool on) {
		_hasStraw = on;
	}

	public virtual void Scoop(ScoopableObject.ScoopType type, Material mat, GameObject sourceObject) {
		heldType = type;
		heldMaterial = mat;
		heldObject = (type == ScoopableObject.ScoopType.Object || type == ScoopableObject.ScoopType.DirtCup) ? sourceObject : null;

		this.SetMatieral(mat);

		if (type != ScoopableObject.ScoopType.PouringWater && IsFull == false) {
			sheathSound.Play();
			animationManager.Scoop();
		}
		if (heldObject != null) heldObject.SetActive(false);

		IsFull = true;
		cooldownTimer = cooldownDuration;
	}

	protected virtual void TryDescoop() {
		//Handles descooping into a container
		if (IsFull && heldType is not ScoopableObject.ScoopType.Object) {
			if (Physics.Raycast(transform.position, transform.forward, out var hit, 3f)) {
				if (hit.collider.TryGetComponent<Container>(out var container)) {
					if (container.TryAdd(heldType, heldMaterial)) {
						unsheathSound.Play();
						animationManager.Descoop();
						EmptyCup();
						cooldownTimer = cooldownDuration;
						return;
					}
				}
			}
		}

		// Handle object descooping
		if (heldType is ScoopableObject.ScoopType.Object && heldObject != null) {
			heldObject.transform.position = spawnPoint.position;
			heldObject.SetActive(true);
		} else if (heldType is ScoopableObject.ScoopType.Dirt || heldType is ScoopableObject.ScoopType.DirtCup) {
			if (dirtCupPrefab != null) {
				GameObject.Instantiate(dirtCupPrefab).transform.position = spawnPoint.position;
			}
		} else {
			//If we think there should be an object but its null, make dirt to still have something spawn.
			if (heldType is ScoopableObject.ScoopType.Object) {
				if (dirtCupPrefab != null) {
					GameObject.Instantiate(dirtCupPrefab).transform.position = spawnPoint.position;
				}
			}
		}

		if (IsFull) {
			unsheathSound.Play();
			animationManager.Descoop();
			EmptyCup();
		}
		cooldownTimer = cooldownDuration;
	}

	public void EmptyCup() {
		this.SetMatieral(defaultMaterial);

        heldType = ScoopableObject.ScoopType.None;

		IsFull = false;
		heldObject = null;
		heldMaterial = null;
	}

	// Protected accessors for derived classes
	public ScoopableObject.ScoopType HeldType => heldType;
	protected Material HeldMaterial => heldMaterial;
	protected GameObject HeldObject => heldObject;

	private void SetMatieral(Material mat) {
		var mats = cupBodyRenderer.materials;
		if (mats.Length > 1) {
			mats[1] = mat;
			cupBodyRenderer.materials = mats;
		}
	}
}