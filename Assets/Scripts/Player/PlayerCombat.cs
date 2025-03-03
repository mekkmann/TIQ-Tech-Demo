using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Animator _animator;

    #region Input
    private InputAction _lightAttack;
    #endregion

    private void OnEnable()
    {
        PlayerControls temp = GetComponent<Player>().PlayerControls;
        _lightAttack = temp.Player.LightAttack;
        _lightAttack.Enable();
        _lightAttack.performed += LightAttack;
    }
    private void OnDisable()
    {
        _lightAttack.Disable();
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void LightAttack(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("lightAttack");
    }
}
