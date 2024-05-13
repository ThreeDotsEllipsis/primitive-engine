
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TowerDefense;

class GridItem : GameObject
{
    public event EventHandler<Type> OnSelect;

    private Selectable _selectable;
    private Type _type;

    public GridItem(GameObject gameObject, Vector2 position, float scale)
    {
        _type = gameObject.GetType();
        _selectable = new Selectable(position, scale, 2, gameObject.Texture, gameObject.SourceRectangle, gameObject.SourceRectangle);
        _selectable.OnSelect += HandleSelect;
    }

    public void HandleSelect(object sender, EventArgs args)
    {
        OnSelect?.Invoke(this, _type);
    }

    public override void HandleInput()
    {
        _selectable.HandleInput();
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphicsDevice)
    {
        _selectable.Draw(spriteBatch, graphicsDevice);
    }
}

class Grid : GameObject
{
    private List<GridItem> _items;

    public Vector2 Size { get; protected set; }
    public int ColumnAmount { get; protected set; }
    public float SizePerItem { get; protected set; }
    public float Gap { get; protected set; }

    public Grid(Vector2 size, int columnAmount, float gap)
    {
        Size = size;
        ColumnAmount = columnAmount;
        SizePerItem = Size.X / columnAmount;
        Gap = gap;

        _items = new();
    }

    public Vector2 GetPosition()
    {
        var position = new Vector2(0, 0);
        position.X = (_items.Count % ColumnAmount) * SizePerItem;
        position.Y = (_items.Count / ColumnAmount) * SizePerItem;

        position += new Vector2(SizePerItem, SizePerItem) / 2f;

        return position;
    }

    public void AddItem(GameObject gameObject)
    {
        var wideSide = Math.Max(gameObject.SourceRectangle.Width, gameObject.SourceRectangle.Height);

        var fraction = 1 + Gap / SizePerItem;

        var scale = SizePerItem / (float)wideSide / fraction;
        var gridItem = new GridItem(gameObject, GetPosition(), scale);

        _items.Add(gridItem);
    }

    public override void HandleInput()
    {
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphicsDevice)
    {
        foreach (var item in _items)
        {
            item.Draw(spriteBatch, graphicsDevice);
        }
    }
}