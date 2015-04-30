using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsPractical1
{
    class Camera
    {
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        // The three components that describe the position and direction of the camera.
        private Vector3 up;
        private Vector3 eye;
        private Vector3 focus;
        
        // The movement speed of the camera.
        private float speed;

        public Camera(Vector3 camEye, Vector3 camFocus, Vector3 camUp,
            float aspectRatio = 4.0f / 3.0f, float speed = 1.0f)
        {
            this.up = camUp;
            this.eye = camEye;
            this.focus = camFocus;

            this.speed = speed;

            this.updateViewMatrix();
            // Set the projection 45 degrees downwards with near and far clipping planes of 1 and 300.
            this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                aspectRatio, 1.0f, 300.0f);
        }

        private void updateViewMatrix()
        {
            this.viewMatrix = Matrix.CreateLookAt(this.eye, this.focus, this.up);
        }

        public Matrix ViewMatrix
        {
            get { return this.viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return this.projectionMatrix; }
        }

        public Vector3 Eye
        {
            get { return this.eye; }
            // Recalculate the view matrix after the eye vector has been set.
            set { this.eye = value; this.updateViewMatrix(); }
        }

        public Vector3 Focus
        {
            get { return this.focus; }
            // Recalculate the view matrix after the focus vector has been set.
            set { this.focus = value; this.updateViewMatrix(); }
        }

        public float Speed
        {
            get { return this.speed; }
        }
    }
}
