using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame{

	private string name;
	private bool enabled, hasPicture;
	public GameObject goSelf;
	public Canvas canvasSelf;
	public Image imageSelf;

	public Frame(int _number){
		name = "Frame " + _number;

		goSelf = new GameObject();
		goSelf.name = name;
		goSelf.gameObject.tag = "Frame";
		goSelf.transform.parent = GameObject.Find("Project").transform;

		goSelf.AddComponent<Canvas>();
		canvasSelf = goSelf.GetComponent<Canvas>();
		canvasSelf.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasSelf.sortingOrder = -1;

		goSelf.AddComponent<Image>();
		imageSelf = goSelf.GetComponent<Image>();


		hasPicture = false;
		enabled = true;
	}

	public void Enable(){
		if(!enabled){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.CompareTag("Frame") && obj.name == this.name){
					obj.gameObject.SetActive(true);
					enabled = true;
				}
			}
		} else {
			return;
		}
	}

	public void Disable(){
		if(enabled){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.CompareTag("Frame") && obj.name == this.name){
					obj.gameObject.SetActive(false);
					enabled = false;
				}
			}
		} else {
			return;
		}
	}

	public void setName(int num){
		name = "Frame " + num;
	}

	public string getName(){
		return name;
	}

	public bool getHasPicture(){
		return hasPicture;
	}

	public void setHasPicture(bool p){
		hasPicture = p;
	}

	public bool isEnabled(){
		return enabled;
	}
}

public class FlikittCore : MonoBehaviour
{
	public int currentFrame = 1;
	public List<Frame> frames;

	void Start(){
		//Is for a new project...
		if(frames == null){
			frames = new List<Frame>();
			NewPage();
		}
	}

	public void NewPage(){
		currentFrame++;
		Frame frame = new Frame(currentFrame);
		frames.Add(frame);
	}

	public void LoadPage(int frame){
		currentFrame = frame;
	}

	public Frame getCurrentFrame(){
		for(int i = 0; i < frames.Count; i++){
			if(frames[i].getName() == (string)("Frame " + currentFrame.ToString())){
				return frames[i];
			}
		}
		return null;
	}
}
