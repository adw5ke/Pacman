using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class AnimatedSprite : MonoBehaviour {
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] sprites = new Sprite[0];

    public float animationTime = 0.25f;               // speed of animation
    public int animationFrame { get; private set; }   // index into the sprites array

    public int advance = 1;  // control whether or not animation should advance
    public bool loop = true;

    private void Awake() {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    // loop through array of sprites, reset index to 0 if run off of the end of the array
    private void Advance() {
        // if the sprite renderer is not enabled, do not advance
        if (!this.spriteRenderer.enabled) {
            return;
        }

        // increment the index
        this.animationFrame += advance;

        // loop back to beginning of array
        if (this.animationFrame >= this.sprites.Length && this.loop) {
            this.animationFrame = 0;
        }

        // update the current sprite
        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length) {
            this.spriteRenderer.sprite = this.sprites[this.animationFrame];
        }
    }

    public void SetAnimationFrame(int animationFrame) {
        this.animationFrame = animationFrame;
    }

    // reset the animation
    public void Restart() {
        this.animationFrame = -1;
        Advance();
    }
}
