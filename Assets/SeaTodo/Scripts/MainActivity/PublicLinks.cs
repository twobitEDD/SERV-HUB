using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicLinks : MonoBehaviour
{
    public string linkToGooglePlay;
    public string linkToConnections;
    
    public static PublicLinks Instance { private set; get; }
    private void Awake() => Instance = this;
}
