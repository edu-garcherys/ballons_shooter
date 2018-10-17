/*
 * Classe de sprite animés
 * 
 * todo : descriptif
 */
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

// todo : commentaires
namespace Sprites
{
	class SpriteGenericAnimated : SpriteGeneric
	{
		int _elapsedTime;			// The time since we last updated the frame

		int _frameNbRows;
		int _frameNbCols;
		Point _frameIndex;

		int _frameCount;			// The number of frames that the animation contains
		int _frameTime;				// The time (ms) we display a frame until the next one

		Rectangle _rectSource = new Rectangle();		// The area of the image strip we want to display

		public bool _looping;		// Determines if the animation will keep playing or deactivate after one run

		#region constructeurs
		public SpriteGenericAnimated(Game game)
			: base(game)
		{

		}
		#endregion

		/// <summary>
		/// Initialisation d'un sprite static
		/// </summary>
		public override void Initialize()
		{
			_elapsedTime = 0;

			base.Initialize();
		}

		/// <summary>
		/// Chargement de la texture animée 2D et des informations relatives
		/// </summary>
		/// <param name="textureName">Chemin et nom de la texture dans "Content"</param>
		/// <param name="frameWidth">Largueur de la frame</param>
		/// <param name="frameHeight">Hauteur de la frame</param>
		/// <param name="frameNbRows">Nombre de ligne dans l'image d'animation</param>
		/// <param name="frameNbCols">Nombre de colonnes dans l'image d'animation</param>
		/// <param name="frameTime">Durée pour chaque frame (en ms)</param>
		/// <param name="looping">Animation en boucle</param>
		public void LoadContent(string textureName, int frameWidth, int frameHeight, int frameNbRows, int frameNbCols, int frameTime, bool looping)
		{
			base.LoadContent(textureName);

			_frameWidth = frameWidth;
			_frameHeight = frameHeight;
			_frameNbRows = frameNbRows;
			_frameNbCols = frameNbCols;
			_frameCount = _frameNbRows * _frameNbCols;
			_frameTime = frameTime;

			_frameIndex = new Point(0, 0);

			_looping = looping;
		}

		/// <summary>
		/// Gérer le changement de frame pour l'animation du sprite
		/// + appel à la gestion de l'animation
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			// do not update if not active
			if (!this._isactive) return;

			// update the elapsed time
			_elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (_elapsedTime > _frameTime)
			{
				_frameIndex.X++;

				// mise à jour de l'index de la frame animée
				if (_frameIndex.X >= _frameNbCols)
				{
					_frameIndex.X = 0;
					_frameIndex.Y++;
				}

				if (_frameIndex.Y >= _frameNbRows)
				{
					_frameIndex = new Point(0, 0);

					// if we are not looping deactivate the animation
					if (!_looping)
						_isactive = false;
				}

				// reset the elapsed time to zero
				_elapsedTime = 0;
			}

			// grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
			_rectSource = new Rectangle(
				_frameIndex.X * _frameWidth,
				_frameIndex.Y * _frameHeight, 
				_frameWidth,
				_frameHeight);

			base.Update(gameTime);
		}

		/// <summary>
		/// Affichage d'un sprite animé
		/// </summary>
		/// <param name="spriteBatch"></param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (_isvisible && _isactive)
				spriteBatch.Draw(_texture, _rectDestination, _rectSource, Color.White, _rotation, Vector2.Zero, SpriteEffects.None, 0);
		}

	}
}
