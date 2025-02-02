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

    public LayerMask groundLayer;
    private float rayDistance;
    private float default_ControllerHeight;
    private Vector3 default_CamPos;
    private float camHeight;

    private FPSPlayeAnimation playeAnimation;

    [SerializeField]
    private WeaponManager weapon_Manager;
    private FPSWeapon current_Weapon;

    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private WeaponManager handsWeapon_Manager;
    private FPSHandsWeapon current_Hands_Weapon;
    void Start()
    {
        //GameObject("FPS View").Find();
        firstPerson_view = transform.Find("FPS View").transform; 
        characterController = GetComponent<CharacterController>();
        speed = walkSpeed;
        is_moving = false;

        rayDistance = characterController.height * 0.5f + characterController.radius;
        default_ControllerHeight = characterController.height;
        default_CamPos = firstPerson_view.localPosition;

        playeAnimation = GetComponent<FPSPlayeAnimation>();

        weapon_Manager.weapons[0].SetActive(true);
        current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();
        
        handsWeapon_Manager.weapons[0].SetActive(true);
        current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
    }

    void Update()
    {
        PlayerMovement();
        SelectWeapon();
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
            PlayerCrouchingAndSprinting();
            moveDir = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDir = transform.TransformDirection(moveDir) * speed;
     
            PlayerJump();
        }

        moveDir.y -= gravity * Time.deltaTime;

        is_grounded = (characterController.Move(moveDir * Time.deltaTime) & CollisionFlags.Below) != 0;

        is_moving = characterController.velocity.magnitude > 0.15f;

        HandleAnimations();

    }

    void PlayerCrouchingAndSprinting()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(!is_crouching)
            {
                is_crouching = true;
            }
            else
            {
                if(CanGetUp())
                {
                    is_crouching = false;
                }
            }

            StopCoroutine(MoveCameraCrouch());
            StartCoroutine(MoveCameraCrouch());
        }

        if(is_crouching)
        {
            speed = crouchSpeed;
        }
        else
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }
        }
        playeAnimation.PlayerCrouch(is_crouching);
    }

    bool CanGetUp()
    {
        Ray groundRay = new Ray(transform.position, transform.up);
        RaycastHit groundHit;
        if (Physics.SphereCast(groundRay, characterController.radius + 0.5f, out groundHit, rayDistance, groundLayer))
        {
            if(Vector3.Distance(transform.position, groundHit.point) < 2.3f)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator MoveCameraCrouch()
    {
        characterController.height = is_crouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;
        characterController.center = new Vector3(0f, characterController.height / 2f, 0f);

        camHeight = is_crouching ? default_CamPos.y / 1.5f : default_CamPos.y;

        while(Mathf.Abs(camHeight - firstPerson_view.localPosition.y) > 0.01f)
        {
            firstPerson_view.localPosition = Vector3.Lerp(firstPerson_view.localPosition, new Vector3(default_CamPos.x, camHeight, default_CamPos.z), Time.deltaTime * 11f);

            yield return null;
        }

    }
    void PlayerJump()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            if(is_crouching)
            {
                if(CanGetUp())
                {
                    is_crouching = false;

                    playeAnimation.PlayerCrouch(is_crouching);
                    StopCoroutine(MoveCameraCrouch());
                    StartCoroutine(MoveCameraCrouch());
                }
            }
            else
            {
                moveDir.y = jumpSpeed;
            }
        }
    }

    void HandleAnimations()
    {
        playeAnimation.Movement(characterController.velocity.magnitude);
        playeAnimation.PlayerJump(characterController.velocity.y);

        if(is_crouching && characterController.velocity.magnitude > 0f)
        {
            playeAnimation.PlayerCrouchWalk(characterController.velocity.magnitude);
        } 

        if(Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            if(is_crouching)
            {
                playeAnimation.Shoot(false);
            }
            else
            {
                playeAnimation.Shoot(true);
            }

            current_Weapon.Shoot();
            current_Hands_Weapon.Shoot();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            playeAnimation.ReloadGun();
            current_Hands_Weapon.Reload();
        }
    }

    void SelectWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!handsWeapon_Manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }

                current_Hands_Weapon = null;
                handsWeapon_Manager.weapons[0].SetActive(true);
                current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
            }

            if (!weapon_Manager.weapons[0].activeInHierarchy) 
            {
                for(int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[0].SetActive(true);
                current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();

                playeAnimation.ChangeController(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!handsWeapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }

                current_Hands_Weapon = null;
                handsWeapon_Manager.weapons[1].SetActive(true);
                current_Hands_Weapon = handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();
            }

            if (!weapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[1].SetActive(true);
                current_Weapon = weapon_Manager.weapons[1].GetComponent<FPSWeapon>();

                playeAnimation.ChangeController(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!handsWeapon_Manager.weapons[02].activeInHierarchy)
            {
                for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }

                current_Hands_Weapon = null;
                handsWeapon_Manager.weapons[2].SetActive(true);
                current_Hands_Weapon = handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();
            }

            if (!weapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[2].SetActive(true);
                current_Weapon = weapon_Manager.weapons[2].GetComponent<FPSWeapon>();

                playeAnimation.ChangeController(false);
            }
        }

    }
}
