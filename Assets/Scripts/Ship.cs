using UnityEngine;

public enum Status { Running, GameOver }

public class Ship : MonoBehaviour {
    /// <summary>
    /// rigedbody of the ship for easy access
    /// </summary>
    public Rigidbody2D rb;
    public AsteroidManager Asteroids;
    public StarManager Stars;
    public GameObject[] Flames;
    /// <summary>
    /// maximum speed of the ship
    /// </summary>
    public float MaxSpeed = 1.2f;
    /// <summary>
    /// reference to the game over display
    /// </summary>
    public Canvas GameOver;
    /// <summary>
    /// distance from the ship where nothing should be visible anymore
    /// </summary>
    public int Viewport = 15;
    /// <summary>
    /// some random private variables to store state from input update to render time
    /// </summary>
    private float rotation, acceleration;
    private Status status = Status.Running;
    /// <summary>
    /// record rotation and accelleration controls in frame update
    /// </summary>
    void Update() {
        // ignore inputs when game is over
        if (status != Status.Running) return;
        acceleration = Input.GetAxisRaw("Vertical");
        rotation = Input.GetAxisRaw("Horizontal");
    }
    /// <summary>
    /// time to calculate stuff to render
    /// calculate rotation and velocity updates
    /// as well as bound check for the stars
    /// </summary>
    void FixedUpdate() {
        Flames[0].SetActive(acceleration > 0f);
        Flames[1].SetActive(rotation < 0f);
        Flames[2].SetActive(rotation > 0f);
        CalculateVelocity();
        CalculateRotation();
        // check which asteroids are left for the stars and send in some new ones if neccessary
        Asteroids.CheckAsteroids();
        // check if stars gone nova and create new ones if necessary
        Stars.CheckStars();
        if (Input.GetKey(KeyCode.Escape)) ResetGame();
    }
    /// <summary>
    /// how to exit vi?!!!
    /// </summary>
    /// <param name="other">that something that hit the ship, should be an asteroid or a fragment thereof</param>
    void OnCollisionEnter2D(Collision2D other) {
        status = Status.GameOver;
        acceleration = rotation = 0;
        GameOver.enabled = true;
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
        status = Status.Running;
        GameOver.enabled = false;
        // move the ship back to the origin with no velocity
        rb.position = new Vector2(0, 0);
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        // this time the galaxy will look the best it has ever been! 
        Stars.Start();
        Asteroids.Start();
    }
}
