using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BallonsShooter;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Sprites
{
  class SpriteBird : SpriteGenericAnimated
  {
    private int SPEEDXMIN = 1;
    private int SPEEDXMAX = 5;

    protected int _elapsedTimeMs;           // gestion du temps entre les appels à Update()
    protected int _elapsedTimeBtwHitMs = 1500;    // délai entre les tirs
    protected bool _fireflag = false;

    //private Vector2 _speed;
    private Random rnd = new Random(DateTime.Now.Millisecond);

    SoundEffect _sound;

    public SpriteBird(Game game)
      : base(game)
    {
      _speed = new Vector2(rnd.Next(SPEEDXMIN, SPEEDXMAX), rnd.Next(-1, 1));
      _elapsedTime = (int)(100 - 2 * _speed.X);

      _elapsedTimeMs = 0;
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

    public virtual void Update(GameTime gameTime, Player J1 = null, Player J2 = null, bool soundEffectOn = true)
    {
      // update position according to _speed
      Position = new Vector2(Position.X + _speed.X, Position.Y + _speed.Y);

      _elapsedTimeMs += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

      // attente pour second tir
      if (_elapsedTimeMs > _elapsedTimeBtwHitMs)
      {
        _fireflag = false;
        _elapsedTimeMs = 0;
      }

      if (!_fireflag)
      {
        // Gamer hit ?
        KeyboardState kbstate = Keyboard.GetState();
        if (J1 != null && (
          kbstate.IsKeyDown(J1.Controls["FIRE01"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE02"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE03"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE04"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE05"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE06"])) && _rectDestination.Contains(J1.Position.X, J1.Position.Y))
        {
          // sound explosion
          // http://soundbible.com/tags-bird.html
          // https://www.zapsplat.com/sound-effect-category/cartoon/
          _fireflag = true;
          if (soundEffectOn)
          {
            _sound = _game.Content.Load<SoundEffect>("sound/boing");
            _sound.Play();
          }
          J1.Score -= 1;
        }
        if (J2 != null && (
          kbstate.IsKeyDown(J2.Controls["FIRE01"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE02"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE03"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE04"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE05"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE06"])) && _rectDestination.Contains(J2.Position.X, J2.Position.Y))
        {
          // sound explosion
          _fireflag = true;
          if (soundEffectOn)
          {
            _sound = _game.Content.Load<SoundEffect>("sound/boing");
            _sound.Play();
          }
          J2.Score -= 1;
        }
      }

      // mange bird position inside screen
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
      if ((Position.Y < -_texture.Height) || (Position.Y > _game.GraphicsDevice.DisplayMode.Height))
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
