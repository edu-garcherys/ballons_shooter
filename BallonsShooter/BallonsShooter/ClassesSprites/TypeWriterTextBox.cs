using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprites
{
  class TypeWriterTextBox
  {
    [Flags]
    public enum TwtbEffects
    {
      NONE = 0,
      BACKGROUND = 1
    };

    public enum TwtbPosition { FULLSCREEN, CENTER };

    private Game _game;

    private Rectangle _tbRectangle;
    private TwtbPosition _tbRectanglePosition;
    private Vector2 _tbRectangleSize;
    private TwtbEffects _effects;
    private Texture2D _bgColor;

    private SpriteFont _font;
    private Color _fontColor = Color.WhiteSmoke;
    private String _text;
    private String _parsedText;
    private String _typedText;
    private double _typedTextLength;
    private int _delayInMs;
    private bool _isDoneDrawing;
    private bool _isLoop;

    public Rectangle TbRectangle { private get => _tbRectangle; set => _tbRectangle = value; }
    public string Text { get => _text == null ? "" : _text; set => _text = value; }
    public TwtbEffects Effects { set => _effects = value; }
    internal TwtbPosition TbRectanglePosition { get => _tbRectanglePosition; set => _tbRectanglePosition = value; }
    public int DelayInMs { set => _delayInMs = value; }

    public TypeWriterTextBox(Game game)
    {
      _game = game;

      // full screen by default
      _tbRectangleSize = new Vector2(500,100);
      _delayInMs = 100;
    }

    public virtual void Initialize()
    {
      
    }

    public virtual void LoadContent()
    {
      _bgColor = _game.Content.Load<Texture2D>("solidwhite");
      _font = _game.Content.Load<SpriteFont>("fonts/moire_bold_24");

      _isDoneDrawing = false;
      _isLoop = true;
      _parsedText = Text;

      UpdateTextboxRectangle();
    }

    public virtual void UnloadContent()
    {

    }

    public virtual void Update(GameTime gameTime)
    {
      UpdateTextboxRectangle();

      if (!_isDoneDrawing)
      {
        // no typing
        if (_delayInMs == 0)
        {
          _typedText = _parsedText;
          _isDoneDrawing = true;
        }
        else if (_typedTextLength < _parsedText.Length)
        {
          // compute text current length
          _typedTextLength = _typedTextLength + (gameTime.ElapsedGameTime.TotalMilliseconds / _delayInMs);

          // is finished ?
          if (_typedTextLength >= _parsedText.Length)
          {
            _typedTextLength = _parsedText.Length;
            _isDoneDrawing = true;
          }
          // set new typed text
          _typedText = _parsedText.Substring(0, (int)_typedTextLength);
        }
      }
      else
      {
        if (_isLoop)
        {
          // reset text writer text
          _typedTextLength = 0;
          _parsedText = ParseText(Text);
          _typedText = "";
          _isDoneDrawing = false;
        }
      }

    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // draw effect
      if (_effects.HasFlag(TwtbEffects.BACKGROUND))
      {
        spriteBatch.Draw(_bgColor, TbRectangle, Color.White * 0.5f);
      }

      // draw text
      spriteBatch.DrawString(
        _font,
        ParseText(_typedText),
        new Vector2(TbRectangle.X, TbRectangle.Y),
        _fontColor);
    }

    private String ParseText(String text)
    {
      String line = String.Empty;
      String returnString = String.Empty;

      // protect text == null
      text = text == null ? "" : text;
      String[] wordArray = text.Split(' ');

      foreach (String word in wordArray)
      {
        if (_font.MeasureString(line + word).Length() > _tbRectangle.Width)
        {
          returnString = returnString + line + '\n';
          line = String.Empty;
        }

        line = line + word + ' ';
      }

      return returnString + line;
    }

    private void UpdateTextboxRectangle()
    {
      int maxWidth = _game.GraphicsDevice.Viewport.Width;
      int maxHeight = _game.GraphicsDevice.Viewport.Height;

      switch (_tbRectanglePosition)
      {
        case TwtbPosition.FULLSCREEN:
          TbRectangle = new Rectangle(0, 0, maxWidth, maxHeight);
          break;
        case TwtbPosition.CENTER:
          TbRectangle = new Rectangle((int)(maxWidth - _tbRectangleSize.X) / 2, (int)(maxHeight - _tbRectangleSize.Y) / 2, (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
      }

    }
  }
}
