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
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    Player _joueur1;
    Player _joueur2;

    // la vague de ballons
    BallonsWave _ballonswave;

    // backgroung texture
    Texture2D backgroundTexture;
    Rectangle mainFrame;

    DisplayText _start_message;

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

      _start_message = new DisplayText(this, DisplayText.ViewportPosition.CENTER, DisplayText.TextEffect.FADEINOUT);
      _start_message.Fontcolor = Color.Red;
      _start_message.Text = "INSERT COIN";
      
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

      _start_message.LoadContent();
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

      // manage player keyboard moves
      _joueur1.Move(Keyboard.GetState());
      _joueur1.Update(gameTime);

      _joueur2.Move(Keyboard.GetState());
      _joueur2.Update(gameTime);

      _ballonswave.Update(gameTime, _joueur1, _joueur2);

      _start_message.Update(gameTime);
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

      _joueur1.Draw(_spriteBatch);
      _joueur2.Draw(_spriteBatch);

      _start_message.Draw(_spriteBatch);

      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
