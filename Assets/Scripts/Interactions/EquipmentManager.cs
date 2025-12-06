using UnityEngine;

public enum EquipmentType { None, Straw, BucketHandle, JetPack }

[DisallowMultipleComponent]
public class EquipmentManager : MonoBehaviour
{
    [Header("Cup reference")]
    public CupController cup;                       // Uses cup.spawnPoint to drop on unequip

    [Header("Cup visuals (inactive by default)")]
    public GameObject strawVisual;                  // Child on the cup
    public GameObject bucketHandleVisual;           // Child on the cup
    public GameObject jetPackVisual;           		// Child on the cup

    [Header("Equipment Properties")]
    public EquipmentType CurrentType { get; private set; } = EquipmentType.None;
    public bool EquippedOn => CurrentType != EquipmentType.None;

    private EquipmentInteractable currentPickup;   // World pickup that was taken

    [Header("AnimationManager")]
    public AnimationManager animationManager;

    void Awake()
    {
        this.cup ??= GetComponentInChildren<CupController>(); //"this" is redundant, but good to have to differentiate between local and object vars. Also null coalescing operator ^.^ 
        this.SetVisuals(EquipmentType.None);
    }

    private void SetVisuals(EquipmentType eqipType)
    {
		this.CurrentType = eqipType;	//Avoids us needing to set it and call this function.
        if (strawVisual) strawVisual.SetActive(eqipType is EquipmentType.Straw);
        if (bucketHandleVisual) bucketHandleVisual.SetActive(eqipType is EquipmentType.BucketHandle);
		if (jetPackVisual) jetPackVisual.SetActive(eqipType is EquipmentType.JetPack);
		
        // Gate squirting on the cup (expects small helper in CupController)
		if (cup) cup.SetStrawEquipped(eqipType is EquipmentType.Straw);
    }

    private bool IsZiplining()
    {
        // Returns active GameObjects with Zipline tag
        // Fine for single-player, would need to be changed for multi-player
        var tagged = GameObject.FindGameObjectsWithTag("Zipline");
        foreach (var go in tagged)
        {
            var z = go.GetComponent<Zipline>();
            if (z && z.isActiveAndEnabled && z.zipping)
                return true;
        }
        return false;
    }

    public bool TryEquip(EquipmentInteractable pickup)
    {
        if (EquippedOn || pickup == null) return false;

        switch (pickup.type)
        {
            case EquipmentType.Straw: 			SetVisuals(EquipmentType.Straw); 			break;
            case EquipmentType.BucketHandle: 	SetVisuals(EquipmentType.BucketHandle); 	break;
            case EquipmentType.JetPack: 		SetVisuals(EquipmentType.JetPack); 	break;
            default: return false;
        }

        AudioManager.audioManagerInstance.PlaySFX(AudioManager.audioManagerInstance.equipment);

        currentPickup = pickup;

        // Hide world pickup while equipped (no physics changes)
        pickup.gameObject.SetActive(false);

        // Play scoop animation
        animationManager.Scoop();

        return true;
    }

    public bool TryUnequip()
    {
        if (!EquippedOn) return false;
        if (IsZiplining())				 { Debug.LogWarning("[EquipmentManager] Cannot unequip while ziplining."); 						return false; }
        if (!cup || !cup.spawnPoint)	 { Debug.LogWarning("[EquipmentManager] Cup or spawnPoint missing; cannot drop equipment."); 	return false; }

        if (currentPickup)
        {
            // Translate to pour spawn point and reactivate
            Transform pour = cup.spawnPoint;
            pour.rotation = Quaternion.Euler(pour.eulerAngles.x, pour.eulerAngles.y, currentPickup.transform.eulerAngles.z);
            currentPickup.transform.SetPositionAndRotation(pour.position, pour.rotation);
            currentPickup.gameObject.SetActive(true);
        }

        // Play descoop animation
        animationManager.Descoop();

        AudioManager.audioManagerInstance.PlaySFX(AudioManager.audioManagerInstance.equipment);

        currentPickup = null;
        this.SetVisuals(EquipmentType.None);

        return true;
    }
}