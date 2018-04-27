using System;
using UnityEngine;

public static class CoordUtils {
	public static Vector3 latLonToWorldSpace(float lat, float lon, float radius, Vector3 offset, float altitude) {
		float totalR = radius + altitude;
		return Quaternion.AngleAxis(lon, -Vector3.up) * Quaternion.AngleAxis(lat, -Vector3.right) * new Vector3(0, 0, totalR) + offset;
	}
}