using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float m_speed = 3.0f;
    [SerializeField] private float m_turnSpeed = 0.5f;
    private Vector3 m_moveDir = Vector3.zero;
    private Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        m_moveDir = new Vector3(inputVec.x, 0, inputVec.y);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        m_rigidbody.velocity = m_moveDir * m_speed;
        if (m_moveDir != Vector3.zero)
        {
            transform.LookAt(this.transform.position + m_moveDir.normalized);
        }
    }
}
