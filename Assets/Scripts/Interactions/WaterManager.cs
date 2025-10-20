using UnityEngine;
using System.Linq;
using Unity.VisualScripting;


//The manager is used by both the straw and the jetpack to produce water droplets and track water consumption
public class WaterManager : MonoBehaviour {
	//Public~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public WaterProjectileConfig projectileConfig;
	
	//Private~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[Header("Debugging")]
	[SerializeField]
	private bool ConsumesWater = true;
	[ReadOnly, SerializeField]
	private float currentWater = 0f;
	public float WaterLevel => this.currentWater;
	
    private CupController cupController;
	private Collider[] selfColliders;

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	// public bool HasWater => this.currentWater > 0f;

    private bool HasWater() {
        // Access CupController's properties directly
        return cupController.IsFull
            && (cupController.HeldType == ScoopableObject.ScoopType.Water ||
                cupController.HeldType == ScoopableObject.ScoopType.PouringWater);
    }

	//###################################################################################################
	void Awake() {
		if (!this.StrictTryGetComponent(out cupController)) this.enabled = false;

		this.selfColliders = GetComponentsInChildren<Collider>();

		// Setup projectile layer collision rules
		if (this.projectileConfig != null && this.projectileConfig.TryGetProjectileLayer(out int projLayer)) {
			Physics.IgnoreLayerCollision(projLayer, projLayer);
		}
	}

	void Update() {
        // Refills if previously empty
        if (currentWater == 0 && HasWater()) {
            currentWater = 100f;
        }
        if (currentWater > 0 && !HasWater()) {
            currentWater = 0;
        }
	}

	// This method gets called by the animation state when scooping
	public void OnWaterScooped(ScoopableObject.ScoopType type) {
		// Fill water capacity for water types
		if (type == ScoopableObject.ScoopType.Water || type == ScoopableObject.ScoopType.PouringWater) {
			currentWater = projectileConfig != null ? projectileConfig.maxWater : 100f;
		}
	}

    // This method gets called by the animation state when cup is emptied
    public void OnCupEmptied() {
        this.currentWater = 0f;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


	//This is called by the Squirt SquirtMechanic and JetpackMechanic scripts to produce water.
	//Returns if spawning is successful. TODO: add water consumption
	public void SpawnWaterDroplet(Transform spawner) => this.GenerateWaterDroplet(spawner);
	public void SpawnWaterDroplet(params Transform[] spawners) {
		foreach (var spawner in spawners) this.GenerateWaterDroplet(spawner);
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool ProcessSquirting() { //Returns if there is enough water available to process squirt.
		if (projectileConfig == null) return false;
		if (this.ConsumesWater is false) return true;
        // Consume water continuously
        float consume = projectileConfig.squirtRate * Time.deltaTime;
        currentWater = Mathf.Max(0f, currentWater - consume);
		if (currentWater == 0f) {
			this.cupController.EmptyCup();
			return false;
		} else return true;
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void GenerateWaterDroplet(Transform spawner) {
		if (spawner == null || this.projectileConfig == null) return;

		// Calculate spawn position and rotation
		const float spawnOffset = 0.06f;
		Vector3 spawnPos = spawner.position + spawner.forward * spawnOffset;
		Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, spawner.forward);

		// Create projectile
		if (CreateProjectile(spawnPos, spawnRot, out GameObject go)) {
			SetupProjectilePhysics(go, spawner.forward); // Setup physics
			InitializeWaterProjectile(go); // Initialize WaterProjectile component
			// ProcessSquirting();
		}
	}

	private bool CreateProjectile(Vector3 position, Quaternion rotation, out GameObject go) {
		if (projectileConfig.waterProjectilePrefab != null) {
			go = Instantiate(projectileConfig.waterProjectilePrefab, position, rotation);
		} else {
			// Fallback: create sphere
			go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			(go.transform.position, go.transform.rotation, go.transform.localScale) = (position, rotation, Vector3.one * 0.1f);
			var mr = go.GetComponent<MeshRenderer>();
			if (mr && projectileConfig.waterMaterial) mr.material = projectileConfig.waterMaterial;
		}

		// Set layer
		if (this.projectileConfig.TryGetProjectileLayer(out int projLayer)) go.layer = projLayer;
		return go != null; //This is from original SpawnWaterDroplet function, this needs reworked
	}

	private void SetupProjectilePhysics(GameObject go, Vector3 direction) {
		var col = go.GetOrAddComponent<Collider, SphereCollider>();
		var rb = go.GetOrAddComponent<Rigidbody>();

		col.isTrigger = true;
		rb.useGravity = true;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rb.interpolation = RigidbodyInterpolation.Interpolate;

		// Apply force
		// var cupVel = cupController.movementController.CurrentVelocity * 0.75f;
		// cupVel.y = 0;
		// rb.linearVelocity = cupVel;
		rb.AddForce(direction * projectileConfig.muzzleSpeed, ForceMode.VelocityChange);
	}

	private void InitializeWaterProjectile(GameObject go) {
		var proj = go.GetOrAddComponent<WaterProjectile>();
		// Filter out null/disabled colliders
		var validColliders = selfColliders?.Where((col) => col != null && col.enabled).ToArray();
		proj.Init(projectileConfig.damage, projectileConfig.lifetime, validColliders);
	}

	//###################################################################################################
	//Utility functions
		
}
