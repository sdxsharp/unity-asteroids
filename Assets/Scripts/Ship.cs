using UnityEngine;

public class Ship : MonoBehaviour {
    /// <summary>
    /// rigedbody of the ship for easy access
    /// </summary>
    public Rigidbody2D rb;
    public AsteroidManager Asteroids;
    public StarManager Stars;
    public BulletManager Bullets;
    public GameObject[] Flames;
    /// <summary>
    /// maximum speed of the ship
    /// </summary>
    public float MaxSpeed = 1.2f;
    /// <summary>
    /// time in physics update frames until the next bullet is spawned
    /// </summary>
    public float ReloadSpeed = 30;
    /// <summary>
    /// reference to the game over display
    /// </summary>
    public Canvas GameOver;
    /// <summary>
    /// reference to the press any key display
    /// </summary>
    public Canvas AnyKey;
    /// <summary>
    /// reference to the HUD
    /// </summary>
    public UnityEngine.UI.Text Score;
    /// <summary>
    /// time in physics update frames until the game can be respawned
    /// </summary>
    public int GameOverTimeout = 200;
    /// <summary>
    /// distance from the ship where bullets misteriously disappear and your worst asteroid dreams come from
    /// </summary>
    public int Viewport = 15;
    /// <summary>
    /// some random private variables to store state from input update to render time
    /// </summary>
    private float rotation, acceleration, fire;
    /// <summary>
    /// counter for the number of collected powerups
    /// </summary>
    private int num;
    /// <summary>
    /// record rotation and accelleration controls in frame update
    /// </summary>
    void Update() {
        if (fire <= -1) return;
        acceleration = Input.GetAxisRaw("Vertical");
        rotation = Input.GetAxisRaw("Horizontal");
    }
    /// <summary>
    /// time to calculate stuff to render
    /// calculate rotation and velocity updates
    /// as well as bound checks every couple of frames
    /// </summary>
    void FixedUpdate() {
        Flames[0].SetActive(acceleration > 0f);
        Flames[1].SetActive(rotation < 0f);
        Flames[2].SetActive(rotation > 0f);
        // if not in game over mode...
        if (fire > -1) {
            CalculateVelocity();
            CalculateRotation();
        }
        // if its time for bounds check of bullets, asteroids, and stars
        if (fire == 0) {
            // reset the frame counter
            fire = ReloadSpeed;
            // check if the bullets are still in viewport ad spawn a new one
            Bullets.CheckBullets();
            // check which asteroids are left for the stars and send in some new ones if neccessary
            Asteroids.CheckAsteroids();
            // check if stars gone nova and create new ones if necessary
            Stars.CheckStars();
        } else fire -= 1; // decrease frame counter
        // enable press any key message if the timeout has elapsed
        if (fire == -1 * GameOverTimeout) AnyKey.enabled = true;
        // reload the game if any key was pressed after game over
        if (fire <= -1 * GameOverTimeout && Input.anyKey) {
            // reset the frame counter
            fire = ReloadSpeed;
            // hide the messages
            ResetGame();
        }
        // display the number of asteroids in radar range, number of bullet hits, and the number of powerups collected
        Score.text = $"{Asteroids.Score} {Bullets.Score} Powerups: {num}";
    }
    /// <summary>
    /// how to exit vi?!!!
    /// </summary>
    /// <param name="other">that something that hit the ship, should be an asteroid or a fragment thereof</param>
    void OnCollisionEnter2D(Collision2D other) {
        var asteroid = other.gameObject;
        // determine if the asteroid is big enough to cause damage to the ship
        if (asteroid.GetComponent<Rigidbody2D>().mass > .3f) {
            // enter the game over mode
            fire = -1;
            // ignore inputs
            acceleration = rotation = 0;
            // display the game over text
            GameOver.enabled = true;
        } else {
            Asteroids.DestroyAsteroid(asteroid);
            // spit out an other bullet
            Bullets.CheckBullets();
            // increase the number of powerups collectes
            num++;
        }
    }
    /// <summary>
    /// calculate the new velocity when up is pressed
    /// it would be nice if it would take a while to speed up t maxspeed some day thus the function
    /// </summary>
    private void CalculateVelocity() {
        if (acceleration > 0) rb.velocity = transform.rotation * new Vector2(0, 1) * acceleration * MaxSpeed;
    }
    /// <summary>
    /// calculate new rotation when left or right is pressed
    /// </summary>
    private void CalculateRotation() {
        // rotate the ship by one degree (every physics update)
        rb.rotation -= rotation;
        // spin a little bit slower if the ship was sinning before
        if (rotation > 0 && rb.angularVelocity > 1) rb.angularVelocity -= rotation;
        if (rotation < 0 && rb.angularVelocity < -1) rb.angularVelocity -= rotation;
        // retain a little bit of angular velocity
        rb.angularVelocity -= rotation * Random.Range(.6f, .9f);
    }
    /// <summary>
    /// reset all game items
    /// </summary>
    private void ResetGame() {
        GameOver.enabled = false;
        AnyKey.enabled = false;
        // move the ship back to the origin with no velocity
        rb.position = new Vector2(0, 0);
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        // this time the galaxy will look the best it has ever been! 
        Stars.Start();
        // delete all old asteroids and spawn some new ones
        Asteroids.Start();
        // delete all bullets
        Bullets.Start();
        // reset the number of powerups collected
        num = 0;
    }
}
