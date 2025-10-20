using UnityEngine;
using System.Collections.Generic;

public class JetpackMechanic : MonoBehaviour {
	//Public~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[Header("Squirting Setup")]
	public WaterProjectileConfig projectileConfig;
	public Transform lNozzle, rNozzle;
	//Private~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private CupController cupController;  // Reference to CupController on the same GameObject
	private WaterManager waterManager;
	private float fireTimer = 0f;

	//###################################################################################################
	void Awake() {
		if (!this.AttemptComponentFetch(out this.cupController)) this.enabled = false;
		if (!this.AttemptComponentFetch(out this.waterManager)) this.enabled = false;

		// Setup projectile layer collision rules
		if (this.projectileConfig != null) {
			int projLayer = LayerMask.NameToLayer(this.projectileConfig.projectileLayerName);
			if (projLayer != -1) Physics.IgnoreLayerCollision(projLayer, projLayer, true);
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

		// Handle continuous squirting
		if (cupController.equipmentManager.CurrentType is EquipmentType.JetPack && Input.GetKey(KeyCode.Space)) {
			this.ProcessSquirting();
		}
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ProcessSquirting() {
		if (this.waterManager.ProcessSquirting() is false) return;
		if (this.waterManager.ProcessSquirting() is false) return;
		if (this.waterManager.ProcessSquirting() is false) return;
		  // Emit droplets at fire rate
        fireTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, projectileConfig.fireRate * 3);

		this.cupController.movementController.SetVerticalVelocity(4f); //TODO: Expose velocity and interpolate
		while (fireTimer >= interval) {
			fireTimer -= interval;
			this.waterManager.SpawnWaterDroplet(this.lNozzle);
			this.waterManager.SpawnWaterDroplet(this.rNozzle);
		}
	}


	//###################################################################################################
	
	//TODO: move into a utility script
	private bool AttemptComponentFetch<T>(out T result) where T : Component {
        result = this.gameObject.GetComponent<T>();
		if (result == null) {
			Debug.LogError($"{this.GetType().Name} requires a {typeof(T)} component on the same GameObject!");
			return false;
		} else return true;
	}
	
	// Debug visualization
	private void OnDrawGizmosSelected() {
		if (this.lNozzle && this.rNozzle) {
			Gizmos.color = Color.cyan;

			Gizmos.DrawWireSphere(this.lNozzle.position, 0.02f);
			Gizmos.DrawWireSphere(this.rNozzle.position, 0.02f);

			Gizmos.DrawRay(this.lNozzle.position, this.lNozzle.forward * 0.5f);
			Gizmos.DrawRay(this.rNozzle.position, this.rNozzle.forward * 0.5f);
		}
	}
}
