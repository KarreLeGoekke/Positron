using Raylib_cs;
using System;
using System.Numerics;

class PlayerController {
    public float speed;
    public float smooth;

    // Positionsvariabler
    public float X;
    public float Y;

    // Riktningsvariabler
    private float xDir = 0;
    private float yDir = 0;

    private Entity self;
    private bool attackDebounce = false;
    private float attackI = 50;
    private float sizeX;
    private float sizeY;
    public PlayerController(Entity entity, float posX, float posY)
    {
        entity.position.X = posX;
        entity.position.Y = posY;
        sizeX = entity.size.X;
        sizeY = entity.size.Y;
        X = posX;
        Y = posY;
        self = entity;
        speed = entity.speed;
    }

    public void Update(){
        if (self.alive == false) self.Destroy(); // Self-destruktion om spelaren är död

        // Rörelse

        xDir = Lambda.lerp(xDir, Lambda.getAxis("Horizontal"), smooth);
        yDir = Lambda.lerp(yDir, Lambda.getAxis("Vertical"), smooth);
        
        Vector2 movement = new Vector2(xDir, yDir); // Direktionsvektor

        if(movement.Length() > 1f) movement = Vector2.Normalize(movement); // Normalisera riktningsvektor (Så att det går inte snabbare när det gåt diagonellt)

        X += movement.X * speed;
        Y += movement.Y * speed;

        // Attack

        if (Raylib.IsKeyDown(KeyboardKey.KEY_ENTER) && attackDebounce == false) {
            Vector2 prePos = new Vector2(X + 32, Y + 32);
            X += movement.X * speed * 20;
            Y += movement.Y * speed * 20;
            Vector2 newPos =  new Vector2(X + 32, Y + 32);
            attackDebounce = true;
            cooldown(); // Cooldownsfunktion
            barRecharge(); // Börja proceduren
            Lambda.AttackEvent(prePos, newPos);

            async void cooldown()
            {
                await Task.Delay(5000);
                attackDebounce = false;
            }

            async void barRecharge()
            {
                attackI = 0;
                System.Timers.Timer t = new System.Timers.Timer();
                t.Interval = 10;
                t.Elapsed += OnAttackTimedEvent; // Aktivera funktionen varje gång timern ger ut ett intervall
                t.AutoReset = true;
                t.Enabled = true;

                await Task.Delay(5001); // Köra efter 5 sekunder
                t.Enabled = false;
                t.Dispose();
            }
        }

        void OnAttackTimedEvent(Object source, System.Timers.ElapsedEventArgs e){
            if (attackI < 50) attackI++; // Om indexet är inte över 50 går det upp med 1 steg
        }

        // Rörelse final

        // Låt inte X & Y axeln gå ut från fönstern
        X = Math.Clamp(X, 0, 896);
        Y = Math.Clamp(Y, 0, 576);

        self.position.X = X;
        self.position.Y = Y;
    }

    public dynamic GetStatus(string status)
    {
        if (status == "attackDebounce") return attackDebounce;
        if (status == "attackDebounceTimer") return attackI;

        return false;
    }
}