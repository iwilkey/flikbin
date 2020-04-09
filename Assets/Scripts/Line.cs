using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Line : MonoBehaviour
{
	DrawingManager dm;

	public LineRenderer renderer;
	List<Vector2> points;

	private Color color;
	private float width;

	void Awake(){
		dm = GameObject.Find("Drawing Manager").GetComponent<DrawingManager>();
	}

	void Start(){
		color = dm.color;
		width = dm.width;
	}

	public void UpdateLine(Vector2 mousePos){
		if(points == null){
			points = new List<Vector2>();
			SetPoint(mousePos);
			return;
		}

		if(Vector2.Distance(points.Last(), mousePos) > .001f) SetPoint(mousePos);
	} 

	void SetPoint(Vector2 point){
		points.Add(point);

		//This is where I edit the attributes of the line renderer
		renderer.SetColors(color, color);
		renderer.SetWidth(width, width);
		renderer.positionCount = points.Count;
		renderer.SetPosition(points.Count - 1, point);
	}
}
