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
      BACKGROUND = 1,
      TYPEWRITER = 2
    };

    private Game _game;



    private Rectangle _textboxRectangle;
    private TwtbEffects _effects;
    Texture2D _backgroundColor;

    private SpriteFont _font;
    private Color _fontColor = Color.Black;
    private String _text;
    private String _parsedText;
    private String _typedText;
    private double _typedTextLength;
    private int _delayInMilliseconds;
    private bool _isDoneDrawing;
    private bool _isLoop;

    public Rectangle TextboxRectangle { private get => _textboxRectangle; set => _textboxRectangle = value; }
    public string Text { get => _text; set => _text = value; }
    public TwtbEffects Effects { set => _effects = value; }

    public TypeWriterTextBox(Game game)
    {
      _game = game;
    }

    public virtual void Initialize()
    {
      
    }

    public virtual void LoadContent()
    {
      _backgroundColor = _game.Content.Load<Texture2D>("solidred");

      _font = _game.Content.Load<SpriteFont>("fonts/moire_bold_24");
      _delayInMilliseconds = 75;
      _isDoneDrawing = false;
      _isLoop = true;

      TextboxRectangle = new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);
      _parsedText = parseText(Text);
    }

    public virtual void UnloadContent()
    {

    }

    public virtual void Update(GameTime gameTime)
    {
      bool isTypewriter = _effects.HasFlag(TwtbEffects.TYPEWRITER);

      if (isTypewriter)
      {

        if (!_isDoneDrawing)
        {
          if (_delayInMilliseconds == 0)
          {
            _typedText = _parsedText;
            _isDoneDrawing = true;
          }
          else if (_typedTextLength < _parsedText.Length)
          {
            _typedTextLength = _typedTextLength + gameTime.ElapsedGameTime.TotalMilliseconds / _delayInMilliseconds;

            if (_typedTextLength >= _parsedText.Length)
            {
              _typedTextLength = _parsedText.Length;
              _isDoneDrawing = true;
            }

            _typedText = _parsedText.Substring(0, (int)_typedTextLength);
          }
        }
        else
        {
          if (_isLoop)
          {
            _typedTextLength = 0;
            _parsedText = parseText(Text);
            _typedText = "";
            _isDoneDrawing = false;
          }
        }
      }
      else
      {
        _typedText = _text;
      }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      bool isBackground = _effects.HasFlag(TwtbEffects.BACKGROUND);

      if (isBackground)
        spriteBatch.Draw(_backgroundColor, TextboxRectangle, Color.White);

      spriteBatch.DrawString(
        _font, 
        parseText(_typedText), 
        new Vector2(TextboxRectangle.X, TextboxRectangle.Y), 
        _fontColor);
    }

    private String parseText(String text)
    {
      String line = String.Empty;
      String returnString = String.Empty;
      String[] wordArray = text.Split(' ');

      foreach (String word in wordArray)
      {
        if (_font.MeasureString(line + word).Length() > TextboxRectangle.Width)
        {
          returnString = returnString + line + '\n';
          line = String.Empty;
        }

        line = line + word + ' ';
      }

      return returnString + line;
    }
  }
}
