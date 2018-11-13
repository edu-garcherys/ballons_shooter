using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Media;
using Sprites;
using System.Collections.Generic;
using System;

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
      EXIT, START, RESTART, SOUND
    };

    protected IDictionary<GameControlsKeys, Keys> _gameControls = new Dictionary<GameControlsKeys, Keys>()
    {
      {GameControlsKeys.EXIT, Keys.Enter },
      {GameControlsKeys.START, Keys.D8 },
      {GameControlsKeys.RESTART, Keys.D9 },
      {GameControlsKeys.SOUND, Keys.D6 }
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

    TypeWriterTextBox _twtbCFPT;
    TypeWriterTextBox _twtbExplain;
    TypeWriterTextBox _twtbWaiting;
    TypeWriterTextBox _twtbWinner;

    Timer _timer;

    // game parameter
    int _nbPointsMax = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NbPoints"));
    bool _soundEffectOn = ConfigurationManager.AppSettings.Get("SoundEffects") == "ON" ? true : false;
    int _elapsedButtonDelayMs = 100;  // delay for activate command button in ms
    int _elapsedButtonTimeMs = 0;

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
        "viseurs/viseur_70_fin_red",
        SpriteGeneric.ViewportPosition.CENTER_LEFT,
        Convert.ToInt32(ConfigurationManager.AppSettings.Get("BetweenFireDelay")),
        new Dictionary<string, Keys>() {
          {"UP", Keys.W },
          {"RIGHT", Keys.D},
          {"DOWN", Keys.S },
          {"LEFT", Keys.A },
          {"FIRE01", Keys.F }
        }
        );
      _joueur1.Initialize();

      _joueur2 = new Player(
        this,
        ConfigurationManager.AppSettings.Get("J2Name"),
        Player.PlayerScreenPosition.RIGHT,
        "viseurs/viseur_70_fin_blue",
        SpriteGeneric.ViewportPosition.CENTER_RIGHT,
        Convert.ToInt32(ConfigurationManager.AppSettings.Get("BetweenFireDelay")),
        new Dictionary<string, Keys>() {
          {"UP", Keys.Up },
          {"RIGHT", Keys.Right },
          {"DOWN", Keys.Down },
          {"LEFT", Keys.Left },
          {"FIRE01", Keys.NumPad4 }
        }
        );
      _joueur2.Initialize();

      // instantiation des ballons
      _ballonswave = new BallonsWave(this, 1500);

      // birds init
      _birdR = new SpriteBird(this);
      _birdB = new SpriteBird(this);

      // textes
      _twtbCFPT = new TypeWriterTextBox(this);
      _twtbCFPT.Initialize();
      _twtbCFPT.Text = ConfigurationManager.AppSettings.Get("CFPTMessage");
      _twtbCFPT.DelayInMs = 20;
      _twtbCFPT.FontColor = Color.Black;
      _twtbCFPT.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTERBOTTOM;
      _twtbCFPT.Effects = TypeWriterTextBox.TwtbEffects.BACKGROUND | TypeWriterTextBox.TwtbEffects.NOTYPEWRITTER;

      _twtbExplain = new TypeWriterTextBox(this);
      _twtbExplain.Initialize();
      _twtbExplain.Text = ConfigurationManager.AppSettings.Get("ExplainMessage");
      _twtbExplain.FontColor = Color.DarkSlateGray;
      _twtbExplain.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTERTOPMIDDLE;
      _twtbExplain.Effects = TypeWriterTextBox.TwtbEffects.BACKGROUND | TypeWriterTextBox.TwtbEffects.NOTYPEWRITTER;

      // typewritertextbox waiting message
      _twtbWaiting = new TypeWriterTextBox(this);
      _twtbWaiting.Initialize();
      _twtbWaiting.Text = ConfigurationManager.AppSettings.Get("StartMessage");
      _twtbWaiting.FontColor = Color.Yellow;
      _twtbWaiting.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTER;
      _twtbWaiting.Effects = TypeWriterTextBox.TwtbEffects.NONE;

      // typewritertextbox winner message
      _twtbWinner = new TypeWriterTextBox(this);
      _twtbWinner.Initialize();
      _twtbWinner.Text = "En attente du gagnant";
      _twtbWinner.FontColor = Color.WhiteSmoke;
      _twtbWinner.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTER;
      _twtbWinner.Effects = TypeWriterTextBox.TwtbEffects.BACKGROUND;

      // init waiting message
      _dsWaiting = new DrawSentence(
        this,
        DrawSentence.TextPosition.CENTER,
        DrawSentence.TextEffect.FADEINOUT);
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

      _twtbExplain.LoadContent();
      _twtbCFPT.LoadContent();
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

      _twtbExplain.UnloadContent();
      _twtbCFPT.UnloadContent();
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
      // game exit
      if (Keyboard.GetState().IsKeyDown(_gameControls[GameControlsKeys.EXIT]))
        Exit();

      // manage commands
      if (Keyboard.GetState().IsKeyDown(_gameControls[GameControlsKeys.SOUND]))
      {
        _elapsedButtonTimeMs += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (_elapsedButtonTimeMs > _elapsedButtonDelayMs)
        {
          _elapsedButtonTimeMs = 0;
          _soundEffectOn = !_soundEffectOn;
        }
        
      }

      // état du jeu
      switch (_gameState)
      {
        // attente des joueurs
        case GameState.WAITING:
          _twtbCFPT.Update(gameTime);
          _twtbExplain.Update(gameTime);
          _twtbWaiting.Update(gameTime);
          _dsWaiting.Update(gameTime);

          _birdR.Update(gameTime, null, null, _soundEffectOn);
          _birdB.Update(gameTime, null, null, _soundEffectOn);

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
          _joueur1.Move(Keyboard.GetState(), _soundEffectOn);
          _joueur1.Update(gameTime);
          _joueur2.Move(Keyboard.GetState(), _soundEffectOn);
          _joueur2.Update(gameTime);

          _birdR.Update(gameTime, _joueur1, _joueur2, _soundEffectOn);
          _birdB.Update(gameTime, _joueur1, _joueur2, _soundEffectOn);

          // update ballons wave and scores
          _ballonswave.Update(gameTime, _joueur1, _joueur2, _soundEffectOn);

          // end game
          if (_joueur1.Score >= _nbPointsMax)
          {
            _twtbWinner.Text = ConfigurationManager.AppSettings.Get("J1WinnerMsg");
            _twtbWinner.FontColor = Color.Red;
            _gameState = GameState.FINISHED;
          }
          if (_joueur2.Score >= _nbPointsMax)
          {
            _twtbWinner.Text = ConfigurationManager.AppSettings.Get("J2WinnerMsg");
            _twtbWinner.FontColor = Color.Blue;
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

          DisplayControls();

          _twtbExplain.Draw(_spriteBatch);
          _twtbWaiting.Draw(_spriteBatch);
          _twtbCFPT.Draw(_spriteBatch);
          //_dsWaiting.Draw(_spriteBatch);

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

          DisplayControls();

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

          DisplayControls();

          _joueur1.Draw(_spriteBatch);
          _joueur2.Draw(_spriteBatch);

          _twtbWinner.Draw(_spriteBatch);

          break;
      }

      _spriteBatch.End();

      base.Draw(gameTime);
    }

    private void DisplayControls()
    {
      // sound icon
      _spriteBatch.Draw(Content.Load<Texture2D>(_soundEffectOn ? "icon_volume" : "icon_volume_muted"), new Vector2(10, 10), Color.White);
      _spriteBatch.Draw(Content.Load<Texture2D>("logoinformatique_round"), new Vector2(GraphicsDevice.Viewport.Width - 138, GraphicsDevice.Viewport.Height - 138), Color.White);
    }
  }
}
