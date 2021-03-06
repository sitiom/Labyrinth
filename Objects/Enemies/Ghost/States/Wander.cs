using Godot;
using System;

namespace Labyrinth.Objects.Enemies.Ghost.States
{
	public class Wander : State
	{
		[Export] public int Speed;

		private Vector2 _direction;
		private Timer _wanderTimer;
		private Sprite _sprite;
		private readonly Random _random = new Random();

		public override void _Ready()
		{
			_wanderTimer = GetNode<Timer>("WanderTimer");
		}

		public override void Enter(KinematicBody2D host)
		{
			_sprite = GetParent().GetParent().GetNode<Sprite>("Sprite");
			host.GetNode<AnimationPlayer>("AnimationPlayer").Play("float");
			_wanderTimer.Start();
			_direction.x = _random.Next(-50, 50);
			_direction.y = _random.Next(-50, 50);
		}

		public override void Update(KinematicBody2D host, float delta)
		{
			_sprite.FlipH = _direction.x >= 0;
			host.MoveAndSlide(_direction.Normalized() * Speed);
		}

		private void _on_WanderTimer_timeout()
		{
			_wanderTimer.Stop();
			EmitSignal(nameof(Finished), "Wander");
		}
	}
}
