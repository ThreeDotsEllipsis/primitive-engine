using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TowerDefense;

class TowerPlot : GameObject
{
    public event EventHandler OnTowerSelect;

    private Selectable _selectable;

    override public float Scale { get { return _selectable.Scale; } }
    override public Rectangle SourceRectangle { get { return _selectable.SourceRectangle; } }

    public bool Disabled { get; set; }

    override public Vector2 WorldPosition
    {
        get { return _selectable.WorldPosition; }
        set
        {
            _selectable.WorldPosition = value;
        }
    }

    public TowerPlot(Vector2 position, float scale)
    {
        var plot = AssetManager.GetAsset<Texture2D>("Sprites/LevelSheet");
        var plotSource = new Rectangle(495, 635, 110, 50);

        _selectable = new Selectable(position, scale, 2, plot, plotSource, plotSource);
        _selectable.OnDoubleSelect += HandleSelection;
    }

    public void HandleSelection(object sender, EventArgs args)
    {
        OnTowerSelect?.Invoke(this, null);
    }

    public override void HandleInput()
    {
        if (Disabled) return;

        _selectable.HandleInput();
    }

    public override void Update(GameTime gameTime)
    {
        if (Disabled) return;

        _selectable.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphicsDevice)
    {
        _selectable.Draw(spriteBatch, graphicsDevice);
    }
}