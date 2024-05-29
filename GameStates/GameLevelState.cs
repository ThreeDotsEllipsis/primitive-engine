using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TowerDefense;

public class GameLevelState : GameState
{
    WalkPath walkPath;
    WaveManager waveManager;
    List<Button> skipButtons;

    public override void LoadContent(ContentManager contentManager)
    {
        skipButtons = new();
        walkPath = new();
        waveManager = new(null, walkPath);

        waveManager.OnNextWave += HandleNextWave;
        waveManager.OnWaveUpdate += HandleWaveUpdate;

        walkPath.Initialize();
        waveManager.Initialize();

        foreach (var node in walkPath.GetStartNodes())
        {
            var source = new Rectangle(0, 0, 60, 60);
            var texture = DebugTexture.GenerateCircleTexture(source.Width, Color.Gray);
            var skipWaveTime = new Button(null, "0", node.Position, 1f, texture, source, Rectangle.Empty, null);
            skipWaveTime.Interact.OnClick += HandleSkipWave;

            skipButtons.Add(skipWaveTime);
            AddGameObject(skipWaveTime);
        }

        LoadLevel("level_editor");

        AddGameObject(waveManager);
    }

    private void HandleWaveUpdate(object sender, int waveNumber)
    {
        foreach (var button in skipButtons)
        {
            button.Sprite.Hidden = true;
            button.Interact.Disabled = true;
            button.Label.TextColor = Color.Transparent;
        }
    }

    private void HandleNextWave(object sender, EventArgs args)
    {
        foreach (var button in skipButtons)
        {
            button.Sprite.Hidden = false;
            button.Interact.Disabled = false;
            button.Label.TextColor = Color.White;
        }
    }

    private void HandleSkipWave(object sender, EventArgs args)
    {
        waveManager.SkipWait();
    }

    public void LoadLevel(string filename)
    {
        // TODO dont have to do this anymore
        foreach (var gameObject in MetaManager.LoadLevelEditor(filename))
        {
            AddGameObject(gameObject);
            if (gameObject is TowerPlot plot)
            {
                plot.OnTowerSelect += HandleTowerSelect;
            }
        }
    }

    // TODO this as well
    private void HandleTowerSelect(object sender, TowerType type)
    {
        var plot = (TowerPlot)sender;

        if (type == TowerType.Archer)
        {
            var tower = new ArcherTower(null, plot, walkPath, plot.WorldPosition, plot.Scale);
            AddGameObject(tower);
        }
    }

    public override void UnloadContent(ContentManager contentManager)
    {
        AssetManager.UnloadAssets();
    }

    public override void HandleInput()
    {
        var state = Keyboard.GetState();

        if (Input.IsKeyJustPressed(Keys.Escape))
        {
            SwitchState(new WorldMapState());
        }

        base.HandleInput();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (var button in skipButtons)
        {
            if (!button.Sprite.Hidden)
            {
                button.Label.Text = ((int)waveManager.WaveTimer.TimeLeft).ToString();
            }
        }
    }
}