using Raylib_cs;
using System.Numerics;

// Initialisation & Konstanter

const int WINDOW_WIDTH = 960;
const int WINDOW_HEIGHT = 640;

Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Positron");
Raylib.SetTargetFPS(12);

// Resurser (Bild & Ljud)

Texture2D positron = Raylib.LoadTexture(@"resources/positron.png");
Texture2D electron = Raylib.LoadTexture(@"resources/electron.png");

Raylib.InitAudioDevice();
Sound gameMusic = Raylib.LoadSound(@"resources/Music.mp3");

// Initialisera enheter

Entity player = new Entity(0, 0, 64, 64, 12, 1, positron, true, false); // Skapa ett enhet som är spelaren
player.name = "Positron"; // Enhetens namn är "Positron"
Lambda.player = player; // Lambda-systemen ska veta att denna enheten är spelaren

PlayerController playerController = new PlayerController(player, 0, 0); // Skapa ett spelarekontrollare

UIElement staminaBar = new UIElement(); // Skapa ett UI-Element där det visar hur mycket man har för att göra ett attack

// Inställningar för spelaren

playerController.speed = 15;
playerController.smooth = 0.5f; 

Raylib.PlaySound(gameMusic);

// Skapa ett "random" variabel (Använder det senare)
Random rnd = new Random();

// Loop

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();

    Raylib.ClearBackground(Color.WHITE); // Bakrunden är vit
    playerController.Update(); // Uppdatera spelarekontrollerare

    Lambda.Update(); // Uppdatera Lambda-systemen

    Raylib.DrawRectangleLinesEx(new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT), 8, Color.RED); // Skapa ett röd gräns

    // HUD
    staminaBar.DrawBar(new Vector2(0, 0), new Vector2(128, 32), playerController.GetStatus("attackDebounceTimer") / 100); // Rita UI Elementen

    // Debug
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F3)) {
        Lambda.debugMode = !Lambda.debugMode; // Om F3 klickas växlar "Debug Mode"

        if (Lambda.debugMode) Console.WriteLine("Debug Mode Activated");
        else Console.WriteLine("Debug Mode Deactivated");
    }

    // Det skulle finnas 2 fiender: Electroner & Atomer
    // Det finns ett 20% (1/5) chans att det skulle skapas ett atom istället för ett elektron

    int chance = rnd.Next(3);
    if (chance == 2) {
        Lambda.SpawnEntity( // Om random-variablen som börjar från 1 och slutar till 5 ger ut nummret 5 då skapas ett atom
        rnd.Next(0, WINDOW_WIDTH - 64), 
        rnd.Next(0, WINDOW_HEIGHT - 64), 
        64, 64, 
        4, 2, 
        electron, 
        5000, 
        "Atom", 
        3, 
        2);
        Console.WriteLine("BOOM SHAKALAKA");
    }
    else {
        Lambda.SpawnEntity( // Men om det ger ut nummret som är *inte* 5 då skapas ett elektron
            rnd.Next(0, WINDOW_WIDTH - 64), 
            rnd.Next(0, WINDOW_HEIGHT - 64), 
            64, 64, 
            4, 2, 
            electron, 
            5000, 
            "Electron", 
            1, 
            1);
        Console.WriteLine("BOOGIE WOOGIE");
    }
    Raylib.EndDrawing();
}