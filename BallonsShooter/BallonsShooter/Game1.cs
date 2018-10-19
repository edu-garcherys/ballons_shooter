using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Sprites;
using System.Collections.Generic;

namespace BallonsShooter
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : Game
  {
    /// <summary>
    /// gestion de l'état du jeux
    ///   WAITING : attente joueurs
    ///   PLAY : partie en cours
    ///   FINISHED : affichage scores
    /// </summary>
    protected enum GameState { WAITING, PLAY, FINISHED };

    /// <summary>
    /// etat courant du jeu
    /// </summary>
    GameState _gameState = GameState.WAITING;

    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    // les joueurs
    Player _joueur1;
    Player _joueur2;

    // la vague de ballons
    BallonsWave _ballonswave;

    // backgroung texture
    Texture2D backgroundTexture;
    Rectangle mainFrame;

    // les messages
    DisplaySentence _message_waiting;

    TypeWriterTextBox _twtb_waiting;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
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
      _joueur1 = new Player(
        this,
        "Joueur Bleu",
        Player.PlayerScreenPosition.LEFT,
        "viseurs/viseur_70_fin_blue",
        SpriteGeneric.ViewportPosition.CENTER_LEFT,
        2500,
        new Dictionary<string, Keys>() {
          {"UP", Keys.W },
          {"RIGHT", Keys.D },
          {"DOWN", Keys.S },
          {"LEFT", Keys.A },
          {"FIRE01", Keys.Space }
        }
        );
      _joueur1.Initialize();

      _joueur2 = new Player(
        this,
        "Joueur Rouge",
        Player.PlayerScreenPosition.RIGHT,
        "viseurs/viseur_70_fin_red",
        SpriteGeneric.ViewportPosition.CENTER_RIGHT,
        2500,
        new Dictionary<string, Keys>() {
          {"UP", Keys.NumPad8 },
          {"RIGHT", Keys.NumPad6},
          {"DOWN", Keys.NumPad2 },
          {"LEFT", Keys.NumPad4 },
          {"FIRE01", Keys.NumPad0 }
        }
        );
      _joueur2.Initialize();

      // instantiation des ballons
      _ballonswave = new BallonsWave(this, 500);

      // typewritetextbox waiting message
      _twtb_waiting = new TypeWriterTextBox(this);
      _twtb_waiting.Initialize();
      _twtb_waiting.Text = "PLEASE INSERT COIN !";
      _twtb_waiting.Effects = TypeWriterTextBox.TwtbEffects.TYPEWRITER;

      // init waiting message
      _message_waiting = new DisplaySentence(
        this, 
        DisplaySentence.TextPosition.CENTER, 
        DisplaySentence.TextEffect.COLORLOOP | DisplaySentence.TextEffect.FADEINOUT);
      _message_waiting.Font_color = Color.Red;
      _message_waiting.Font_scale = 2.0f;
      _message_waiting.Text = "PLEASE INSERT COIN !";

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // l'application démarre en plein écran et haute resolution
      _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width - 50;
      _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height - 50;
      _graphics.IsFullScreen = false;
      _graphics.ApplyChanges();

      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // start players
      _joueur1.LoadContent();
      _joueur2.LoadContent();

      // load background texture
      backgroundTexture = Content.Load<Texture2D>("background/background_white");
      mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

      _message_waiting.LoadContent();

      _twtb_waiting.LoadContent();
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      _joueur1.UnloadContent();
      _joueur2.UnloadContent();
            
      _ballonswave.UnloadContent();

      _twtb_waiting.UnloadContent();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // jeu en attente
      switch (_gameState)
      {
        case GameState.WAITING:
          _message_waiting.Update(gameTime);
          // changement état du jeu si touche Enter
          if (Keyboard.GetState().IsKeyDown(Keys.Enter))
          {
            _gameState = GameState.PLAY;
          }
          break;
        case GameState.PLAY:
          // manage player keyboard moves
          _joueur1.Move(Keyboard.GetState());
          _joueur1.Update(gameTime);

          _joueur2.Move(Keyboard.GetState());
          _joueur2.Update(gameTime);

          _ballonswave.Update(gameTime, _joueur1, _joueur2);
          break;
        case GameState.FINISHED:
          break;
      }

      _twtb_waiting.Update(gameTime);

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

      // dessine le fond
      _spriteBatch.Draw(backgroundTexture, mainFrame, Color.White);

      switch (_gameState)
      {
        case GameState.WAITING:
          _message_waiting.Draw(_spriteBatch);
          _twtb_waiting.Draw(_spriteBatch);
          break;
        case GameState.PLAY:

          // affichage des ballons
          _ballonswave.Draw(_spriteBatch);

          _joueur1.Draw(_spriteBatch);
          _joueur2.Draw(_spriteBatch);
          break;
        case GameState.FINISHED:
          break;
      }      

      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
