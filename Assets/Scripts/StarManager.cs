using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour {
    /// <summary>
    /// reference to the ship to know where to draw
    /// </summary>
    public Ship Ship;
    /// <summary>
    /// reference to the star prefab
    /// </summary>
    public GameObject Protostar;
    /// <summary>
    /// number of stars drawn randomly
    /// </summary>
    public float MaxStars = 300;
    /// <summary>
    /// all the stars we ever created (and didn't go supernova yet)
    /// </summary>
    private List<GameObject> Stars = new List<GameObject>();
    /// <summary>
    /// create a few more stars at random positions
    /// </summary>
    public void Start() {
        // destroy each game object and empty the list
        foreach (var star in Stars) Destroy(star);
        Stars.Clear();
        // let's create some stars
        for (var i = 0; i < MaxStars; i++) CreateStar(Ship.rb.position, 0, Ship.Viewport * 1.5f);
    }
    /// <summary>
    /// chek if some stars are out of the viewport and draw somme more stars at random positions outside of the viewport
    /// </summary>
    public void CheckStars() {
        // check every star we keep track of
        foreach (var star in Stars.ToArray()) {
            // skip if star is still inside of the ships extended viewport
            if ((Ship.transform.position - star.transform.position).sqrMagnitude < Ship.Viewport * Ship.Viewport * 1.5f) continue;
            // destroy the game object...
            Destroy(star);
            // ...and erase any evidence it ever existed
            Stars.Remove(star);
        }
        // create some new stars if necessary
        if (Stars.Count > MaxStars) return;
        // create stars in bursts of 20, because you know, SCIENCE!
        for (var i = 0; i < 20; i++) CreateStar(Ship.rb.position, Ship.Viewport, Ship.Viewport * 1.2f);
    }
    /// <summary>
    /// create star with random color around at a random position around a given position
    /// </summary>
    /// <param name="pos">center position around which the star should be crated</param>
    /// <param name="mindist">minimum distance to create the star around the given position</param>
    /// <param name="maxdist">maximum distance to create the star around the given position</param>
    private void CreateStar(Vector2 pos, float mindist, float maxdist) {
        // random scale for the size of the star
        var scale = Random.Range(0.2f, 0.6f);
        // position of the star at random position around the origin
        var vector = pos + (Random.insideUnitCircle.normalized * Random.Range(mindist, maxdist));
        // crete the game object from the prefab at the random position
        var star = Instantiate(Protostar, vector, Quaternion.identity);
        // add the random size of the star
        star.transform.localScale = new Vector3(scale, scale, scale);
        // adopt the newly created star so stars will stay below the StarManager in hierarchy view
        star.transform.parent = transform;
        // remember that there was a star here once, even if it went supernova
        Stars.Add(star);
    }
}
