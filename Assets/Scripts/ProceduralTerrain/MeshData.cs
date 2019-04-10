using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public Vector3[] vertices;
	public Vector2[] uvs;

	public int[] triangles;

	private int triangleIndex;

	public MeshData(int meshWidth, int meshHeight)
	{
		this.vertices = new Vector3[meshWidth * meshHeight];
		this.uvs = new Vector2[meshWidth * meshHeight];
		this.triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
		this.triangleIndex = 0;
	}

	public void AddTriangle(int a, int b, int c)
	{
		this.triangles[this.triangleIndex] = a;
		this.triangles[this.triangleIndex + 1] = b;
		this.triangles[this.triangleIndex + 2] = c;
		this.triangleIndex += 3;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = this.vertices;
		mesh.uv = this.uvs;
		mesh.triangles = this.triangles;
		mesh.RecalculateNormals();
		return mesh;
	}

}
