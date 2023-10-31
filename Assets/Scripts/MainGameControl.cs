using UnityEditor;
using UnityEngine;

public class MainGameControl : MonoBehaviour
{
	public GameObject player;
	public Material material;
	public Camera playerCamera;
	public Gradient terrainGradient1;
	public Gradient terrainGradient2;

	public static SettingsUpdate settingsUpdate;
	public static Perlin3D noise = new Perlin3D();
	public static Controls controls;
	public static Generator generator;
 
	void Start()
	{
		controls = gameObject.GetComponent<Controls>();
		Controls.player = player;
		Controls.playerCamera = playerCamera;

		settingsUpdate = gameObject.GetComponent<SettingsUpdate>();
		generator = new Generator(material, player.transform, terrainGradient1, terrainGradient2);	
		
		StartCoroutine(generator.UpdatePlanet());
	}
}
