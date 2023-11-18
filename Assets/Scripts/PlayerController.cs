using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour, IDamagable, IWeaponHoldable
{
    [Serializable]
    public class AttributesOfPlayer
    {
        const int numbersOfTypes = 2;

        [SerializeField]
        private NetworkVariable<float[]> attributes = new(new float[numbersOfTypes], NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public enum AttributeType
        {
            STRENGTH, INTELLIGENCE
        }

        public AttributesOfPlayer(float strength, float intelligence)
        {
            attributes.Value[(int)AttributeType.STRENGTH] = strength;
            attributes.Value[(int)AttributeType.INTELLIGENCE] = intelligence;
        }

        public void ChangeValueOfAttribute(AttributeType type, float delta)
        {
            var label = delta > 0 ? "Increased" : "decreased";
            ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"{Math.Abs(delta)} {type.ToString().ToLowerInvariant()} {label}");
            attributes.Value[(int)type] += delta;
        }

        public bool CapableOf(AttributeType type, float requiredValue)
        {
            if (attributes.Value[(int)type] >= requiredValue)
            {
                ChangeValueOfAttribute(type, requiredValue * -1);
                return true;
            }
            ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"Attribute {type.ToString().ToLowerInvariant()} doesn't meet the requirement");
            return false;
        }
    }

    [Header("Children")]
    public Transform cam;
    public Transform orientation;
    public Transform cameraPos;
    public Transform weaponHoldPos;

    [Header("Prefabs")]
    [SerializeField] private GameObject orientationPrefab;
    [SerializeField] private GameObject weaponPosPrefab;

    [Header("Backpack")]
    public BackpackController backpack;

    [Header("Combat")]
    [SerializeField] private float totalHelath;
    [SerializeField] private WeaponController currentWeapon;
    public NetworkVariable<float> CurrentHealth { get; set; } = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayerMask;
    private bool isGrounded;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [SerializeField]
    public AttributesOfPlayer attributes;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Rigidbody rb;

    private Interactable currentInteractable;

    public float TotalHealth
    {
        get => totalHelath;
        set
        {
            TotalHealth = totalHelath;
        }
    }

    public WeaponController CurrentWeapon
    {
        get => currentWeapon;
        set
        {
            currentWeapon = value;
            currentWeapon.GetComponent<WeaponController>().owner = transform;
            UpdateWeaponRefServerRpc(currentWeapon.GetComponent<NetworkObject>());
        }
    }

    public ulong ClientId { get; private set; }

    public NetworkObjectReference OwnerReference => throw new NotImplementedException();

    [ServerRpc]
    private void UpdateWeaponRefServerRpc(NetworkObjectReference weaponRef, ServerRpcParams serverRpcParams = default)
    {
        weaponRef.TryGet(out var weaponObj);
        weaponObj.GetComponent<WeaponController>().owner = NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject.transform;
    }

    void Start()
    {
        if (!IsOwner)
        {
            StartCoroutine(IEFindOrientationRef());
            return;
        }

        Debug.Log("Player: Executing start function for player " + NetworkManager.Singleton.LocalClientId);

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        ClientId = NetworkManager.Singleton.LocalClientId;
        GameManager.instance.SetClientHealthServerRpc(totalHelath, GetComponent<NetworkObject>());
        cam = Camera.main.transform;

        //Update Health UI
        ClientUIController.instance.UpdateHealthBarValue(1);
        CurrentHealth.OnValueChanged += (float previous, float to) => ClientUIController.instance.UpdateHealthBarValue(to / TotalHealth);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (!NetworkManager.Singleton.IsHost)
        {
            SpawnNestedChildrenServerRpc();
            return;
        }

        SpawnNestedChildren();
        InitializeAttributes();
    }

    private void InitializeAttributes()
    {
        attributes = new(100, 100);
    }

    private IEnumerator IEFindOrientationRef()
    {
        yield return new WaitUntil(() => transform.Find("Orientation(Clone)") != null);
        orientation = transform.Find("Orientation(Clone)");
        weaponHoldPos = orientation.Find("WeaponPos(Clone)");
    }

    private void SpawnNestedChildren()
    {
        var _orientationObj = Instantiate(orientationPrefab).transform;
        _orientationObj.GetComponent<NetworkObject>().Spawn();
        _orientationObj.SetParent(transform);
        orientation = _orientationObj;

        var _weaponPos = Instantiate(weaponPosPrefab).transform;
        _weaponPos.GetComponent<NetworkObject>().Spawn();
        _weaponPos.SetParent(_orientationObj);
        weaponHoldPos = _weaponPos;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnNestedChildrenServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong _clientId = serverRpcParams.Receive.SenderClientId;
        PlayerController clientObj = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject.GetComponent<PlayerController>();
        Debug.Log("Player: Client Id for player's clientObj is " + _clientId);

        var _orientationObj = Instantiate(orientationPrefab).transform;
        _orientationObj.GetComponent<NetworkObject>().SpawnWithOwnership(_clientId);
        _orientationObj.SetParent(clientObj.transform);

        var _weaponPos = Instantiate(weaponPosPrefab).transform;
        _weaponPos.GetComponent<NetworkObject>().SpawnWithOwnership(_clientId);
        _weaponPos.SetParent(_orientationObj);

        Debug.Log("Player: Client " + _clientId + " isn't host, calling client rpc");
        SetReferenceForClientRpc(_clientId, _orientationObj.GetComponent<NetworkObject>(), _weaponPos.GetComponent<NetworkObject>());
    }

    [ClientRpc]
    private void SetReferenceForClientRpc(ulong _clientId, NetworkObjectReference _orientationObjRef, NetworkObjectReference _weaponPosRef)
    {
        if (_clientId != ClientId || NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Player: Local client " + ClientId + " doesn't match targeted id " + _clientId + ", returning");
            return;
        }

        if (_orientationObjRef.TryGet(out NetworkObject _orientationObj) && _weaponPosRef.TryGet(out NetworkObject _weaponPosObj))
        {
            orientation = _orientationObj.GetComponent<Transform>();
            weaponHoldPos = _weaponPosObj.GetComponent<Transform>();
            Debug.Log("Player: orientation is set to " + orientation);
        }
        else
        {
            Debug.Log("Player: Cannot resolve orientation ref");
        }
    }

    public void Death() { }

    private void HandleInput()
    {
        // Handle cursor states and key inputs
        if (ClientUIController.instance != null && ClientUIController.instance.IsUsingUIElements())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            horizontalInput = 0;
            verticalInput = 0;
        }
        else
        {
            if (Cursor.visible == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            // Key map
            if (Input.GetKeyDown(KeyCode.E))
            {
                ClientUIController.instance.OpenBackpackPanel();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CurrentWeapon.Reload();
            }

            if (Input.GetMouseButton(0) && CurrentWeapon != null)
            {
                CurrentWeapon.Fire();
            }
        }
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }

    private void HandleVisualHint()
    {
        Debug.DrawRay(cameraPos.position, cam.forward, Color.white);
        if (Physics.Raycast(cameraPos.position, cam.forward, out RaycastHit hitInfo, 5f))
        {
            hitInfo.collider.GetComponent<IHintDisplayable>()?.DisplayHintText();
        }
        else if (ClientUIController.instance != null)
        {
            ClientUIController.instance.CloseHintText();
        }
    }

    private void HandleInteract()
    {
        if (Physics.Raycast(cameraPos.position, cam.forward, out RaycastHit hitInfo, 5f))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                currentInteractable = hitInfo.collider.GetComponent<Interactable>();
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log($"PlayerController: Interacting with {currentInteractable}");
                    currentInteractable.InteractServerRpc(ClientId);
                }

                else if (Input.GetKeyUp(KeyCode.F) && currentInteractable.isRequireHoldToInteract && currentInteractable.GetIsInteracting())
                {
                    Debug.Log($"PlayerController: Stop interacting!");
                    currentInteractable.StopInteractServerRpc(ClientId);
                }

            }

        }

        else if (currentInteractable != null && currentInteractable.GetIsInteracting())
        {
            Debug.Log($"PlayerController: Stop interacting!");
            currentInteractable.StopInteractServerRpc(ClientId);
            currentInteractable = null;
        }
    }

    private void HandleSpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void HandleDrug()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        HandleGroundCheck();
        HandleMovement();
        HandleSpeedControl();
        HandleDrug();
    }

    private void HandleMovement()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    public bool GetIsOwner() => IsOwner;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        HandleInput();
        HandleVisualHint();
        HandleInteract();
    }

    public Transform GetWeaponHoldPosition() => weaponHoldPos;

    public Quaternion GetFacingDirection() => cam.rotation;
}

