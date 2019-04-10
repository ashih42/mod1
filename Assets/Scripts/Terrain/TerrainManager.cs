using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

public class TerrainManager : MonoBehaviour
{
	private const float RADIUS_MODIFIER = 200f;

	private const int MAX_POSITION = 20000;
	private const int MAX_TALLNESS = 10000;

	private const int TERRAIN_WIDTH = 256;				// area = width * width
	private const int TERRAIN_HEIGHT = 50;				// tallness = height

	private Terrain terrain;
	private NavMeshSurface navMeshSurface;

	private void Awake()
	{
		this.terrain = this.GetComponent<Terrain>();
		this.navMeshSurface = this.GetComponent<NavMeshSurface>();
		this.navMeshSurface.BuildNavMesh();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "WaterParticle")
			collision.rigidbody.drag = 0f;
	}

	public void GenerateTerrainFromFile(string filename)
	{
		List<Vector3> positivePeaks = new List<Vector3>();
		List<Vector3> negativePeaks = new List<Vector3>();

		this.ParsePeaksFromFile(filename, positivePeaks, negativePeaks);
		this.GenerateTerrainFromPeaks(positivePeaks, negativePeaks);
		this.navMeshSurface.BuildNavMesh();
	}

	/* Assumptions about .mod1 file: Given (x, y, z), ...
	 * 0 <= x, y <= MAX_POSITION
	 * 0 <= z <= MAX_TALLNESS
	 */
	private void ParsePeaksFromFile(string filename, List<Vector3> positivePeaks, List<Vector3> negativePeaks)
	{
		StreamReader streamReader = new StreamReader("Assets/Maps/" + filename);
		string mapText = streamReader.ReadToEnd();
		streamReader.Close();

		string[] tokens = mapText.Split();
		tokens = tokens.Where(token => !string.IsNullOrEmpty(token)).ToArray();

		foreach (string token in tokens)
			this.ValidateAndAddPeak(token, positivePeaks, negativePeaks);
	}

	private void ValidateAndAddPeak(string token, List<Vector3> positivePeaks, List<Vector3> negativePeaks)
	{
		const string TOKEN_PATTERN = @"\(\d+,\d+,\d+\)";
		const string NUMBER_PATTERN = @"\d+";
		string FORMAT_MESSAGE = string.Format("Format: (x, y, z), where \n x in [0, {0}]\n y in [0, {0}]\n z in [0, {1}]", MAX_POSITION, MAX_TALLNESS);

		Match tokenMatch = Regex.Match(token, TOKEN_PATTERN);

		if (tokenMatch.Success)
		{
			MatchCollection numberMatches = Regex.Matches(token, NUMBER_PATTERN, RegexOptions.None);
			int x = Int32.Parse(numberMatches[0].Groups[0].Value);
			int y = Int32.Parse(numberMatches[1].Groups[0].Value);
			int z = Int32.Parse(numberMatches[2].Groups[0].Value);      // tallness

			if (!(0 <= x && x <= MAX_POSITION && 0 <= y && y <= MAX_POSITION && 0 <= z && z <= MAX_TALLNESS))
				throw new ParserException("Invalid term: " + "<color=red>" + token + "</color>" + "\n\n" + FORMAT_MESSAGE);

			if (z == 0)
				negativePeaks.Add(new Vector3(x, y, z));
			else
				positivePeaks.Add(new Vector3(x, y, z));
		}
		else
			throw new ParserException("Invalid term: " + "<color=red>" + token + "</color>" + "\n\n" + FORMAT_MESSAGE);
	}

	private void GenerateTerrainFromPeaks(List<Vector3> positivePeaks, List<Vector3> negativePeaks)
	{
		this.terrain.terrainData.heightmapResolution = TERRAIN_WIDTH + 1;
		this.terrain.terrainData.size = new Vector3(TERRAIN_WIDTH, TERRAIN_HEIGHT, TERRAIN_WIDTH);

		this.terrain.terrainData.SetHeights(0, 0, GenerateHeightsFromPeaks(positivePeaks, negativePeaks));
	}

	private float[,] GenerateHeightsFromPeaks(List<Vector3> positivePeaks, List<Vector3> negativePeaks)
	{
		float[,] heights = new float[TERRAIN_WIDTH, TERRAIN_WIDTH];

		foreach (Vector3 peak in positivePeaks)
			this.AddPositivePeak(peak, heights);

		foreach (Vector3 peak in negativePeaks)
			this.AddNegativePeak(peak, heights);

		// Rescale all heights to [0, 1]
		float max_tallness = heights.Cast<float>().Max();

		if (max_tallness != 0f)
		{
			for (int i = 0; i < TERRAIN_WIDTH; i++)
				for (int j = 0; j < TERRAIN_WIDTH; j++)
					heights[i, j] /= max_tallness;
		}
		return heights;
	}

	private void AddPositivePeak(Vector3 peak, float[,] heights)
	{
		int peakX = (int)(peak.x / MAX_POSITION * (TERRAIN_WIDTH - 1));
		int peakY = (int)(peak.y / MAX_POSITION * (TERRAIN_WIDTH - 1));
		float peakZ = (float)(peak.z / MAX_TALLNESS);      // tallness
		float radius = peakZ * RADIUS_MODIFIER;

		radius = Math.Min(radius, peakX);
		radius = Math.Min(radius, TERRAIN_WIDTH - 1 - peakX);
		radius = Math.Min(radius, peakY);
		radius = Math.Min(radius, TERRAIN_WIDTH - 1 - peakY);

		for (int i = 0; i < TERRAIN_WIDTH; i++)
		{
			for (int j = 0; j < TERRAIN_WIDTH; j++)
			{
				float distanceFromPeak = Mathf.Sqrt(Mathf.Pow(i - peakX, 2) + Mathf.Pow(j - peakY, 2));

				if (distanceFromPeak > radius)
					continue;

				float heightStart = peakZ;
				float heightEnd = 0f;
				float t = distanceFromPeak / radius;

				heights[i, j] += GetBezierPoint(heightStart, heightEnd, t);
			}
		}
	}

	private void AddNegativePeak(Vector3 peak, float[,] heights)
	{
		int peakX = (int)(peak.x / MAX_POSITION * (TERRAIN_WIDTH - 1));
		int peakY = (int)(peak.y / MAX_POSITION * (TERRAIN_WIDTH - 1));
		float peakZ = heights[peakX, peakY] / 2f;      // tallness
		float radius = peakZ * RADIUS_MODIFIER;

		radius = Math.Min(radius, peakX);
		radius = Math.Min(radius, TERRAIN_WIDTH - 1 - peakX);
		radius = Math.Min(radius, peakY);
		radius = Math.Min(radius, TERRAIN_WIDTH - 1 - peakY);

		for (int i = 0; i < TERRAIN_WIDTH; i++)
		{
			for (int j = 0; j < TERRAIN_WIDTH; j++)
			{
				float distanceFromPeak = Mathf.Sqrt(Mathf.Pow(i - peakX, 2) + Mathf.Pow(j - peakY, 2));

				if (distanceFromPeak > radius)
					continue;

				float heightStart = peakZ;
				float heightEnd = 0f;
				float t = distanceFromPeak / radius;

				heights[i, j] -= GetBezierPoint(heightStart, heightEnd, t); ;
			}
		}
	}

	private float GetBezierPoint(float start, float end, float t)
	{
		float p0 = start;
		float p1 = Mathf.Lerp(start, end, 0.05f);
		float p2 = Mathf.Lerp(start, end, 0.95f);
		float p3 = end;

		// B(t) = (1-t)^3 * P0 + 3(1-t)^2 * t * P1 + 3(1-t) * t^2 * P2 + t^3 * P3, 0 < t < 1
		return Mathf.Pow(1 - t, 3) * p0 +
			3 * Mathf.Pow(1 - t, 2) * t * p1 +
			3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
			Mathf.Pow(t, 3) * p3;
	}
	
}
