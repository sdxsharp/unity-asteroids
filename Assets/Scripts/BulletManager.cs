using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {
    /// <summary>
    /// reference the ship to know where to spawn a new bullet from
    /// </summary>
    public Ship Ship;
    /// <summary>
    /// reference to the bullet prefab to be clowned on spawning a bullet
    /// </summary>
    public GameObject Bullet;
    /// <summary>
    /// spawning speed of the bullet
    /// should be faster than the speed of the ship!
    /// </summary>
    public float Speed = 2f;
    /// <summary>
    /// list of the live bullets flying around
    /// </summary>
    private List<GameObject> Bullets = new();
    /// <summary>
    /// counter for bullet hits
    /// </summary>
    private int num;
    /// <summary>
    /// return the score for bullet hits
    /// </summary>
    public string Score { get { return $"Hits: {num}"; } }
    /// <summary>
    /// only useful to restart the game
    /// </summary>
    public void Start() {
        // reset the counter for bullet hits
        num = 0;
        // destroy all bullet game objects...
        foreach (var b in Bullets) Destroy(b);
        // ...and the references to them
        Bullets.Clear();
    }
    /// <summary>
    /// check if any of the bullets is out of the viewport and spawn a new one in front of the ship
    /// </summary>
    public void CheckBullets() {
        // check every bullet we know of
        // while using a copy of the list as we may modify the list
        foreach (var bullet in Bullets.ToArray()) {
            // skip if bullet is still inside of ships viewport
            if ((Ship.transform.position - bullet.transform.position).sqrMagnitude < Ship.Viewport * Ship.Viewport) continue;
            // destroy bullets reference from the list and the game object
            DestroyBullet(bullet, 0);
        }
        // the offset from the ships center of gravity to the tip. yes, I know it's misaligned from the x axis
        var shiptip = new Vector2(0.037f, 0.5f);
        // have a new bullet spawn in front of the ship
        var b = Instantiate(Bullet, (Vector3)Ship.rb.position + (Ship.transform.rotation * shiptip), Quaternion.identity);
        // set the velocity of the bullet in the current direction of the ship
        b.GetComponent<Rigidbody2D>().velocity = Ship.transform.rotation * new Vector2(0, 1) * Speed;
        // adopt the fresh, piping hot bullet so it will stay below the BulletManager in hierarchy view
        b.transform.parent = transform;
        // remember the milk!
        Bullets.Add(b);
    }
    /// <summary>
    /// destroy the bullet game object and the reference to it
    /// </summary>
    /// <param name="bullet">gae obkect to be destroyed</param>
    public void DestroyBullet(GameObject bullet, int hits = 1) {
        // remove the game object when found in live bullet list
        // (haven't seen this happen, but don't destroy bullets twice. And it looks more sleek)
        if (Bullets.Remove(bullet)) Destroy(bullet);
        // increase the counter for bullet hits
        num += hits;
    }
}
