using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
	FlikittCore FlikittCore;

	UserInterface UserInterface;

	public GameObject PencilLine;

	public Color color;

	public string colorName;

	public float width;

	private int currentLine;

	private Line activeLine;

	private Touch touch;

	void Start(){
		FlikittCore = GameObject.Find("Flikitt Core").GetComponent<FlikittCore>();
		UserInterface = GameObject.Find("User Interface").GetComponent<UserInterface>();
		colorName = "White";
		width = 0.07f;
	}

	void Update(){
		LineAttributor();
		TouchToDraw();
	}

	void TouchToDraw(){
		if(Input.touchCount > 0){
			if(UserInterface.canDraw){
				touch = Input.GetTouch(0);
				if(touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Moved){
					currentLine++;
					GameObject line = Instantiate(PencilLine);
					line.name = "Line " + currentLine;

					activeLine = line.GetComponent<Line>();

					int currentFrame = FlikittCore.currentFrame;
					line.transform.parent = FlikittCore.frames[currentFrame - 1].goSelf.transform;
				}
			}
		}

		if(activeLine != null){
			Vector2 touchPos = touch.position;
			touchPos = Camera.main.ScreenToWorldPoint(touchPos);
			activeLine.UpdateLine(touchPos);
		}
	}

	void LineAttributor(){
		switch(colorName){
			case "Red":
				color = Color.red;
				break;

			case "Orange":
				color = new Color(255.0f/255.0f, 165.0f/255.0f, 0.0f, 1.0f);
				break;

			case "Yellow":
				color = Color.yellow;
				break;

			case "Green":
				color = Color.green;
				break;

			case "Blue":
				color = Color.blue;
				break;

			case "Indigo":
				color = new Color(75.0f/255.0f, 0.0f, 130.0f/255.0f, 1.0f);
				break;

			case "Purple":
				color = new Color(128.0f/255.0f, 0.0f, 128.0f/255.0f, 1.0f);
				break;

			case "White":
				color = Color.white;
				break;

			case "Black":
				color = Color.black;
				break;

			default:
				color = Color.white;
				colorName = "White";
				break;
		}
	}

}
