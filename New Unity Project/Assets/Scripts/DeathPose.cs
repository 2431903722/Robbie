using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPose : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
