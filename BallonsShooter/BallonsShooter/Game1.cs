using Microsoft.Xna.Framework;
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
    
    Player _joueur1;
    
    // la vague de ballons
    BallonsWave _ballonswave;

    // backgroung texture
    Texture2D backgroundTexture;
    Rectangle mainFrame;
    
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
      _joueur1 = new Player(this, "Joueur Bleu", "viseur_blue");
      _joueur1.Initialize();

      // instantiation des ballons
      _ballonswave = new BallonsWave(this, 500);

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // l'application démarre en plein écran et haute resolution
      _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width-100;
      _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height-100;
      _graphics.IsFullScreen = false;
      _graphics.ApplyChanges();

      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // start gamer 1
      _joueur1.LoadContent();

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
      _joueur1.UnloadContent();
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
      
      _ballonswave.Update(gameTime, _joueur1);

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

      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
