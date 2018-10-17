﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Sprites;

namespace BallonsShooter
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : Game
  {
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    // viseur
    SpriteGeneric _viseur;

    // la vague de ballons
    BallonsWave _ballonswave;

    // gestion de la souris
    MouseState _currentMouseState;

    // backgroung texture
    Texture2D backgroundTexture;
    Rectangle mainFrame;

    DisplayText msg_joueur1;
    int score1 = 0;
    
    Song song;


    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);

      // l'application démarre en plein écran
      _graphics.IsFullScreen = true;

      Window.Title = "CFPT Ballons Shooter";

      Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // instantiation du viseur
      _viseur = new SpriteGeneric(this);

      // instantiation des ballons
      _ballonswave = new BallonsWave(this, 500);

      // instantiation des messages
      msg_joueur1 = new DisplayText(this);

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // chargement de la texture pour le viseur
      _viseur.LoadContent("viseur");

      // positionnement initial du viseur : centre de l'écran
      _viseur.SetPosition(SpriteGeneric.ViewportPosition.CENTER);

      // chargement des polices
      msg_joueur1.LoadContent();
      msg_joueur1.Text = "Joueur 1 : " + score1;

      // load background texture
      backgroundTexture = Content.Load<Texture2D>("background/watch-tower-802102_1920");
      mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
           
      
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      _viseur.UnloadContent();
      _ballonswave.UnloadContent();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // le viseur doit suivre la souris
      _currentMouseState = Mouse.GetState();
      _viseur.Position = new Vector2(
        _currentMouseState.X,
        _currentMouseState.Y);
      _viseur.Update(gameTime);

      // play sound
      if (_currentMouseState.LeftButton == ButtonState.Pressed)
      {
        song = Content.Load<Song>("sound/M1 Garand Single-SoundBible.com-1941178963");
        MediaPlayer.Play(song);
      }

      _ballonswave.Update(gameTime, _currentMouseState);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _spriteBatch.Begin();

      // Draw the background
      _spriteBatch.Draw(backgroundTexture, mainFrame, Color.White);

      // affichage des ballons
      _ballonswave.Draw(_spriteBatch);

      // affichage du viseur
      _viseur.Draw(_spriteBatch);

      // Draw Hello World
      msg_joueur1.Text = "Joueur 1 : " + score1++;
      msg_joueur1.Draw(_spriteBatch);      

      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}