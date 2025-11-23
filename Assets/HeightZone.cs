using UnityEngine;
using System;
using System.Collections.Generic;

public class HeightZone : MonoBehaviour
{
    /*[SerializeField] public List<ScoopableObject.ScoopType> fallObjects = new();
    public List<ScoopableObject.ScoopType> FallObject => fallObjects;*/

    [Serializable]
    public struct PreloadItem
    {
        public ScoopableObject.ScoopType type;
    }

    [Header("objects that will cause the cup to fall in this zone")]

    public List<ScoopableObject.ScoopType> fallObjects = new();
    public List<ScoopableObject.ScoopType> FallObject => fallObjects;

}
