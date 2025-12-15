# FlashCards
This is a personal flashcard application designed and used by the developer themselves. The goal is to provide a simple, fast, and unobtrusive tool for daily vocabulary repetition without complex interfaces, heavy configuration, or external dependencies.

The user opens the app → starts learning immediately → repeats words at their own pace — no setup, no cloud, no distractions.

Core requirements:

Add/edit/delete cards locally.
New or less-known words appear more often.
No rigid spaced-repetition algorithm — the user alone decides whether a word is “learned.”
Learned cards are removed from the current session but retain progress for future sessions.
All data is stored locally in an SQLite file (flashcards.db), ensuring full privacy and offline operation.
Architecture and Design Rationale
The application is built as a WPF (.NET 8) desktop app with a clean separation of concerns and a focus on maintainability, simplicity, and user experience.

1. Data Layer: DatabaseService
Uses SQLite via Microsoft.Data.Sqlite.
The database file (flashcards.db) is created in the same directory as the executable (AppDomain.CurrentDomain.BaseDirectory).
Exposes methods to create, read, update cards.
No ORM — direct SQL for transparency and minimal dependencies.
Fields like KnownLevel and LastReviewed support flexible repetition logic without enforcing rules.
Why?
Full local control, no internet dependency, easy to inspect (flashcards.db is a plain file), and avoids bloat from external libraries.

2. Data Models
Card: persistent entity mapped 1:1 to the SQLite table.
SessionCard: transient wrapper used only during a learning session. Holds UI state (IsFlipped, IsLearned, swipe count).
Why separate models?
Keeps persistent data clean and decouples UI behavior from storage logic. Session state doesn’t pollute the database.

3. UI Architecture: Modular Panels
All functionality is implemented as UserControl panels:

AddCardUCPanel: card creation/editing with flip animation (front = word/sentence, back = translation).
EditCardsPanel: list of all cards with click-to-edit.
LearningSessionPanel: learning interface with “I know / I don’t know” buttons and card flip on click.
All panels are hosted in MainWindow via ContentControl and overlaid on a semi-transparent dark background (Darkening).

Why this structure?

Isolation: each feature is self-contained.
Reusability: same add/edit panel used for both new and existing cards.
Simplicity: no navigation framework needed — just set .Content.
4. Learning Logic: LearningSessionManager
Loads all cards from DB.
Builds a weighted random pool of 15 cards (lower KnownLevel → higher selection probability).
During session:
“I don’t know” → card stays in queue, KnownLevel decreases, user sees it again.
“I know” → card marked as learned, KnownLevel increases, removed from session.
Progress is saved to DB immediately on every action.
Why no strict SRS?
The user values subjective control over algorithmic enforcement. The system adapts to behavior, not the other way around.

5. User Experience Details
Watermark textboxes for clean input hints.
Rounded, animated buttons with visual feedback on press.
Escape key closes any active panel (via BaseHandlerControl).
Card flip on mouse click (toggles front/back).
Responsive sizing relative to main window.
Why custom UI over standard controls?
To match the “minimal but pleasant” aesthetic while avoiding external UI libraries.

How to Run the Application
Option 1: Download the Executable 
Go to the Releases page.
Download FlashCards.exe.
Run it on Windows 10 or 11 — no .NET installation required, as it’s a self-contained, single-file publish.
On first launch, a file flashcards.db will appear in the same folder — this is your word database.

Option 2: Build from Source
Prerequisites:

Windows 10/11
.NET 8 SDK
Steps:

in bash u need write
123
git clone https://github.com/nagibator-228-337/FlashCards.git
cd FlashCards
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
The executable will be located at:
FlashCards/bin/Release/net8.0-windows/win-x64/publish/FlashCards.exe

Option 2: Open and Run the Project in Visual Studio
Install Visual Studio 2022
Go to the official website: https://visualstudio.microsoft.com/
Download and install Visual Studio Community 2022.
Install the .NET 8 SDK
The project targets .NET 8, so you’ll need the SDK installed. You can download it here:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0
(If you install Visual Studio with the “.NET desktop development” workload, .NET 8 is usually included—but it’s safest to verify.)
Make sure the WPF workload is installed
During Visual Studio installation (or via the Visual Studio Installer later), ensure the “.NET desktop development” workload is checked. This includes everything needed for WPF apps (XAML editor, designer, templates, etc.).
Open the project
Download or clone the repository to your computer.
Navigate to the folder containing FlashCards.sln.
Double-click FlashCards.sln — it will open automatically in Visual Studio.
Build and run
In Visual Studio, press Ctrl+Shift+B to build the solution, or just press F5 to build and launch the app.
The application will start, and you’ll see the main window with options to add cards, edit them, or start a learning session.

