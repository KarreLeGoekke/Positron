using Raylib_cs;
using System;
using System.Numerics;

class Entity {
    public bool alive = false;
    public int health = 3;
    public Vector2 position;
    public Vector2 size;
    public int xIndex = 0;
    public int yIndex = 0;
    public bool player;
    public string name = "Entity";
    public float speed;
    public bool locked;
    public bool invincible;

    private Texture2D sprite;
    private int columns;
    private int rows;
    private int sprites;
    private Rectangle source;
    private Rectangle destination;
    private Vector2 centerPos;
    private Color color = Color.WHITE;

    public Entity(int positionX, int positionY, int sizeX, int sizeY, int numX, int numY, Texture2D texture, bool isPlayer, bool invulnerable) {
        sprite = texture;
        sprite.width = sizeX * numX;
        sprite.height = sizeY * numY;
        position.X = positionX;
        position.Y = positionY;
        columns = numX;
        rows = numY;
        size = new Vector2(sizeX / columns, sizeY / rows);
        sprites = numX * numY;
        player = isPlayer;
        alive = true;
        locked = true;
        centerPos = this.position + new Vector2(sizeX / 2, sizeY / 2);

        // Ställa in Spritesheet
        source = new Rectangle(xIndex * sizeX, yIndex * sizeY, sizeX, sizeY);
        destination = new Rectangle(position.X, position.Y, sizeX, sizeY);

        if (player) locked = false; // Om enheten är ett spelare så är det avlåst

        Lambda.Entities.Add(this); // Lägga enheten till listan så det ritas
    }

    public void switchIndex(){ // Varje gång den här funktionen körs växlar sprite-indexet
        xIndex++;
        if (xIndex == columns && rows != 1) { // Om X-indexet är det sista och det finns mer än 1 rad växlar X-indexet till 0 och Y-indexet 1 nummer högre
            yIndex++;
            xIndex = 0;
        }
        else if (xIndex == columns && rows == 1) xIndex = 0; // Men om X-indexet är det sista OCH det finns bara 1 rad växlar bara X-indexet till 0
        else if (xIndex == columns && yIndex == rows) { // Om både X-indexet och Y-indexet är det sista växlar både av dem till 0
            yIndex = 0;
            xIndex = 0;
        }
    }

    public void Draw() {
        switchIndex(); // Växla index

        if (!alive) return; // Om det är död, rita inte

        source.x = source.width * xIndex;
        source.y = source.height * yIndex;

        destination.x = position.X;
        destination.y = position.Y;

        centerPos = this.position + new Vector2(size.X / 2, size.Y / 2); // Få mellanpunkten av enheten

        Raylib.DrawTexturePro(sprite, source, destination, new Vector2(0, 0), 0f, color); // Rita enheten

        if (Lambda.debugMode == true) {
            Raylib.DrawText($"{xIndex + 1}", (int)destination.width + (int)destination.x + 4, (int)destination.height + (int)destination.y + 4, 24, Color.BLACK);
            Raylib.DrawText($"{name}\nHealth: {health}", (int)destination.x - 4, (int)destination.height + (int)destination.y + 4, 8, Color.BLACK);

            // Debug Mode för Enhet
            
            // Det skriver vilken X-index enheten är på
            // Det också skriver hälsan
        }

        if (health == 0) Destroy(); // Om enheten har ingen hälsa dör det
    }

    public void Follow(Vector2 pos){
        Vector2 direction = Vector2.Normalize(pos - this.position); // Få riktningen så att enheten kan "titta" vektoren.
    
        this.position = this.position + direction * this.speed; // Positionen ska bli bytas så att det går närmare till vektoren.

        if (Lambda.debugMode == true) {
            Raylib.DrawLineEx(centerPos, centerPos + direction * 100, 2, Color.RED); // Det skriver ett linje där det visar riktningen
        }
    }

    public async void Damage(int amount){
        if (this.invincible) return; // Om det är osårbar händer ingenting 
        Lambda.debugPrint("Caused " + amount + " damage"); // Debug mode: Konsolen skriver ut hur mycket hälsa har tagits bort från enheten
        this.health -= amount;
        this.invincible = true;
        color = Color.LIGHTGRAY; // Färgen byter så att det visar att det är osårbar för 2 sekunder

        await Task.Delay(2000);
        this.invincible = false;
        color = Color.WHITE; // Färgen byter till sitt originalfärg så att det visar att det har slutat vara osårbar
    }

    public void Destroy(){
        alive = false; // Döda det
        Lambda.Entities.Remove(this); // Ta bort enheten från listan
        Lambda.debugPrint($"Removed entity: {this.name}"); // Debug mode: Visa att enheten är 100% tagits bort
    }
}