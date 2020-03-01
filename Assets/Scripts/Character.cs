using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] private float speed = 6f;
    Vector3 moveDirection;

    public delegate void MovementDelegate(Vector3 position);
    public MovementDelegate OnMove;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        OnMove += FlowField.Instance.GeneratePathTo;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection *= speed * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);

        if (moveDirection.magnitude > 0)
            OnMove?.Invoke(transform.position);
    }
}
