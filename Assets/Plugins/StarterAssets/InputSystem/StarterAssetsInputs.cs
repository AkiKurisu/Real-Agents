using UnityEngine;
using UnityEngine.InputSystem;
namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnFocus(InputValue value)
		{
			if (value.isPressed)
			{
				SetCursorState(cursorLocked = !cursorLocked);
			}
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			if (!cursorLocked) return;
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			if (!cursorLocked) return;
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			if (!cursorLocked) return;
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			if (!cursorLocked) return;
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

}