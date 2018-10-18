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
using Microsoft.Xna.Framework.Audio;

namespace BallonsShooter
{
  class Player
  {
    private Game1 _game;

    private String _name = "Joueur X";

    // controls keys    
    private IDictionary<string, Keys> _controls = new Dictionary<string, Keys>()
    {
      {"UP", Keys.Up },
      {"RIGHT", Keys.Right },
      {"DOWN", Keys.Down },
      {"LEFT", Keys.Left },
      {"FIRE01", Keys.Space }
    };

    SpriteGeneric _sprite_viseur;
    private String _viseur_texture_name;
    private float _movespeed = 10;

    public Vector2 Position { get => _sprite_viseur.Position; set => _sprite_viseur.Position = value; }

    int _score = 0;
    public int Score { get => _score; set => _score = value; }
    public IDictionary<string, Keys> Controls { get => _controls; set => _controls = value; }

    DisplayText _score_message;

    SoundEffect _sound_fire;

    public Player(Game1 game, String name, String viseur_texture_name)
    {
      _game = game;
      _name = name;
      _viseur_texture_name = viseur_texture_name;
    }
    public virtual void Initialize()
    {
      _sprite_viseur = new SpriteGeneric(_game);
      _score_message = new DisplayText(_game);
    }

    public virtual void LoadContent()
    {
      // chargement de la texture pour le viseur
      _sprite_viseur.LoadContent(_viseur_texture_name);

      // positionnement initial du viseur : centre de l'écran
      _sprite_viseur.SetPosition(SpriteGeneric.ViewportPosition.CENTER_LEFT);

      // load sound effect
      _sound_fire = _game.Content.Load<SoundEffect>("sound/firesong");

      _score_message.LoadContent();
    }

    public virtual void UnloadContent()
    {
      _sprite_viseur.UnloadContent();
    }

    public void Move(KeyboardState state)
    {
      float X = _sprite_viseur.Position.X;
      float Y = _sprite_viseur.Position.Y;

      // manage player keyboard moves
      X -= state.IsKeyDown(Controls["LEFT"]) ? _movespeed : 0;
      X += state.IsKeyDown(Controls["RIGHT"]) ? _movespeed : 0;
      Y -= state.IsKeyDown(Controls["UP"]) ? _movespeed : 0;
      Y += state.IsKeyDown(Controls["DOWN"]) ? _movespeed : 0;

      if (state.IsKeyDown(Controls["FIRE01"]))
      {
        _sound_fire.Play();
        /*
        _fire_song = _game.Content.Load<Song>("sound/M1 Garand Single-SoundBible.com-1941178963");
        MediaPlayer.IsRepeating = false;
        MediaPlayer.Play(_fire_song);
        */
        // MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
      }
      // make sure that the player does not go out of bounds
      X = MathHelper.Clamp(X, 0, _game.GraphicsDevice.Viewport.Width);
      Y = MathHelper.Clamp(Y, 0, _game.GraphicsDevice.Viewport.Height);

      _sprite_viseur.Position = new Vector2(X, Y);

      _score_message.Text = _name + " : " + Score;
      //message = X + " " + Y;

    }

    public virtual void Update(GameTime gameTime)
    {
      _sprite_viseur.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // affichage du viseur
      _sprite_viseur.Draw(spriteBatch);

      // affichage du score
      _score_message.Draw(spriteBatch);
    }

    void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
    {
      // 0.0f is silent, 1.0f is full volume
      //MediaPlayer.Volume -= 0.1f;
      //MediaPlayer.Play(_fire_song);
    }

  }
}
