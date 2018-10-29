using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Configuration;
using System.Collections.Specialized;
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

    public enum GameControlsKeys
    {
      EXIT, START, RESTART
    };

    protected IDictionary<GameControlsKeys, Keys> _gameControls = new Dictionary<GameControlsKeys, Keys>()
    {
      {GameControlsKeys.EXIT, Keys.Escape },
      {GameControlsKeys.START, Keys.Enter },
      {GameControlsKeys.RESTART, Keys.Back }
    };

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

    // bird
    SpriteBird _birdR;
    SpriteBird _birdB;

    // backgroung texture
    Texture2D backgroundTexture;
    Texture2D backgroundTexture_front;
    Rectangle mainFrame;

    // les messages
    DrawSentence _dsWaiting;

    TypeWriterTextBox _twtbWaiting;
    TypeWriterTextBox _twtbWinner;

    Timer _timer;

    int _nbPointsMax = 10;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);

      Window.Title = ConfigurationManager.AppSettings.Get("ApplicationTitle");

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
        ConfigurationManager.AppSettings.Get("J1Name"),
        Player.PlayerScreenPosition.LEFT,
        "viseurs/viseur_70_fin_blue",
        SpriteGeneric.ViewportPosition.CENTER_LEFT,
        2000,
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
        ConfigurationManager.AppSettings.Get("J2Name"),
        Player.PlayerScreenPosition.RIGHT,
        "viseurs/viseur_70_fin_red",
        SpriteGeneric.ViewportPosition.CENTER_RIGHT,
        2000,
        new Dictionary<string, Keys>() {
          {"UP", Keys.Up },
          {"RIGHT", Keys.Right},
          {"DOWN", Keys.Down },
          {"LEFT", Keys.Left },
          {"FIRE01", Keys.NumPad0 }
        }
        );
      _joueur2.Initialize();

      // instantiation des ballons
      _ballonswave = new BallonsWave(this, 1500);

      // birds init
      _birdR = new SpriteBird(this);
      _birdB = new SpriteBird(this);

      // typewritertextbox waiting message
      _twtbWaiting = new TypeWriterTextBox(this);
      _twtbWaiting.Initialize();
      _twtbWaiting.Text = ConfigurationManager.AppSettings.Get("WelcomeMessage");
      _twtbWaiting.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTER;
      _twtbWaiting.Effects = TypeWriterTextBox.TwtbEffects.NONE;

      // typewritertextbox winner message
      _twtbWinner = new TypeWriterTextBox(this);
      _twtbWinner.Initialize();
      _twtbWinner.Text = "En attente du gagnant";
      _twtbWinner.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTER;
      _twtbWinner.Effects = TypeWriterTextBox.TwtbEffects.NONE;

      // init waiting message
      _dsWaiting = new DrawSentence(
        this,
        DrawSentence.TextPosition.CENTER,
        DrawSentence.TextEffect.COLORLOOP | DrawSentence.TextEffect.FADEINOUT);
      _dsWaiting.Font_color = Color.Red;
      _dsWaiting.Font_scale = 2.0f;
      _dsWaiting.Text = "PLEASE INSERT COIN !";

      // display chrono
      _timer = new Timer(this);

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
      _graphics.IsFullScreen = ConfigurationManager.AppSettings.Get("ApplicationMode") == "FULLSCREEN" ? true : false;
      _graphics.ApplyChanges();

      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // birds
      _birdR.LoadContent("birds/bird_red", 91, 70, 3, 1, 50, true);
      _birdR.SetPosition(SpriteGeneric.ViewportPosition.RANDOMRIGHT_OUTSIDE);
      _birdB.LoadContent("birds/bird_blue", 91, 70, 3, 1, 50, true);
      _birdB.SetPosition(SpriteGeneric.ViewportPosition.RANDOMLEFT_OUTSIDE);

      // start players
      _joueur1.LoadContent();
      _joueur2.LoadContent();

      // load background texture
      backgroundTexture = Content.Load<Texture2D>("background/background_autumn");
      backgroundTexture_front = Content.Load<Texture2D>("background/background_autumn_front02");
      mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

      _dsWaiting.LoadContent();

      _twtbWaiting.LoadContent();
      _twtbWinner.LoadContent();

      base.LoadContent();
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

      _twtbWaiting.UnloadContent();
      _twtbWinner.UnloadContent();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (Keyboard.GetState().IsKeyDown(_gameControls[GameControlsKeys.EXIT]))
        Exit();

      // jeu en attente
      switch (_gameState)
      {
        // attente des joueurs
        case GameState.WAITING:
          _twtbWaiting.Update(gameTime);
          _dsWaiting.Update(gameTime);

          _birdR.Update(gameTime);
          _birdB.Update(gameTime);

          // changement état du jeu si touche Enter
          if (Keyboard.GetState().IsKeyDown(_gameControls[GameControlsKeys.START]))
          {
            _gameState = GameState.PLAY;
          }
          break;
        // jeu en cours
        case GameState.PLAY:
          _timer.Update(gameTime);

          // manage player keyboard moves
          _joueur1.Move(Keyboard.GetState());
          _joueur1.Update(gameTime);

          _joueur2.Move(Keyboard.GetState());
          _joueur2.Update(gameTime);

          _birdR.Update(gameTime);
          _birdB.Update(gameTime);

          // update ballons wave and scores
          _ballonswave.Update(gameTime, _joueur1, _joueur2);

          // end game
          if (_joueur1.Score >= _nbPointsMax)
          {
            _twtbWinner.Text = ConfigurationManager.AppSettings.Get("J1WinnerMsg");
            _gameState = GameState.FINISHED;
          }
          if (_joueur2.Score >= _nbPointsMax)
          {
            _twtbWinner.Text = ConfigurationManager.AppSettings.Get("J2WinnerMsg");
            _gameState = GameState.FINISHED;
          }
          break;
        // le jeu est fini
        case GameState.FINISHED:
          _birdR.Update(gameTime);
          _birdB.Update(gameTime);

          _twtbWinner.Update(gameTime);
          if (Keyboard.GetState().IsKeyDown(_gameControls[GameControlsKeys.RESTART]))
          {
            _gameState = GameState.WAITING;
            _timer.Restart();
            this.Initialize();
          }
          break;
      }

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

      switch (_gameState)
      {
        case GameState.WAITING:
          _spriteBatch.Draw(backgroundTexture, mainFrame, Color.White);

          // draw birds
          _birdR.Draw(_spriteBatch);
          _birdB.Draw(_spriteBatch);

          // draw background front
          _spriteBatch.Draw(backgroundTexture_front, mainFrame, Color.White);

          _twtbWaiting.Draw(_spriteBatch);
          _dsWaiting.Draw(_spriteBatch);

          break;
        case GameState.PLAY:
          _spriteBatch.Draw(backgroundTexture, mainFrame, Color.White);

          // affichage des ballons
          _ballonswave.Draw(_spriteBatch);

          // draw birds
          _birdR.Draw(_spriteBatch);
          _birdB.Draw(_spriteBatch);

          // draw background front
          _spriteBatch.Draw(backgroundTexture_front, mainFrame, Color.White);

          _joueur1.Draw(_spriteBatch);
          _joueur2.Draw(_spriteBatch);

          _timer.Draw(_spriteBatch);
          break;
        case GameState.FINISHED:
          _spriteBatch.Draw(backgroundTexture, mainFrame, Color.White);

          // draw birds
          _birdR.Draw(_spriteBatch);
          _birdB.Draw(_spriteBatch);

          // draw background front
          _spriteBatch.Draw(backgroundTexture_front, mainFrame, Color.White);

          _joueur1.Draw(_spriteBatch);
          _joueur2.Draw(_spriteBatch);

          _twtbWinner.Draw(_spriteBatch);

          break;
      }

      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
