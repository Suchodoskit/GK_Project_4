﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SoftEngine;
using SharpDX;
using System.IO;
using System.Reflection;

namespace GK_Project_4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private Device device;
        Mesh[][]meshes;
        Camera Fixedcamera = new Camera();
        Camera MovingObjectMovingCamera = new Camera();
        Camera MovingObjectFixedCamera = new Camera();
        Camera ActCamera;
        Timer Rendring;
        List<Light> lights;
        DirectionalLight dl = new DirectionalLight(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        PointLight pl = new PointLight(new Vector3(0, 3, 0), new Vector3(1, 1, 1));
        double t = 0.0;
        float eps = 0.1f;


        void Rendering(object sender, object e)
        {
            device.Clear(255, 0, 0, 0);
            t = t + 0.05;
            dl.Direction = new Vector3((float)Math.Sin(t),-0.4f, (float)Math.Cos(t));

            meshes[4][0].Position = new Vector3(meshes[4][0].Position.X, meshes[4][0].Position.Y, meshes[4][0].Position.Z + eps);
            meshes[4][0].Rotation = new Vector3(meshes[4][0].Rotation.X + eps, meshes[4][0].Rotation.Y, meshes[4][0].Rotation.Z);
            if (meshes[4][0].Position.Z >= 4) eps = -eps;
            if (meshes[4][0].Position.Z <= -4) eps = -eps;

            meshes[3][0].Rotation = new Vector3(meshes[3][0].Rotation.X+0.1f, meshes[3][0].Rotation.Y, meshes[3][0].Rotation.Z);
            MovingObjectMovingCamera.Target = meshes[4][0].Position;
            MovingObjectMovingCamera.Position = new Vector3(meshes[4][0].Position.X - 15.0f, meshes[4][0].Position.Y + 15.0f, meshes[4][0].Position.Z);
            MovingObjectFixedCamera.Target = meshes[4][0].Position;

            device.MyRender(ActCamera, meshes, lights);
            device.Present();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Width = 640; pictureBox1.Height = 480;
            DirectBitmap bmp = new DirectBitmap(640,480);
            device = new Device(bmp,pictureBox1,new Vector3(0,0,0),1);

            meshes = new Mesh[5][];

            meshes[0] = new Mesh[1];
            meshes[0][0] = ProduceTable(5);

            meshes[1] = await device.LoadJSONFileAsync(@"Meshes/Ball.babylon", false, System.Drawing.Color.Red, 1.0f);
            meshes[1][0].Position = new Vector3(0, 1.0f, 0);


            var m = MakeBillardTriangle(meshes[1][0], new Vector3(0.0f, 1.5f, 0.0f), 1.0f);
            for (int i = 1; i <= m.Length; i++)
            {
                meshes[i] = m[i - 1];
            }

            Fixedcamera.Position = new Vector3(0, 20f, -20f);
            Fixedcamera.Target = new Vector3(0,0,0);
            Fixedcamera.Up = new Vector3(0, 1, 0);
            MovingObjectFixedCamera.Up = new Vector3(0, 1, 0);
            MovingObjectFixedCamera.Position = new Vector3(20, 10, 0);
            MovingObjectMovingCamera.Up = new Vector3(0, 1, 0);
            ActCamera = Fixedcamera;

            lights = new List<Light>();
            lights.Add(pl);

            Rendring = new Timer();
            Rendring.Tick += Rendering;
            Rendring.Interval = 50;
            Rendring.Start();

        }

        private Mesh[][] MakeBillardTriangle(Mesh ball, Vector3 Position, float radius)
        {
            Mesh[][] Balls = new Mesh[4][];
            Position.Z = -(float)2 * (float)Math.Sqrt(3) * radius;
            Position.X = radius;
            for (int i = 0; i < Balls.Length; i++)
            {
                Balls[i] = new Mesh[1];
                Balls[i][0] = ball.Clone();
                Balls[i][0].Position = Position;
            }
            float pos = -5.0f * radius;
            pos = -2.0f * radius;
            for (int i = 0; i < 2; i++)
            {
                Balls[i][0].Position = new Vector3(Balls[i][0].Position.X + pos, Balls[i][0].Position.Y, Balls[i][0].Position.Z + (float)3 * (float)Math.Sqrt(3) * radius);
                pos += 2 * radius;
            }
            pos = -1.0f * radius;
            Balls[2][0].Position = new Vector3(Balls[2][0].Position.X + pos, Balls[2][0].Position.Y, Balls[2][0].Position.Z + (float)2 * (float)Math.Sqrt(3) * radius);

            Balls[3][0].Position = new Vector3(Balls[3][0].Position.X + pos, Balls[3][0].Position.Y, Balls[2][0].Position.Z + (float)-2 * (float)Math.Sqrt(3) * radius);
            for (int i=0;i<Balls[3][0].Faces.Length;i++)
            {
                Balls[3][0].Faces[i].Color = System.Drawing.Color.White;
            }

            return Balls;
        }

        private Mesh ProduceCube()
        {
            var mesh = new SoftEngine.Mesh("Cube", 8, 12);
            mesh.Vertices[0] = new Vertex(); mesh.Vertices[0].Coordinates = new Vector4(-1, 1, 1, 1);
            mesh.Vertices[1] = new Vertex(); mesh.Vertices[1].Coordinates = new Vector4(1, 1, 1, 1);
            mesh.Vertices[2] = new Vertex(); mesh.Vertices[2].Coordinates = new Vector4(-1, -1, 1, 1);
            mesh.Vertices[3] = new Vertex(); mesh.Vertices[3].Coordinates = new Vector4(1, -1, 1, 1);
            mesh.Vertices[4] = new Vertex(); mesh.Vertices[4].Coordinates = new Vector4(-1, 1, -1, 1);
            mesh.Vertices[5] = new Vertex(); mesh.Vertices[5].Coordinates = new Vector4(1, 1, -1, 1);
            mesh.Vertices[6] = new Vertex(); mesh.Vertices[6].Coordinates = new Vector4(1, -1, -1, 1);
            mesh.Vertices[7] = new Vertex(); mesh.Vertices[7].Coordinates = new Vector4(-1, -1, -1, 1);

            mesh.Faces[0] = new Face { A = 0, B = 1, C = 2, Color = System.Drawing.Color.Green };
            mesh.Faces[1] = new Face { A = 1, B = 2, C = 3, Color = System.Drawing.Color.Gold };
            mesh.Faces[2] = new Face { A = 1, B = 3, C = 6, Color = System.Drawing.Color.Gray };
            mesh.Faces[3] = new Face { A = 1, B = 5, C = 6, Color = System.Drawing.Color.HotPink };
            mesh.Faces[4] = new Face { A = 0, B = 1, C = 4, Color = System.Drawing.Color.Red };
            mesh.Faces[5] = new Face { A = 1, B = 4, C = 5, Color = System.Drawing.Color.White };

            mesh.Faces[6] = new Face { A = 2, B = 3, C = 7, Color = System.Drawing.Color.Violet };
            mesh.Faces[7] = new Face { A = 3, B = 6, C = 7, Color = System.Drawing.Color.Yellow };
            mesh.Faces[8] = new Face { A = 0, B = 2, C = 7, Color = System.Drawing.Color.Orange};
            mesh.Faces[9] = new Face { A = 0, B = 4, C = 7, Color = System.Drawing.Color.Olive };
            mesh.Faces[10] = new Face { A = 4, B = 5, C = 6, Color = System.Drawing.Color.LightSalmon };
            mesh.Faces[11] = new Face { A = 4, B = 6, C = 7, Color = System.Drawing.Color.LightCyan };
            return mesh;
        }

        private Mesh ProduceTable(float scale=1)
        {
            var mesh = new SoftEngine.Mesh("Table", 9 * 5, 16 * 4);
            float actz = -1*scale;
            float actx = -0.5f*scale;
            for (int i = 0; i < 45; i++)
            {
                float x = actx;
                float y = 0;
                float z = actz;
                mesh.Vertices[i] = new Vertex();
                mesh.Vertices[i].Coordinates = new Vector4(actx, 0, actz, 1);
                mesh.Vertices[i].Normal = new Vector4(0, 1, 0, 0);
                if (actz < 1*scale) { actz += 0.25f*scale; }
                else { actz = -1*scale;actx += 0.25f*scale; }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mesh.Faces[i*16+j*2] = new Face { A = i*9+j, B = i*9+j + 1, C = i*9+j + 1 + 9, Color = System.Drawing.Color.Green };
                    mesh.Faces[i*16+j*2+1] = new Face { A = i*9+j, B = i * 9 + j + 9, C = i*9+j + 1 + 9, Color = System.Drawing.Color.Green };
                }
            }
            return mesh;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            device.shading = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            device.shading = 1;
        }

        private void Fixed_CheckedChanged(object sender, EventArgs e)
        {
            ActCamera = Fixedcamera;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            ActCamera = MovingObjectMovingCamera;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            ActCamera = MovingObjectFixedCamera;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            device.BackgroundLight = new Vector3(0, 0, 0);
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            device.BackgroundLight = new Vector3(0.5f, 0.5f, 0.5f);
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            device.shading = 2;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            device.m = (sender as TrackBar).Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                if (!lights.Exists(l => l == dl)) { lights.Add(dl); }
            }
            else if (lights.Exists(l => l == dl)) { lights.Remove(dl); }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                if (!lights.Exists(l => l == pl)) { lights.Add(pl); }
            }
            else if (lights.Exists(l => l == pl)) { lights.Remove(pl); }
        }
    }
}
