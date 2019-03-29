using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TruffleSnuffle
{
    public static class BoundingRenderer
    {
        static GraphicsDevice gfx;
        static VertexBuffer sphereVertexBuffer;
        static VertexBuffer cubeVertexBuffer;
        static BasicEffect effect;
        static int tessellation = 16;

        public static void InitializeGraphics(GraphicsDevice graphicsDevice)
        {
            gfx = graphicsDevice;
            effect = new BasicEffect(gfx);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;
            VertexPositionColor[] sphereVerts = new VertexPositionColor[tessellation *
            3 + 2];
            int index = 0;
            float step = MathHelper.TwoPi / (float)tessellation;
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //create circle on the XY plane first
                sphereVerts[index++] = new VertexPositionColor(new
                Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f), Color.White);
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //next the XZ  circle
                sphereVerts[index++] = new VertexPositionColor(new
                Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)), Color.White);
            sphereVerts[index++] = sphereVerts[index - tessellation - 1]; // close the circle
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //finally the YZ circle
                sphereVerts[index++] = new VertexPositionColor(new Vector3(0f,
                (float)Math.Cos(a), (float)Math.Sin(a)), Color.White);

            sphereVerts[index++] = sphereVerts[index - tessellation - 1]; // close the circle
            sphereVertexBuffer = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionColor), sphereVerts.Length, BufferUsage.None);
            sphereVertexBuffer.SetData(sphereVerts);
            int[] boxpos = new int[] { 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1,
0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0,
0, 1, 0 };
            VertexPositionColor[] boxVerts = new VertexPositionColor[17];
            for (index = 0; index < boxpos.Length; index += 3)
                boxVerts[index / 3] = new VertexPositionColor(new
                Vector3(boxpos[index], boxpos[index + 1], boxpos[index + 2]), Color.White);
            cubeVertexBuffer = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionColor), boxVerts.Length, BufferUsage.None);
            cubeVertexBuffer.SetData(boxVerts);
        }

        public static void RenderBox(BoundingBox box, Matrix view, Matrix projection, Color wireColour)
        {
            gfx.SetVertexBuffer(cubeVertexBuffer);
            effect.World =
            Matrix.CreateScale(box.Max - box.Min) *
            Matrix.CreateTranslation(box.Min);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = wireColour.ToVector3();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gfx.DrawPrimitives(PrimitiveType.LineStrip, 0, 16);
            }
        }
        public static void RenderSphere(BoundingSphere sphere, Matrix view, Matrix projection, Color wireColour)
        {
            gfx.SetVertexBuffer(sphereVertexBuffer);
            effect.World = Matrix.CreateScale(sphere.Radius) *
            Matrix.CreateTranslation(sphere.Center);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = wireColour.ToVector3();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gfx.DrawPrimitives(PrimitiveType.LineStrip, 0, tessellation);
                // draw first circle
                gfx.DrawPrimitives(PrimitiveType.LineStrip, tessellation,
                tessellation); // draw second circle
                gfx.DrawPrimitives(PrimitiveType.LineStrip, (tessellation) * 2 + 1,
                tessellation); // draw third circle
            }
        }
    }
}
