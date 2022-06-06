using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;           
    public GameObject lifeDisplay; // display in the bottom left-hand corner
    public GameObject canvas;      // used in transition when the state is reset
    public GameObject walls;
    public GameObject winScreen;
    public GameObject gate;
    public GameObject blackScreen;
    public int ghostMultiplier { get; private set; } = 1;  // keeps track of how many points are awarded with each ghost eaten during one power pellet cycle
    public int score { get; private set; }

    [SerializeField]
    private int lives = 4;
    public int level { get; private set; } = 0;

    [SerializeField]
    private int pelletsRemaining = 244;
    private int totalPellets = 244;  // for internal game use
    private int fruitAppearance = 0; // keeps track of how many times a fruit has appeared in a level (max 2 per level)

    public bool gameInProgress { get; private set; } = false;
    public bool roundInProgress { get; private set; } = false;
    private bool extraLife = true;          // give the player an extra life at 10000 points (only happens once)
    private bool toggleOneUp = false;
    private bool togglePelletSound = true;  // alternate between 2 pellet eaten sounds
    private bool resetFromNewRound = false; // true if the game state is reset from winning a round
    private bool resetFromStart = true;     // true if the game state is reset from starting up the game
    private bool isFrightened = false;      // true if the ghosts are currently in frightened mode

    private AudioManager audioManager;
    private Sound siren;
    private int currentSiren = 1;

    public Sprite[] fruits;
    private int[] fruitPoints = { 100, 300, 500, 700, 1000, 2000, 3000, 5000 };

    public Fruit currentFruit;
    public int ghostsEaten = 0; // number of ghosts currently eaten
                                // TODO: if all 4 ghosts are eaten, then cancel frightened mode

    public Text gameOverText;
    public Text scoreText;
    public Text highScoreText;
    public Text ghostMultiplierText;
    public Text oneUp;
    public Text fruitScoreText;

    private void Awake() {
        // DontDestroyOnLoad(this);
        this.audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start() {
        InvokeRepeating("ToggleOneUp", 0.125f, 0.25f);
        InvokeRepeating("AdjustSiren", 0.125f, 0.50f);
        this.siren = this.audioManager.Get("siren_1");
        NewGame();
    }

    private void Update() {
        SetFruit();
        SetHighScore();

        // check for game restart
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(0);
            NewGame();
        }

        // award an extra life once when the score reaches 10000
        if (this.score >= 10000 && this.extraLife) {
            this.extraLife = false;
            SetLives(this.lives + 1);
            this.audioManager.Play("extend");
        }

        // display fruit once a 25% and 75% of the pelelts have been eaten
        if (this.pelletsRemaining <= 0.75 * this.totalPellets && this.fruitAppearance == 0) {
            StartCoroutine(DisplayFruit());
            this.fruitAppearance++;
        }
        if (this.pelletsRemaining <= 0.25 * this.totalPellets && this.fruitAppearance == 1) {
            StartCoroutine(DisplayFruit());
            this.fruitAppearance++;
        }

        // pause/unpause game
        if(Input.GetKeyDown(KeyCode.Space)) {   
            if(this.gameInProgress && this.roundInProgress) {
                if (Time.timeScale == 1) {
                    Time.timeScale = 0;
                    this.pacman.isKeysEnabled = false;
                    this.audioManager.PauseAll();
                } else if (Time.timeScale == 0) {
                    Time.timeScale = 1;
                    this.pacman.isKeysEnabled = true;
                    this.audioManager.UnpauseAll();
                }
            }            
        }
    }

    // start a new game
    private void NewGame() {
        this.audioManager.PauseAll();
        this.level = 0;
        this.gameInProgress = true;
        this.resetFromNewRound = true;
        SetScore(0);
        SetLives(4);
        NewRound();
    }

    // called when all pellets have been collected
    // resets pellets, ghosts, pacman, and any relevant variables
    public void NewRound() {

        // redundants calls to pause all sounds
        this.audioManager.PauseAll();
        this.siren.source.Stop();
        this.audioManager.StopAll();

        this.gameOverText.enabled = false;
        this.pelletsRemaining = 244;

        // foreach (Transform pellet in this.pellets) {
        //     pellet.gameObject.SetActive(true);
        // }

        // redundant calls to pacman reset state
        // this code is called twice, once here and once in ResetState()
        this.pacman.ResetState();
        this.pacman.movement.SetDirection(Vector2.zero);
        this.pacman.isKeysEnabled = false;

        this.level++;

        // resetFromNewRound is true except for if the game has just started (when this.level == 1)
        if(this.level > 1) {
            this.resetFromNewRound = true;
        }

        this.fruitAppearance = 0;
        this.currentFruit.gameObject.SetActive(false);
        this.togglePelletSound = true;
        this.currentSiren = 1;
        this.siren = this.audioManager.Get("siren_1");

        // SetFruit();
        ResetState();
        this.resetFromNewRound = false;
    }

    // resets ghosts and pacman, handles transition between rounds
    public void ResetState() {
        StartCoroutine(ResetStateUtility());
    }

    private IEnumerator ResetStateUtility() {

        // destroy the fruit if it is displayed
        this.currentFruit.gameObject.SetActive(false);

        // reset all the pellets if the player won the previous round
        if(this.resetFromNewRound) {
            foreach (Transform pellet in this.pellets) {
                pellet.gameObject.SetActive(true);
            }
        }

        // display pacman and ghosts UI for 2 seconds before the level starts
        if(level > 1 || !this.resetFromStart) {
            canvas.transform.GetChild(9).gameObject.SetActive(true);

            // ensure that all sounds are stopped
            this.audioManager.StopAll();
            this.siren.source.Stop();
            yield return new WaitForSecondsRealtime(2);

            canvas.transform.GetChild(9).gameObject.SetActive(false);
            this.siren.source.Play();
        }

        if(this.resetFromStart) {
            this.resetFromStart = false;
        }

        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState();

        // change this later
        this.roundInProgress = true;
        this.audioManager.UnpauseAll();
    }

    private IEnumerator WonRound() {
        this.roundInProgress = false;

        // freeze pacman and disable movement
        this.pacman.gameObject.GetComponent<AnimatedSprite>().enabled = false;
        this.pacman.isKeysEnabled = false;

        this.pacman.animation.advance = 0;
        this.pacman.movement.speed = 0.0f;
        this.pacman.movement.SetDirection(Vector2.zero, true);
        this.pacman.animation.Restart();

        // stop all sounds
        this.siren.source.Stop();
        this.audioManager.StopAll();

        // stop all ghosts temporarily 
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].movement.SetDirection(Vector2.zero, true);
            this.ghosts[i].home.terminate = true;
        }

        yield return new WaitForSeconds(2);

        this.currentFruit.gameObject.SetActive(false);

        // set all ghosts to inactive
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.gate.SetActive(true);

        // flash walls and black screen to transition to a new round
        for (int i = 0; i < 4; i++) {
            this.walls.SetActive(false);
            this.winScreen.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            this.walls.SetActive(true);
            this.winScreen.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
        
        this.blackScreen.gameObject.SetActive(true);
        this.pacman.spriteRenderer.enabled = false;        
        yield return new WaitForSeconds(0.25f);

        this.blackScreen.gameObject.SetActive(false);
        this.pacman.spriteRenderer.enabled = true;
        this.gate.SetActive(false);

        NewRound();
    }

    private void GameOver() {
        this.gameInProgress = false;
        this.gameOverText.enabled = true;
        this.currentFruit.gameObject.SetActive(false);

        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    // display the number of lives (pacman icons) in the bottom left-hand corner
    private void SetLives(int lives) {
        this.lives = lives;
        for(int i = 0; i < 5; i++) {
            if (i < this.lives) {
                lifeDisplay.transform.GetChild(i).gameObject.SetActive(true);
            } else {
                lifeDisplay.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetScore(int score) {
        this.score = score;
        this.scoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetHighScore() {
        if(this.score > HighScore.score) {
            HighScore.score = this.score;

            // set the highscore in the static class highscore
            // this.highScore.score = this.score;
        }
        this.highScoreText.text = HighScore.score.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten() {
        StartCoroutine(PacmanDeathUtility());
    }

    // handles the pacman death sequence
    private IEnumerator PacmanDeathUtility() {
        // freeze pacman and disable movement
        this.roundInProgress = false;
        this.pacman.isKeysEnabled = false;
        this.siren.source.Stop();

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;

        // disable all ghosts sprites
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }

        // start pacman death animation
        this.pacman.RotateUp();
        this.pacman.DeathSequence();
        StartCoroutine(PlayDeathSound());

        // TODO: update the lives after the death sequence
        // i.e. wait until the level state reset instead of updating the lives immediately
        SetLives(this.lives - 1);

        yield return new WaitForSeconds(2.5f);

        // if the player won the round, start a new round after 3s; otherwise call GameOver()
        if (this.lives > 0) {
             this.blackScreen.gameObject.SetActive(true);
            this.pacman.spriteRenderer.enabled = false;        
            yield return new WaitForSeconds(0.25f);

            this.blackScreen.gameObject.SetActive(false);
            // this.pacman.spriteRenderer.enabled = true;
            Invoke(nameof(ResetState), 0.0f);
        } else {
            Invoke(nameof(GameOver), 0.0f);
        }
    }

    // play pacman's death sound
    private IEnumerator PlayDeathSound() {
        this.audioManager.Play("death_1");
        yield return new WaitForSeconds(1.2f);
        this.audioManager.Stop("death_1");
        this.audioManager.Play("death_2");
        yield return new WaitForSeconds(this.audioManager.Get("death_2").clip.length);
        this.audioManager.Play("death_2");
    }

    // TOOD: implement this (this is not used anywhere yet)
    public void DisableGhostEaten() {
        if(this.ghostsEaten > 0) {
            this.ghostsEaten--;
        }
        // disable the retreating sound only if all there are no ghosts retreating
        if(this.ghostsEaten == 0) {
            this.audioManager.Stop("retreating");
            ResumeSound();
        } 
    }

    // resume the correct sound (power pellet sound or siren) after ghost eaten has been disabled
    // TODO: fix this; if ghost frighened mode ends while a ghost is eaten (and is traveling back to the house),
    // then the siren will start playing insteading of waiting for the ghost to reach the house
    private void ResumeSound() {
        if(this.gameInProgress && this.roundInProgress) {
            if(this.isFrightened) {
                this.audioManager.Play("power_pellet");
            } else {
                this.siren.source.Play();
            }
        }        
    }

    public void GhostEaten(Ghost ghost) {
        StartCoroutine(GhostEatenUtility(ghost));
    }

    // handles the ghost eaten sequence
    private IEnumerator GhostEatenUtility(Ghost ghost) {
        this.ghostsEaten++;
        this.audioManager.Play("eat_ghost");

        // set the score based on the ghost's score multiplier
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);

        // temporary display the points from eating the ghost
        this.ghostMultiplierText.text = points.ToString();
        this.ghostMultiplierText.gameObject.transform.position = ghost.transform.position;
        this.ghostMultiplierText.gameObject.SetActive(true);

        this.ghostMultiplier *= 2;

        // turn off the ghost and pacman's sprite renderers
        ghost.gameObject.SetActive(false);
        this.pacman.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        // freeze everything for 1 second
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;

        // switch to retreating sound effect
        this.audioManager.Stop("power_pellet");

        // do not restart the retreating sound if a ghost is already eaten
        if(this.ghostsEaten <= 1) {
            this.audioManager.Play("retreating");
        }

        this.ghostMultiplierText.gameObject.SetActive(false);
        ghost.gameObject.SetActive(true);

        this.pacman.gameObject.GetComponent<SpriteRenderer>().enabled = true;

        // enable ghost eaten state
        // TODO: set duration
        ghost.ghostEaten.Enable(10.0f);

        // disable ghost eaten is handled by each individual ghost
        // see GhostEaten.cs
    }

    public void PelletEaten(Pellet pellet) {
        if (pellet.GetType() != typeof(PowerPellet)) {
            // toggle the pellet eaten sound when regular pellets are eaten
            if (this.togglePelletSound) {
                FindObjectOfType<AudioManager>().Play("munch_1");
            } else {
                FindObjectOfType<AudioManager>().Play("munch_2");
            }
        }
        this.togglePelletSound = !this.togglePelletSound;

        pellet.gameObject.SetActive(false);
        this.pelletsRemaining--;

        SetScore(this.score + pellet.points);

        // the round was won
        if(this.pelletsRemaining == 0) {
            StartCoroutine(WonRound());
        }
    }

    public void PowerPelletEaten(PowerPellet pellet) {
        // enable frightened mode for the pellet's duration
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        this.isFrightened = true;

        PelletEaten(pellet);
        PlayPowerPelletSound();

        // reset power pellet cycle if another one is eaten
        CancelInvoke(nameof(ResetGhostMultiplier));
        CancelInvoke(nameof(StopPowerPelletSound));
        CancelInvoke(nameof(SetIsFrightenedFalse));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        Invoke(nameof(StopPowerPelletSound), pellet.duration);
        Invoke(nameof(SetIsFrightenedFalse), pellet.duration);
    }

    // set the fruit sprite and the points based on the level
    private void SetFruit() {
        this.currentFruit.spriteRenderer.sprite = fruits[LevelToIndex()];
        this.currentFruit.points = fruitPoints[LevelToIndex()];
    }

    public void CollectFruit(Fruit fruit) {
        StartCoroutine(CollectFruitUtility(fruit));
    }

    // collect the fruit and briefly display the points earned
    private IEnumerator CollectFruitUtility(Fruit fruit) {
        this.audioManager.Play("eat_fruit");
        SetScore(this.score + fruit.points);
        this.fruitScoreText.text = fruit.points.ToString();

        fruit.gameObject.SetActive(false);
        this.fruitScoreText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2);

        this.fruitScoreText.gameObject.SetActive(false);
    }

    // display the fruit on screen for 8 seconds
    private IEnumerator DisplayFruit() {
        this.currentFruit.gameObject.SetActive(true);
        yield return new WaitForSeconds(8.0f);
        this.currentFruit.gameObject.SetActive(false);
    }

    // convert the level to an index in the list of fruit sprites
    // ex: level 1 is the cherry sprite (index 0), level 2 is the strawberry (index 1),
    // levels 3 and 4 are the orange (both index 2), level 5 and 6 are the apple (both index 3), etc...
    private int LevelToIndex() {
        float temp = (float)this.level;
        if (temp == 1.0f) {
            return 0;
        } else if (temp < 13.0f) {
            int result = (int)Mathf.Ceil(temp / 2.0f);
            return result;
        } else {
            return 7;
        }
    }

    // adjusts the siren sound based on how many pellets are eaten
    private void AdjustSiren() {
        if(this.gameInProgress && this.roundInProgress) {
            if (this.pelletsRemaining <= this.totalPellets * 1 && this.currentSiren == 1) {
                this.siren.source.Stop();
                this.siren = this.audioManager.Get("siren_1");
                if (!this.isFrightened) {
                    this.siren.source.Play();
                }
                this.currentSiren++;
            }
            if (this.pelletsRemaining <= this.totalPellets * 0.5167f && this.currentSiren == 2) {
                this.siren.source.Stop();
                this.siren = this.audioManager.Get("siren_2");
                if (!this.isFrightened) {
                    this.siren.source.Play();
                }
                this.currentSiren++;
            }
            if (this.pelletsRemaining <= this.totalPellets * 0.24167f && this.currentSiren == 3) {
                this.siren.source.Stop();
                this.siren = this.audioManager.Get("siren_3");
                if (!this.isFrightened) {
                    this.siren.source.Play();
                }
                this.currentSiren++;
            }
            if (this.pelletsRemaining <= this.totalPellets * 0.1167f && this.currentSiren == 4) {
                this.siren.source.Stop();
                this.siren = this.audioManager.Get("siren_4");
                if (!this.isFrightened) {
                    this.siren.source.Play();
                }
                this.currentSiren++;
            }
            if (this.pelletsRemaining <= this.totalPellets * 0.04167 && this.currentSiren == 5) {
                this.siren.source.Stop();
                this.siren = this.audioManager.Get("siren_5");
                if (!this.isFrightened) {
                    this.siren.source.Play();
                }
                this.currentSiren++;
            }
        }
    }

    // one up text in the top left-hand corner should flash off and on throughout the game
    // eating a ghost or dying will cause the one up toggle animation to pause briefly (this is differerent from the original game)
    private void ToggleOneUp() {
        oneUp.gameObject.SetActive(this.toggleOneUp);
        this.toggleOneUp = !this.toggleOneUp;
    }

    private void PlayPowerPelletSound() {
        this.siren.source.Stop();
        this.audioManager.Play("power_pellet");
    }

    private void StopPowerPelletSound() {
        this.audioManager.Stop("power_pellet");
        this.siren.source.Play();
    }

    // private bool HasRemainingPellets() {
    //     return this.pelletsRemaining != 0;    
    // }

    private void ResetGhostMultiplier() {
        this.ghostMultiplier = 1;
    }
    private void SetIsFrightenedFalse() {
        this.isFrightened = false;
    }
}
