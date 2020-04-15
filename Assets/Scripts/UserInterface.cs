using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSwatch {
	private string name;
	private Image imageSelf;
	private bool enabled;
	private GameObject GOSelf;

	public ColorSwatch(string _name, Image _imageSelf){
		name = _name;
		imageSelf = _imageSelf;
		enabled = false;
		GOSelf = GameObject.Find(name + "_Swatch");
	}

	public string getName() {return name;}
	public bool isEnabled() {return enabled;}

	public void enable(){
		imageSelf.color = new Color(imageSelf.color.r, imageSelf.color.g, imageSelf.color.b, 1.0f);
		GOSelf.GetComponent<Image>().color = imageSelf.color;
		enabled = true;
	}

	public void disable(){
		imageSelf.color = new Color(imageSelf.color.r, imageSelf.color.g, imageSelf.color.b, 0.4f);
		GOSelf.GetComponent<Image>().color = imageSelf.color;
		enabled = false;
	}
}

public class UserInterface : MonoBehaviour
{
	FlikittCore FlikittCore;
	CameraManager CameraManager;
	DrawingManager DrawingManager;
	public bool canDraw, canPlay;
	private List<ColorSwatch> colorSwatches = new List<ColorSwatch>();

	//UI Vars
	public Text frameCounter;
	public Sprite cam, deletePic, notPlaying, playing;
	public Image captureButton;
	public Slider fpsSlider, thicknessSlider;

	void Start(){
		FlikittCore = GameObject.Find("Flikitt Core").GetComponent<FlikittCore>();
		CameraManager = GameObject.Find("Camera Manager").GetComponent<CameraManager>();
		DrawingManager = GameObject.Find("Drawing Manager").GetComponent<DrawingManager>();

		fpsSlider.maxValue = 15.0f; fpsSlider.minValue = 0.5f;

		string[] colors = new string[9]{"Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Purple", "White", "Black"};
		for(int i = 0; i < colors.Length; i++){
			Image swatchImage = GameObject.Find(colors[i] + "_Swatch").GetComponent<Image>();
			ColorSwatch colorSwatch = new ColorSwatch(colors[i], swatchImage);
			colorSwatches.Add((new ColorSwatch(colors[i], swatchImage)));
		}
		DisableAllSwatches();
		EnableSwatch("White");
	}

	void DisableAllSwatches(){
		for(int i = 0; i < colorSwatches.Count; i++){
			colorSwatches[i].disable();
		}
	}

	void EnableSwatch(string color){
		for(int i = 0; i < colorSwatches.Count; i++){
			if(colorSwatches[i].getName() == color){
				colorSwatches[i].enable();
			}
		}
	}

	void Update(){

		int cframe = FlikittCore.currentFrame;
		int projlength = FlikittCore.project.getAllFrames().Count;

		FlikittCore.project.setFps(fpsSlider.value);
		DrawingManager.width = thicknessSlider.value;

		frameCounter.text = cframe + " / " + projlength;

		//If frame one, or some frame doesn't have a picture on it
		if(FlikittCore.project.getAllFrames().Count > 1){
			bool allFine = true;
			for(int i = 0; i < FlikittCore.project.getAllFrames().Count; i++){
				if(!FlikittCore.project.getFrame(i).getHasPicture()) allFine = false;
			}

			if (allFine){
				canPlay = true;
			} else {
				canPlay = false;
			}
		} else {
			canPlay = false;
		}

		if(!canPlay){
			Image playButton = GameObject.Find("Play / Pause").GetComponent<Image>();
			playButton.color = new Color(playButton.color.r, playButton.color.g, playButton.color.b, 0.1f);
		} else {
			Image playButton = GameObject.Find("Play / Pause").GetComponent<Image>();
			playButton.color = new Color(playButton.color.r, playButton.color.g, playButton.color.b, 1.0f);
		}

		//Turning off camera feed event
		if(FlikittCore.getCurrentFrame().getHasPicture()){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.gameObject.name == "Camera Feed"){
					obj.gameObject.SetActive(false);
				}
			}

			captureButton.sprite = deletePic;
		} else {
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.gameObject.name == "Camera Feed"){
					obj.gameObject.SetActive(true);
				}
			}
			captureButton.sprite = cam;
		}

		//Muting frame selectors
		if(!FlikittCore.isPlaying){
			//Muting forward button event
			if(!FlikittCore.getCurrentFrame().getHasPicture()){
				Image forward = GameObject.Find("Forward").GetComponent<Image>();
				forward.color = new Color(forward.color.r, forward.color.g, forward.color.b, 0.1f);
			} else {
				Image forward = GameObject.Find("Forward").GetComponent<Image>();
				forward.color = new Color(forward.color.r, forward.color.g, forward.color.b, 1.0f);
			}

			//Muting back button event
			if(FlikittCore.currentFrame != 1){
				if(!FlikittCore.getCurrentFrame().getHasPicture()){
					Image back = GameObject.Find("Back").GetComponent<Image>();
					back.color = new Color(back.color.r, back.color.g, back.color.b, 0.1f);
				} else {
					Image back = GameObject.Find("Back").GetComponent<Image>();
					back.color = new Color(back.color.r, back.color.g, back.color.b, 1.0f);
				}
			} else{
				Image back = GameObject.Find("Back").GetComponent<Image>();
				back.color = new Color(back.color.r, back.color.g, back.color.b, 0.1f);
			}
		} else {
			Image forward = GameObject.Find("Forward").GetComponent<Image>();
			forward.color = new Color(forward.color.r, forward.color.g, forward.color.b, 0.1f);
			Image back = GameObject.Find("Back").GetComponent<Image>();
			back.color = new Color(back.color.r, back.color.g, back.color.b, 0.1f);
		}

		//Playing events
		if(FlikittCore.isPlaying){
			Image playButton = GameObject.Find("Play / Pause").GetComponent<Image>();
			playButton.sprite = playing;

			Image camButton = GameObject.Find("Capture Button").GetComponent<Image>();
			camButton.color = new Color(camButton.color.r, camButton.color.g, camButton.color.b, 0.1f);

			Image micButton = GameObject.Find("Microphone").GetComponent<Image>();
			micButton.color = new Color(micButton.color.r, micButton.color.g, micButton.color.b, 0.1f);

		} else {
			Image playButton = GameObject.Find("Play / Pause").GetComponent<Image>();
			playButton.sprite = notPlaying;

			Image camButton = GameObject.Find("Capture Button").GetComponent<Image>();
			camButton.color = new Color(camButton.color.r, camButton.color.g, camButton.color.b, 1.0f);

			Image micButton = GameObject.Find("Microphone").GetComponent<Image>();
			micButton.color = new Color(micButton.color.r, micButton.color.g, micButton.color.b, 1.0f);
		}

		//This is bad way to do this, find a way to meld them together...
		if(Input.touchCount > 0){
			Vector3 fingerPosition1 = Input.mousePosition;
			fingerPosition1.z = Mathf.Infinity;
			RaycastHit2D hit1 = Physics2D.Raycast(fingerPosition1, fingerPosition1 - Camera.main.ScreenToWorldPoint(fingerPosition1), Mathf.Infinity);
			if(hit1.collider != null){
				canDraw = false;
			} else {
				canDraw = true;
			}
		}

		//For buttons (happens once)
		if(Input.GetMouseButtonDown(0)){
			Vector3 fingerPosition = Input.mousePosition;
			fingerPosition.z = Mathf.Infinity;
			RaycastHit2D hit = Physics2D.Raycast(fingerPosition, fingerPosition - Camera.main.ScreenToWorldPoint(fingerPosition), Mathf.Infinity);
			if(hit.collider != null){
				switch(hit.collider.gameObject.name){
					case "Forward":

						if(!FlikittCore.isPlaying){
							if(FlikittCore.getCurrentFrame().getHasPicture()){
								if(cframe + 1 > projlength){
									FlikittCore.NewPage();
								} else {
									FlikittCore.LoadPage(cframe + 1);
								}
							}
						}

						break;

					case "Back":

						if(!FlikittCore.isPlaying){
							if(cframe - 1 <= 0){
								FlikittCore.LoadPage(1);
							} else {
								if(FlikittCore.getCurrentFrame().getHasPicture()){
									FlikittCore.LoadPage(cframe - 1);
								}
							}
						}

						break;

					case "Capture Button":

						if(!FlikittCore.isPlaying){
							if(FlikittCore.getCurrentFrame().getHasPicture()){
								CameraManager.DeleteCapture();
							} else {
								CameraManager.Capture();
							}
						}

						break;

					case "Play / Pause":

						if(!FlikittCore.isPlaying){
							if(canPlay){
								FlikittCore.isPlaying = true;
								FlikittCore.StartPlay();
							}
						} else {
							FlikittCore.LoadPage(1);
							FlikittCore.isPlaying = false;
						}

						break;

					case "Red_Swatch":

						DrawingManager.colorName = "Red";
						DisableAllSwatches();
						EnableSwatch("Red");
						break;

					case "Orange_Swatch":

						DrawingManager.colorName = "Orange";
						DisableAllSwatches();
						EnableSwatch("Orange");
						break;

					case "Yellow_Swatch":

						DrawingManager.colorName = "Yellow";
						DisableAllSwatches();
						EnableSwatch("Yellow");
						break;

					case "Green_Swatch":

						DrawingManager.colorName = "Green";
						DisableAllSwatches();
						EnableSwatch("Green");
						break;

					case "Blue_Swatch":

						DrawingManager.colorName = "Blue";
						DisableAllSwatches();
						EnableSwatch("Blue");
						break;

					case "Indigo_Swatch":

						DrawingManager.colorName = "Indigo";
						DisableAllSwatches();
						EnableSwatch("Indigo");
						break;

					case "Purple_Swatch":

						DrawingManager.colorName = "Purple";
						DisableAllSwatches();
						EnableSwatch("Purple");
						break;

					case "White_Swatch":

						DrawingManager.colorName = "White";
						DisableAllSwatches();
						EnableSwatch("White");
						break;

					case "Black_Swatch":

						DrawingManager.colorName = "Black";
						DisableAllSwatches();
						EnableSwatch("Black");
						break;


					default:
						break;
				}
			}
		}
	}
}
