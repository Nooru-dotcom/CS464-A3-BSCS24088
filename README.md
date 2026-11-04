# CS464-A3-BSCS24088 — Collect, Dodge, Survive

**Student:** BSCS24088  
**Course:** CS464 · Game Development · ITU Fall 2026  
**Assignment:** 03 — Collect, Dodge, Survive  

> **Note:** No Assignment 02 repo exists for this student. This project was started from `CS464-A1-BSCS24088` and built up to meet all A3 requirements, as permitted by the assignment brief.

---

## Controls

| Key | Action |
|-----|--------|
| W / A / S / D or Arrow Keys | Move |
| Space | Jump |

---

## How to Win / Lose

- **Win:** Collect all coins in the level (reach the `targetScore`) then step onto the glowing **Goal** zone. The win panel appears and time freezes.
- **Lose a life:** Fall into a kill zone (off the edge) or get your health reduced to 0 by hazards. You respawn at the last checkpoint (or the start) with a brief i-frame window.
- **Lose the game:** Lose all 3 lives. The lose panel appears and time freezes.
- **Restart:** Press **Restart** on either panel. Time is restored to 1× *before* the scene reloads.

---

## Levels

| Level | New Idea | Target Coins | Description |
|-------|----------|:---:|-------------|
| Level01 | Coins + Goal + Bounce Pad | 5 | Safe intro: collect, bounce, reach the goal |
| Level02 | Spikes (Hazard) + Knockback | 5 | Spike fields and pits teach knockback and fall death |
| Level03 | Guard (Patrol → Chase → Return) + Spikes | 5 | A guard patrols; enter its range and it chases you |
| Level04 | Crusher (warns first) + Guard + Checkpoint | 6 | A timed crusher in a corridor + mid-level checkpoint |
| Level05 | Speed Power-Up + Ping-Pong Guard + All Hazards | 6 | Full mix: power-up to outrun the guard and avoid crushers |

---

## Five Numbers (with reasons)

| # | Value | What it is | Why |
|---|-------|------------|-----|
| 1 | **2.5 m/s** | Guard patrol speed | Slow enough for the player to watch and plan, quick enough to feel like a real threat on the narrow bridges in Level03 |
| 2 | **5 m** | Guard chase range | Covers the width of the main patrol corridors so that a careless player triggers the chase but a careful one can slip past the edge of the patrol path |
| 3 | **9 N·s** | Knockback impulse | Sends the player roughly 1.5–2 m on open ground, enough to feel punishing without launching them off an edge from far away |
| 4 | **1.0 s** | I-frames duration | One second of invulnerability after a hit; long enough that the flash is clearly visible and a second hazard right behind the first cannot combo-kill instantly |
| 5 | **5 s** | Speed power-up duration | Five seconds is enough to outpace the Level05 guard across the full arena before the boost expires, but short enough that collecting the second power-up mid-run is a real strategic choice |

---

## Four Short Answers

**Q1 — Why does a trigger need a Rigidbody on at least one object?**  
Unity's physics engine only fires `OnTriggerEnter`/`OnTriggerStay`/`OnTriggerExit` callbacks when at least one of the two colliders has a Rigidbody (dynamic or kinematic) attached to the same GameObject (or any parent). Without it, the overlap is detected geometrically but the message is never sent, so the trigger silently does nothing.

**Q2 — Which ForceMode did you pick for the bounce pad, and why?**  
`ForceMode.VelocityChange` — it applies a one-shot velocity change that ignores mass, so the same bounce pad launches any object to the same height regardless of weight. This fits a bounce pad better than `Impulse` (which divides by mass and would feel different if the player's mass ever changes) and far better than `Force` or `Acceleration` (per-step pushes that would need to run for multiple frames).

**Q3 — What rule does your power-up follow when collected a second time?**  
**Refresh rule.** The second pickup cancels the running boost coroutine on `PlayerController` and starts a fresh 5-second countdown from zero. This means collecting back-to-back pickups is always worth it and never "wastes" the remaining time — the player is rewarded for grabbing every orb.

**Q4 — Why does arrival by `==` fail for a moving agent?**  
`Vector3.MoveTowards` steps the agent by `speed × deltaTime` each frame. Unless that step lands exactly on the target position (which almost never happens with floating-point arithmetic), the agent skips past it. The `==` check fires only at the exact position, so the agent overshoots and never triggers it, causing it to oscillate or keep walking past the waypoint forever. Checking `Vector3.Distance < arriveDistance` catches any frame in which the agent is *close enough*, regardless of step size.

---

## Video Walkthrough

🎥 **[Watch on Gdrive (unlisted)](https://drive.google.com/file/d/19XV5dgni1mMYwSiXZGTGUSw-lOx1OlRx/view?usp=sharing)** 

The video shows: all five levels played through, at least one life lost, a full game-over loss, and a clean win.

---

## Repository Structure

```
Assets/
├── Scripts/          # All gameplay scripts (A3)
│   ├── PlayerController.cs
│   ├── Health.cs
│   ├── GameManager.cs
│   ├── Coin.cs
│   ├── BouncePad.cs
│   ├── Hazard.cs
│   ├── Crusher.cs
│   ├── EnemyPatrol.cs
│   ├── PowerUp.cs
│   ├── KillZone.cs
│   ├── Goal.cs
│   └── Checkpoint.cs
├── Editor/
│   └── LevelBuilder.cs   # CS464 > Build All Levels menu
├── Scenes/
│   ├── Level01.unity … Level05.unity
│   └── SampleScene.unity
└── Prefabs/          # Created by LevelBuilder at runtime
```

---

*Started from: CS464-A1-BSCS24088 (no A2 exists for this student)*
