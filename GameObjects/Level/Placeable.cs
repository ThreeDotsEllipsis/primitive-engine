using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TowerDefense;

public class Placeable : GameObject
{
    public Sprite Sprite { get; }
    public CollisionShape Shape { get; }
    public Interact Interact { get; }

    public bool FollowMouse { get; set; }
    public Type Type { get; protected set; }

    public Placeable(GameObject parent, Sprite sprite, Type type, Vector2 position, float scale) : base(parent)
    {
        Sprite = new Sprite(this, sprite.Texture, sprite.SourceRectangle, 2);
        Shape = new CollisionShape(this, Sprite.Size);
        Interact = new Interact(this, Sprite, Shape);

        Type = type;
        LocalPosition = position;
        LocalScale = scale;
    }

    public override void HandleInput()
    {
        if (EditLevelState.EditState != EditState.LevelEditor) return;

        base.HandleInput();

        var keyState = Keyboard.GetState();

        if (Interact.IsSelected)
        {
            if (Input.IsKeyJustPressed(Keys.D))
            {
                QueueFree();
            }
            if (Input.IsKeyJustPressed(Keys.F))
            {
                FollowMouse = true;
            }

            if (keyState.IsKeyDown(Keys.LeftShift))
            {
                LocalScale += Input.GetWheelValue() / 1000f;
                LocalScale = Math.Clamp(Scale, 0.1f, 10f);
            }
        }

        if (FollowMouse && Input.IsMouseJustPressed(MouseButton.Left))
        {
            FollowMouse = false;
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var mouseState = Mouse.GetState();

        if (FollowMouse)
        {
            LocalPosition = mouseState.Position.ToVector2();
        }
    }

    public Placeable Clone()
    {
        return new Placeable(Parent, Sprite, Type, WorldPosition, Scale);
    }
}