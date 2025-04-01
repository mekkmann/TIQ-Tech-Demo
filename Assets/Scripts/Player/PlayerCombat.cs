using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _playerMovement;

    [SerializeField] private AnimationClip _lightAttackClip;

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
        _playerMovement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
    }

    private void LightAttack(InputAction.CallbackContext context)
    {
        if (_playerMovement.IsRolling) return;

        // turn off movement for attack
        _playerMovement.StopMoving(_lightAttackClip.length);

        _animator.SetTrigger("lightAttack");
    }
}
