# Anatomiva — Interactive Anatomy & Movement Lab

Anatomiva is a personal project for learning human anatomy and biomechanics through bilingual quizzes and interactive movement experiments.

I designed the project and built the original Python/Pygame prototype, including the body map and quizzes organized by body part. That version is available in [`legacy/`](legacy/). The C# desktop version adds movement simulations, guided missions and targeted review.

— Angela Plescia

## Run on your PC

Extract the distribution ZIP and open **Installa_Anatomiva.exe**. The installer creates a desktop shortcut and a Start menu entry for the current user. Alternatively, open **Portatile/Anatomiva.exe** directly.

No Python, terminal, development environment, account, internet connection or server is needed. The executable uses .NET Framework 4, a Windows 10 component. The installer targets Windows 10 x64 or later; the AnyCPU application runs as a 64-bit process on Windows x64. The files are unsigned. Native Windows installation and rendering have not been tested in this Linux build environment; see `docs/RELEASE_STATUS.md`.

## What is implemented

- **Explore:** 12 body-part cards, an original illustrated map and an equivalent keyboard-accessible list.
- **Anatomy quizzes:** two levels per category, five questions per level, with Italian and English versions of all 120 questions, explanations and saved answer snapshots.
- **Movement lab:** elbow, knee and ankle schematics with angle sliders, a reference segment, target markers, and animation/pause controls.
- **Arm levers:** change load mass and distance on a horizontal forearm; inspect external torque and compare it with the same load at 30 cm.
- **Four guided missions:** meet the model target and answer a concept question; checks and feedback are saved locally. Both conditions are required to complete a mission.
- **Targeted review:** revisit up to five unresolved mistakes per session. A question remains pending while its latest answer is wrong, including answers from review. Later correct answers clear it without rewriting historical quiz results.
- **Progress:** quiz best scores, recent attempts, lab checks, completed missions, bilingual result review, and CSV export containing quizzes, review and labs.
- **Preferences:** language and larger text; native mouse/keyboard controls, scrollable pages and no telemetry.

## Models and limits

Joint drawings are 2D teaching schematics, not individualized biomechanical models. Elbow/knee angles are flexion from a straight reference; ankle angles use a perpendicular-foot neutral position, with negative dorsiflexion and positive plantar flexion. Slider ranges and mission targets are chosen for the simulation, not clinical ROM limits or prescribed exercises.

The lever experiment fixes the forearm horizontally and uses only the external load: `torque = mass × 9.81 × perpendicular distance`. Limb weight, acceleration, muscle force, tendon moment arms and joint reaction forces are omitted. The numeric result is therefore not a measurement or prediction of a person's physical effort. All drawings and icon artwork are created in code for this project; no competitor assets are used.

## Source map

| File/folder | Responsibility |
| --- | --- |
| `src/Core.cs` | Catalogue, quiz sessions, answer snapshots, atomic local storage and legacy import |
| `src/Learning.cs` | Review planning, mechanics, mission definitions and evaluation |
| `src/App.cs` | Shared application window, navigation, quizzes, results and preferences |
| `src/LearningPages.cs` | Lab, targeted review and lab progress pages |
| `src/LabView.cs` | Interactive controls and original movement diagrams |
| `src/ProgressExport.cs` | CSV generation independent of UI dialogs |
| `src/Theme.cs` | Theme and original body-map drawing |
| `src/Program.cs` | Startup, recovery and isolated test entry point |
| `src/UiTests.cs` | Real control-handler regression scenario and screen captures |
| `content/catalog.xml` | Embedded bilingual anatomy catalogue |
| `tests/CoreTests.cs` | Catalogue, quiz, review, mechanics, storage, import and export checks |
| `packaging/` | App icon, manifest, Windows build command and NSIS installer |
| `legacy/` | Original Python prototype, preserved and not needed at runtime |

## Build and test

On a Windows development machine, run **packaging/build-windows.cmd**. It locates the .NET Framework C# compiler, builds the application and test runner, runs the domain tests, and builds the installer if NSIS is on PATH. Optional catalogue and icon authoring utilities use Python and Pillow; the checked-in XML and icon are sufficient for normal builds.

On Linux with Mono, compile all `src/*.cs` into a Windows Forms executable, referencing `System.Windows.Forms`, `System.Drawing` and `System.Xml.Linq`; embed `content/catalog.xml` as `catalog.xml` and `packaging/app.ico` as `app.ico`, and include the icon/manifest. Domain tests compile `Core.cs`, `Learning.cs`, `ProgressExport.cs` and `tests/CoreTests.cs`, with the XML catalogue embedded. Run the tests from the repository root.

To run the graphical regression scenario on a development machine with an active display: `Anatomiva.exe --ui-test ABSOLUTE_EMPTY_DIRECTORY` (or `mono build/Anatomiva.exe --ui-test ABSOLUTE_EMPTY_DIRECTORY`). It uses an isolated store and writes synthetic results, captures and an export in that directory. It does not use the normal user profile. See actual test evidence in `docs/TEST_REPORT.md`.

## Local data and upgrade

The new profile is `%LOCALAPPDATA%\AngelaPlescia\Anatomiva\progress.xml`, schema 2, with a previous-version `.bak` file. At first launch, if no Anatomiva profile exists, compatible Body Explorer progress is copied from the old profile and converted without changing the old file. Existing Anatomiva data is never replaced by legacy import. The apps do not synchronize after this one-time import.

Reset removes quizzes, review results and lab checks, retaining preferences. Uninstalling retains the profile; the Italian guide explains full removal. Future unsupported schemas are rejected instead of silently overwritten.

## Project documentation

- [Architecture and design decisions](docs/ARCHITETTURA.md)
- [Test results and coverage](docs/TEST_REPORT.md)
- [Windows build status](docs/RELEASE_STATUS.md)
- [Educational references](docs/FONTI.md)
