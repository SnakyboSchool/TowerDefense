﻿using UnityEngine;
using System.Collections;

public class GameGUI:MonoBehaviour {
	private Building building;
	private PlayerData playerData;
	private SpawnBuilding spawnBuilding;
	
	private bool pressed;
	
	private Vector3 guiPosition;
	private Transform target;
	
	public Texture stone;
	public Texture wood;
	public Texture gold;
	
	private string selectedBuilding;
	private string resourceName;
	private string buildingName;
	
	public GUIStyle sellStyle;
	public GUIStyle buyStyle;
	
	void Start(){
		playerData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerData>();

		Deselect();
	}
	
	void Update() {
	//	Debug.Log (target);

		if(target != null)
			guiPosition = Camera.main.WorldToScreenPoint(target.position);

		if(Input.GetMouseButton(0)) {
			pressed = true;
		} else {
			pressed = false;
		}

		pressed = false;
		
		if(Input.GetMouseButton(1)) {
			Deselect();
		}
		
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, 100)) {
				if(selectedBuilding == "") {
					switch(hit.transform.gameObject.tag) {
					case "EmptyPlot":
						target = hit.transform;
						spawnBuilding = target.gameObject.GetComponent<SpawnBuilding>();
						break;
					case "LumberMill":
						target = hit.transform;
						building = target.gameObject.GetComponent<LumberMill>();
						
						resourceName = "Wood";
						buildingName = "Lumber Mill";
						break;
					case "Mine":
						target = hit.transform;
						building = target.gameObject.GetComponent<Mine>();
						
						resourceName = "Stone";
						buildingName = "Mine";
						break;
					}

					if(target != null)
						selectBuilding(target);
				}
			}
		}
	}
	
	void selectBuilding(Transform target) {
		pressed = true;
		
		guiPosition = Camera.main.WorldToScreenPoint(target.position);
		selectedBuilding = target.gameObject.tag;
		
		StartCoroutine("SlightDelay");
	}
	
	IEnumerator SlightDelay() {
		yield return new WaitForSeconds(0.02f);
	}
	
	void OnGUI() {
		if(selectedBuilding == "EmptyPlot") {
			if(GUI.Button(new Rect(guiPosition.x - 25, Screen.height + -guiPosition.y - 100, 50, 50), Tower.name)) {
				CreateBuilding(Building.BuildingType.Tower);
			} else if(GUI.Button(new Rect(guiPosition.x - 100, Screen.height + -guiPosition.y + 25, 50, 50), LumberMill.name)) {
				CreateBuilding(Building.BuildingType.WoodWorks);
			} else if (GUI.Button(new Rect(guiPosition.x + 50, Screen.height + -guiPosition.y + 25, 50, 50), Mine.name)) {
				CreateBuilding(Building.BuildingType.Mine);
			}
		} else if(selectedBuilding != "EmptyPlot" && selectedBuilding != "" && selectedBuilding != null) {
			GUI.BeginGroup(new Rect(guiPosition.x - 100, Screen.height + -guiPosition.y - 150, 200, 150));
			GUI.Box(new Rect(0, 0, 200, 150), buildingName);
			GUI.Label(new Rect(5, 0, 200, 20), "" + resourceName + ": ");
			GUI.Box(new Rect(140, 30, 60, 120), "");
			
			OpenPanel();
			GUI.EndGroup();
		}
	}
	
	void OpenPanel() {
		if(building == null)
			return;

		GUI.Label(new Rect(85, 25, 200, 20), "LVL: " + (int)building.currentLevel);

		if(building.currentLevel != building.maxLevel) {
			GUI.DrawTexture(new Rect(0, 50, 20, 20), wood);
			GUI.DrawTexture(new Rect(0, 70, 20, 20), stone);
			
			GUI.Label(new Rect(25, 50, 30, 20), "-" + building.woodCostForNextLevel, buyStyle);
			GUI.Label(new Rect(25, 72, 30, 20), "-" + building.stoneCostForNextLevel, buyStyle);
			GUI.Label(new Rect(18, 27, 30, 20), "Cost");
			
			if (GUI.Button(new Rect(5, 95, 50, 50), "Upgrade")) {
				if(playerData.woodAmount >= building.woodCostForNextLevel) {
					if(playerData.stoneAmount >= building.stoneCostForNextLevel) {
						building.SwitchLevel(building.currentLevel + 1);
					}
				}
			}
		}
		
		GUI.DrawTexture(new Rect(140, 50, 20, 20), wood);
		GUI.DrawTexture(new Rect(140, 70, 20, 20), stone);
		GUI.Label(new Rect(165, 50, 30, 20), "+" + building.woodSellPrice, sellStyle);
		GUI.Label(new Rect(165, 72, 30, 20), "+" + building.stoneSellPrice, sellStyle);
		
		if (GUI.Button(new Rect(145, 95, 50, 50), "Sell")) {
			//Selling Logic
		}
	}
	
	void CreateBuilding(Building.BuildingType type) {
		spawnBuilding.CreateBuilding(type);
		spawnBuilding.tag = "Untagged";
		
		selectedBuilding = "";
	}

	void Deselect() {
		selectedBuilding = "";
		target = null;
		building = null;
		spawnBuilding = null;
	}
}