using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
	FlikittCore FlikittCore;
	public bool canDraw;
	public Text frameCounter;

	void Start(){
		FlikittCore = GameObject.Find("Flikitt Core").GetComponent<FlikittCore>();
	}

	void Update(){

		int cframe = FlikittCore.currentFrame;
		int projlength = FlikittCore.frames.Count;

		frameCounter.text = cframe + " / " + projlength;

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

						if(cframe + 1 > projlength){
							FlikittCore.NewPage();
						} else {
							FlikittCore.LoadPage(cframe + 1);
						}

						break;

					case "Back":

						if(cframe - 1 <= 0){
							FlikittCore.LoadPage(1);
						} else {
							FlikittCore.LoadPage(cframe - 1);
						}

						break;

					default:
						break;
				}
			}
		}
	}
}
