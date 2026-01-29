# Hallow-Sword (Unity / C#)

A 2D, fast-paced Unity, boss-fight game built around a **state-swapping mechanic**: the boss alternates between **vulnerable** and **invincible**, and who can deal damage flips depending on the current state. A timer shows how long the boss will remain in its current state, pushing you to plan your movement and attacks around tight windows.

> **Demo:** (add link here)  

---

## Core Objective

Defeat the boss by reducing its health to zero while managing your limited health and the boss’s alternating damage rules.

- **Player Health:** 5  
- **Boss Health:** 300  
- **Player Damage:** 20 per hit  
- **Boss Damage:** 1 per hit

---

## Key Mechanic: Fire State Swap

The boss switches between two states:

### Invincible
- **Boss can damage the player**
- **Player cannot damage the boss**
- **Visually signified by the boss being on fire**

### Vulnerable
- **Player can damage the boss**
- **Boss cannot damage the player**
- **Boss will no longer be on fire**

A **state timer** is displayed to indicate how long the boss will remain in its current state, so you can time aggression vs. evasion.

---

## Controls

Designed primarily for **controller play** (optimal for Xbox).

### Controller (Xbox / PlayStation)

| Action | Control |
|---|---|
| Horizontal Movement | Left Stick (left/right) |
| Fast Fall | Left Stick down |
| Jump | **South Button** (A / X) |
| Dash | **East Button** (B / Circle) |
| Attack Up | **West Button** (X / Square) |
| Attack Forward | **North Button** (Y / Triangle) |

> Keyboard supported, but strongly discouraged

---

## Movement & Combat Mechanics

- **Wall Jumping:** You can jump onto and off the **left and right camera borders**, which are treated as walls.
- **Double Jump:** You can jump again mid-air.
- **Dash Cancel:** Attacking while dashing **cancels the remaining dash**.
- **Chained Tech:** You can **jump → dash → jump** (momentum and timing matter).
- **Invincibility Frames:** You get brief i-frames after taking damage.

---

## Strategy Tips (Optional but helpful)

- When the boss is **on fire**, focus on **movement and survival** use dashes and wall tech to reposition.
- When the boss is **not on fire**, play **aggressively** you have a limited damage window, so confirm hits.
- Use **dash cancel** intentionally: it can help you adjust spacing or convert a dash into a quick punish.

---

## Tech Stack

- **Engine:** Unity  
- **Language:** C#  

---

## How to Run

Clone the repo and run on Unity.

