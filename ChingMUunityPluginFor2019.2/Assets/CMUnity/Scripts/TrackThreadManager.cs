using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackThreadManager : MonoBehaviour
{
    private void Awake()
    {
        CMVrpn.CMUnityStart();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        CMVrpn.CMUnityQuit();
    }
}
