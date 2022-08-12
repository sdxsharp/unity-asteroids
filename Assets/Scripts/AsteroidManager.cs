using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour {
    /// <summary>
    /// reference to the ship to know in which direction the asteroids need to fly
    /// </summary>
    public Ship Ship;
    /// <summary>
    /// prefabs of all the asteroids to use
    /// </summary>
    public GameObject[] AsteroidTemplates;
    /// <summary>
    /// initial number of asteroids (close to the ship, the rest will be spawned outside of the viewport)
    /// </summary>
    public float InitialNumber = 5;
    /// <summary>
    /// the farthest possible distance of the initially spawned asteroids
    /// </summary>
    public float InitialFar = 8;
    /// <summary>
    /// the closest possible distance of the initially spawned asteroids
    /// </summary>
    public float InitialNear = 3;
    /// <summary>
    /// the minimum speed of the asteroids (slow asteroids are boring)
    /// </summary>
    public float MinSpeed = 0.8f;
    /// <summary>
    /// the maximum speed of the asteroids (shoud be about the same as the ship)
    /// </summary>
    public float MaxSpeed = 1.2f;
    /// <summary>
    /// the maximum angular velocity of an asteroid
    /// </summary>
    public float MaxRotation = 30;
    /// <summary>
    /// don't spawn any more asteroids when mass limit is reached
    /// </summary>
    public float MaxTotalMass = 10;
    /// <summary>
    /// increase the mass limit by that factor to make it harder to avoid asteroids over time
    /// </summary>
    public float MassIncrease = .2f;
    /// <summary>
    /// current mass of the asteroids, to avoid looping over every small piece
    /// </summary>
    private float AsteroidMass, AsteroidMassIncrease;
    /// <summary>
    /// list of all asteroids and fragments
    /// </summary>
    private List<GameObject> Asteroids = new List<GameObject>();
    /// <summary>
    /// initialy place some asteroids close to the ship with random directions
    /// </summary>
    public void Start() {
        // just clean up before we try again
        foreach (var a in Asteroids) Destroy(a);
        Asteroids.Clear();
        AsteroidMass = 0;
        AsteroidMassIncrease = 0;
        for (var i = 0; i < InitialNumber; i++) {
            // distriute distance from the ship equally
            var dist = (InitialFar - InitialNear) / InitialNumber * i + InitialNear;
            // create a new asteroid at a random position between min and max initial distance and give it a random rirection
            AddAsteroid(CreateAsteroid(Ship.rb.position, dist), Random.insideUnitCircle.normalized);
        }
    }
    /// <summary>
    /// check for stray asteroids and lead new ones to the promised land
    /// </summary>
    public void CheckAsteroids() {
        // check all the asteroids and fragments we know of...
        foreach (var a in Asteroids.ToArray()) {
            // skip if asteroid is still inside of ships viewport
            if ((Ship.transform.position - a.transform.position).sqrMagnitude < Ship.Viewport * Ship.Viewport * 3f) continue;
            // disintegrate asteroid instantly if it's just too far away to be useful anymore
            RemoveAsteroid(a);
        }
        // send in the minions if there are not enough anymore
        if (AsteroidMass < MaxTotalMass + AsteroidMassIncrease) {
            // create a new asteroid far away from the ship...
            var a = CreateAsteroid(Ship.rb.position, Ship.Viewport);
            // ... but send it directly to its last known position 
            AddAsteroid(a, (Ship.transform.position - a.transform.position).normalized);
        } else AsteroidMassIncrease += MassIncrease; // increase the maximum total mass semi-regularly
    }
    /// <summary>
    /// make sure the givenn asteroid is never heard from again
    /// </summary>
    /// <param name="a">asteroid lost in space or otherwise desintegrated</param>
    public void RemoveAsteroid(GameObject asteroid) {
        // we are not responsible for what ever that supposed asteroid is supposed to be if it's not in the all knowing list
        if (!Asteroids.Contains(asteroid)) return;
        // substract the androidsmass from the calculation, so we can spawn more asteroids soon
        AsteroidMass -= asteroid.GetComponent<Rigidbody2D>().mass;
        // destroy the game object...
        Asteroids.Remove(asteroid);
        // ...and every evidence that it was ever there
        Destroy(asteroid);
    }
    /// <summary>
    /// create an asteroid at a random position in some distance
    /// </summary>
    /// <param name="position">position around where there is a no go zone</param>
    /// <param name="dist">distance from the no go zone</param>
    /// <returns></returns>
    private GameObject CreateAsteroid(Vector2 position, float dist) {
        // get a random position in some distance
        var vector = position + (Random.insideUnitCircle.normalized * dist);
        // spawn a new game object from a random one of the asteroid prefabs
        var result = Instantiate(AsteroidTemplates[Random.Range(0, AsteroidTemplates.Length)], vector, Quaternion.identity);
        // apply some random angular velocity to the new asteroid
        result.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-1 * MaxRotation, MaxRotation);
        return result;

    }
    /// <summary>
    /// addasteroid to the list and remember its mass
    /// </summary>
    /// <param name="a">asteroid to be remembered forever</param>
    /// <param name="velocity">velocity to be applied to the asteroid</param>
    private void AddAsteroid(GameObject a, Vector2 velocity) {
        var rb = a.GetComponent<Rigidbody2D>();
        rb.velocity = velocity * Random.Range(MinSpeed, MaxSpeed);
        // remember the milk!
        AsteroidMass += rb.mass;
        // adopt the newly born asteroid so it will stay below the AsteroidManager in hierarchy view
        a.transform.parent = this.transform;
        Asteroids.Add(a);
    }
}
