using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sprites
{
  class DisplayText
  {
    public enum ViewportPosition { TOPLEFTCENTER, TOPCENTER, TOPRIGHTCENTER, BOTTOMLEFTCENTER, BOTTOMCENTER, BOTTOMRIGHTCENTER, CENTER };
    public enum TextEffect { NONE, FADEINOUT, FADEIN };

    protected Game _game;								            // référence vers la classe du jeu
    protected SpriteFont _font;

    protected ViewportPosition _viewportposition;
    protected string _text;
    public string Text
    {
      protected get { return _text; }
      set { _text = value; }
    }


    Color _fontcolor = Color.Black;
    float _alpha = 1;
    float _alpha_step = 0.01f;

    public Color Fontcolor { get => _fontcolor; set => _fontcolor = value; }

    protected Vector2 _position;

    TextEffect _texteffect = TextEffect.NONE;


    #region constructor
    public DisplayText(Game game, ViewportPosition p, TextEffect texteffect = TextEffect.NONE)
    {
      _game = game;
      _viewportposition = p;
      _texteffect = texteffect;

      if (texteffect == TextEffect.FADEIN)
      {
        _alpha = 0;
      }
    }
    #endregion

    public void LoadContent()
    {
      // Chargement de la font depuis les ressources
      _font = _game.Content.Load<SpriteFont>("fonts/moire_bold_24");
    }

    public virtual void Update(GameTime gameTime)
    {
      // Fade In Out Repeat
      switch (_texteffect)
      {
        case TextEffect.FADEIN:
          _alpha += _alpha < 1 ? _alpha_step : 0;
          break;
        case TextEffect.FADEINOUT:
          _alpha += _alpha_step;
          if (_alpha < 0 || _alpha > 1)
          {
            _alpha_step *= -1;
          }
          break;
        case TextEffect.NONE:
          _alpha = 1;
          break;
      }


      _fontcolor = new Color(_fontcolor, _alpha);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // Mise à jour de la position du message
      SetPosition();

      // Définir le centre de la chaine de caractère
      Vector2 fontOrigin = _font.MeasureString(Text) / 2;

      // Dessiner le message
      spriteBatch.DrawString(
        _font,
        _text,
        _position,
        _fontcolor,
        0,
        fontOrigin,
        1.0f,
        SpriteEffects.None,
        0.8f);
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
