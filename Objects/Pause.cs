using Godot;

namespace Labyrinth.Objects
{
	public class Pause : Control
	{
		public override void _Input(InputEvent @event)
		{
			if(Input.IsActionJustPressed("esc"))
			{
				GetTree().Paused = !GetTree().Paused;
				Visible = GetTree().Paused;
			}
		}
	}
}
