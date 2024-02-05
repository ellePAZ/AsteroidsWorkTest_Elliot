using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
	[Header("Movement parameters")]
	[SerializeField] float _rotationSpeed;
	[SerializeField] float _boosterForce;
	[SerializeField] float _maxVelocity;
	[SerializeField] float _decelerationSpeed;

	[Header("Inputs")]
	[SerializeField] InputAction _turn;
	[SerializeField] InputAction _boost;

	Vector3 _moveForce;

    #region Input Mapping Setup
    private void OnEnable()
	{
		_turn.Enable();
		_boost.Enable();
	}

	private void OnDisable()
	{
		_turn.Disable();
		_boost.Disable();
	}
	#endregion

	private void Start()
	{
		_moveForce = Vector2.zero;
	}

	void Update()
	{
		var rotationInput = _turn.ReadValue<float>();
		Turn(rotationInput);

		var boostInput = _boost.IsPressed();
		Boost(boostInput);
	}

	private void FixedUpdate()
	{
		transform.position = transform.position + (_moveForce * Time.deltaTime);
	}

	void Turn(float input)
	{
		transform.Rotate(0, 0, input * Time.deltaTime * _rotationSpeed);
	}

	void Boost(bool boosting)
	{
		if (boosting)
		{
			_moveForce += transform.up * Time.deltaTime * _boosterForce;
			if (_moveForce.magnitude > _maxVelocity)
			{
				var unit = _moveForce.normalized;
				_moveForce = unit * _maxVelocity;
			}
		}
		else
		{
			_moveForce -= _moveForce * Time.deltaTime * _decelerationSpeed;
		}        
	}
}
