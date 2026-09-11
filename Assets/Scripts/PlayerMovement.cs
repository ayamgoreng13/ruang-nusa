using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movement;
    Vector2 lastMove;

    void Update()
    {
        // 1. Ambil input murni dari keyboard
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 2. Jika ada input diagonal (jalan miring)
        if (movement.x != 0 && movement.y != 0)
        {
            // Utamakan arah Kiri/Kanan, abaikan animasi Atas/Bawah
            movement.y = 0;
        }

        // 3. Cek pergerakan & kirim parameter ke Animator
        if (movement != Vector2.zero)
        {
            lastMove = movement;

            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        // 4. Kirim arah terakhir ke Idle Blend Tree
        animator.SetFloat("LastMoveX", lastMove.x);
        animator.SetFloat("LastMoveY", lastMove.y);
    }

    void FixedUpdate()
    {
        // Tetap gunakan input fisik asli agar pergerakan diagonal jalannya tetap miring secara fisik
        Vector2 physicsInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.MovePosition(rb.position + physicsInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}