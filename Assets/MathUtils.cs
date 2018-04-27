using System;
using UnityEngine;

public static class MathUtils {
	public static float degToRad(float angle)
	{
		return Mathf.PI * angle / 180f;
	}
}