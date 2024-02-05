using UnityEngine;
using Utilities;

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
            _minBounds = Camera.main.ScreenToWorldPoint(Vector3.zero);
            _maxBounds = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));

            _direction = Random.insideUnitCircle.normalized;
            _speed = _maxSpeed/*Random.Range(_minSpeed, _maxSpeed)*/;
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            transform.position = MovementUtilities.GetScreenWrapPosition(_minBounds, _maxBounds, transform);
        }
    }
}
