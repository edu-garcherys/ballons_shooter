﻿using System;
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
    public enum PlayerScreenPosition { LEFT, RIGHT };

    private Game1 _game;

    private String _name = "Joueur X";
    private PlayerScreenPosition _position = PlayerScreenPosition.LEFT;

    // controls keys    
    private IDictionary<string, Keys> _controls;

    SpriteGeneric _sprite_viseur;
    SpriteGeneric.ViewportPosition _sprite_initiale_position;
    private String _viseur_texture_name;
    private float _movespeed = 5;

    public Vector2 Position { get => _sprite_viseur.Position; set => _sprite_viseur.Position = value; }

    int _score = 0;
    public int Score { get => _score; set => _score = value; }
    public IDictionary<string, Keys> Controls { get => _controls; set => _controls = value; }

    private TypeWriterTextBox _score_message;

    private string[] _fireSounds = { "sound/fire_blast", "sound/fire_sniper_reload" };  // not used
    SoundEffect _sound_fire;

    protected int _elapsedTimeMs;       // gestion du temps entre les appels à Update()
    protected int _elapsedTimeBtwFireMs;    // délai entre les tirs
    protected bool _fireflag = true;

    public Player(
      Game1 game,
      String name,
      PlayerScreenPosition p,
      String viseur_texture_name,
      SpriteGeneric.ViewportPosition init_pos,
      int elapsedTimeBtwFireMs,
      Dictionary<string, Keys> controls)

    {
      _game = game;
      _name = name;
      _position = p;
      _viseur_texture_name = viseur_texture_name;
      _sprite_initiale_position = init_pos;

      _elapsedTimeMs = 0;
      _elapsedTimeBtwFireMs = elapsedTimeBtwFireMs;

      _controls = controls;
    }
    public virtual void Initialize()
    {
      _sprite_viseur = new SpriteGeneric(_game);

      _score_message = new TypeWriterTextBox(_game);
      _score_message.TbRectanglePosition = _position == PlayerScreenPosition.LEFT ? TypeWriterTextBox.TwtbPosition.LEFTTOP : TypeWriterTextBox.TwtbPosition.RIGHTTOP;
      _score_message.Effects = TypeWriterTextBox.TwtbEffects.BACKGROUND | TypeWriterTextBox.TwtbEffects.NOTYPEWRITTER;
      _score_message.FontColor = _position == PlayerScreenPosition.LEFT ? Color.Red : Color.Blue;
      _score_message.BgColor = Color.White;
      _score_message.BgTransparency = 0.9f;
    }

    public virtual void LoadContent()
    {
      // chargement de la texture pour le viseur
      _sprite_viseur.LoadContent(_viseur_texture_name);

      // positionnement initial du viseur : centre de l'écran
      _sprite_viseur.SetPosition(_sprite_initiale_position);

      // load sound effect
      _sound_fire = _game.Content.Load<SoundEffect>("sound/firesong_sniper_reload");
      _score_message.LoadContent();
    }

    public virtual void UnloadContent()
    {
      _sprite_viseur.UnloadContent();
    }

    public void Move(KeyboardState state, bool soundEffect = true)
    {
      float X = _sprite_viseur.Position.X;
      float Y = _sprite_viseur.Position.Y;

      // manage player keyboard moves
      X -= state.IsKeyDown(Controls["LEFT"]) ? _movespeed : 0;
      X += state.IsKeyDown(Controls["RIGHT"]) ? _movespeed : 0;
      Y -= state.IsKeyDown(Controls["UP"]) ? _movespeed : 0;
      Y += state.IsKeyDown(Controls["DOWN"]) ? _movespeed : 0;

      if ((
        state.IsKeyDown(Controls["FIRE01"]) ||
        state.IsKeyDown(Controls["FIRE02"]) ||
        state.IsKeyDown(Controls["FIRE03"]) ||
        state.IsKeyDown(Controls["FIRE04"]) ||
        state.IsKeyDown(Controls["FIRE05"]) ||
        state.IsKeyDown(Controls["FIRE06"]))&& _fireflag)
      {
        if (soundEffect)
          _sound_fire.Play();
        _fireflag = false;
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

      //message = X + " " + Y;

    }

    public virtual void Update(GameTime gameTime)
    {
      _elapsedTimeMs += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

      // attente pour second tir
      if (_elapsedTimeMs > _elapsedTimeBtwFireMs)
      {
        _fireflag = true;
        _elapsedTimeMs = 0;
      }

      _sprite_viseur.Update(gameTime);

      _score_message.Update(gameTime);

    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // affichage du viseur
      _sprite_viseur.Draw(spriteBatch);

      // affichage du score
      _score_message.Text = _name + " : " + Score;
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
