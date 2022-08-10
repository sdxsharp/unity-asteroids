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
    /// how many stars in the main arms of the galaxy (plus a third as much in the side arms)
    /// number of stars drawn randomly around the galaxy
    /// </summary>
    public float MaxStars = 300;
    /// <summary>
    /// all the stars we ever created (and didn't go supernova yet)
    /// </summary>
    private List<GameObject> Stars = new List<GameObject>();
    /// <summary>
    /// create a galaxy around the ship and a few more stars at random positions
    /// </summary>
    public void Start() {
        // let's try to draw an even more beatiful galaxy after a respawn
        foreach (var star in Stars) Destroy(star);
        Stars.Clear();
        // let's make a galaxy!
        for (var i = 0; i < MaxStars; i++) {
            // one arm...
            CreateArm(i, 0, .4f, 1);
            // ...two arms (uh, oh, that one is a bit small)...
            CreateArm(i, 90, .3f, 3);
            // ...three arms...
            CreateArm(i, 180, .6f, 1);
            // ...and finally a fourth one! yes, I know that it's not very big
            CreateArm(i, 270, .4f, 3);
            // and just sprinkle a few stars around it
            CreateStar(Ship.rb.position, Ship.Viewport / 3f, Ship.Viewport * 1.2f);
        }
    }
    /// <summary>
    /// chek if some stars are out of the viewport and draw somme more stars at random positions outside of the viewport
    /// </summary>
    public void CheckStars() {
        // check every star we keep track of
        foreach (var star in Stars.ToArray()) {
            // skip if star is still inside of the ships extended viewport
            if ((Ship.transform.position - star.transform.position).sqrMagnitude < Ship.Viewport * Ship.Viewport * 2f) continue;
            // only destroy a fraction of the stars in the galaxy each update
            if ((transform.position - star.transform.position).sqrMagnitude < Ship.Viewport * Ship.Viewport * 1.2f && Random.Range(1f, 1000f) > 2f) continue;
            // destroy the game object...
            Destroy(star);
            // ...and erase any evidence it ever existed
            Stars.Remove(star);
        }
        // create some new stars if necessary
        if (Stars.Count > MaxStars*3.666) return;
        // create stars in bursts of 20, because you know, SCIENCE!
        for (var i = 0; i < 20; i++) CreateStar(Ship.rb.position, Ship.Viewport, Ship.Viewport * 1.2f);
    }
    /// <summary>
    /// draw stars in a spiral outward pattern
    /// </summary>
    /// <param name="i">number of the star</param>
    /// <param name="start">start arm at this angle</param>
    /// <param name="dist">draw star whithin this random distance from the perfect spiral point</param>
    /// <param name="mod">fraction of stars to be drawn (for minor arms > 1)</param>
    private void CreateArm(int i, int start, float dist, int mod) {
        // draw fewer stars in minor arms (but draw some more in the center)
        if (i % mod != 0 && i > mod * 10) return;
        // just put the stars of the minor arms in the central region of the major arms
        if (mod > 1 && i < mod * 10) start += 90;
        // add more blur from the perfect spiral the farther from the center
        var blur = dist * 1.5f * i / MaxStars;
        // normalize for one rotation
        var norm = i * 360f / MaxStars;
        // i've no idea what that is but it just sounds cool (should be calculated according to max stars and outer radius or something)
        var SpiralDrag = 20;
        // base point on the spiral, going a bit further from the center with every degree of the turn
        var SpiralVector = (Quaternion.Euler(0, 0, start + norm) * new Vector2(1, 0).normalized * i / SpiralDrag);
        // create a star somewhere around the point on the spiral
        CreateStar((Vector3)Ship.rb.position + SpiralVector, 0, dist + blur);
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
        // random amount of the blue part of the color of the star
        // above .5 the blue part of the color is 1 with a varying degree of the red and green parts
        // below .5 the blue part varies between .0 and 1 and the red and green is 1 (yellow)
        var blue = Random.Range(0.0f, 1.2f);
        // random amount of the red part of the color of the star
        // above .5 the red part of the color is 1 with a varying degree of the green and blue parts
        // otherwise the color is between yellow and blue
        var red = Random.Range(0.3f, 1.0f);
        // the varying part of red and green when star is blue or yellow otherwise
        var yellow = blue > .5f ? blue : 1;
        // position of the star at random position around the origin
        var vector = pos + (Random.insideUnitCircle.normalized * Random.Range(mindist, maxdist));
        // crete the game object from the prefab at the random position
        var star = Instantiate(Protostar, vector, Quaternion.identity);
        // add the random size of the star
        star.transform.localScale = new Vector3(scale, scale, scale);
        // adopt the newly created star so stars will stay below the StarManager in hierarchy view
        star.transform.parent = transform;
        // set a random color to the star between white and red, yellow, or blue
        star.GetComponent<SpriteRenderer>().color = new Color(red > .5f ? 1 : yellow, red > .5f ? red : yellow, red > .5f ? red : (blue > .5f ? 1 : (.7f - blue)));
        // remember that there was a star here once, even if it went supernova
        Stars.Add(star);
    }
}
