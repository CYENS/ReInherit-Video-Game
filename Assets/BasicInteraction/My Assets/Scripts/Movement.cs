using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float m_speed = 3.0f;
    [SerializeField] private float m_turnSpeed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.wKey.isPressed)
            transform.Translate(Vector3.forward * Time.deltaTime * m_speed);
        if (Keyboard.current.sKey.isPressed)
            transform.Translate(-1 * Vector3.forward * Time.deltaTime * m_speed);
        if (Keyboard.current.aKey.isPressed)
            transform.Rotate(0, -1 * m_turnSpeed, 0);
        if (Keyboard.current.dKey.isPressed)
            transform.Rotate(0, 1 * m_turnSpeed, 0);
    }
}
