#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BallonsShooter;
using Microsoft.Xna.Framework.Audio;
using System;
#endregion

namespace Sprites
{
  // ajout de la gravité
  class SpriteBallon : SpriteGeneric
  {
    protected bool _isShooted = false;                    // le ballon a été touché ?
    protected SpriteGenericAnimated _explosionAnimated;   // animation de l'explosion

    // gestion de la vie d'un ballon
    protected bool _toBeRemoved;
    public bool ToBeRemoved
    {
      get { return _toBeRemoved; }
    }

    private Random _rnd = new Random(DateTime.Now.Millisecond);

    private string[] _explosionSounds = { "sound/ballon_explo_01", "sound/ballon_explo_02", "sound/ballon_explo_03" };
    SoundEffect _sound;

    #region constructeurs
    public SpriteBallon(Game game)
      : base(game)
    {
      _isShooted = false;

      _explosionAnimated = new SpriteGenericAnimated(game);
      _explosionAnimated.LoadContent("explosion_5x5", 64, 64, 5, 5, 25, false);
      _explosionAnimated.IsActive = false;
      _explosionAnimated.IsVisible = false;

      // todo: adaptation automatique de la taille de l'explosion à la taille du ballon
      _explosionAnimated.Scale = 2.3f;

      _toBeRemoved = false;
    }
    #endregion

    /// <summary>
    /// Processus de mise à jour du ballon
    /// 
    /// On calcul si le ballon est encore actif (i.e. dans l'écran et non détruit).
    /// On vérifie qu'il y un clic sur le ballon, dans ce cas on change l'état du ballon (on active l'explosion)
    /// 
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="mouse"></param>
    public void Update(GameTime gameTime, Player J1, Player J2, bool soundEffectOn = true)
    {
      // vérifie si le ballon est mort -> desactivé et explosion finie ou hors de l'écran
      _toBeRemoved =
        (
          (_isactive == false) && (_explosionAnimated.IsActive == false)
        )
        ||
        (
          (_rectDestination.Right < 0) ||
          (_rectDestination.Left > _game.GraphicsDevice.Viewport.Width) ||
          (_rectDestination.Top > _game.GraphicsDevice.Viewport.Height)
        );

      if (_toBeRemoved) return;

      KeyboardState kbstate = Keyboard.GetState();

      if (_isactive)
      {
        // player 1
        if ((
          kbstate.IsKeyDown(J1.Controls["FIRE01"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE02"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE03"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE04"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE05"]) ||
          kbstate.IsKeyDown(J1.Controls["FIRE06"])) && _rectDestination.Contains(J1.Position.X, J1.Position.Y))
        {
          // on active l'explosion
          _explosionAnimated.IsActive = true;
          _explosionAnimated.IsVisible = true;
          _explosionAnimated.Position = this._position;
          _isShooted = true;

          // sound explosion
          if (soundEffectOn)
          {
            _sound = _game.Content.Load<SoundEffect>(_explosionSounds[_rnd.Next(0, _explosionSounds.Length)]);
            _sound.Play();
          }

          // on desactive le ballon
          _isactive = false;

          // calcul des points
          J1.Score++;
        }
        // player 2
        if ((
          kbstate.IsKeyDown(J2.Controls["FIRE01"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE02"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE03"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE04"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE05"]) ||
          kbstate.IsKeyDown(J2.Controls["FIRE06"])) && _rectDestination.Contains(J2.Position.X, J2.Position.Y))
        {
          // on active l'explosion
          _explosionAnimated.IsActive = true;
          _explosionAnimated.IsVisible = true;
          _explosionAnimated.Position = this._position;
          _isShooted = true;

          // sound explosion
          _sound = _game.Content.Load<SoundEffect>(_explosionSounds[_rnd.Next(0, _explosionSounds.Length)]);
          _sound.Play();

          // on desactive le ballon
          _isactive = false;

          // calcul des points
          J2.Score++;
        }
      }

      // mise à jour de l'animation de l'explosion
      _explosionAnimated.Update(gameTime);

      // après collision, l'image du ballon est réduite (effet fade-out)
      if (_isShooted)
      {
        _scale -= 0.01f;
        if (_scale < 0)
        {
          IsVisible = false;
          Scale = 0;
        }
      }

      base.Update(gameTime);
    }

    /// <summary>
    /// Affichage du ballon et/ou de l'explosion
    /// </summary>
    /// <param name="spriteBatch"></param>
    public override void Draw(SpriteBatch spriteBatch)
    {
      // affichage du ballon
      if (!_isShooted) base.Draw(spriteBatch);

      // affichage de l'explosion (pas dessus)
      _explosionAnimated.Draw(spriteBatch);
    }

  }
}
