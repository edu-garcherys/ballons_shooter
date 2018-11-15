using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BallonsShooter
{
  class Timer
  {

    TypeWriterTextBox _txtTimer;
    protected int _elapsedTimeMs = 0;


    public Timer(Game game)
    {

      _txtTimer = new TypeWriterTextBox(game);
      _txtTimer.TbRectanglePosition = TypeWriterTextBox.TwtbPosition.CENTERTOP;
      _txtTimer.Effects = TypeWriterTextBox.TwtbEffects.BACKGROUND | TypeWriterTextBox.TwtbEffects.NOTYPEWRITTER;
      _txtTimer.FontColor = Color.Black;
      _txtTimer.BgTransparency = 1.0f;      
      _txtTimer.Text = "00:00:00.00";

      _txtTimer.LoadContent();
    }
    public void Restart()
    {
      _elapsedTimeMs = 0;
    }

    public void Update(GameTime gameTime)
    {
      _elapsedTimeMs += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
      TimeSpan ts = TimeSpan.FromMilliseconds(_elapsedTimeMs);
      _txtTimer.Text = ts.ToString(@"hh\:mm\:ss\.f");
      _txtTimer.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      _txtTimer.Draw(spriteBatch);
    }

  }
}
