using UnityEngine;
using Utilities;
using static UnityEditor.FilePathAttribute;

namespace Enemies
{
    public class AsteroidMovement : MonoBehaviour
    {
        [SerializeField] float _minSpeed;
        [SerializeField] float _maxSpeed;

        Vector3 _minBounds;
        Vector3 _maxBounds;

        Vector3 _direction;
        float _speed;

        private void Start()
        {
            MovementUtilities.GetMinMaxBounds(out _minBounds, out _maxBounds);

            _direction = Random.insideUnitCircle.normalized;
            _speed = Random.Range(_minSpeed, _maxSpeed);

            transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            transform.position = MovementUtilities.GetScreenWrapPosition(_minBounds, _maxBounds, transform);
        }
    }
}
