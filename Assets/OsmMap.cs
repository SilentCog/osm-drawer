using System;
using System.Collections.Generic;
using UnityEngine;
using OsmParser;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OsmMap : MonoBehaviour {
	private static readonly float PLANET_SIZE = 100000;
	private static readonly float BUILDING_HEIGHT = 0.1f;
	private static readonly Vector3 WORLD_OFFSET = new Vector3(0, 0, -PLANET_SIZE);
	private List<Vector3> vertices;

	void Start () {
		OsmData osmData = Parser.parseOsmDataFile(PathUtils.mockDataPath("basicQuery.xml"), new string[]{"building"});
		Dictionary<string, OsmLayer> layers = osmData.layers;

		Mesh mesh;
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		// Vector3[] normals = new Vector3[osmData.nodeCount];

		float offsetLat = -(osmData.rect.lat.min + osmData.rect.lat.max) / 2;
		float offsetLon = -(osmData.rect.lon.min + osmData.rect.lon.max) / 2;


		foreach (KeyValuePair<string, OsmLayer> layerKVP in layers) {
			OsmLayer layer = layerKVP.Value;
			
			foreach (OsmWay way in layer.ways) {
				bool firstNode = true;
				foreach (OsmNode node in way.nodes) {
					vertices.Add(Parser.nodeToWorldSpace(
						node: node,
						radius: PLANET_SIZE,
						worldOffset: WORLD_OFFSET,
						offsetLat: offsetLat,
						offsetLon: offsetLon,
						altitude: 0
					));
					vertices.Add(Parser.nodeToWorldSpace(
						node: node,
						radius: PLANET_SIZE,
						worldOffset: WORLD_OFFSET,
						offsetLat: offsetLat,
						offsetLon: offsetLon,
						altitude: BUILDING_HEIGHT
					));

					if (!firstNode) {
						triangles.Add(vertices.Count - 1);
						triangles.Add(vertices.Count - 2);
						triangles.Add(vertices.Count - 3);

						triangles.Add(vertices.Count - 2);
						triangles.Add(vertices.Count - 4);
						triangles.Add(vertices.Count - 3);
					}

					firstNode = false;
				}
			}
		}
		
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		
		// mesh.vertices = vertices;
		// mesh.normals = normals;

		// int[] triangles = new int[xSize * ySize * 6];
		// for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		// {
		// 	for (int x = 0; x < xSize; x++, ti += 6, vi++)
		// 	{
		// 		triangles[ti] = vi;
		// 		triangles[ti + 3] = triangles[ti + 2] = vi + 1;
		// 		triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
		// 		triangles[ti + 5] = vi + xSize + 2;
		// 	}
		// }

		// mesh.triangles = triangles;
	}

	// private void OnDrawGizmos()
	// {
	// 	if (vertices == null)
	// 	{
	// 		return;
	// 	}

	// 	Gizmos.color = Color.black;
	// 	for (int i = 0; i < vertices.Count; i++)
	// 	{
	// 		Vector3 node = transform.TransformPoint(vertices[i]);
	// 		Gizmos.DrawSphere(node, 0.03f);
	// 	}
	// }
}
