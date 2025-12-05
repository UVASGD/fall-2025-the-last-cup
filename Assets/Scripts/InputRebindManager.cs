using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputRebindingManager : MonoBehaviour
{
    public static InputRebindingManager Instance { get; private set; }

    [Header("Input Actions Asset")]
    public InputActionAsset inputActions;

    private const string REBINDS_KEY = "InputRebinds";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRebinds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveRebinds()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(REBINDS_KEY, rebinds);
        PlayerPrefs.Save();
        Debug.Log("Input rebinds saved");
    }

    public void LoadRebinds()
    {
        string rebinds = PlayerPrefs.GetString(REBINDS_KEY, string.Empty);
        if (!string.IsNullOrEmpty(rebinds))
        {
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("Input rebinds loaded");
        }
    }

    public void ResetToDefaults()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey(REBINDS_KEY);
        PlayerPrefs.Save();
        Debug.Log("Input rebinds reset to defaults");
    }

    public InputAction GetAction(string actionName)
    {
        return inputActions.FindAction(actionName);
    }
}