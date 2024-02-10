using System.Collections;
using System.Collections.Generic;
using Modules.Localization;
using UnityEngine;

public class SetupLocalizationObject : MonoBehaviour
{
    public LocalizationObject localizationObject;
    
    private void Awake()
    {
        LocalizationObject.SetupInstanceCustom(localizationObject);
    }
}
