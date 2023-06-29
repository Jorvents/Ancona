using System.Numerics;
using Raylib_cs;

namespace Ancona;

public class Blender
{
    public Blender(int triangleCount)
        {
            TheMesh = new Mesh();
            unsafe
            {
                int verts = triangleCount * 3;

                TheMesh.vertexCount = verts;
                TheMesh.triangleCount = triangleCount;

                TheMesh.vboId = null;
                TheMesh.vaoId = 0;

                TheMesh.colors = null;
                TheMesh.animNormals = null;
                TheMesh.boneIds = null;
                TheMesh.indices = null;
                TheMesh.boneWeights = null;

                TheMesh.vertices = (float*)Raylib.MemAlloc(sizeof(float) * 3 * verts);
                TheMesh.normals = (float*)Raylib.MemAlloc(sizeof(float) * 3 * verts);
                TheMesh.texcoords = (float*)Raylib.MemAlloc(sizeof(float) * 2 * verts);
            }
            CurrentVert = 0;
        }

        public void AddTriangle(Vector3[] verts, Vector3[] normals, Vector2[] textureCoords)
        {
            WriteVert(verts[0], CurrentVert);
            WriteNorm(normals[0], CurrentVert);
            WriteUV(textureCoords[0], CurrentVert);
            CurrentVert++;

            WriteVert(verts[1], CurrentVert);
            WriteNorm(normals[1], CurrentVert);
            WriteUV(textureCoords[1], CurrentVert);
            CurrentVert++;

            WriteVert(verts[2], CurrentVert);
            WriteNorm(normals[2], CurrentVert);
            WriteUV(textureCoords[2], CurrentVert);
            CurrentVert++;
        }

        public Mesh GetMesh()
        {
            Raylib.UploadMesh(ref TheMesh, false);
            return TheMesh;
        }

        protected Mesh TheMesh;

        protected int CurrentVert = 0;

        protected void WriteVert(Vector3 value, int vertCount)
        {
            unsafe
            {
                TheMesh.vertices[vertCount * 3] = value.X;
                TheMesh.vertices[vertCount * 3 + 1] = value.Y;
                TheMesh.vertices[vertCount * 3 + 2] = value.Z;
            }
        }

        protected void WriteNorm(Vector3 value, int vertCount)
        {
            unsafe
            {
                TheMesh.normals[vertCount * 3] = value.X;
                TheMesh.normals[vertCount * 3 + 1] = value.Y;
                TheMesh.normals[vertCount * 3 + 2] = value.Z;
            }
        }

        protected void WriteUV(Vector2 value, int vertCount)
        {
            unsafe
            {
                TheMesh.texcoords[vertCount * 2] = value.X;
                TheMesh.texcoords[vertCount * 2 + 1] = value.Y;
            }
        }
}