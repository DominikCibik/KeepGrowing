using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum MovementType { Horizontal, Vertical, DiagonalBounce, Random, Circular }
    public MovementType movementType;

    public float speed = 2f;
    private Vector2 direction;

    private float randomMoveTimer;
    private Vector2 circularCenter;
    private float angle;
    public float sizeIncrease = 0.1f;
    public float sizeDecreaseFactor = 0.5f;

    private float screenLeft = -8f;
    private float screenRight = 8f;
    private float screenTop = 4f;
    private float screenBottom = -4f;

    private int foodCount = 0; // Počet jedla, ktoré nepriateľ zjedol

    void Start()
    {
        switch (movementType)
        {
            case MovementType.Horizontal:
                direction = Vector2.left;
                break;
            case MovementType.Vertical:
                direction = Vector2.down;
                break;
            case MovementType.DiagonalBounce:
                direction = new Vector2(1, 1).normalized;
                break;
            case MovementType.Random:
                SetRandomDirection();
                break;
            case MovementType.Circular:
                circularCenter = transform.position;
                angle = 0f;
                break;
        }
    }

    void Update()
    {
        switch (movementType)
        {
            case MovementType.Horizontal:
                MoveHorizontal();
                break;
            case MovementType.Vertical:
                MoveVertical();
                break;
            case MovementType.DiagonalBounce:
                MoveDiagonalBounce();
                break;
            case MovementType.Random:
                MoveRandom();
                break;
            case MovementType.Circular:
                MoveCircular();
                break;
        }

        // Skontroluj, či je nepriateľ mimo obrazovky
        CheckOutOfBounds();
    }

    // --- Pohyby ---
    void MoveHorizontal()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (transform.position.x < screenLeft || transform.position.x > screenRight)
        {
            direction.x *= -1f;  // Odrazí sa na opačnú stranu
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, screenLeft, screenRight), transform.position.y);
        }
    }

    void MoveVertical()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (transform.position.y < screenBottom || transform.position.y > screenTop)
        {
            direction.y *= -1f;  // Odrazí sa na opačnú stranu
            transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, screenBottom, screenTop));
        }
    }

    void MoveDiagonalBounce()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        Vector2 pos = transform.position;

        if (pos.x < screenLeft || pos.x > screenRight)
            direction.x *= -1;
        if (pos.y < screenBottom || pos.y > screenTop)
            direction.y *= -1;
    }

    void MoveRandom()
    {
        randomMoveTimer -= Time.deltaTime;
        if (randomMoveTimer <= 0f)
        {
            SetRandomDirection();
            randomMoveTimer = Random.Range(1f, 3f);
        }

        transform.Translate(direction * speed * Time.deltaTime);
    }

    void SetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void MoveCircular()
    {
        angle += speed * Time.deltaTime;
        float radius = 2f;
        float x = circularCenter.x + Mathf.Cos(angle) * radius;
        float y = circularCenter.y + Mathf.Sin(angle) * radius;
        transform.position = new Vector2(x, y);
    }

    // --- Kontrola, či nepriateľ nie je mimo obrazovky a odrazí sa ---
    void CheckOutOfBounds()
    {
        if (transform.position.x < screenLeft || transform.position.x > screenRight)
        {
            direction.x *= -1f;  // Odrazí sa na opačnú stranu
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, screenLeft, screenRight), transform.position.y);
        }

        if (transform.position.y < screenBottom || transform.position.y > screenTop)
        {
            direction.y *= -1f;  // Odrazí sa na opačnú stranu
            transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, screenBottom, screenTop));
        }
    }

    // --- Kolízia s jedlom, hráčom a inými nepriateľmi ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            // Zvýši sa veľkosť nepriateľa pri zjedení jedla
            transform.localScale += new Vector3(0.1f, 0.1f, 0);
            Destroy(other.gameObject);  // Zničí jedlo
            foodCount++;  // Pripočíta počet zjedeného jedla
        }
        else if (other.CompareTag("Laxative"))
        {
            transform.localScale *= sizeDecreaseFactor;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            float mySize = transform.localScale.x;
            float otherSize = other.transform.localScale.x;

            if (Mathf.Approximately(mySize, otherSize))
                return;

            if (mySize > otherSize)
            {
                transform.localScale += new Vector3(0.1f, 0.1f, 0);  // Zvýši sa veľkosť pri zjedení menšieho nepriateľa
                Destroy(other.gameObject);  // Zničí menšieho nepriateľa alebo hráča
            }
            else
            {
                other.transform.localScale += new Vector3(0.1f, 0.1f, 0);  // Druhý sa zväčší pri zjedení
                Destroy(gameObject);  // Tento nepriateľ je zničený
            }
        }
    }
}

