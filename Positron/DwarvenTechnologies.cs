using Raylib_cs;
using System;
using System.Numerics;

static class Lambda
{
    // Variabler som alla scriptar kan läsa och skriva på
    public static bool debugMode = false;
    public static List<Entity> Entities = new List<Entity>();
    public static List<Tuple<Vector2, Vector2>> DebugCollisionLines = new List<Tuple<Vector2, Vector2>>();
    public static Entity player;

    public static float lerp(float min, float max, float alpha){
        return (min + max) / 2;
    }

    public static int getAxis(string direction)
    {
        if (direction == "Horizontal")
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
                return 1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
                return -1;
            }
        }
        else if (direction == "Vertical")
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) {
                return 1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) {
                return -1;
            }
        }
        return 0;
    }

    public static unsafe bool CheckCollisionRectangleLine(Rectangle rec, Vector2 p1, Vector2 p2)
    {
        // Få 4 punkter av ett rektangel

        Vector2 pb1 = new Vector2(rec.x, rec.y);
        Vector2 pb2 = new Vector2(rec.x + rec.width, rec.y);
        Vector2 pb3 = new Vector2(rec.x, rec.y + rec.height);
        Vector2 pb4 = new Vector2(rec.x + rec.width, rec.y + rec.height);

        // Skapa ett collision point (värdelöst)
        Vector2 vec2 = new Vector2(0, 0);
        Vector2* cp = &vec2;

        // Kontrollera om linjen kolliderar med en av kanterna
        bool onetwocontact = Raylib.CheckCollisionLines(p1, p2, pb1, pb2, cp);
        bool twothreecontact = Raylib.CheckCollisionLines(p1, p2, pb2, pb3, cp);
        bool threefourcontact = Raylib.CheckCollisionLines(p1, p2, pb3, pb4, cp);
        bool fouronecontact = Raylib.CheckCollisionLines(p1, p2, pb4, pb1, cp);

        // Om det görs returnerar det sant, men om inte returnerar det falskt då
        if (onetwocontact == true || twothreecontact == true || threefourcontact == true || fouronecontact == true) return true;
        else return false;
    }

    public static void AttackEvent(Vector2 startPos, Vector2 endPos){
        debugPrint($"Received signal with Vectors {startPos} and {endPos}"); // Debug Mode: Skriva ut att det har fått signaler från spelaren att det ska attackera
        DebugCollisionLines.Add(Tuple.Create<Vector2, Vector2>(startPos, endPos)); // Debug Mode: Skapa ett linje där det visar vart man har attackerad
        foreach(Entity entity in Entities) { // För varje enhet in enhetslistan...
            if (entity.player == true) continue; // Om enheten är spelaren skippar det till den nästa enhet (iteration)
            bool collided = CheckCollisionRectangleLine(new Rectangle(entity.position.X, entity.position.Y, entity.size.X, entity.size.Y), startPos, endPos); // Titta om attacklinjen kolliderar med denna enheten

            if (collided) {
                debugPrint($"Collided with {entity.name}!"); // Debug mode: Skriver ut vilken enhet linjen har kolliderade med
                entity.Damage(1); // Skada enheten
            }
        }
    }

    public static void Update(){
        if (debugMode) {
            foreach(Tuple<Vector2, Vector2> tuple in DebugCollisionLines) { // Debug Mode: För varje linje som är på listan...
                Raylib.DrawLineEx(tuple.Item1, tuple.Item2, 2, Color.RED); // Debug Mode: Skriv ett linje som går från det första vektoren till den andra vektoren (koordinater för attacklinjen)
            }
        }

        for(int i = 0; i < Entities.Count(); i++){ // För varje enhet på enhetslistan...
            if (Entities[i].player == false && Entities[i].locked == false) Entities[i].Follow(player.position); // Om enheten är inte ett spelare och är inte låst så följer det spelaren

            Entities[i].Draw(); // Rita enheten
        }
    }

    private static bool spawnDebounce = false; // Bara så att det spawnar inte varje millisekund
    public async static void SpawnEntity(int positionX, int positionY, int sizeX, int sizeY, int numX, int numY, Texture2D texture, int cooldown, string name, int health, int waitTime){
        if (spawnDebounce) return; // Om debouncet är aktivt spawnar det inte

        spawnDebounce = true; // Aktivera debouncet
        await Task.Delay(cooldown); // Vänta for ett stund
        spawnDebounce = false; // Avaktivera debouncet
        Entity entity = new Entity(positionX, positionY, sizeX, sizeY, numX, numY, texture, false, true); // Skapa enheten
        entity.speed = new Random().Next(5, 15); // Ställ in hastigheten

        // Skapa namnet, samt med nummret
        int integer = Entities.FindIndex(entity => entity.name == name);
        if (integer >= 0) entity.name = $"{name} {integer}";
        else entity.name = name;

        entity.health = health;
        debugPrint($"Spawned entity: {entity.name}"); // Debug Mode: Skriva ut namnet av enheten som var spawnade

        await Task.Delay(waitTime); // När denna timern är klart då är det inte osårbar och låst
        entity.locked = false;
        entity.invincible = false;
    }

    public static void debugPrint(string text){
        if (debugMode) Console.WriteLine($"LAMBDA DEBUG: {text}"); // Debug Mode: Självförklarande, det skriver utt medellanden.
    }
}