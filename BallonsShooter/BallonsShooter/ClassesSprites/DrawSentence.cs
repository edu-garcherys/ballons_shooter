using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Sprites
{
  class DrawSentence
  {
    #region enum
    /// <summary>
    /// screen position
    /// </summary>
    public enum TextPosition
    {
      LEFTCENTERTOP,
      LEFTCENTERBOTTOM,
      CENTERTOP,
      CENTER,
      CENTERBOTTOM,
      RIGHTCENTERTOP,
      RIGHTCENTERBOTTOM,
      FREE
    };

    /// <summary>
    /// text effects flags
    /// </summary>
    [Flags]
    public enum TextEffect
    {
      NONE = 0,
      FADEIN = 1,
      FADEOUT = 2,
      FADEINOUT = 4,
      COLORLOOP = 8,
      BACKGROUND = 16,
      ZOOMIN = 32
    };
    #endregion

    /// <summary>
    /// Loop colors set
    /// </summary>
    static private Color[] _font_colors = { Color.Black, Color.Blue, Color.Red, Color.Green, Color.Brown, Color.Crimson, Color.DarkMagenta, Color.DimGray, Color.ForestGreen, Color.Gold, Color.Gray };

    private Game _game;                                           // référence vers la classe du jeu

    private string _text;                                         // le texte à afficher

    private SpriteFont _font;                                     // font sprite
    private String _font_content = "fonts/moire_bold_24";         // font ressource name (inside Content.mgcb)
    private Color _font_color = Color.Black;                      // couleur par défaut
    private float _font_size = 1.0f;                             // zoom


    private TextPosition _viewportposition = TextPosition.FREE;   // position prédéfine
    private Vector2 _text_position_coordinates;                   // coordonnées du centre du texte


    private TextEffect _text_effect = TextEffect.NONE;

    float _elapsedTimeBtwColorMs = 500;                           // durée pour le changement de couleurs (ms)
    float _elapsedTimeMs = 0;                                     // gestion du changement de couleurs
    int _font_color_loop_index = 0;                               // gestion du changement de couleurs
    private float _font_color_alpha = 1;                          // alpha channel
    private float _font_color_alpha_step = 0.01f;                 // alpha effect step

    #region accesseurs
    public string Text { private get { return _text == null ? "" : _text; } set { _text = value; } }
    public Color Font_color { private get => _font_color; set => _font_color = value; }
    public float Font_scale { private get => _font_size; set => _font_size = value < 0 ? 0 : value; }
    public string Font_content { private get => _font_content; set => _font_content = value; }
    #endregion

    #region constructor
    public DrawSentence(Game game, TextPosition p, TextEffect texteffect = TextEffect.NONE)
    {
      _game = game;
      _viewportposition = p;
      _text_effect = texteffect;

      if (texteffect == TextEffect.FADEIN)
      {
        _font_color_alpha = 0;
      }
    }
    #endregion

    public virtual void Initialize()
    {
      if (_text_effect.HasFlag(TextEffect.ZOOMIN))
        _font_size = 0f;
    }
    public void LoadContent()
    {
      // Chargement de la font depuis les ressources
      _font = _game.Content.Load<SpriteFont>(Font_content);
    }

    public virtual void Update(GameTime gameTime)
    {
      bool isFadeIn = _text_effect.HasFlag(TextEffect.FADEIN);
      bool isFadeOut = _text_effect.HasFlag(TextEffect.FADEOUT);
      bool isFadeInOut = _text_effect.HasFlag(TextEffect.FADEINOUT);
      bool isColorLoop = _text_effect.HasFlag(TextEffect.COLORLOOP);
      bool isZoomIn = _text_effect.HasFlag(TextEffect.ZOOMIN);

      // fade in effect
      if (isFadeIn)
      {
        _font_color_alpha += _font_color_alpha < 1 ? _font_color_alpha_step : 0;
      }

      // fade out effect
      if (isFadeOut)
      {
        _font_color_alpha -= _font_color_alpha > 1 ? 0 : _font_color_alpha_step;
      }

      // fade in-out repeat loop
      if (isFadeInOut)
      {
        _font_color_alpha += _font_color_alpha_step;
        if (_font_color_alpha < 0 || _font_color_alpha > 1)
        {
          _font_color_alpha_step *= -1;
        }
      }

      // text color loop (@tobe improved)
      if (isColorLoop)
      {
        _elapsedTimeMs += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

        // le temps est dépassé, on ajoute un nouveau ballon
        if (_elapsedTimeMs > _elapsedTimeBtwColorMs)
        {
          // next color or reset
          _font_color_loop_index = (_font_color_loop_index + 1 >= _font_colors.Length) ? 0 : ++_font_color_loop_index;
          _font_color = _font_colors[_font_color_loop_index];
          _elapsedTimeMs = 0;
        }
      }

      // text zoom from x1 to x4
      if (isZoomIn)
      {
        Font_scale += Font_scale < 4.0f ? 0.1f : 0f;
      }

      _font_color = new Color(_font_color, _font_color_alpha);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // mise à jour de la position du message
      UpdatePosition();

      // définir le centre de la chaine de caractère
      Vector2 fontOrigin = _font.MeasureString(Text) / 2;

      // draw background
      if (_text_effect.HasFlag(TextEffect.BACKGROUND))
      {
        int border = 5;
        int X = (int)(_text_position_coordinates.X - fontOrigin.X) - border;
        int Y = (int)(_text_position_coordinates.Y - fontOrigin.Y) - border;
        int dX = (int)_font.MeasureString(Text).X + 2 * border;
        int dY = (int)_font.MeasureString(Text).Y + 2 * border;
        spriteBatch.Draw(
          _game.Content.Load<Texture2D>("solidwhite"),
          new Rectangle(X, Y, dX, dY),
          Color.White * .5f
          );
      }
      // dessiner le message sprite
      spriteBatch.DrawString(
        _font,
        Text,
        _text_position_coordinates,
        _font_color,
        0,
        fontOrigin,
        Font_scale,
        SpriteEffects.None,
        1.0f);
    }

    /// <summary>
    /// mise à jour des coordonnées du texte
    /// </summary>
    public void UpdatePosition()
    {
      Vector2 coordinates = Vector2.Zero;

      Vector2 fontOrigin = _font.MeasureString(Text) / 2;

      int maxWidth = _game.GraphicsDevice.Viewport.Width;
      int maxHeight = _game.GraphicsDevice.Viewport.Height;

      switch (_viewportposition)
      {
        case TextPosition.LEFTCENTERTOP:
          coordinates = new Vector2(maxWidth / 3 - fontOrigin.X / 2, 2 * fontOrigin.Y);
          break;
        case TextPosition.CENTERTOP:
          coordinates = new Vector2(maxWidth / 2, 2 * fontOrigin.Y);
          break;
        case TextPosition.RIGHTCENTERTOP:
          coordinates = new Vector2(2 * (maxWidth / 3) + fontOrigin.X / 2, 2 * fontOrigin.Y);
          break;
        case TextPosition.LEFTCENTERBOTTOM:
          coordinates = new Vector2(maxWidth / 3 - fontOrigin.X / 2, maxHeight - 2 * fontOrigin.Y);
          break;
        case TextPosition.CENTERBOTTOM:
          coordinates = new Vector2(maxWidth / 2, maxHeight - 2 * fontOrigin.Y);
          break;
        case TextPosition.RIGHTCENTERBOTTOM:
          coordinates = new Vector2(2 * (maxWidth / 3) + fontOrigin.X / 2, maxHeight - 2 * fontOrigin.Y);
          break;
        case TextPosition.CENTER:
          coordinates = new Vector2(maxWidth / 2, maxHeight / 2);
          break;
        default:
          coordinates = _text_position_coordinates;
          break;
      }

      // application des coordoonées
      _text_position_coordinates = coordinates;

    }
  }
}
