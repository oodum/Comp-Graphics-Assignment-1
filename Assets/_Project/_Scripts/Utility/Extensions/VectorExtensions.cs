using UnityEngine;
namespace Extensions {
    public static class VectorExtensions {
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
            return new(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }
        
        public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
            return new(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
        }

        public static Vector3 UnFlatten(this Vector2 vector) {
	        return new(vector.x, 0, vector.y);
        }

        public static Vector2 Flatten(this Vector3 vector) {
	        return new(vector.x, vector.z);
        }
    }
}
