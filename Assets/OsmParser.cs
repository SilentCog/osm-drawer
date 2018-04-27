using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace OsmParser {

	public class OsmData {
		public int nodeCount = 0;
		public LatLonRect rect = new LatLonRect();
		public Dictionary<string, OsmLayer> layers = new Dictionary<string, OsmLayer>();
	}

	public class OsmLayer {
		public List<OsmWay> ways = new List<OsmWay>();
	}

	public class OsmWay
	{
		public List<OsmNode> nodes = new List<OsmNode>();
	}

	public struct OsmNode
	{
		public float lat;
		public float lon;

		public override string ToString() {
			return lat + "," + lon;
		}
	}

	public class LatLonRect {
		public Range lat = new Range();
		public Range lon = new Range();

		public void update(float newLat, float newLon) {
			lat.update(newLat);
			lon.update(newLon);
		}

		public override string ToString() {
			return "Lat: " + lat + "; Lon: " + lon;
		}
	}

	public class Range {
		public float min = float.PositiveInfinity;
		public float max = float.NegativeInfinity;

		public void update(float newVal) {
			if (newVal < min) min = newVal;
			else if (newVal > max) max = newVal; // expects more than 1 data point
		}

		public override string ToString() {
			return min + "-" + max;
		}
	}

	public static class Parser
	{
		public static Vector3 nodeToWorldSpace(OsmNode node, float radius, Vector3 worldOffset, float offsetLat, float offsetLon, float altitude) {
			return CoordUtils.latLonToWorldSpace(node.lat + offsetLat, node.lon + offsetLon, radius, worldOffset, altitude);
		}

		public static OsmData parseOsmDataFile(string path, string[] filterLayers)
		{
			return parseOsmData(XDocument.Load(path), filterLayers);
		}

		public static OsmData parseOsmData(XDocument dataDoc, string[] filterLayers)
		{
			OsmData data = new OsmData();
			Dictionary<string, OsmNode> nodes = new Dictionary<string, OsmNode>();

			Dictionary<string, int> tags = new Dictionary<string, int>();

			foreach(XElement relationEl in dataDoc.Root.Descendants("relation")) {
				foreach(XElement tagEl in relationEl.Descendants("tag")) {
					string k = tagEl.Attribute("k").Value;
					string v = tagEl.Attribute("v").Value;
					string id = k + " - " + v;
					if (!tags.ContainsKey(id)) tags.Add(id, 0);

					tags[id]++;
				}
			}

			string str = "";

			foreach (KeyValuePair<string, int> kvp in tags) {
				str += kvp.Key + ": " + kvp.Value + "\n";
			}

			Debug.Log(str);

			foreach(XElement nodeEl in dataDoc.Root.Descendants("node"))
			{
				data.nodeCount++;
				nodes.Add(nodeEl.Attribute("id").Value, parseOsmNode(nodeEl, data.rect));
			}

			foreach (XElement wayEl in dataDoc.Root.Descendants("way"))
			{
				string id = wayEl.Attribute("id").Value;
				if (!data.layers.ContainsKey(id)) data.layers.Add(id, new OsmLayer());


				data.layers[id].ways.Add(parseOsmWay(dataDoc, wayEl, nodes));
			}

			return data;
		}

		private static OsmWay parseOsmWay(XDocument datadoc, XElement wayEl, Dictionary<string, OsmNode> nodes)
		{
			OsmWay way = new OsmWay();

			foreach (XElement ndEl in wayEl.Descendants("nd"))
			{
				string nodeRef = ndEl.Attribute("ref").Value;
				if (nodes.ContainsKey(nodeRef)) way.nodes.Add(nodes[nodeRef]);
			}

			return way;
		}

		private static OsmNode parseOsmNode(XElement nodeEl, LatLonRect rect)
		{
			OsmNode node = new OsmNode();

			node.lat = float.Parse(nodeEl.Attribute("lat").Value);
			node.lon = float.Parse(nodeEl.Attribute("lon").Value);

			rect.update(node.lat, node.lon);

			return node;
		}

		// private static bool wayInLayer(XElement wayEl, string layer) {
		// 	foreach (XElement tag in wayEl.Descendants("tag")) {
		// 		if (wayEl.Attribute("k").Value == layer) return true; 
		// 	}

		// 	return false;
		// }

		private static bool isClockwise(OsmWay way) {

		}

		// private static float getEdgeSlope
	}
}
