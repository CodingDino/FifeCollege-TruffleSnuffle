using Microsoft.Xna.Framework;

namespace TruffleSnuffle
{
    class Camera
    {
        public Vector3 offset = Vector3.Zero;           // Camera's offset from its target
        public Vector3 target = Vector3.Zero;           // Camera's focus point (often the player)
        public float FOV = MathHelper.ToRadians(45);    // Angle of camera lense, larger = sees more to the sides (fish eye)
        public float aspectRatio = 16f / 9f;            // Ratio of width of view to height of view
        public float nearPlane = 1f;                    // Camera can't see closer to itself than this
        public float farPlane = 100000f;                // Camera can't see farther from itself than this

    }
}
