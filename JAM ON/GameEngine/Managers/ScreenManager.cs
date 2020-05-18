using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;

namespace CPI311.GameEngine
{
    public static class ScreenManager
    {
        private static GraphicsDeviceManager graphics;
        public static GraphicsDevice GraphicsDevice
        {
            get { return graphics.GraphicsDevice; }
        }

        public static void Initialize(GraphicsDeviceManager g)
        {
            ScreenManager.graphics = g;
        }
        public static Viewport DefaultViewport
        {
            get { return new Viewport(0, 0, Width, Height); }
        }

    public static int Width
    {
        get
        {
            return GraphicsDevice.PresentationParameters.BackBufferWidth;
        }
        set
        {
            graphics.PreferredBackBufferWidth = value;
            graphics.ApplyChanges();
        }
    }

    public static int Height
    {
        get
        {
            return GraphicsDevice.PresentationParameters.BackBufferHeight;
        }
        set
        {
            graphics.PreferredBackBufferHeight = value;
            graphics.ApplyChanges();
        }
    }

    public static bool IsFullScreen
    {
        get { return graphics.IsFullScreen; }
        set { graphics.IsFullScreen = value; graphics.ApplyChanges(); }
    }

    public static void Setup(int width = 0, int height = 0)
    {
        Setup(IsFullScreen, width, height);
    }

    public static void Setup(bool fullScreen,
            int width = 0, int height = 0)
    {
        if (width > 0)
            graphics.PreferredBackBufferWidth = width;
        if (height > 0)
            graphics.PreferredBackBufferHeight = height;
        graphics.IsFullScreen = fullScreen;
        graphics.ApplyChanges();
    }
}

}
