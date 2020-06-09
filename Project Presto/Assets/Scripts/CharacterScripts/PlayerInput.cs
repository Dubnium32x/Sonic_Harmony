using UnityEngine;

[System.Serializable]
// The four cardinal directions for input. 
public enum CardinalDirection {
		none
        , right
        , left
        , up
        , down
}
    
    // Actions the player may take. 
public enum ActionUpDown{
		none
        , actionUp
        , actionDown 
}

public enum Action
{
	none
	, some
}

public enum ControlLocked {
        locked
        , unlocked 
}
	
public class PlayerInput
{
    // Must use Serialize Field so we can inspect these private strings.
	[SerializeField] private string horizontalName = "Horizontal";
	[SerializeField] private string verticalName = "Vertical";
	[SerializeField] private string actionName = "Action";

	public var horizontal { get; private set; }
	public var vertical { get; private set; }

	public CardinalDirection direction { get; set; } = CardinalDirection.none;
	public ActionUpDown playerAction { get; set } = ActionUpDown.none;
	public Action action { get; set; } = Action.none;
	public ControlLocked control { get; set; } = ControlLocked.unlocked;
    private float unlockTimer;
	
	
	public void InputUpdate()
	{
		UpdateAxises();
		UpdateAction();
	}

	private void UpdateAxises()
	{
		horizontal = control == ControlLocked.unlocked ? Input.GetAxis(horizontalName) : 0;
		vertical = Input.GetAxis(verticalName);  
		action = Action.none;
		
		// Adjust the direction enum based upon the axis.
		if (horizontal > 0)
		{
			direction = CardinalDirection.right;
		}

		if (horizontal < 0)
		{
			direction = CardinalDirection.left;
		}

		if (vertical > 0)
		{
			direction = CardinalDirection.up;
		}

		if (vertical < 0)
		{
			direction = CardinalDirection.down;
		}
	}

	private void UpdateAction()
	{
		if (Input.GetButton(actionName))
		{
			if (action.none)
			{
				action = Action.some;
				// Is the purpose to always have them couched if there is no action?
				playerAction  = ActionUpDown.down;
			}
		}
		else
		{
			if (action.Up)
			{
				action = Action.none;
				playerAction = ActionUpDown.up;
			}
		}
	}

	public void LockHorizontalControl(float time)
	{
		unlockTimer = time;
		control = controlLocked.locked;
	}

	public void UnlockHorizontalControl(float deltaTime)
	{
		if (unlockTimer > 0)
		{
			unlockTimer -= deltaTime;

			if (unlockTimer <= 0)
			{
				unlockTimer = 0;
				control = controlLocked.unlocked;
			}
		}
	}
}
