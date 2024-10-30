using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;
namespace Input {
    [CreateAssetMenu(fileName = "Input Processor", menuName = "Scriptable Objects/Input Processor")]
    public class PlayerInputProcessor : ScriptableObject, IFightActions, IPassiveActions {
        public PlayerInputActions InputActions { get; private set; }

        public event Action OnUpPressed;
        public event Action OnDownPressed;
        public event Action OnLeftPressed;
        public event Action OnRightPressed;
        public event Action<Vector2> OnMovePressed;
        public event Action OnCastPressed;

        void OnEnable() {
            InputActions = new();
            InputActions.Fight.SetCallbacks(this);
            InputActions.Passive.SetCallbacks(this);
            InputActions.Enable();
        }

        void OnDisable() {
            DisableInputs();
            InputActions.Disable();
        }
        public void DisableInputs() {
            Debug.Log("PlayerInputProcessor: Disabling actions");
            InputActions.Fight.Disable();
            InputActions.Passive.Disable();
        }
        public void SetFight() {
            DisableInputs();
            Debug.Log("PlayerInputProcessor: Fight actions enabled");
            InputActions.Fight.Enable();
        }

        public void SetPassive() {
            DisableInputs();
            Debug.Log("PlayerInputProcessor: Fight actions enabled");
            InputActions.Passive.Enable();
        }

        public void OnUp(InputAction.CallbackContext context) {
            if (context.performed) OnUpPressed?.Invoke();
        }
        public void OnDown(InputAction.CallbackContext context) {
            if (context.performed) OnDownPressed?.Invoke();
        }
        public void OnLeft(InputAction.CallbackContext context) {
            if (context.performed) OnLeftPressed?.Invoke();
        }
        public void OnRight(InputAction.CallbackContext context) {
            if (context.performed) OnRightPressed?.Invoke();
        }
        public void OnPrimaryCast(InputAction.CallbackContext context) {
            if (context.performed) OnCastPressed?.Invoke();
        }
        public void OnSecondaryCast(InputAction.CallbackContext context) {
            if (context.performed) OnCastPressed?.Invoke();
        }
        public void OnMove(InputAction.CallbackContext context) {
            if (context.performed) OnMovePressed?.Invoke(context.ReadValue<Vector2>());
        }
    }
}
