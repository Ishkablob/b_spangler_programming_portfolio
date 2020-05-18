using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public static class InputManager
    {
        static KeyboardState PreviousKeyboardState { get; set; }
        static KeyboardState CurrentKeyboardState { get; set; }
        static MouseState PreviousMouseState { get; set; }
        static MouseState CurrentMouseState { get; set; }

        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState = Mouse.GetState();
        }

        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        { return CurrentKeyboardState.IsKeyDown(key); }

        public static bool IsKeyPressed(Keys key)
        { return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key); }

        public static bool IsKeyReleased(Keys key)
        { return CurrentKeyboardState.IsKeyUp(key) && PreviousKeyboardState.IsKeyDown(key); }
        
        public static Vector2 GetMousePosition()
        { return new Vector2(CurrentMouseState.X, CurrentMouseState.Y); }

        public static MouseState GetPreviousMouseState()
        { return PreviousMouseState; }

        public static bool IsMousePressed(int mouseButton)
        { return PreviousMouseState.LeftButton == ButtonState.Released &&
                CurrentMouseState.LeftButton == ButtonState.Pressed; }

        public static bool IsMouseReleased()
        { return PreviousMouseState.LeftButton == ButtonState.Pressed &&
                CurrentMouseState.LeftButton == ButtonState.Released; }

        public static bool IsMouseHeld()
        {
            return PreviousMouseState.LeftButton == ButtonState.Pressed &&
                  CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsSecondMouseHeld()
        {
            return PreviousMouseState.RightButton == ButtonState.Pressed &&
                  CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool IsMiddleMouseHeld()
        {
            return PreviousMouseState.MiddleButton == ButtonState.Pressed &&
                  CurrentMouseState.MiddleButton == ButtonState.Pressed;
        }
    }
}
