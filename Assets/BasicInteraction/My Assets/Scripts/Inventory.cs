using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private bool m_hasKey = false;

    public bool GetHasKey()
    {
        return m_hasKey;
    }

    private void Update()
    {
        //Model key pick up for testing
        if (Keyboard.current.qKey.wasPressedThisFrame)
            m_hasKey = !m_hasKey;
    }
}
