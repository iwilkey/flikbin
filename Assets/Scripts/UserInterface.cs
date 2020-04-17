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
	public Sprite cam, deletePic, notPlaying, playing, pencilOn, pencilOff, eraserOn, eraserOff;
	public Image captureButton, pencil, eraser, selectCam, selectDraw;
	public Slider fpsSlider, thicknessSlider;
	public Toggle contShot;

	public string mode;

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

		SetMode("Capture");

		contShot.isOn = false;
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

	public void SetMode(string _mode){

		mode = _mode;
		foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
		 	if(obj.IsChildOf(GameObject.Find("User Interface").transform) && obj.name != "User Interface"){
		 		obj.gameObject.SetActive(false);
		 	}
		 }

		foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
			if(obj.gameObject.CompareTag("EssentialUI")){
				obj.gameObject.SetActive(true);
			}
		}

		if(_mode == "Drawing"){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.gameObject.CompareTag("DrawingTools")){
					obj.gameObject.SetActive(true);
				}

				if(obj.name == "Thickness Slider"){
					for(int i = 0; i < obj.childCount; i++){
						obj.GetChild(i).gameObject.SetActive(true);
						for(int p = 0; p < obj.GetChild(i).childCount; p++){
							obj.GetChild(i).GetChild(p).gameObject.SetActive(true);
						}
					}
				}
			}
		} else if (_mode == "Capture"){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.gameObject.CompareTag("CaptureTools")){
					obj.gameObject.SetActive(true);
				}

				if(obj.name == "FPS Slider"){
					for(int i = 0; i < obj.childCount; i++){
						obj.GetChild(i).gameObject.SetActive(true);
						for(int p = 0; p < obj.GetChild(i).childCount; p++){
							obj.GetChild(i).GetChild(p).gameObject.SetActive(true);
						}
					}
				}

				if(obj.name == "Continuous Shot Toggle"){
					for (int i = 0; i < obj.childCount; i++){
						obj.GetChild(i).gameObject.SetActive(true);
						for(int p = 0; p < obj.GetChild(i).childCount; p++){
							obj.GetChild(i).GetChild(p).gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	void Update(){

		int cframe = FlikittCore.currentFrame;
		int projlength = FlikittCore.project.getAllFrames().Count;

		FlikittCore.project.setFps(fpsSlider.value);
		DrawingManager.width = thicknessSlider.value;

		frameCounter.text = cframe + " / " + projlength;

		if(FlikittCore.drawMode == "Pencil"){
			pencil.sprite = pencilOn;
			eraser.sprite = eraserOff;
		} else {
			pencil.sprite = pencilOff;
			eraser.sprite = eraserOn;
		}

		if(Input.touchCount == 2){
			FlikittCore.drawMode = "Eraser";
		}

		if(Input.touchCount >= 3){
			FlikittCore.drawMode = "Pencil";
		}
			
		if(contShot.isOn){
			FlikittCore.continuousShot = true;
		} else {
			FlikittCore.continuousShot = false;
		}

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
		if(mode == "Capture"){
			if(!canPlay){
				Image playButton = GameObject.Find("Play / Pause").GetComponent<Image>();
				playButton.color = new Color(playButton.color.r, playButton.color.g, playButton.color.b, 0.1f);
			} else {
				Image playButton = GameObject.Find("Play / Pause").GetComponent<Image>();
				playButton.color = new Color(playButton.color.r, playButton.color.g, playButton.color.b, 1.0f);
			}
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

		if(mode == "Capture"){
			selectDraw.color = new Color(selectDraw.color.r, selectDraw.color.g, selectDraw.color.b, 0.1f);
			selectCam.color = new Color(selectCam.color.r, selectCam.color.g, selectCam.color.b, 1.0f);
		} else {
			selectDraw.color = new Color(selectDraw.color.r, selectDraw.color.g, selectDraw.color.b, 1.0f);
			selectCam.color = new Color(selectCam.color.r, selectCam.color.g, selectCam.color.b, 0.1f);
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

		if(mode == "Capture"){
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
		} else {
			if (FlikittCore.project.getAllFrames().Count < FlikittCore.currentFrame + 1){
				Image forward = GameObject.Find("Forward").GetComponent<Image>();
				forward.color = new Color(forward.color.r, forward.color.g, forward.color.b, 0.1f);
			}
		}

		if(FlikittCore.drawMode == "Eraser"){
			Vector3 fingerPosition2 = Input.mousePosition;
			fingerPosition2.z = Mathf.Infinity;
			RaycastHit2D hit2 = Physics2D.Raycast(fingerPosition2, fingerPosition2 - Camera.main.ScreenToWorldPoint(fingerPosition2), Mathf.Infinity);
			if(hit2.collider != null){
				print(hit2.collider.gameObject.name);
			}
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

		if(Input.touchCount == 0 && FlikittCore.isShooting){
			FlikittCore.isShooting = false;
			print("DID!");
			if(FlikittCore.project.getAllFrames().Count - 1 >= 1)
				FlikittCore.project.deleteFrame(FlikittCore.project.getFrame(FlikittCore.project.getAllFrames().Count - 1));
				FlikittCore.LoadPage(FlikittCore.currentFrame - 1);
		}

		/*
		MODE MANAGEMENT!
		*/

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
									if(mode != "Drawing") FlikittCore.NewPage();
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

						if(!FlikittCore.continuousShot){
							if(!FlikittCore.isPlaying){
								if(FlikittCore.getCurrentFrame().getHasPicture()){
									CameraManager.DeleteCapture();
								} else {
									CameraManager.Capture();
								}
							}
						} else {
							if(!FlikittCore.isPlaying){
								if(FlikittCore.getCurrentFrame().getHasPicture()){
									CameraManager.DeleteCapture();
								} else {
									FlikittCore.isShooting = true;
									FlikittCore.StartShot();
								}
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

					case "Pencil":

						if(FlikittCore.drawMode != "Pencil") {FlikittCore.drawMode = "Pencil";}
						break;

					case "Eraser":

						if(FlikittCore.drawMode != "Eraser") {FlikittCore.drawMode = "Eraser";}
						break;

					case "SelectCapture":

						if(mode != "Capture") { SetMode("Capture"); }
						break;

					case "SelectDrawing":

						bool canProceed = true;
						for(int i = 0; i < FlikittCore.project.getAllFrames().Count; i++){
							if(!FlikittCore.project.getFrame(i).getHasPicture()) canProceed = false;
						}

						if(canProceed) { if(mode != "Drawing") { SetMode("Drawing"); } }
						break;


					default:
						break;
				}
			}
		}
	}
}
