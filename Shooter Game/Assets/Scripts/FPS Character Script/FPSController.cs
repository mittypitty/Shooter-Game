using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private Transform firstPerson_view;
    private Transform firstPerson_camera;

    private Vector3 firstPerson_view_rotation = Vector3.zero;

    public float walkSpeed = 6.75f;
    public float runSpeed = 10f;
    public float crouchSpeed = 4f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    private float speed;

    private bool is_moving, is_grounded, is_crouching;

    private float inputX, inputY;
    private float inputX_set, inputY_set;
    private float inputModifyFactor;

    private bool limitDiagonalSpeed = true;

    private float antiBumpFactor = 0.75f;

    private CharacterController characterController;
    private Vector3 moveDir = Vector3.zero;

    void Start()
    {
        //GameObject("FPS View").Find();
        firstPerson_view = transform.Find("FPS View").transform; 
        characterController = GetComponent<CharacterController>();
        speed = walkSpeed;
        is_moving = false;
    }

    void Update()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if(Input.GetKey(KeyCode.W))
            {
                inputY_set = 1f;
            }
            else
            {
                inputY_set = -1f;
            }
        }
        else
        {
            inputY_set = 0f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                inputX_set = -1f;
            }
            else
            {
                inputX_set = 1f;
            }
        }
        else
        {
            inputX_set = 0f;
        }

        inputY = Mathf.Lerp(inputY, inputY_set, Time.deltaTime * 19f);
        inputX = Mathf.Lerp(inputX, inputX_set, Time.deltaTime * 19f);

        inputModifyFactor = Mathf.Lerp(inputModifyFactor, (inputY_set != 0 && inputX_set != 0 && limitDiagonalSpeed) ? 0.75f : 1.0f, Time.deltaTime * 19f);

        firstPerson_view_rotation = Vector3.Lerp(firstPerson_view_rotation, Vector3.zero, Time.deltaTime * 5f);
        firstPerson_view.localEulerAngles = firstPerson_view_rotation;

        if(is_grounded)
        {
            moveDir = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDir = transform.TransformDirection(moveDir) * speed;
        }

        moveDir.y -= gravity * Time.deltaTime;

        is_grounded = (characterController.Move(moveDir * Time.deltaTime) & CollisionFlags.Below) != 0;

        is_moving = characterController.velocity.magnitude > 0.15f;


    }
}
