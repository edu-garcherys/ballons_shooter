using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Sprites
{
  class SpriteBird : SpriteGenericAnimated
  {
    private int SPEEDXMIN = 1;
    private int SPEEDXMAX = 5;

    private Vector2 _speed;
    private Random rnd = new Random(DateTime.Now.Millisecond);

    public SpriteBird(Game game)
      : base(game)
    {
      _speed = new Vector2(rnd.Next(SPEEDXMIN, SPEEDXMAX), rnd.Next(-1, 1));
      _elapsedTime = (int)(100 - 2 * _speed.X);
    }

    public override void Initialize()
    {
      base.Initialize();
    }

    public override void LoadContent(string textureName)
    {
      base.LoadContent(textureName);
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      // update position according to _speed
      Position = new Vector2(Position.X + _speed.X, Position.Y + _speed.Y);

      if (Position.X < -_texture.Width)
      {
        _speed = new Vector2(rnd.Next(SPEEDXMIN, SPEEDXMAX), rnd.Next(-1, 1));
        _elapsedTime = (int)(100 - 2 * _speed.X);
        SetPosition(SpriteGeneric.ViewportPosition.RANDOMLEFT_OUTSIDE);
      }
      if (Position.X > _game.GraphicsDevice.DisplayMode.Width)
      {
        _speed = new Vector2(-1 * rnd.Next(SPEEDXMIN, SPEEDXMAX), rnd.Next(-1, 1));
        _elapsedTime = (int)(100 - 2 * _speed.X);
        SetPosition(SpriteGeneric.ViewportPosition.RANDOMRIGHT_OUTSIDE);
      }

      if ( (Position.Y < -_texture.Height) || (Position.Y > _game.GraphicsDevice.DisplayMode.Height))
      {
        _speed = new Vector2(_speed.X, -_speed.Y);
      }

      base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      SpriteEffects se;
      se = SpriteEffects.None;
      if (_speed.X < 0)
        se = SpriteEffects.FlipHorizontally;

      if (_isvisible && _isactive)
        spriteBatch.Draw(_texture, _rectDestination, _rectSource, Color.White, _rotation, Vector2.Zero, se, 0);

    }

  }
}
