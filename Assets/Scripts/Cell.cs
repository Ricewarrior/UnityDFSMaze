using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {
    public GameObject floor;
    public GameObject westWall;
    public GameObject eastWall;
    public GameObject northWall;
    public GameObject southWall;

    public bool allWallsIntact
    {
        get
        {
            return westWall.renderer.enabled &&
            eastWall.renderer.enabled &&
            northWall.renderer.enabled &&
            southWall.renderer.enabled;
        }
    }

}
