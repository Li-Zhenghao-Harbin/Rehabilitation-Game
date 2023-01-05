using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSensor : Base {

    bool[] mouseActive = new bool[playerCount]; // check if mouse is clicked, in sensor, represented for the active button
    bool[] hasSetParticularPoints = new bool[playerCount]; // 
    const int xx = 0; // position to store x
    const int yy = 1; // position to store y
    int targetPlayer = 0;
    const int particularPointsCount = 5;
    const int coordinatePointsCount = 2;
    int[,,] particularPoints = new int[playerCount, particularPointsCount, coordinatePointsCount]; // store the x and y of particular points of each player
    bool[,] activeParticularPoints = new bool[playerCount, 5]; // check if moved to particular points
    int[,] beginPoint = new int[playerCount, coordinatePointsCount]; // store the x and y of the begin point of each player
    int[,] endPoint = new int[playerCount, coordinatePointsCount]; // store the x and y of the end point of each player
    const int interval = 200; // distance between particular points
    const int coInterval = 80; // radius of detecting circle

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
