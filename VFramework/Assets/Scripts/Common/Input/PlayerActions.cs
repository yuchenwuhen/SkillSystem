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

        public PlayerAction Left3D;
        public PlayerAction Right3D;
        public PlayerAction Front3D;
        public PlayerAction Back3D;
        public PlayerTwoAxisAction Move3D;

        public PlayerAction Jump;


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

            Left3D = CreatePlayerAction("Move Left 3D");
            Right3D = CreatePlayerAction("Move Right 3D");
            Front3D = CreatePlayerAction("Move Up 3D");
            Back3D = CreatePlayerAction("Move Down 3D");
            Move3D = CreateTwoAxisPlayerAction(Left3D, Right3D, Front3D, Back3D);

            Jump =  CreatePlayerAction("Jump");
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

            playerActions.Left3D.AddDefaultBinding(InputControlType.DPadLeft);
            playerActions.Left3D.AddDefaultBinding(Key.A);
            playerActions.Right3D.AddDefaultBinding(InputControlType.DPadRight);
            playerActions.Right3D.AddDefaultBinding(Key.D);
            playerActions.Front3D.AddDefaultBinding(InputControlType.DPadUp);
            playerActions.Front3D.AddDefaultBinding(Key.W);
            playerActions.Back3D.AddDefaultBinding(InputControlType.DPadDown);
            playerActions.Back3D.AddDefaultBinding(Key.S);

            playerActions.Jump.AddDefaultBinding(Key.Space);
            playerActions.Jump.AddDefaultBinding(InputControlType.Action1);

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
