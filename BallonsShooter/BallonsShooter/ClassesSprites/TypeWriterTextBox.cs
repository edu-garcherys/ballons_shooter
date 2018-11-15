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
      NOTYPEWRITTER = 2
    };

    public enum TwtbPosition { FULLSCREEN, LEFTTOP, RIGHTTOP, CENTER, CENTERTOP, CENTERTOPMIDDLE, CENTERBOTTOM, CENTERBOTTOMMIDDLE };

    private int _marginHorizontal = 50;
    private int _marginVertical = 50;

    private Game _game;

    private Rectangle _tbRectangle;
    private TwtbPosition _tbRectanglePosition;
    private Vector2 _tbRectangleSize;
    private TwtbEffects _effects;
    private Color _bgColor = Color.White;
    private float _bgTransparency = 1.0f;

    private SpriteFont _font;
    private Color _fontColor = Color.WhiteSmoke;
    private String _text;
    private String _parsedText;
    private String _typedText;
    private double _typedTextLength;
    private int _delayInMs;
    private bool _isDoneDrawing;
    private bool _isLoop;
    private int _loopEndDelay = 1000;
    private int _loopEndDelayElapsed = 0;

    public Rectangle TbRectangle
    {
      private get => _tbRectangle;
      set => _tbRectangle = value;
    }
    public string Text
    {
      get => _text == null ? "" : _text;
      set => _text = value;
    }
    public TwtbEffects Effects { set => _effects = value; }
    internal TwtbPosition TbRectanglePosition { get => _tbRectanglePosition; set => _tbRectanglePosition = value; }
    public int DelayInMs { set => _delayInMs = value; }
    public Color FontColor { set => _fontColor = value; }
    public Color BgColor { get => _bgColor; set => _bgColor = value; }
    public float BgTransparency { get => _bgTransparency; set => _bgTransparency = value; }
    public int MarginVertical { get => _marginVertical; set => _marginVertical = value; }

    public TypeWriterTextBox(Game game)
    {
      _game = game;

      BgTransparency = 1.0f;
      // default rectangle
      _tbRectangleSize = new Vector2(500, 100);
      _delayInMs = 100;
    }

    public virtual void Initialize()
    {

    }

    public virtual void LoadContent()
    {
      _font = _game.Content.Load<SpriteFont>("fonts/CaptureSmallz");

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

      // no typing effect
      if (_effects.HasFlag(TwtbEffects.NOTYPEWRITTER))
      {
        _typedText = Text;
        return;
      }

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
        _loopEndDelayElapsed += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

        if (_isLoop && _loopEndDelayElapsed > _loopEndDelay)
        {
          // reset text writer text
          _typedTextLength = 0;
          _parsedText = ParseText(Text);
          _typedText = "";
          _isDoneDrawing = false;
          _loopEndDelayElapsed = 0;
        }
      }

    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      // draw effect
      if (_effects.HasFlag(TwtbEffects.BACKGROUND))
      {
        spriteBatch.Draw(_game.Content.Load<Texture2D>("solidwhite"), TbRectangle, BgColor * BgTransparency);
      }

      // draw text
      spriteBatch.DrawString(
        _font,
        ParseText(_typedText),
        new Vector2(TbRectangle.X + _marginHorizontal / 2, TbRectangle.Y),
        _fontColor);
    }

    private String ParseText(String text)
    {

      String line = String.Empty;
      String returnString = String.Empty;

      String[] wordArray = Text.Split(' ');

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

      // update rectangle to max screen size
      int nbLines = ((int)(_font.MeasureString(Text).X + _marginHorizontal * 2) / maxWidth) + 1;
      _tbRectangleSize = new Vector2(
        nbLines == 1 ? _font.MeasureString(Text).X + 50 : maxWidth - (_marginHorizontal * 2),
        nbLines * _font.MeasureString(Text).Y
        );

      int availableWidth = maxWidth - (_marginHorizontal * 2);
      int availableHeight = maxHeight - (MarginVertical * 2);
      int center = (int)(availableWidth - _tbRectangleSize.X) / 2 + _marginHorizontal;
      int left = (int)((availableWidth / 4) - (_tbRectangleSize.X / 2));
      int right = (int)((availableWidth / 4) * 3 - (_tbRectangleSize.X / 2));

      int middle = (int)(availableHeight - _tbRectangleSize.Y) / 2;
      int top = MarginVertical;
      int bottom = (int)(availableHeight - _tbRectangleSize.Y);

      switch (_tbRectanglePosition)
      {
        case TwtbPosition.FULLSCREEN:
          TbRectangle = new Rectangle(_marginHorizontal, MarginVertical, maxWidth - _marginHorizontal, maxHeight - MarginVertical);
          break;
        case TwtbPosition.CENTER:
          TbRectangle = new Rectangle(center, middle, (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
        case TwtbPosition.CENTERTOP:
          TbRectangle = new Rectangle(center, 0, (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
        case TwtbPosition.CENTERBOTTOM:
          TbRectangle = new Rectangle(center, bottom, (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
        case TwtbPosition.CENTERBOTTOMMIDDLE:
          TbRectangle = new Rectangle(center, (int)((maxHeight - _tbRectangleSize.Y) * 2 / 3), (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
        case TwtbPosition.CENTERTOPMIDDLE:
          TbRectangle = new Rectangle(center, (int)((maxHeight - _tbRectangleSize.Y) * 1 / 3), (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
        case TwtbPosition.LEFTTOP:
          TbRectangle = new Rectangle(left, MarginVertical, (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
        case TwtbPosition.RIGHTTOP:
          TbRectangle = new Rectangle(right, MarginVertical, (int)_tbRectangleSize.X, (int)_tbRectangleSize.Y);
          break;
      }

    }
  }
}
