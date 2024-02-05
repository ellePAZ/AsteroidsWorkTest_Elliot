using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipGunController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] Transform _firingTransform;
    [SerializeField] GameObject _bullet;
    
    [Header("Inputs")]
    [SerializeField] InputAction _fire;

    private void OnEnable()
    {
        _fire.Enable();
    }

    private void OnDisable()
    {
        _fire.Disable();
    }

    private void Start()
    {
        _fire.performed += _ => Fire();
    }

    void Fire()
    {
        Instantiate(_bullet, _firingTransform.position, transform.rotation);
    }
}
