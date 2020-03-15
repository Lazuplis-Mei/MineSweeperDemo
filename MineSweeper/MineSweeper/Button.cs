using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{

    class MouseStateEventArgs : EventArgs
    {
        public MouseState MouseState { get; set; }
        public MouseStateEventArgs(MouseState mouseState)
        {
            MouseState = mouseState;
        }
    }

    delegate void MouseStateEventHandler(Button sender, MouseStateEventArgs e);

    class Button : IGameComponent, IUpdateable, IDrawable
    {

        public static Button FocusedButton;

        #region Propertys

        public Texture2D Sprite { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Vector2 TextPosition;
        public Color BackGround { get; set; }
        public Color ForeGround { get; set; }
        public Color HoveredColor { get; set; }
        public Color PressedColor { get; set; }
        public bool IsMouseOver { get; private set; }
        public bool IsPressed { get; private set; }
        public bool IsRPressed { get; private set; }
        public bool IsFocused { get; private set; }
        public Vector2 Position;
        public int Width => Sprite.Width;
        public int Height => Sprite.Height;
        public bool Fixed { get; set; }

        #endregion

        #region Events

        public event MouseStateEventHandler Pressed;
        public event MouseStateEventHandler RPressed;
        public event MouseStateEventHandler BothPressed;
        public event MouseStateEventHandler Clicked;
        public event MouseStateEventHandler RClicked;
        public event MouseStateEventHandler BothClicked;

        #endregion

        #region Updateable

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value) EnabledChanged?.Invoke(this, EventArgs.Empty);
                _enabled = value;
            }
        }
        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value) UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                 _updateOrder = value;
            }
        }
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        #endregion

        #region Drawable

        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder != value) DrawOrderChanged?.Invoke(this, EventArgs.Empty);
                _drawOrder = value;
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value) VisibleChanged?.Invoke(this, EventArgs.Empty);
                _visible = value;
            }
        }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        #endregion

        #region private fields

        GraphicsDevice _graphicsDevice;
        SpriteBatch _spriteBatch;
        MouseState _lastMouseState;
        Color _color;
        Rectangle _collision;
        bool _enabled;
        bool _visible;
        int _drawOrder;
        int _updateOrder;

        #endregion

        public Button(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            if (_graphicsDevice == null || _graphicsDevice.IsDisposed)
            {
                throw new InvalidOperationException("GraphicsDevice不可用");
            }
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            
        }
        public void Initialize()
        {
            Enabled = true;
            Visible = true;
            _color = BackGround;
            
        }

        public virtual void Update(GameTime gameTime)
        {
            _collision = Sprite.Bounds;
            _collision.Location = Position.ToPoint();
            MouseState mouse = Mouse.GetState();
            
            IsMouseOver = _collision.Contains(mouse.Position);
            

            if (IsMouseOver)
            {
                bool leftbuttonjustpressed = _lastMouseState.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed;
                bool rightbuttonjustpressed = _lastMouseState.RightButton == ButtonState.Released && mouse.RightButton == ButtonState.Pressed;
                if (leftbuttonjustpressed && rightbuttonjustpressed)
                {
                    IsFocused = true;
                    FocusedButton = this;
                    BothPressed?.Invoke(this, new MouseStateEventArgs(mouse));
                }
                else
                {
                    if (leftbuttonjustpressed && mouse.RightButton == ButtonState.Released)
                    {
                        IsFocused = true;
                        FocusedButton = this;
                        Pressed?.Invoke(this, new MouseStateEventArgs(mouse));
                    }
                    if (rightbuttonjustpressed && mouse.LeftButton == ButtonState.Released)
                    {
                        IsFocused = true;
                        FocusedButton = this;
                        RPressed?.Invoke(this, new MouseStateEventArgs(mouse));
                    }
                }

                if (IsPressed && mouse.LeftButton == ButtonState.Released && IsRPressed && mouse.RightButton == ButtonState.Released)
                {
                    IsFocused = false;
                    IsRPressed = false;
                    IsPressed = false;
                    BothClicked?.Invoke(this, new MouseStateEventArgs(mouse));
                }
                else
                {
                    if (IsPressed && mouse.LeftButton == ButtonState.Released)
                    {
                        IsFocused = false;
                        IsPressed = false;
                        if (mouse.RightButton == ButtonState.Pressed)
                        {
                            IsRPressed = false;
                            BothClicked?.Invoke(this, new MouseStateEventArgs(mouse));
                        }
                        else
                        {
                            Clicked?.Invoke(this, new MouseStateEventArgs(mouse));
                        }
                    }
                    if (IsRPressed && mouse.RightButton == ButtonState.Released)
                    {
                        IsFocused = false;
                        IsRPressed = false;
                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            IsPressed = false;
                            BothClicked?.Invoke(this, new MouseStateEventArgs(mouse));
                        }
                        else
                        {
                            RClicked?.Invoke(this, new MouseStateEventArgs(mouse));
                        }

                    }
                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (IsFocused)
                    {
                        _color = PressedColor;
                        IsPressed = true;
                    }
                    else
                    {
                        _color = BackGround;
                    }
                }
                if (mouse.RightButton == ButtonState.Pressed)
                {
                    if (IsFocused)
                    {
                        _color = PressedColor;
                        IsRPressed = true;
                    }
                    else
                    {
                        _color = BackGround;
                    }
                }

                if (mouse.LeftButton == ButtonState.Released && mouse.RightButton == ButtonState.Released)
                {
                    _color = HoveredColor;
                    IsPressed = false;
                    IsRPressed = false;
                }
            }
            else
            {
                IsPressed = false;
                IsRPressed = false;
                if (IsFocused && (mouse.LeftButton == ButtonState.Pressed || mouse.RightButton == ButtonState.Pressed))
                {
                    _color = HoveredColor;
                }
                else
                {
                    IsFocused = false;
                    _color = BackGround;
                }
            }

            _lastMouseState = mouse;
        }
        public virtual void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (Sprite != null) _spriteBatch.Draw(Sprite, Position, Fixed ? BackGround : _color);
            if (Font != null && !string.IsNullOrWhiteSpace(Text))
                _spriteBatch.DrawString(Font, Text, Position + TextPosition, ForeGround);
            _spriteBatch.End();
        }

        public void DoClick()
        {
            Clicked?.Invoke(this, null);
        }

    }
}
