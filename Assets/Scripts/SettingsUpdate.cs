using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;   
using static ShapeSettings;

public class SettingsUpdate : MonoBehaviour
{
	public Toggle rp;
	public Toggle hd;

	int curGradient = 1;

	public void RotateCamera(bool value) {
		Controls.rotateCamera = value;
	}
	public void RotatePlayer(bool value) {
		Controls.rotatePlayer = value;
		if (!value) {
			Controls.player.transform.position = Vector3.zero;
			return;
		}
		
		hd.isOn = false;
	}
	public void HighDetailed(bool value) {
		if (!value) {
			PrivateSettings.LODs = new int[7] {
				128,
				256,
				512,
				1024,
				4096,
				8192,
				16384
			};
	
			MainGameControl.generator.Regenerate();
			
			return;
		}
		
		rp.isOn = false;
		Controls.rotatePlayer = false;
		Controls.player.transform.position = Vector3.zero;
		
		PrivateSettings.LODs = new int[7] {
			256,
			512,
			1024,
			4096,
			8129,
			16384,
			16384
		};
	}
	public void Regenerate() {
		MainGameControl.generator.Regenerate();
	}
	public void NewSeed() {
		NoiseFilter.Randomize((int)(Random.value * 1000));
		MainGameControl.generator.Regenerate();
	}
	public void OnFrequencyUpdate(float value) {
		frequency = value;
	}
	public void OnOctavesUpdate(float value) {
		octaves = (int)value;
	}
	public void OnLacunarityUpdate(float value) {
		lacunarity = value;
	}
	public void OnPersistanceUpdate(float value) {
		persistance = value;
	}
	public void OnHeightMultiplierUpdate(float value) {
		heightMultiplier = (int)value;
	}
	public void SwitchGradient() {
		if (curGradient == 0) {
			curGradient = 1;
			Generator.curGradient = Generator.terrainGradient1;
		}
		else {
			curGradient = 0;
			Generator.curGradient = Generator.terrainGradient2;
		}
		MainGameControl.generator.Regenerate();
	}
	public void Randomize() {
		frequency = Random.Range(0.00001f, 0.002f);
		octaves = Random.Range(0, 15);  
		lacunarity = Random.Range(0.4f, 3f);
		persistance = Random.Range(0.2f, 0.8f);
		if (Random.Range(0f, 1f) > 0.5f) SwitchGradient();
	}
	
	//[SerializeField] UnityEngine.UI.Slider sizeSlider;
	//
	//void Start()
	//{
	//    sizeSlider.onValueChanged.AddListener((value) => {
	//        size = (int)value;
	//        MainGameControl.generator.UpdatePlanet();
	//    });
	//}
	//
	//public void GenerateRandom()
	//{
	//    //seed = rand.Next(1, int.MaxValue - 1);
	//    //MainGameControl.noise.Randomize(seed);
	//    //size = rand.Next(3, 5);
	//    MainGameControl.generator.UpdatePlanet();
	//}

	//System.Random rand = new System.Random();
	//
	//
	//
	//
	//public void OnSeedUpdate(string value)
	//{
	//    string newSeed = "";
	//
	//    foreach(char letter in value) {
	//        newSeed += letter;
	//    }
	//    if (newSeed == "") newSeed = "1";
	//    while (newSeed.Length > 9) newSeed = newSeed.Substring(1);
	//
	//    seed = int.Parse(newSeed);
	//    MainGameControl.generator.UpdatePlanet();
	//}
	//
	//
	//public void OnSizeUpdate(float value)
	//{
	//    value *= 5;
	//    value += 1;
	//    size = (int)value;
	//    MainGameControl.generator.UpdatePlanet();
	//}
}
