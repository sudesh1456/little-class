# Little Class

An English preschool game for ages 3–6 (Nursery first). Unity 6. A child taps a game; every prompt is on the screen in big type. Nothing is collected, and the app does not use the network.

App id `com.sudeshchhipa.littleclass`.

## What's in this first build

Five Nursery games, English only:

| Game | Room | What the child does |
|---|---|---|
| Letters | English | Hear that A is for apple, then tap A. Letters A–E. |
| Counting | Maths | Count the dots, tap how many (1–5). |
| Colors | Maths | Tap the color you are asked for. |
| Animals | World | Tap who says moo, woof, meow, quack, or baa. |
| Twinkle Twinkle | Rhymes | Fill the missing word in each line. |

A wrong tap says "Try again" and stays on the round. Finishing a game puts a check on the classroom list. Grown-ups can see how many of the five are done. LKG and UKG are named there and not playable yet.

Voice clips (Piper, or Azure `en-US-AnaNeural`) are the next pass. This build shows the words so it can be played before any audio is rendered.

## Opening in Unity

1. Unity 6 LTS (`I:\Unity\Editors\6000.3.25f1`, with Android Build Support).
2. Unity Hub → Add project from disk → this folder.
3. Open `Assets/Scenes/Main.unity` and press Play. The game also starts itself in an empty scene.

## Checks

```bash
dotnet run --project Tools/ContentCheck
dotnet build Tools/UnityCompileCheck
```
