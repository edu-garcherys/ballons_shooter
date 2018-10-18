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

    protected ViewportPosition _viewportposition;
    protected string _text;
    public string Text
    {
      protected get { return _text; }
      set { _text = value; }
    }


    Color _fontcolor = Color.Black;
    public Color Fontcolor { get => _fontcolor; set => _fontcolor = value; }

    public enum ViewportPosition { TOPLEFTCENTER, TOPCENTER, TOPRIGHTCENTER, BOTTOMLEFTCENTER, BOTTOMCENTER, BOTTOMRIGHTCENTER, CENTER };
    protected Vector2 _position;

    #region constructor
    public DisplayText(Game game, ViewportPosition p)
    {
      _game = game;
      _viewportposition = p;

    }
    #endregion

    public void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _font = _game.Content.Load<SpriteFont>("fonts/moire_bold_24");

    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // update position
      SetPosition();

      // Find the center of the string
      Vector2 FontOrigin = _font.MeasureString(Text) / 2;
      // Draw the string
      spriteBatch.DrawString(_font, _text, _position, _fontcolor, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.8f);
    }


    public void SetPosition()
    {
      Vector2 coordonnees = Vector2.Zero;

      Vector2 fontOrigin = _font.MeasureString(_text) / 2;

      int maxWidth = _game.GraphicsDevice.Viewport.Width;
      int maxHeight = _game.GraphicsDevice.Viewport.Height;

      switch (_viewportposition)
      {
        case ViewportPosition.TOPLEFTCENTER:
          coordonnees = new Vector2(maxWidth / 3 - fontOrigin.X / 2, 2 * fontOrigin.Y);
          break;
        case ViewportPosition.TOPCENTER:
          coordonnees = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, 2 * fontOrigin.Y);
          break;
        case ViewportPosition.TOPRIGHTCENTER:
          coordonnees = new Vector2(2 * (maxWidth / 3) + fontOrigin.X / 2, 2 * fontOrigin.Y);
          break;
        case ViewportPosition.BOTTOMLEFTCENTER:
          coordonnees = new Vector2(maxWidth / 3 - fontOrigin.X / 2, maxHeight - 2 * fontOrigin.Y);
          break;
        case ViewportPosition.BOTTOMCENTER:
          coordonnees = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, maxHeight - 2 * fontOrigin.Y);
          break;
        case ViewportPosition.BOTTOMRIGHTCENTER:
          coordonnees = new Vector2(2 * (maxWidth / 3) + fontOrigin.X / 2, maxHeight - 2 * fontOrigin.Y);
          break;
        case ViewportPosition.CENTER:
          coordonnees = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);
          break;
      }

      // application des coordoonées
      _position = coordonnees;

    }
  }
}
