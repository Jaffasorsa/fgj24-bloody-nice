using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class BackgroundController : MonoBehaviour
{
	[SerializeField]
	private GameObject skyPrefab;

	[SerializeField]
	private GameObject nightSkyPrefab;

	[SerializeField]
	private Transform skyContainer;

	[SerializeField]
	private Transform cloudEmitter;

	[SerializeField]
	private float skyTileSize = 20.48f;

	[SerializeField]
	private float cloudAltitude = 20f;

	[SerializeField]
	private float spaceAltitude = 100f;

	[SerializeField, Min(1f)]
	private float spaceFade = 10f;

	[SerializeField]
	private Transform target;

	private ParticleSystem cloudEmitterPS;

	private List<(GameObject, SpriteRenderer)> skyPool = new List<(GameObject, SpriteRenderer)>();
	private List<(int, GameObject, SpriteRenderer)> activeSkyTiles = new List<(int, GameObject, SpriteRenderer)>(); // For linear search
	private Dictionary<int, GameObject> skyTileDict = new Dictionary<int, GameObject>(); // For random search

	private List<(GameObject, SpriteRenderer)> nightSkyPool = new List<(GameObject, SpriteRenderer)>();
	private List<(int, GameObject, SpriteRenderer)> activeNightSkyTiles = new List<(int, GameObject, SpriteRenderer)>(); // For linear search
	private Dictionary<int, GameObject> nightSkyTileDict = new Dictionary<int, GameObject>(); // For random search

	private int CurrentTile => (int)Mathf.Round(target.position.x / skyTileSize);
	private bool CloudsEnabled => target.position.y > cloudAltitude && target.position.y < spaceAltitude;
	private float SkyTransparency => 1f - Mathf.Clamp01((target.position.y - spaceAltitude) / spaceFade);
	private int lastTile = -10;

	private void Awake()
	{
		cloudEmitterPS = cloudEmitter.GetComponent<ParticleSystem>();
	}

	private void LateUpdate()
	{
		UpdateBackground();
	}

	private void UpdateBackground()
	{
		if (CloudsEnabled && !cloudEmitterPS.isEmitting) cloudEmitterPS.Play();
		else if (!CloudsEnabled && cloudEmitterPS.isEmitting) cloudEmitterPS.Stop();

		cloudEmitter.position = target.position;

		skyContainer.transform.position = Vector3.up * target.position.y;

		if (CurrentTile != lastTile)
		{
			UpdateSky(ref skyPool, ref activeSkyTiles, ref skyTileDict, skyPrefab);
			UpdateSky(ref nightSkyPool, ref activeNightSkyTiles, ref nightSkyTileDict, nightSkyPrefab);
		}

		foreach ((int, GameObject, SpriteRenderer) tile in activeSkyTiles)
		{
			tile.Item3.color = new Color(1f, 1f, 1f, SkyTransparency);
		}

		void UpdateSky(ref List<(GameObject, SpriteRenderer)> pool, ref List<(int, GameObject, SpriteRenderer)> tileList, ref Dictionary<int, GameObject> tileDict, GameObject prefab)
		{
			lastTile = CurrentTile;

			List<(int, GameObject, SpriteRenderer)> tilesToBeRemoved = new List<(int, GameObject, SpriteRenderer)>();

			// Free up expired tiles
			foreach ((int, GameObject, SpriteRenderer) tile in tileList)
			{
				if (tile.Item1 < CurrentTile - 1 || tile.Item1 > CurrentTile + 1)
				{
					pool.Add((tile.Item2, tile.Item3));
					tilesToBeRemoved.Add(tile);
				}
			}

			while (tilesToBeRemoved.Count > 0)
			{
				tileList.Remove(tilesToBeRemoved[0]);
				tileDict.Remove(tilesToBeRemoved[0].Item1);
				tilesToBeRemoved.RemoveAt(0);
			}

			// Update sky
			for (int i = CurrentTile - 1; i <= CurrentTile + 1; i++)
			{
				if (!tileDict.ContainsKey(i))
				{
					(int, GameObject, SpriteRenderer) tile;

					if (pool.Count > 0)
					{
						tile = (i, pool[0].Item1, pool[0].Item2);
						pool.RemoveAt(0);
					}
					else
					{
						GameObject tileObject = Instantiate(prefab);
						tileObject.transform.parent = skyContainer;
						tile = (i, tileObject, tileObject.GetComponent<SpriteRenderer>());
					}

					tile.Item2.transform.localPosition = Vector3.right * i * skyTileSize;

					tileList.Add(tile);
					tileDict.Add(tile.Item1, tile.Item2);
				}
			}
		}
	}
}
