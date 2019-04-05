using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace TruffleSnuffle
{
    class GameObject
    {
        // Enum for different bounding types
        public enum BoundingType
        {
            BOX, 
            SPHERE
        }


        // Model
        public Model model;
        public Matrix[] transforms;

        // Transform data
        public Vector3 position = Vector3.Zero;
        public Vector3 rotation = Vector3.Zero;
        public Vector3 scale = Vector3.One;

        // Physics
        public Vector3 velocity = Vector3.Zero;
        public Vector3 acceleration = Vector3.Zero;
        public Vector3 collisionScale = Vector3.One;
        public Vector3 collisionOffset = Vector3.Zero;
        public List<GameObject> collidingWith = new List<GameObject>();
        public BoundingType boundingType = BoundingType.BOX;

        // Function to load our model in from file
        public void LoadModel(ContentManager content, string name)
        {
            // Load in our model from the content pipeline
            model = content.Load<Model>(name);

            // From our model, copy the transforms for each mesh (in model space)
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
        }

        // Draws the model to the screen
        public void Draw(Camera camera)
        {
            // Loop through the meshes in the 3D model, drawing each one in turn
            foreach (ModelMesh mesh in model.Meshes)
            {
                // Loop through each effect on the mesh
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // To render we need three things - world matrix, view matrix, and projection matrix
                    // But we actually start in model space - this is where our world starts before transforms

                    // ------------------------------
                    // MESH BASE MATRIX
                    // ------------------------------
                    // Our meshes start with world = model space, so we use our transforms array
                    effect.World = transforms[mesh.ParentBone.Index];


                    // ------------------------------
                    // WORLD MATRIX
                    // ------------------------------
                    // Transform from model space to world space in order - scale, rotation, translation.

                    // 1. Scale
                    // Scale our model by multiplying the world matrix by a scale matrix
                    // XNA does this for use using CreateScale()
                    effect.World *= Matrix.CreateScale(scale);

                    // 2. Rotation
                    // Rotate our model in the game world
                    effect.World *= Matrix.CreateRotationX(rotation.X); // Rotate around the x axis
                    effect.World *= Matrix.CreateRotationY(rotation.Y); // Rotate around the y axis
                    effect.World *= Matrix.CreateRotationZ(rotation.Z); // Rotate around the z axis

                    // 3. Translation / position
                    // Move our model to the correct place in the game world
                    effect.World *= Matrix.CreateTranslation(position);

                    // ------------------------------
                    // VIEW MATRIX
                    // ------------------------------
                    // This puts the model in relation to where our camera is, and the direction of our camera.
                    effect.View = Matrix.CreateLookAt(
                        camera.target + camera.offset,
                        camera.target,
                        Vector3.Up
                        );


                    // ------------------------------
                    // PROJECTION MATRIX
                    // ------------------------------
                    // Projection changes from view space (3D) to screen space (2D)
                    // Can be either orthographic or perspective

                    // Perspective
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        camera.FOV,
                        camera.aspectRatio,
                        camera.nearPlane,
                        camera.farPlane);

                    // Orthographic
                    //effect.Projection = Matrix.CreateOrthographic(
                    //    1600, 900, 1f, 10000f
                    //    );

                    // ------------------------------
                    // LIGHTING
                    // ------------------------------
                    // Some basic lighting, feel free to tweak and experiment
                    effect.LightingEnabled = true;
                    effect.Alpha = 1.0f;
                    effect.AmbientLightColor = new Vector3(1.0f);

                }

                // Draw the current mesh using the effects we set up
                mesh.Draw();
            }

            Matrix view = Matrix.CreateLookAt(
                        camera.target + camera.offset,
                        camera.target,
                        Vector3.Up
                        );

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                        camera.FOV,
                        camera.aspectRatio,
                        camera.nearPlane,
                        camera.farPlane);

            // Draw the bounds
            Color boundsColor = Color.Gray;
            // Change the color if we are colliding with something
            if (collidingWith.Count > 0)
            {
                boundsColor = Color.White;
            }

            // Render our bounds based on the type
            switch (boundingType)
            {
                case BoundingType.BOX:
                    BoundingRenderer.RenderBox(GetBoundingBox(), view, projection, boundsColor);
                    break;

                case BoundingType.SPHERE:
                    BoundingRenderer.RenderSphere(GetBoundingSphere(), view, projection, boundsColor);
                    break;
            }


        }

        public void Update(GameTime gameTime)
        {
            // Apply an overall drag ("friction")
            velocity *= 0.9f;


            // Handle acceleration (apply acceleration to velocity)
            // acceleration = change in velocity over a set period of time (a = dv/dt)
            // dv = a * dt
            // new velocity = old velocity + dv = old velocity + acceleration * time passed
            velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle velocity (apply velocity to position)
            // velocity = change in position over a set period of time (v = dp/dt)
            // dp = v * dt
            // new position = old positoin + dp = old position + velocity * time passed
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public BoundingSphere GetBoundingSphere()
        {
            BoundingSphere bounds = new BoundingSphere();

            // Use the position of our object + the center of the mesh
            bounds.Center = model.Meshes[0].BoundingSphere.Center + position + collisionOffset;

            // Scale our radius based on the model, our gameObject scale, and our collision scale
            // Just use X scale, as collision spheres are the same in all directions
            bounds.Radius = model.Meshes[0].BoundingSphere.Radius * scale.X * collisionScale.X;


            return bounds;
        }

        public BoundingBox GetBoundingBox()
        {
            BoundingBox bounds = new BoundingBox();

            // Find the center first, by starting at the model's world position
            // then adding an offset if the model's center also happens to be offset - scaled by the model's scale.
            bounds.Min = position + model.Meshes[0].BoundingSphere.Center + collisionOffset;

            // Then move this center to the top left corner by subtracting half the size of the model
            // Calculated by its radius and scaled by visual and collision scales
            bounds.Min.X -= (model.Meshes[0].BoundingSphere.Radius) * collisionScale.X * scale.X;
            bounds.Min.Y -= (model.Meshes[0].BoundingSphere.Radius) * collisionScale.Y * scale.Y;
            bounds.Min.Z -= (model.Meshes[0].BoundingSphere.Radius) * collisionScale.Z * scale.Z;

            // Find the max (the opposite corner) by adding on the model size, scaled
            bounds.Max.X = bounds.Min.X + model.Meshes[0].BoundingSphere.Radius * 2 * collisionScale.X * scale.X;
            bounds.Max.Y = bounds.Min.Y + model.Meshes[0].BoundingSphere.Radius * 2 * collisionScale.Y * scale.Y;
            bounds.Max.Z = bounds.Min.Z + model.Meshes[0].BoundingSphere.Radius * 2 * collisionScale.Z * scale.Z;

            return bounds;
        }

        public bool DetectCollision(GameObject otherObject)
        {
            bool collided = false;

            // Check if the two bounds collide
            // Check what kind of bounding WE have
            switch (boundingType)
            {
                case BoundingType.BOX:
                    // WE are a Box!

                    // Check what kind of bounding THEY have
                    switch (otherObject.boundingType)
                    {
                        case BoundingType.BOX:
                            // WE are a box, THEY are a box!
                            collided = GetBoundingBox().Intersects(otherObject.GetBoundingBox());
                            break;

                        case BoundingType.SPHERE:
                            // WE are a box, THEY are a sphere!
                            collided = GetBoundingBox().Intersects(otherObject.GetBoundingSphere());
                            break;
                    }


                    break;

                case BoundingType.SPHERE:
                    // WE are a Sphere!

                    // Check what kind of bounding THEY have
                    switch (otherObject.boundingType)
                    {
                        case BoundingType.BOX:
                            // WE are a Sphere, THEY are a box!
                            collided = GetBoundingSphere().Intersects(otherObject.GetBoundingBox());
                            break;

                        case BoundingType.SPHERE:
                            // WE are a Sphere, THEY are a sphere!
                            collided = GetBoundingSphere().Intersects(otherObject.GetBoundingSphere());
                            break;
                    }

                    break;
            }



            // If we collided, add eachother to our collision lists
            if (collided)
            {
                collidingWith.Add(otherObject);
                otherObject.collidingWith.Add(this);
            }

            return collided;
        }

    }
}
