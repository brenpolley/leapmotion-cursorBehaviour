using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CursorBehavior : MonoBehaviour {

	public int _tickNumber; //number of 'ticks' surrounding the cursor
	public GameObject _tickTemplate; //tick Sprite

	public float _disatnceMin; //minimum distance from objects
	public float _distanceMax; //maximum distance from objects

	private List<GameObject> _ticks; //list of all tick objects
	private List<Vector3> _tickPos; //list of positions of all tick objects

	// Use this for initialization
	void Start () {
		_ticks = new List<GameObject>();
		_tickPos = new List<Vector3>();
		expandTicks();
	}
	
	// Update is called once per frame
	void Update () {
			castRayForward();
	}

	void castRayForward(){
		/*
		* cast a ray directly forward from the cursor
		* if the ray collides with an object resize the distance between the ticks and the cursor
		*/
		float _distance;
		Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 100.0f)){
			_distance = hit.distance;
			for(int i = 0; i < _tickNumber; i++){
				_ticks[i].renderer.enabled = true;
			}
			resizeCursor(_distance);
		}
		// if nothing is in front of the cursor, hide the ticks
		else{
			for(int i = 0; i < _tickNumber; i++){
				_ticks[i].renderer.enabled = false;
			}
		}

	}

	//adjust the distance between the ticks and the cursor
	void resizeCursor(float _dis){

		//clamp the distance to fall within defined distance range
		float _newDistance = Mathf.Clamp(_dis, _disatnceMin, _distanceMax);

		//calculate the ratio of the distance to the distance range
		float _range = _distanceMax - _disatnceMin;
		float _percentDis = Mathf.Clamp(_newDistance/_range, 0f, 1.0f);

		//adjust the x and y positions of each tick based on the ratio calculated above
		for(int i = 0; i < _tickNumber; i++){
			Vector3 _moveTo	= new Vector3(_tickPos[i].x * _percentDis, _tickPos[i].y *_percentDis, _tickPos[i].z);
			_ticks[i].transform.localPosition = _moveTo;
		}
	}

	//distribute the tick objects around the cursor
	void expandTicks(){

		//for each tick object...
		for (int j = 0; j < _tickNumber; j++){

			//...calcucate evenly distributed points around cursor
			float i = (j * 1.0f) / _tickNumber;
			float _angle = i * Mathf.PI * 2f;
			float _degrees = _angle * Mathf.Rad2Deg;
			
			float _x = Mathf.Sin(_degrees * Mathf.Deg2Rad);
			float _y = Mathf.Cos(_degrees * Mathf.Deg2Rad);
			
			Vector3 _pos = new Vector3(_x, _y, 0) + transform.position;

			//clone tick object and position it
			GameObject _tick = Instantiate(_tickTemplate, _pos, Quaternion.identity) as GameObject;
			_tick.transform.parent = transform;

			//hide tick object
			_tick.renderer.enabled = false;

			//rotate tick object to point toward cursor
			_tick.transform.Rotate(-Vector3.forward, _degrees);

			//add tick object and position to lists
			_ticks.Add(_tick);
			_tickPos.Add(_tick.transform.localPosition);
		}
	}

}
