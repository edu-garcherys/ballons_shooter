using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Sprites;

namespace BallonsShooter
{
  class Gamer
  {
    Game _game;
    protected string _name;
    protected string _viseur_name = "viseur_red";
    SpriteGeneric _viseur;

    int _movespeed = 5;

    public string message = "joueur";


    public Vector2 Position { get => _viseur.Position; }

    public Gamer(Game game, String name, String ViseurName)
    {
      _game = game;
      _name = name;
      _viseur_name = ViseurName;
    }

    public virtual void Initialize()
    {
      _viseur = new SpriteGeneric(_game);
    }

    public virtual void LoadContent()
    {
      // chargement de la texture pour le viseur
      _viseur.LoadContent(_viseur_name);

      // positionnement initial du viseur : centre de l'écran
      _viseur.SetPosition(SpriteGeneric.ViewportPosition.CENTER);
    }

    public virtual void UnloadContent()
    {
      _viseur.UnloadContent();
    }

    public void Move(KeyboardState state)
    {
      float X = _viseur.Position.X;
      float Y = _viseur.Position.Y;

      // manage player keyboard moves
      X -= state.IsKeyDown(Keys.Left) ? _movespeed : 0;
      X += state.IsKeyDown(Keys.Right) ? _movespeed : 0;
      Y -= state.IsKeyDown(Keys.Up) ? _movespeed : 0;
      Y += state.IsKeyDown(Keys.Down) ? _movespeed : 0;

      // make sure that the player does not go out of bounds
      X = MathHelper.Clamp(X, 0, _game.GraphicsDevice.Viewport.Width);
      Y = MathHelper.Clamp(Y, 0, _game.GraphicsDevice.Viewport.Height);

      _viseur.Position = new Vector2(X, Y);

      message = X + " " + Y;

    }

    public virtual void Update(GameTime gameTime)
    {
      _viseur.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // affichage du viseur
      _viseur.Draw(spriteBatch);
    }
  }
}
