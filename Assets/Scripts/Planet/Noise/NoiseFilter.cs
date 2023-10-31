using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ShapeSettings;

// Proceed noise output
public static class NoiseFilter
{
	public static Dictionary<Vector3, float> cache = new Dictionary<Vector3, float>(); 
	
	static int offset = 0;

	public static float Evaluate(Vector3 point)
	{
		// Return the result if it's saved
		if (cache.ContainsKey(point)) return cache[point];
		
		float value = GetValue(point, frequency, octaves, lacunarity, persistance);
		value *= heightMultiplier;
		if (value < oceanLevel) value = oceanLevel;

		// Save and return the result
		cache.Add(point, value);
		return value;
	}

	public static void Randomize(int value) {
		offset = value;
	}

	static float GetValue(Vector3 point, float frequency, int octaves, float lacunarity, float persistence)
	{
		float currentFrequency = frequency;
		float sum = Perlin3D.Evaluate(new Vector3(
			point.x * currentFrequency + offset,
			point.y * currentFrequency + offset,
			point.z * currentFrequency + offset));
		float amplitude = 1f;
		float range = 1f;

		for (int o = 0; o < octaves; o++) {
			currentFrequency *= lacunarity;
			amplitude *= persistence;
			range += amplitude;
			sum += Perlin3D.Evaluate(new Vector3(
				point.x * currentFrequency + offset,
				point.y * currentFrequency + offset,
				point.z * currentFrequency + offset)) * amplitude;
		}

		return sum / range;
	}
}