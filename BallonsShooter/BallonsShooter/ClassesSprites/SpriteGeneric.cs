/*
 * classe Generic pour les sprites
 * 
 * Gestion de base pour des sprites.
 * 
 */
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Sprites
{
	public class SpriteGeneric
	{
		protected Game _game;								// référence vers la classe du jeu
		private static Random _random = new Random();		// générateur de nombre aléatoire pour la classe SpriteGeneric

		// gestion de l'affichage du sprite
		protected bool _isvisible;
		public bool IsVisible
		{
			get ; set;
		}

		// liste d'énumeration pour positionner le sprite par rapport à l'écran
		public enum ViewportPosition { TOPLEFT, TOPCENTER, TOPRIGHT, BOTTOMLEFT, BOTTOMCENTER, BOTTOMRIGHT, CENTER, RANDOMTOP_OUTSIDE };

		// gestion de la position du sprite
		protected Vector2 _position;
		/// <summary>
		/// Position (coordonnées) du centre du sprite dans l'écran.
		/// </summary>
		public Vector2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		// gestion du vecteur de déplacement du sprite si active
		protected Vector2 _speed;
		public Vector2 Speed
		{
			set { _speed = value; }
		}

		// gestion de la texture
		protected Texture2D _texture;

		// gestion de l'échelle pour l'affichage du sprite
		protected float _scale;
		public float Scale
		{
			set
			{
				if (_scale < 0) value = 0;
				_scale = value;
			}
		}

		// angle de rotation du sprite
		// todo: implementer la rotation
		protected float _rotation;

		// le sprite est actif ou non pour les calculs durant l'update
		protected bool _isactive;
		public bool IsActive
		{
			get { return _isactive; }
			set { _isactive = value; }
		}

		// gestion du rectangle pour les collisions
		protected Rectangle _rectangle;
		public Rectangle Rectangle
		{
			get { return _rectangle; }
		}


    // todo: commentaires
    private int _frameWidth;     // Width of a given frame
    public int FrameWidth {  get => _frameWidth; protected set => _frameWidth = value; }

    private int _frameHeight;      // Height of a given frame
    public int FrameHeight { get => _frameHeight; protected set => _frameHeight = value; }

    protected Rectangle _rectDestination = new Rectangle();	// The area where we want to display the image strip in the game

		#region constructeur
		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="visible"></param>
		/// <param name="active"></param>
		/// <param name="position"></param>
		/// <param name="speed"></param>
		/// <param name="scale"></param>
		/// <param name="rotation"></param>
		public SpriteGeneric(Game game, bool visible, bool active, Vector2 position, Vector2 speed, float scale, float rotation)
		{
			_game = game;

			_isvisible = visible;
			_isactive = active;

			// position et vitesse initiales
			_position = position;
			_speed = speed;

			// echelle et orientation initiales
			Scale = scale;
			_rotation = rotation;
		}

		/// <summary>
		/// CTor
		/// Le sprite par défaut est visible et actif. 
		/// Il s'affiche dans le coin supérieur gauche sans vitesse de déplacement.
		/// Son échelle et son orientation ne sont pas modifiés.
		/// </summary>
		/// <param name="game"></param>
		public SpriteGeneric(Game game)
			: this(game, true, true, Vector2.Zero, Vector2.Zero, 1f, 0f)
		{
		}
		#endregion

		/// <summary>
		/// Initialisation d'un sprite static
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Chargement de la texture 2D
		/// </summary>
		/// <param name="textureName">Chemin et nom de la texture dans "Content"</param>
		public virtual void LoadContent(string textureName)
		{
      _texture = _game.Content.Load<Texture2D>(textureName);
      FrameHeight = _texture.Height;
      FrameWidth = _texture.Width;
    }

    /// <summary>
    /// Libération de la mémoire
    /// </summary>
    public virtual void UnloadContent()
		{
			if (_texture != null)
				_texture.Dispose();
		}

		/// <summary>
		/// Mise à jour de la position du sprite si il est actif
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Update(GameTime gameTime)
		{
			if (!_isactive) return;

			// déplacement de la position suivant la vitesse
			_position += _speed;

			// rectangle d'affichage pour le sprite
			_rectDestination = new Rectangle(
				(int)_position.X - (int)(FrameWidth * _scale) / 2,
				(int)_position.Y - (int)(FrameHeight * _scale) / 2,
				(int)(FrameWidth * _scale),
				(int)(FrameHeight * _scale));


			// calcul du rectangle englobant nécessaire pour les collisions
			_rectangle = new Rectangle(
				(int)_position.X,
				(int)_position.Y,
				(int)_texture.Width,
				(int)_texture.Height);
		}

		/// <summary>
		/// Affichage du sprite si il est visible
		/// </summary>
		/// <param name="spriteBatch"></param>
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (_isvisible)
				spriteBatch.Draw(_texture, _rectDestination, null, Color.White, _rotation, Vector2.Zero, SpriteEffects.None, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public void SetPosition(ViewportPosition p)
		{
			Vector2 coordonnees = Vector2.Zero;

			int realWidth = (int)(_texture.Width * _scale);
			int realHeight = (int)(_texture.Height * _scale);

			int maxWidth = _game.GraphicsDevice.Viewport.Width - (realWidth / 2);
			int maxHeight = _game.GraphicsDevice.Viewport.Height - (realHeight / 2);

			switch (p)
			{
				case ViewportPosition.TOPLEFT:
					break;
				case ViewportPosition.TOPCENTER:
					coordonnees = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, 0);
					break;
				case ViewportPosition.TOPRIGHT:
					coordonnees = new Vector2(maxWidth, 0);
					break;
				case ViewportPosition.BOTTOMLEFT:
					coordonnees = new Vector2(0, maxHeight);
					break;
				case ViewportPosition.BOTTOMCENTER:
					coordonnees = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, maxHeight);
					break;
				case ViewportPosition.BOTTOMRIGHT:
					coordonnees = new Vector2(maxWidth, maxHeight);
					break;
				case ViewportPosition.CENTER:
					coordonnees = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);
					break;
				case ViewportPosition.RANDOMTOP_OUTSIDE:
					int rand = _random.Next(0, _game.GraphicsDevice.Viewport.Width - realWidth);
					coordonnees = new Vector2((int)(rand + realWidth / 2),(int)(-realHeight / 2));
					break;
			}

			// application des coordoonées
			_position = coordonnees;
		}

		// todo : not yet used
		static Predicate<SpriteGeneric> ToBeRemoved()
		{
			return delegate(SpriteGeneric sprite)
			{
				return !sprite._isvisible;
			};
		}
	}
}
