using System.Collections.Generic;
using Labyrinth.Objects.Enemies.Ghost.States;
using Godot;

namespace Labyrinth.Objects.Enemies.Ghost
{
	public class Entity : KinematicBody2D
	{
		[Signal]
		public delegate void StateChanged();

		public State CurrentState;
		public Stack<State> StateStack = new Stack<State>();
		public readonly Dictionary<string, Node> StatesMap = new Dictionary<string, Node>();
		private KinematicBody2D _player;
		private bool _canChase;

		public override void _Ready()
		{
			StatesMap.Add("Chase", GetNode("States/Chase"));
			StatesMap.Add("Wander", GetNode("States/Wander"));
			StatesMap.Add("Weakened", GetNode("States/Weakened"));

			CurrentState = (Wander)GetNode("States/Wander");

			_player = GetParent().GetParent().GetNode<KinematicBody2D>("Player");

			foreach (Node state in StatesMap.Values)
			{
				state.Connect(nameof(State.Finished), this, nameof(ChangeState));
			}

			StateStack.Push((State)StatesMap["Wander"]);
			ChangeState("Wander");
		}

		public override void _PhysicsProcess(float delta)
		{
			CurrentState.Update(this, delta);
		}
		private void ChangeState(string stateName)
		{
			CurrentState.Exit(this);

			if (stateName == "Dead")
			{
				QueueFree();
				return;
			}

			else if (stateName == "Weakened")
			{
				StateStack.Push((State)StatesMap[stateName]);
			}

			else
			{
				StateStack.Pop();
				StateStack.Push((State)StatesMap[stateName]);
			}

			CurrentState = StateStack.Peek();

			// Pass target to Chase State
			if (stateName == "Chase")
			{
				((Chase)CurrentState).Init((Player.Entity)_player);
			}
			//Pass target to Weakened State
			if (stateName == "Weakened")
			{
				((Weakened)CurrentState).Init((Player.Entity)_player);
			}
			// We don"t want to reinitialize the state if we"re going back to the previous state
			if (stateName != "Previous")
				CurrentState.Enter(this);

			EmitSignal(nameof(StateChanged), CurrentState.Name);
		}

		private void _on_Area2D_body_entered(KinematicBody2D body)
		{
			if (body.IsInGroup("player"))
			{
				_canChase = true;
				GetNode<Timer>("States/Wander/WanderTimer").Stop();
				ChangeState("Chase");
			}
		}

		private void _on_Area2D_body_exited(KinematicBody2D body)
		{
			if (body.IsInGroup("player"))
			{
				_canChase = false;
				ChangeState("Wander");
			}
		}

		private void _on_Hitbox_area_entered(Area2D area)
		{
			if (area.IsInGroup("light"))
			{
				ChangeState("Weakened");
			}
		}

		private void _on_Hitbox_area_exited(Area2D area)
		{
			if (area.IsInGroup("light") == false && !_canChase) ChangeState("Wander");
			else ChangeState("Chase");
		}
	}
}
