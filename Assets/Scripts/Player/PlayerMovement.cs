using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private Animator animator;
    private Rigidbody2D rb;
    private InputActions controls;
    private SpriteRenderer spriteRenderer;

    private Vector2 targetPosition;
    
    // Marcador visual
    public GameObject targetMarker;

    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade máxima que o player pode atingir (sobrescreve stats se quiser)")]
    public float terminalVelocity = 5f; 

    [Tooltip("Distância do alvo onde o player começa a frear automaticamente")]
    public float slowingRadius = 1.5f; 

    [Tooltip("Distância mínima para considerar que 'chegou'")]
    public float stopDistance = 0.1f;

    [Header("Inércia")]
    public float acceleration = 40f; 
    public float deceleration = 40f; // Ajuste para ser menor se quiser mais "slide"

    void Awake()
    {
        controls = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = GetComponent<PlayerStats>();
        
        // Inicializa a velocidade terminal com o valor do stats, mas você pode mudar no Inspector
        terminalVelocity = stats.moveSpeed;
        targetPosition = rb.position; // Começa com alvo na posição atual para não andar sozinho
    }

    private void OnEnable()
    {
        controls.PlayerControls.Enable();
    }
    private void OnDisable()
    {
        controls.PlayerControls.Disable();
    }

    void Update()
    {
        CheckInput();
        UpdateMarkerState();
        UpdateAnimator();
        FlipSprite();
    }

    private void CheckInput()
    {
        // Enquanto segura o botão, atualiza o alvo
        if (controls.PlayerControls.Walk.IsPressed())
        {
            SetTargetPosition();
        }
    }

    private void SetTargetPosition()
    {
        Vector2 screenPosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hitInfo.collider != null)
        {
            targetPosition = hitInfo.point;
        }
    }

    private void UpdateMarkerState()
    {
        // Só mostra o marcador se estivermos longe do alvo
        float distance = Vector2.Distance(targetPosition, transform.position);
        
        if (distance > stopDistance)
        {
            targetMarker.transform.position = targetPosition;
            if (!targetMarker.activeSelf) targetMarker.SetActive(true);
        }
        else
        {
            if (targetMarker.activeSelf) targetMarker.SetActive(false);
        }
    }

    private void FlipSprite()
    {
        // Vira o sprite baseado na velocidade real
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0;
        }
    }

    private void UpdateAnimator()
    {
        // Passa a velocidade real para a animação
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void FixedUpdate()
    {
        // 1. Calcular vetor para o alvo
        Vector2 directionToTarget = targetPosition - rb.position;
        float distance = directionToTarget.magnitude;

        // 2. Calcular a Velocidade Desejada (Arrival Logic)
        Vector2 desiredVelocity = Vector2.zero;

        if (distance > stopDistance)
        {
            // Normaliza a direção
            Vector2 directionNormalized = directionToTarget.normalized;

            // Lógica de Chegada Suave:
            // Se estiver dentro do raio de frenagem, a velocidade é proporcional à distância
            if (distance < slowingRadius)
            {
                // Regra de 3: Quanto mais perto, menor a velocidade desejada
                float rampedSpeed = terminalVelocity * (distance / slowingRadius);
                desiredVelocity = directionNormalized * rampedSpeed;
            }
            else
            {
                // Longe do raio de frenagem: Velocidade Máxima
                desiredVelocity = directionNormalized * terminalVelocity;
            }
        }
        else
        {
            // Chegou no destino (ou muito perto)
            desiredVelocity = Vector2.zero;
        }

        // 3. Aplicar Inércia (MoveTowards)
        // Escolhe taxa de aceleração ou desaceleração dependendo se queremos aumentar ou diminuir a velocidade
        bool isAccelerating = desiredVelocity.magnitude > rb.linearVelocity.magnitude;
        float currentRate = isAccelerating ? acceleration : deceleration;

        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredVelocity, currentRate * Time.fixedDeltaTime);
    }
}