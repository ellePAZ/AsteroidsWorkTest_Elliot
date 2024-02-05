using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class MovementUtilities
    {
        public static Vector3 GetScreenWrapPosition(Vector3 minBounds,  Vector3 maxBounds, Transform transform)
        {
            Vector3 position = transform.position;

            if (position.x < minBounds.x)
                position.x = maxBounds.x;
            if (position.x > maxBounds.x)
                position.x = minBounds.x;

            if (position.y < minBounds.y)
                position.y = maxBounds.y;
            if (position.y > maxBounds.y)
                position.y = minBounds.y;

            return position;
        }
    }
}
