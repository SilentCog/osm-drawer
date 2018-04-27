﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AutoGrid : MonoBehaviour
{
	public int xSize, ySize;
	private Vector3[] vertices;
	private Mesh mesh;

	private void Awake()
	{
		Generate();
	}

	private void Generate()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Auto Grid";

		int vertCount = (xSize + 1) * (ySize + 1);

		vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];

		for (int i = 0, y = 0; y <= ySize; y++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				vertices[i] = new Vector3(x, y);
				normals[i] = -Vector3.forward;
			}
		}

		mesh.vertices = vertices;
		mesh.normals = normals;

		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}

		mesh.triangles = triangles;
	}

	private void OnDrawGizmos()
	{
		if (vertices == null)
		{
			return;
		}

		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++)
		{
			Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
		}
	}
}