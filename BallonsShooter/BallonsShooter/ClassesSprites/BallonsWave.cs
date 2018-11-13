#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sprites;
#endregion

namespace BallonsShooter
{
  /// <summary>
  /// Gestion des vagues de ballons
  /// </summary>
  class BallonsWave
  {
    protected Game _game;
    static private Random random;

    protected int _elapsedTimeMs;       // gestion du temps entre les appels à Update()
    protected int _elapsedTimeBtwBallonMs;    // délai entre l'appartion des ballons

    protected List<SpriteBallon> _ballonsList;  // liste (dynamique) des ballons en cours de rendu

    enum GAMESTATE { WAITING, STARTED, FINISHED };

    // listes des textures pour les ballons
    private string[] BallonTextureNames = { "argente_cfpt", "bleu_ciel_nacre_cfpt", "bleu_nuit_cfpt", "dore_cfpt", "fushia_nacre_cfpt", "rose_nacre_cfpt", "rouge_nacre_cfpt" };

    public BallonsWave(Game game, int elapsedTimeBtwBallonMs)
    {
      _game = game;

      _ballonsList = new List<SpriteBallon>();

      _elapsedTimeMs = 0;
      _elapsedTimeBtwBallonMs = elapsedTimeBtwBallonMs;

      // initialisation du générateur aléatoire
      random = new Random();
    }

    /// <summary>
    /// Suppression de l'ensemble des ballons de la liste
    /// </summary>
    public void UnloadContent()
    {
      _ballonsList.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="mouse"></param>
    public void Update(GameTime gameTime, Player J1, Player J2, bool soundEffectOn = true)
    {
      _elapsedTimeMs += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

      // le temps est dépassé, on ajoute un nouveau ballon
      if (_elapsedTimeMs > _elapsedTimeBtwBallonMs)
      {
        // increase speed
        if (_elapsedTimeBtwBallonMs > 5) _elapsedTimeBtwBallonMs -= 10;

        AddNewBallon();
        _elapsedTimeMs = 0;
      }

      // suppression des ballons détruits ou hors écran
      _ballonsList.RemoveAll(b => b.ToBeRemoved == true);

      // mise à jour de tous les ballons
      foreach (SpriteBallon b in _ballonsList)
      {
        b.Update(gameTime, J1, J2, soundEffectOn);
      }

      _game.Window.Title = string.Format("Nombre de ballons : {0}", _ballonsList.Count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
      // affichage de l'ensemble des ballons
      foreach (SpriteBallon b in _ballonsList)
        b.Draw(spriteBatch);
    }

    /// <summary>
    /// Ajout d'un nouveau ballon dans la vague
    /// </summary>
    public void AddNewBallon()
    {
      // création et initialisation du ballon
      SpriteBallon b = new SpriteBallon(_game);
      b.LoadContent("ballons/" + BallonTextureNames[(int)random.Next(7)]);

      float vx = (float)(random.NextDouble() * 2 - 1);
      float vy = (float)(random.NextDouble() + 1);

      b.Speed = new Vector2(vx, vy);
      b.Scale = (float)(0.05f + random.Next(5) * 0.01);
      b.SetPosition(SpriteGeneric.ViewportPosition.RANDOMTOP_OUTSIDE);

      // ajout du ballon dans la vague
      _ballonsList.Add(b);
    }

  }
}
