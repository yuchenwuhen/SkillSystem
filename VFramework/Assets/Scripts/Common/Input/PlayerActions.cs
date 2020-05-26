namespace VFramework.Common
{
	using InControl;
	using UnityEngine;


	public class PlayerActions : PlayerActionSet
	{
		public PlayerAction Fire;
		public PlayerAction Rush;

		public PlayerAction Left;
		public PlayerAction Right;
		public PlayerAction Up;
		public PlayerAction Down;
		public PlayerTwoAxisAction Move;

        public PlayerAction MouseLeft;
        public PlayerAction MouseRight;
        public PlayerAction MouseUp;
        public PlayerAction MouseDown;
        public PlayerTwoAxisAction MouseMove;


        public PlayerActions()
		{
			Fire = CreatePlayerAction( "Fire" );
            Rush = CreatePlayerAction("Rush");

			Left = CreatePlayerAction( "Move Left" );
			Right = CreatePlayerAction( "Move Right" );
			Up = CreatePlayerAction( "Move Up" );
			Down = CreatePlayerAction( "Move Down" );
			Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );

            MouseLeft = CreatePlayerAction("Mouse Left");
            MouseRight = CreatePlayerAction("Mouse Right");
            MouseUp = CreatePlayerAction("Mouse Up");
            MouseDown = CreatePlayerAction("Mouse Down");
            MouseMove = CreateTwoAxisPlayerAction(MouseLeft, MouseRight, MouseDown, MouseUp);
        }


		public static PlayerActions CreateWithDefaultBindings()
		{
			var playerActions = new PlayerActions();

			// How to set up mutually exclusive keyboard bindings with a modifier key.
			// playerActions.Back.AddDefaultBinding( Key.Shift, Key.Tab );
			// playerActions.Next.AddDefaultBinding( KeyCombo.With( Key.Tab ).AndNot( Key.Shift ) );

			playerActions.Fire.AddDefaultBinding( Key.Space );
			playerActions.Fire.AddDefaultBinding( InputControlType.Action1 );
			playerActions.Fire.AddDefaultBinding( Mouse.LeftButton );

			playerActions.Rush.AddDefaultBinding( Key.Space );
			playerActions.Rush.AddDefaultBinding( InputControlType.Action3 );
			playerActions.Rush.AddDefaultBinding( InputControlType.Back );

			playerActions.Up.AddDefaultBinding( Key.UpArrow );
			playerActions.Down.AddDefaultBinding( Key.DownArrow );
			playerActions.Left.AddDefaultBinding( Key.LeftArrow );
			playerActions.Right.AddDefaultBinding( Key.RightArrow );

            playerActions.Up.AddDefaultBinding(Key.W);
            playerActions.Down.AddDefaultBinding(Key.S);
            playerActions.Left.AddDefaultBinding(Key.A);
            playerActions.Right.AddDefaultBinding(Key.D);

            playerActions.MouseUp.AddDefaultBinding(Mouse.PositiveY);
            playerActions.MouseDown.AddDefaultBinding(Mouse.NegativeY);
            playerActions.MouseLeft.AddDefaultBinding(Mouse.NegativeX);
            playerActions.MouseRight.AddDefaultBinding(Mouse.PositiveX);

            playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
			playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );
			playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
			playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );

			playerActions.Left.AddDefaultBinding( InputControlType.DPadLeft );
			playerActions.Right.AddDefaultBinding( InputControlType.DPadRight );
			playerActions.Up.AddDefaultBinding( InputControlType.DPadUp );
			playerActions.Down.AddDefaultBinding( InputControlType.DPadDown );

			playerActions.ListenOptions.IncludeUnknownControllers = true;
			playerActions.ListenOptions.MaxAllowedBindings = 4;
			//playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
			//playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
			playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
			//playerActions.ListenOptions.IncludeMouseButtons = true;
			//playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
			//playerActions.ListenOptions.IncludeMouseButtons = true;
			//playerActions.ListenOptions.IncludeMouseScrollWheel = true;

			playerActions.ListenOptions.OnBindingFound = ( action, binding ) => {
				if (binding == new KeyBindingSource( Key.Escape ))
				{
					action.StopListeningForBinding();
					return false;
				}
				return true;
			};

			playerActions.ListenOptions.OnBindingAdded += ( action, binding ) => {
				Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
			};

			playerActions.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
				Debug.Log( "Binding rejected... " + reason );
			};

			return playerActions;
		}
	}
}
