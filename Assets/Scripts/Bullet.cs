using UnityEngine;

public class Bullet : MonoBehaviour {
    private AsteroidManager Asteroids;
    private BulletManager Bullets;

    void Start() {
        Asteroids = FindObjectOfType<AsteroidManager>();
        Bullets = FindObjectOfType<BulletManager>();
    }
    /// <summary>
    /// collide the bullet with an other object (should be an asteroid of some sort
    /// </summary>
    /// <param name="other">should be an asteroid</param>
    void OnCollisionEnter2D(Collision2D other) {
        // collision sometimes happens before start when really close to an asteroid
        // this causes so much more havoc, we can simply ignore that and go on with our short lives
        if (Bullets == null) return;
        // let the bullet manager know that this bullet found its target
        Bullets.DestroyBullet(gameObject);
        // let the asteroid manager know that its time to disintegrate this asteroid
        Asteroids.RemoveAsteroid(other.gameObject);
    }
}
