# PATHNC for PowerMill

**CAM automation panel for Autodesk PowerMill — C#, Windows Forms, PowerMill API.**





## What's in it

| Area | Highlights | Key files |
|---|---|---|
| **Feature-based machining** | Recognises holes and prismatic features on the loaded model, groups them and generates the drilling / tapping / pocketing toolpaths with material-aware parameters | `Modules/FBM/` |
| **Tool creation** | Builds the shop's tool library programmatically (end mills, ball nose, drills, taps) with holder, overhang and cutting data, instead of clicking through the tool dialog | `Modules/Tools/` |
| **Workplanes** | Creates workplanes from block geometry (top, bottom, sides) and from cylindrical faces, with the Z axis oriented for the machining direction | `Modules/Workplanes/` |
| **Templates** | One-click machining sequences for recurring part families: block setup → workplane → roughing → rest → finishing, with the tools from the library | `Modules/Templates/` |
| **Block & setup** | Block from bounding box or stock file, safe heights, rapid moves and toolpath defaults applied consistently | `Modules/Setup/` |
| **Panel UI** | Code-built Windows Forms panel, one primary action per page, progress feedback in the status bar | `UI/` |

## Architecture

```
UI (Form1, Theme)
  └─► Services / automation classes (one per action, static Run())
        └─► PowerMill API  [COM automation / Delcam.ProductInterface.PowerMILL / macro execution]
```

- The panel never talks to PowerMill directly: each button calls one automation class with a single `Run(string[] args)` entry point, exactly like the NX version. Swapping the CAM system meant rewriting those classes, not the UI.
- Where the API has no managed call, the class issues a PowerMill **macro** string and reads the result back — the equivalent of the NXOpen ↔ UF split on the NX side.
- Every action is wrapped so a failure leaves the project as it was (undo where PowerMill supports it, otherwise re-load of the last saved state).

## What carried over from the NX version, and what didn't

- **Carried over:** panel structure, page/handler pattern, status-bar feedback, the "one automation class per action" rule, the template idea.
- **Rewritten:** everything that touches geometry. PowerMill has no feature-recognition API comparable to NX FBM, so hole and pocket detection is done from the model's faces and edges by the project itself.
- **Not ported (yet):** Shop Documentation, Copilot and Quote. They depend on the SQL history that the NX version builds; the PowerMill side would only need a collector.

## Tech stack

C# 7.3 · .NET Framework 4.7.2 · Windows Forms · Autodesk PowerMill [VERSÃO] API ([COM / .NET assembly / macros]) 

## Build & run

1. Autodesk PowerMill [VERSÃO] (x64); Visual Studio 2022.
2. Open `[NOME].sln`, fix the PowerMill reference `HintPath` if the installation path differs, build **Release | x64**.
3. Start PowerMill, load a model, then run `[NOME].exe` — the panel attaches to the running PowerMill session. [ou: run from PowerMill via macro `[caminho]`]

## Roadmap / ideas

- Setup sheet export (HTML) reusing the NX version's generator
- Tool library synced with the SQL database used by PATHNC (one library, two CAM systems)
- Adaptive clearing template for 5-axis positional work

## License

MIT — see `LICENSE`. Autodesk and PowerMill are trademarks of Autodesk, Inc. and are not part of this repository.
