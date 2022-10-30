using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLocker : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

}
