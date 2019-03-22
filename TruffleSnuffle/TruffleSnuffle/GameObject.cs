using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TruffleSnuffle
{
    class GameObject
    {

        // Model
        public Model model;
        public Matrix[] transforms;

        // Transform data
        public Vector3 position = Vector3.Zero;
        public Vector3 rotation = Vector3.Zero;


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
        public void Draw()
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

                    // 1. Scale (TODO)

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
                        new Vector3(0f, 300f, -400f),
                        Vector3.Zero,
                        Vector3.Up
                        );


                    // ------------------------------
                    // PROJECTION MATRIX
                    // ------------------------------
                    // Projection changes from view space (3D) to screen space (2D)
                    // Can be either orthographic or perspective

                    // Perspective
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45f),
                        16f / 9f,
                        1f,
                        10000f);

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
        }


    }
}
