using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sprites
{
  class DisplayText
  {
    protected Game _game;								// référence vers la classe du jeu
    protected SpriteFont _font;

    protected Vector2 _position;
    public Vector2 Position
    {
      protected get { return _position; }
      set { _position = value; }
    }

    protected string _text;
    public string Text
    {
      protected get { return _text;  }
      set { _text = value; }
    }

    public DisplayText(Game game)
    {
      _game = game;
    }

    public void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _font = _game.Content.Load<SpriteFont>("myfont");
      Position = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height - 50);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // Find the center of the string
      Vector2 FontOrigin = _font.MeasureString(Text) / 2;
      // Draw the string
      spriteBatch.DrawString(_font, Text, Position, Color.Black, 0, FontOrigin, 2.0f, SpriteEffects.None, 0.8f);
    }

  }
}
