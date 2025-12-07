using UnityEngine;

public class SquirtMechanic : MonoBehaviour
{
    [Header("Squirting Setup")]
    public WaterProjectileConfig projectileConfig;
    public Transform strawTip;

    // Reference to CupController on the same GameObject
    private CupController cupController;
    private WaterManager waterManager; //Used to spawn water

    // Squirting-specific state
    public bool squirtOn = false;
    // private float currentWater = 0f;
    private float fireTimer = 0f;
    private Collider[] selfColliders;

    [Header("AnimationManager")]
    public AnimationManager animationManager;

    AudioSource squirtLoopingSource;

    void Awake()
	{
		if (!this.StrictTryGetComponent(out this.cupController)) this.enabled = false;
		if (!this.StrictTryGetComponent(out this.waterManager)) this.enabled = false;

        selfColliders = GetComponentsInChildren<Collider>();

        // Setup projectile layer collision rules
        if (projectileConfig != null)
        {
            int projLayer = LayerMask.NameToLayer(projectileConfig.projectileLayerName);
            if (projLayer != -1)
            {
                Physics.IgnoreLayerCollision(projLayer, projLayer, true);
            }
        }
    }

    void Update()
    {
        // Handle pause menu (same logic as CupController)
        if (Time.timeScale == 0f) return;
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

		// Handle squirting input
		if (Input.GetKeyDown(KeyCode.Mouse1) && cupController.HasStraw && waterManager.WaterLevel > 0) 
		{
			animationManager.Squirt();
			squirtOn = true;
		} else if (Input.GetKey(KeyCode.Mouse1) && cupController.HasStraw ) 
		{
			ProcessSquirting();
		} else if (Input.GetKeyUp(KeyCode.Mouse1) && squirtOn == true) 
		{
			animationManager.Unsquirt();
            AudioManager.audioManagerInstance.StopLoopingSFX(squirtLoopingSource);
            squirtLoopingSource = null;
			squirtOn = false;
			fireTimer = 0f;
		}
    }

    // This method gets called by CupController when straw equipment changes
    public void OnStrawEquipmentChanged(bool hasStraw)
    {
        // Stop ongoing squirt if straw is removed
        if (!hasStraw && squirtOn)
        {
            animationManager.Unsquirt();
            squirtOn = false;
            fireTimer = 0f;
        }
    }


    private void ProcessSquirting()
    {
        if (projectileConfig == null) return;
		if (waterManager.ProcessSquirting() is false) return;

        if (squirtLoopingSource == null)
        {
            squirtLoopingSource = AudioManager.audioManagerInstance.PlayLoopingSFX(
                AudioManager.audioManagerInstance.squirt, false, 1, 5,
                transform
            );
        }

        // Emit droplets at fire rate
        fireTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, projectileConfig.fireRate);

		while (fireTimer >= interval && cupController.movementController._isAimingActive) {
			fireTimer -= interval;
			this.waterManager.SpawnWaterDroplet(this.strawTip);
		}
		
		//Points squitting nozzle towards camera look direction
		var tempForward = this.strawTip.forward;
		tempForward.y = this.cupController.movementController.CinemachineCameraTarget.transform.forward.y + 0.25f; //Add bias to shoot up
		this.strawTip.forward = tempForward;
    }


	// Debug visualization
	void OnDrawGizmosSelected() {
		if (strawTip != null) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(strawTip.position, 0.02f);
			Gizmos.DrawRay(strawTip.position, strawTip.forward * 0.5f);
		}
	}
}