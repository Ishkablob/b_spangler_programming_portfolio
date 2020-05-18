using CPI311.GameEngine;
using CPI311.GameEngine.GUI;
using CPI311.GameEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace CPI311_Final
{
    public class Scene
    {
        public delegate void CallMethod();
        public CallMethod Update;
        public CallMethod Draw;
        public Scene(CallMethod update, CallMethod draw)
        { Update = update; Draw = draw; }
    }

    public class GameNote : GameObject
    {
        Keys InputKey { get; set; }
        Color HighlightColor { get; set; }
        public Effect Effect { get; set; }
        public bool isActive { get; set; }
        public bool Lock { get; set; }
        public GameNote(Keys key, Color color, Effect effect, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
        : base()
        {
            InputKey = key;
            HighlightColor = color;
            Effect = effect;
            isActive = false;
            Lock = false;

            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Model m = Content.Load<Model>("Box");
            (m.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();
            Renderer renderer = new Renderer(m, Transform, camera, light,
            Content, graphicsDevice, 1, 20f, texture, null);
            Add<Renderer>(renderer);
            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }

        public override void Update()
        {
            if (InputManager.IsKeyDown(InputKey))
            {
                Effect.Parameters["DiffuseColor"].SetValue(HighlightColor.ToVector3());
                (this.Renderer.ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = HighlightColor.ToVector3();
                isActive = true;
            }
            else
            {
                Effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                (this.Renderer.ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.White.ToVector3();
                isActive = false;
                Lock = false;
            }
        }
    }

    public class SongNote : GameObject
    {
        public float StartMovingTime { get; set; }
        public int NoteNum { get; set; }
        public SongNote(float time, int notenum, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
        : base()
        {
            StartMovingTime = time;
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Model m = Content.Load<Model>("Box");
            (m.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();
            Renderer renderer = new Renderer(m, Transform, camera, light,
            Content, graphicsDevice, 1, 20f, texture, null);
            Add<Renderer>(renderer);
            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            
            switch(notenum)
            {
                case 0:
                    this.Transform.Position = new Vector3(-10, 4, -10);
                    break;
                case 1:
                    this.Transform.Position = new Vector3(-5, 4, -10);
                    break;
                case 2:
                    this.Transform.Position = new Vector3(0, 4, -10);
                    break;
                case 3:
                    this.Transform.Position = new Vector3(5, 4, -10);
                    break;
                case 4:
                    this.Transform.Position = new Vector3(10, 4, -10);
                    break;
                default:
                    this.Transform.Position = new Vector3(-10, 4, -10);
                    break;
            }

            this.Transform.LocalScale = Vector3.One * 0.5f;
        }

        public override void Update()
        {
            if (StartMovingTime <= 0)
                this.Transform.LocalPosition -= this.Transform.Forward * Time.ElapsedGameTime * 20;
            else
                StartMovingTime -= Time.ElapsedGameTime;
        }
    }

    public class Final : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<string, Scene> scenes;
        Scene currentScene;

        SpriteFont font;
        Texture2D texture;

        Texture2D help1;
        Texture2D help2;

        List<GUIElement> SongSelectElements;
        List<GameNote> noteBlocks;
        List<SongNote> songBlocks;

        Button playButton;
        Button returnButton;
        Button song1Button;
        Button song2Button;
        Button volumeUp;
        Button volumeDown;
        Button options;
        Button optionsBack;
        Button credits;
        Button creditsBack;
        Button songSelectBack;

        // Song1: Toto - Africa; 92 bpm; 269 total notes
        Song song1;
        // Song2: Hatsune Miku - Levan Polka; 118 bpm; 294 total notes
        Song song2;
        Song currentSong;
        Song volumeEffect;

        int highscore1 = -1;
        int highscore2 = -1;
        int highscore3 = -1;

        Effect effectNote1;
        Effect effectNote2;
        Effect effectNote3;
        Effect effectNote4;
        Effect effectNote5;
        Effect effectSongNote;

        Camera camera;
        Light light;

        Model plane;
        Transform planeTransform;

        float songCountdown = 5;

        int totalNotes;
        int missedNotes = 0;
        int notesPlayed;
        int score = -1;

        public Final()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();
            ScreenManager.Initialize(graphics);
            ScreenManager.Setup(false, 1920, 1080);
            scenes = new Dictionary<string, Scene>();
            scenes.Add("Main", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("SongSelect", new Scene(SongSelectUpdate, SongSelectDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            scenes.Add("Results", new Scene(ResultsUpdate, ResultsDraw));
            scenes.Add("Options", new Scene(OptionsUpdate, OptionsDraw));
            scenes.Add("Credits", new Scene(CreditsUpdate, CreditsDraw));
            currentScene = scenes["Main"];


            SongSelectElements = new List<GUIElement>();
            noteBlocks = new List<GameNote>();
            songBlocks = new List<SongNote>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = new Vector3(-2, 15, 38);
            camera.Transform.Rotate(Vector3.Left, 0.20f);
            light = new Light();
            light.Transform = new Transform();

            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");

            #region Set Up Block Shaders
            effectNote1 = Content.Load<Effect>("Note1Shading");
            effectNote1.CurrentTechnique = effectNote1.Techniques[0];
            effectNote1.Parameters["View"].SetValue(camera.View);
            effectNote1.Parameters["Projection"].SetValue(camera.Projection);
            effectNote1.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effectNote1.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effectNote1.Parameters["Shininess"].SetValue(20f);
            effectNote1.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effectNote1.Parameters["SpecularColor"].SetValue(new Vector3(0.09f, 0.09f, 0.09f));
            effectNote1.Parameters["DiffuseTexture"].SetValue(texture);

            effectNote2 = Content.Load<Effect>("Note2Shading");
            effectNote2.CurrentTechnique = effectNote2.Techniques[0];
            effectNote2.Parameters["View"].SetValue(camera.View);
            effectNote2.Parameters["Projection"].SetValue(camera.Projection);
            effectNote2.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effectNote2.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effectNote2.Parameters["Shininess"].SetValue(20f);
            effectNote2.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effectNote2.Parameters["SpecularColor"].SetValue(new Vector3(0.09f, 0.09f, 0.09f));
            effectNote1.Parameters["DiffuseTexture"].SetValue(texture);

            effectNote3 = Content.Load<Effect>("Note3Shading");
            effectNote3.CurrentTechnique = effectNote3.Techniques[0];
            effectNote3.Parameters["View"].SetValue(camera.View);
            effectNote3.Parameters["Projection"].SetValue(camera.Projection);
            effectNote3.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effectNote3.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effectNote3.Parameters["Shininess"].SetValue(20f);
            effectNote3.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effectNote3.Parameters["SpecularColor"].SetValue(new Vector3(0.09f, 0.09f, 0.09f));
            effectNote1.Parameters["DiffuseTexture"].SetValue(texture);

            effectNote4 = Content.Load<Effect>("Note4Shading");
            effectNote4.CurrentTechnique = effectNote4.Techniques[0];
            effectNote4.Parameters["View"].SetValue(camera.View);
            effectNote4.Parameters["Projection"].SetValue(camera.Projection);
            effectNote4.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effectNote4.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effectNote4.Parameters["Shininess"].SetValue(20f);
            effectNote4.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effectNote4.Parameters["SpecularColor"].SetValue(new Vector3(0.09f, 0.09f, 0.09f));
            effectNote4.Parameters["DiffuseTexture"].SetValue(texture);

            effectNote5 = Content.Load<Effect>("Note5Shading");
            effectNote5.CurrentTechnique = effectNote5.Techniques[0];
            effectNote5.Parameters["View"].SetValue(camera.View);
            effectNote5.Parameters["Projection"].SetValue(camera.Projection);
            effectNote5.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effectNote5.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effectNote5.Parameters["Shininess"].SetValue(20f);
            effectNote5.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effectNote5.Parameters["SpecularColor"].SetValue(new Vector3(0.09f, 0.09f, 0.09f));
            effectNote5.Parameters["DiffuseTexture"].SetValue(texture);

            effectSongNote = Content.Load<Effect>("SongNoteShading");
            effectSongNote.CurrentTechnique = effectSongNote.Techniques[0];
            effectSongNote.Parameters["View"].SetValue(camera.View);
            effectSongNote.Parameters["Projection"].SetValue(camera.Projection);
            effectSongNote.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effectSongNote.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effectSongNote.Parameters["Shininess"].SetValue(20f);
            effectSongNote.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effectSongNote.Parameters["SpecularColor"].SetValue(new Vector3(0.09f, 0.09f, 0.09f));
            effectSongNote.Parameters["DiffuseTexture"].SetValue(texture);

            #endregion
            
            song1 = Content.Load<Song>("Toto Africa");
            song2 = Content.Load<Song>("Miku Levan Polka");
            volumeEffect = Content.Load<Song>("noise");

            #region Set Up Buttons
            playButton = new Button();
            playButton.Texture = texture;
            playButton.Text = "   Play Game";
            playButton.Bounds = new Rectangle(50, 75, 300, 20);
            playButton.Action += MainToSongSelect;

            song1Button = new Button();
            song1Button.Texture = texture;
            song1Button.Text = "   Africa - Toto : Difficulty - Medium";
            song1Button.Bounds = new Rectangle(50, 75, 300, 20);
            song1Button.Action += MainToSong1;
            SongSelectElements.Add(song1Button);

            song2Button = new Button();
            song2Button.Texture = texture;
            song2Button.Text = "   Levan Polka - Hatsune Miku : Difficulty - Insane";
            song2Button.Bounds = new Rectangle(50, 125, 350, 20);
            song2Button.Action += MainToSong2;
            SongSelectElements.Add(song2Button);

            returnButton = new Button();
            returnButton.Texture = texture;
            returnButton.Text = "   Return To Main Menu";
            returnButton.Bounds = new Rectangle(50, 100, 300, 20);
            returnButton.Action += ReturnToMain;

            volumeUp = new Button();
            volumeUp.Texture = texture;
            volumeUp.Text = "  +";
            volumeUp.Bounds = new Rectangle(100, 100, 40, 40);
            volumeUp.Action += VolumeUp;

            volumeDown = new Button();
            volumeDown.Texture = texture;
            volumeDown.Text = "  -";
            volumeDown.Bounds = new Rectangle(50, 100, 40, 40);
            volumeDown.Action += VolumeDown;

            options = new Button();
            options.Texture = texture;
            options.Text = "   Options/Rules";
            options.Bounds = new Rectangle(50, 125, 300, 20);
            options.Action += MainToOptions;

            optionsBack = new Button();
            optionsBack.Texture = texture;
            optionsBack.Text = "   Back";
            optionsBack.Bounds = new Rectangle(50, 150, 300, 20);
            optionsBack.Action += ReturnToMain;

            credits = new Button();
            credits.Texture = texture;
            credits.Text = "   Credits";
            credits.Bounds = new Rectangle(50, 175, 300, 20);
            credits.Action += MainToCredits;

            creditsBack = new Button();
            creditsBack.Texture = texture;
            creditsBack.Text = "   Back";
            creditsBack.Bounds = new Rectangle(50, 400, 300, 20);
            creditsBack.Action += ReturnToMain;

            songSelectBack = new Button();
            songSelectBack.Texture = texture;
            songSelectBack.Text = "   Back";
            songSelectBack.Bounds = new Rectangle(50, 175, 300, 20);
            songSelectBack.Action += ReturnToMain;
            SongSelectElements.Add(songSelectBack);
            #endregion

            MediaPlayer.Volume = 0.75f;

            Vector3 position = new Vector3(-10, 4, 25);
            for (int i = 0; i < 5; i++)
            {
                Keys key = new Keys();
                Color color = new Color();
                Effect effectNote = Content.Load<Effect>("Note1Shading");
                switch (i)
                {
                    case 0:
                        key = Keys.Q;
                        color = Color.Blue;
                        effectNote = effectNote1;
                        break;
                    case 1:
                        key = Keys.W;
                        color = Color.Green;
                        effectNote = effectNote2;
                        break;
                    case 2:
                        key = Keys.E;
                        color = Color.Yellow;
                        effectNote = effectNote3;
                        break;
                    case 3:
                        key = Keys.R;
                        color = Color.Red;
                        effectNote = effectNote4;
                        break;
                    case 4:
                        key = Keys.T;
                        color = Color.Purple;
                        effectNote = effectNote5;
                        break;
                }
                GameNote note = new GameNote(key, color, effectNote, Content, camera, GraphicsDevice, light);
                note.Transform.Position = position;
                position.X = position.X + 5;
                noteBlocks.Add(note);
            }

            plane = Content.Load<Model>("Plane");
            planeTransform = new Transform();
            planeTransform.LocalPosition = new Vector3(5, 0, 15);

            help1 = Content.Load<Texture2D>("explanation1");
            help2 = Content.Load<Texture2D>("explanation2");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            currentScene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (currentScene == scenes["Options"])
                GraphicsDevice.Clear(Color.Green);

            else
                GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.DepthStencilState = new DepthStencilState();
            currentScene.Draw();

            base.Draw(gameTime);
        }

        #region Events
        void MainToSongSelect(GUIElement element)
        {
            currentScene = scenes["SongSelect"];
        }

        void MainToSong1(GUIElement element)
        {
            currentScene = scenes["Play"];
            currentSong = song1;
            totalNotes = 269;
            notesPlayed = 0;
            missedNotes = 0;
            score = -1;
            songCountdown = 5;
        }

        void MainToSong2(GUIElement element)
        {
            currentScene = scenes["Play"];
            currentSong = song2;
            totalNotes = 294;
            notesPlayed = 0;
            missedNotes = 0;
            score = -1;
            songCountdown = 5;
        }

        void ReturnToMain(GUIElement element)
        {
            currentScene = scenes["Main"];
        }

        void VolumeUp(GUIElement element)
        {
            if (MediaPlayer.Volume != 1)
                MediaPlayer.Volume += .25f;
            MediaPlayer.Play(volumeEffect);
        }

        void VolumeDown(GUIElement element)
        {
            if (MediaPlayer.Volume != 0)
                MediaPlayer.Volume -= .25f;
            MediaPlayer.Play(volumeEffect);
        }

        void MainToOptions(GUIElement element)
        {
            currentScene = scenes["Options"];
        }

        void MainToCredits(GUIElement element)
        {
            currentScene = scenes["Credits"];
        }
        #endregion

        #region Update and Draw Functions
        void MainMenuUpdate()
        {
            playButton.Update();
            options.Update();
            credits.Update();
        }

        void MainMenuDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "JAM ON!!!\nThe Keyboard Rhythm Game", new Vector2(20, 20), Color.White);
            playButton.Draw(spriteBatch, font);
            options.Draw(spriteBatch, font);
            credits.Draw(spriteBatch, font);
            spriteBatch.End();
        }

        void SongSelectUpdate()
        {
            foreach (GUIElement element in SongSelectElements)
                element.Update();
        }

        void SongSelectDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Select Song:", new Vector2(20, 20), Color.White);
            foreach (GUIElement element in SongSelectElements)
                element.Draw(spriteBatch, font);
            spriteBatch.End();
        }

        void PlayUpdate()
        {
            if (songCountdown >= 0)
                songCountdown -= Time.ElapsedGameTime;

            else if (notesPlayed >= totalNotes && MediaPlayer.State == MediaState.Stopped)
            {
                CalculateScore();
                currentScene = scenes["Results"];
            }

            else if (songCountdown <= 0 && MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(currentSong);
                if (currentSong == song1)
                    Song1Map();
                else if (currentSong == song2)
                    Song2Map();
            }

            foreach (GameNote gameNote in noteBlocks)
                gameNote.Update();

            bool removeBlock = false;
            foreach (SongNote songNote in songBlocks)
            {
                removeBlock = false;
                songNote.Update();
                Vector3 normal;
                foreach (GameNote gameNote in noteBlocks)
                    if (songNote.Collider.Collides(gameNote.Collider, out normal) && gameNote.isActive
                        && !removeBlock && !gameNote.Lock)
                    {
                        notesPlayed++;
                        songBlocks.Remove(songNote);
                        removeBlock = true;
                        gameNote.Lock = true;
                        break;
                    }
                if (removeBlock)
                    break;
                else if (songNote.Transform.Position.Z > 30)
                {
                    notesPlayed++;
                    missedNotes++;
                    songBlocks.Remove(songNote);
                    break;
                }
            }
        }

        void PlayDraw()
        {
            spriteBatch.Begin();
            if (songCountdown >= 0)
                spriteBatch.DrawString(font, "" + songCountdown, new Vector2(20, 20), Color.White);
            else
                spriteBatch.DrawString(font, "Missed Notes: " + missedNotes, new Vector2(20, 20), Color.White);

            if (notesPlayed >= totalNotes)
                spriteBatch.DrawString(font, "You did it!!", new Vector2(20, 40), Color.White);
            spriteBatch.End();

            plane.Draw(planeTransform.World, camera.View, camera.Projection);

            foreach (GameNote note in noteBlocks)
            {
                note.Effect.Parameters["World"].SetValue(note.Transform.World);
                foreach (EffectPass pass in note.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (ModelMesh mesh in note.Renderer.ObjectModel.Meshes)
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                            GraphicsDevice.Indices = part.IndexBuffer;
                            GraphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, part.VertexOffset, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                }
            }

            foreach (SongNote note in songBlocks)
            {
                effectSongNote.Parameters["World"].SetValue(note.Transform.World);
                foreach (EffectPass pass in effectSongNote.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (ModelMesh mesh in note.Renderer.ObjectModel.Meshes)
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                            GraphicsDevice.Indices = part.IndexBuffer;
                            GraphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, part.VertexOffset, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                }
            }
        }

        void ResultsUpdate()
        {
            returnButton.Update();
        }

        void ResultsDraw()
        {
            spriteBatch.Begin();
            returnButton.Draw(spriteBatch, font);
            if (highscore1 != -1)
                spriteBatch.DrawString(font, "Highscore #1: " + highscore1 + "%",
                    new Vector2(ScreenManager.Width / 2, ScreenManager.Height / 2), Color.White);
            else
                spriteBatch.DrawString(font, "Highscore #1: empty",
                    new Vector2(ScreenManager.Width / 2, ScreenManager.Height / 2), Color.White);
            
            if (highscore2 != -1)
                spriteBatch.DrawString(font, "Highscore #2: " + highscore2 + "%",
                    new Vector2(ScreenManager.Width / 2, (ScreenManager.Height / 2) + 50), Color.White);
            else
                spriteBatch.DrawString(font, "Highscore #2: empty",
                    new Vector2(ScreenManager.Width / 2, (ScreenManager.Height / 2) + 50), Color.White);

            if (highscore3 != -1)
                spriteBatch.DrawString(font, "Highscore #3: " + highscore3 + "%",
                    new Vector2(ScreenManager.Width / 2, (ScreenManager.Height / 2) + 100), Color.White);
            else
                spriteBatch.DrawString(font, "Highscore #3: empty",
                    new Vector2(ScreenManager.Width / 2, (ScreenManager.Height / 2) + 100), Color.White);

            spriteBatch.DrawString(font, "Notes Missed: " + missedNotes + "\nTotal Notes: " + totalNotes +
                "\nPercent Notes Played: " + score + "%", new Vector2(ScreenManager.Width / 2,
               (ScreenManager.Height / 2)+150), Color.White);
            spriteBatch.End();
        }

        void OptionsUpdate()
        {
            volumeUp.Update();
            volumeDown.Update();
            optionsBack.Update();
        }

        void OptionsDraw()
        {
            spriteBatch.Begin();
            volumeUp.Draw(spriteBatch, font);
            volumeDown.Draw(spriteBatch, font);
            optionsBack.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, "Volume: " + MediaPlayer.Volume, new Vector2(50, 75), Color.White);
            spriteBatch.DrawString(font, "JAM ON!!! is a rhythm-based game similar to Guitar Hero, where"
                + "\nnote move towards you that each correspond with notes\nfrom the song.\n\n" +
                "You have 5 game blocks, each corresponding to a keyboard key.  Press the\nkey corresponding" +
                "with each game block when a note block intersects it to play the note.", new Vector2(500, 75), Color.White);
            spriteBatch.Draw(help1, new Rectangle(500, 200, help1.Width / 3, help1.Height / 3), Color.White);
            spriteBatch.Draw(help2, new Rectangle(500, 600, help2.Width / 3, help2.Height / 3), Color.White);
            spriteBatch.End();
        }

        void CreditsUpdate()
        {
            creditsBack.Update();
        }

        void CreditsDraw()
        {
            spriteBatch.Begin();
            creditsBack.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, "Africa - Toto: https://www.youtube.com/watch?v=DWfY9GRe7SI" +
                "\nLevan Polka - Hatsune Miku: https://www.youtube.com/watch?v=_aNJUXkBV-c" +
                "\nVolume Sound: https://www.youtube.com/watch?v=iqztd7uMvVI" +
                "\nConvert Youtube to MP3: https://ytmp3.cc/" +
                "\nMP3 Clipper: https://mp3cut.net/", new Vector2(50, 100), Color.White);
            spriteBatch.End();
        }
        #endregion


        // This next region consists of two huge methods that make the playable notes; the notes are given a row to go down and a
        // time value that will send them moving after the value reaches 0
        #region Song Maps
        void Song1Map()
        {
            // Song 1: Toto - Africa; 92 bpm; increment by .62f per quarter note beat; 269 total notes

            // bar 1
            SongNote note = new SongNote(0.1f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(0.1f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(.72f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(1.34f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(1.34f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(1.96f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 2
            note = new SongNote(2.58f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(2.58f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(3.2f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(3.82f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(3.82f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(4.44f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            // bar 3
            note = new SongNote(5.06f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(5.06f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(5.68f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(6.3f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(6.3f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(6.92f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            // bar 4
            note = new SongNote(7.54f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(8f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(8.31f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(8.62f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(8.78f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
            
            note = new SongNote(9.09f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(9.4f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 5
            note = new SongNote(10.02f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(10.02f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(10.64f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.26f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.26f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.88f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 6
            note = new SongNote(12.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(12.96f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.27f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.58f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.74f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.05f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.36f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            // bar 7
            note = new SongNote(14.98f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.98f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.6f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.22f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.22f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.84f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 8
            note = new SongNote(17.46f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.92f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.23f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.54f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.7f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.01f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.32f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            // bar 9
            note = new SongNote(19.94f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.94f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.56f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.18f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.18f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.8f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 10
            note = new SongNote(22.42f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.88f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.19f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.66f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.97f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.28f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            // bar 11
            note = new SongNote(24.9f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.9f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.52f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.14f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.14f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.76f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 12 - verse
            note = new SongNote(27.38f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.38f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.53f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.69f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.84f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
            
            note = new SongNote(28f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
            
            note = new SongNote(28.62f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.77f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(29.23f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(29.54f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(29.69f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 13
            note = new SongNote(31.08f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(31.54f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(32f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 14
            note = new SongNote(32.31f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(32.62f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(32.77f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(33.23f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(33.54f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(33.85f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(34f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(34.46f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 15
            note = new SongNote(34.77f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(35.54f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(35.69f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 16
            note = new SongNote(36f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 17
            note = new SongNote(38.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(38.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(38.91f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(39.22f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(39.37f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(39.68f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(40.14f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(40.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(40.76f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(40.91f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 18
            note = new SongNote(42.3f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(42.45f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(42.60f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(42.75f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(43.21f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 19
            note = new SongNote(43.52f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(43.83f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(43.98f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(44.29f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(44.44f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(45.21f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(45.36f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(45.67f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 20
            note = new SongNote(46.08f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(46.85f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(47f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 21
            note = new SongNote(47.31f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(47.31f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 22
            note = new SongNote(49.79f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(49.79f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(50.1f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(50.41f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(50.72f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(50.87f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(51.18f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(51.49f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(51.8f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(51.95f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(52.1f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 23
            note = new SongNote(53.49f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(53.64f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(53.79f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(53.94f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(54.4f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 24
            note = new SongNote(54.71f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(54.86f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(55.01f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(55.32f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(55.47f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(56.24f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(56.56f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(56.87f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 25
            note = new SongNote(57.18f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(57.95f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(58.2f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 26
            note = new SongNote(58.35f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 27
            note = new SongNote(60.83f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(60.83f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(61.14f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(61.29f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(61.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(61.91f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(62.22f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(62.53f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(62.84f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(63.15f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 28
            note = new SongNote(64.52f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(64.67f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(64.98f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(65.44f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 29
            note = new SongNote(65.75f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(66f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(66.31f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(66.46f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(66.61f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 30
            note = new SongNote(67.06f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(67.68f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(68.3f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(68.92f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(69.54f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(70.16f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 31 - chorus
            note = new SongNote(70.78f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(70.78f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(71.09f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(71.24f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(71.4f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(71.4f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(71.71f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(77.86f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
            
            note = new SongNote(72.02f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(72.33f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(72.48f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(72.64f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(72.79f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(72.94f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
        
            note = new SongNote(73.09f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 32
            note = new SongNote(73.26f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(73.88f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(74.19f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(74.34f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(74.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(75.12f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 33
            note = new SongNote(75.72f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(75.72f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.03f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.34f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.34f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.49f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.64f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.79f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.94f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(76.94f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(77.25f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(77.4f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(77.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(77.70f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(78.01f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 34
            note = new SongNote(78.16f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(78.62f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(78.77f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(78.77f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(78.92f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(79.23f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(79.38f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(80f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 35
            note = new SongNote(80.62f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(80.62f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(80.93f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(81.24f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(81.24f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(81.55f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(81.7f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(81.85f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(82.47f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(82.47f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(82.78f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(82.93f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 36
            note = new SongNote(83.08f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(83.7f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(84.01f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(84.16f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(84.31f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(84.93f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 37
            note = new SongNote(85.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(85.5f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(85.81f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(85.96f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(86.12f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(86.12f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(86.43f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(86.58f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(86.74f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(87.05f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(87.2f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(87.36f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(87.51f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 38
            note = new SongNote(87.96f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(87.96f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(88.27f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(88.42f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(88.58f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(88.73f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(89.2f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(89.97f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(90.12f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 40
            note = new SongNote(90.43f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(90.89f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(91.04f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(91.35f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(92.12f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(92.12f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 41 - end
            note = new SongNote(92.58f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(92.58f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
        }

        void Song2Map()
        {

            // Song 1: Hatsune Miku - Levan Polka; 118 bpm; increment by .48f per quarter note beat; __ total note

            // bar 1
            SongNote note = new SongNote(6.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(7.38f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(7.86f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(8.34f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(8.58f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 2
            note = new SongNote(8.82f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(9.3f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(9.78f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(10.26f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(10.50f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 3
            note = new SongNote(10.98f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.22f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.46f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.7f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(11.94f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(12.18f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(12.42f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(12.66f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(12.66f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 4
            note = new SongNote(12.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.14f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.38f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.62f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.82f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(13.82f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.66f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 5 - verse 1
            note = new SongNote(14.75f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.99f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.23f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.59f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.71f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.83f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.94f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.05f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.16f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.4f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.52f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 6
            note = new SongNote(16.64f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.86f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.08f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.3f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.52f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.74f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.96f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.18f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.29f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 7
            note = new SongNote(18.4f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.62f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.84f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.17f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.28f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.61f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.83f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.94f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.05f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 8
            note = new SongNote(20.16f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.27f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.38f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.49f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.71f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.82f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.93f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.04f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.26f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.48f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.81f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 9
            note = new SongNote(21.92f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.14f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.36f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.58f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.8f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.02f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.13f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.35f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.46f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.57f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 10
            note = new SongNote(23.68f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.79f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.9f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.01f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.12f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.23f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.34f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.45f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.56f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.78f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.89f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.1f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.43f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 11
            note = new SongNote(25.54f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.76f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.98f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.2f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.42f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.64f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.86f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.97f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.08f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.19f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 12
            note = new SongNote(27.3f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.41f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.52f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.63f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.74f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.85f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.96f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.07f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.18f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.4f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.51f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.62f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.95f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 13 - repeat
            note = new SongNote(14.75f + 14.45f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.99f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.23f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.59f + 14.45f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.71f + 14.45f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.83f + 14.45f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.94f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.05f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.16f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.4f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.52f + 14.45f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 14
            note = new SongNote(16.64f + 14.45f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.86f + 14.45f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
        
            note = new SongNote(17.08f + 14.45f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.3f + 14.45f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.52f+ 14.45f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.74f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.96f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.18f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.29f + 14.45f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 15
            note = new SongNote(18.4f + 14.5f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.62f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.84f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.17f + 14.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.28f + 14.5f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.5f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.61f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.83f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.94f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.05f + 14.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 16
            note = new SongNote(20.16f + 14.5f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.27f + 14.5f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.38f + 14.5f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.49f + 14.5f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.6f + 14.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.71f + 14.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.82f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.93f + 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
        
            note = new SongNote(21.04f + 14.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.26f + 14.5f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.48f + 14.5f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.7f + 14.5f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.81f + 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 17
            note = new SongNote(21.92f + 14.55f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.14f + 14.55f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.36f + 14.55f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.58f + 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.8f + 14.55f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.02f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.13f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.35f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.46f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.57f + 14.55f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 18
            note = new SongNote(23.68f + 14.55f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.79f + 14.55f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.9f + 14.55f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.01f + 14.55f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.12f + 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.23f + 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.34f + 14.55f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.45f + 14.55f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.56f + 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.78f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.89f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.1f + 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.43f + 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 19
            note = new SongNote(25.54f + 14.6f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.76f + 14.6f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.98f + 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.2f + 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.42f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.64f + 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.86f + 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.97f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.08f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.19f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 20
            note = new SongNote(27.3f + 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.41f + 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.52f + 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.63f + 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.74f + 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.85f + 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.96f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.07f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.18f + 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.4f + 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.51f + 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.62f + 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.95f + 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 21 - repeat
            note = new SongNote(14.75f + 28.9f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(14.99f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.23f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.59f + 28.9f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.71f + 28.9f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.83f +28.9f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(15.94f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.05f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.16f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.4f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.52f + 28.9f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 22
            note = new SongNote(16.64f + 28.9f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(16.86f + 28.9f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.08f + 28.9f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.3f + 28.9f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.52f + 28.9f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.63f + 28.9f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.74f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(17.96f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.29f + 28.9f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 23
            note = new SongNote(18.4f + 29f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.62f + 29f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(18.84f + 29f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.17f + 29f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.28f + 29f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.5f + 29f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.61f + 29f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.83f + 29f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(19.94f + 2 * 14.5f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.05f + 2 * 14.5f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
        
            //bar 24
            note = new SongNote(20.16f + 2 * 14.55f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.27f + 2 * 14.55f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.38f + 2 * 14.55f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.49f + 2 * 14.55f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.6f + 2 * 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.71f + 2 * 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.82f + 2 * 14.55f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(20.93f + 2 * 14.55f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.04f + 2 * 14.55f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.26f + 2 * 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.37f + 2 * 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.48f + 2 * 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.7f + 2 * 14.55f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(21.81f + 2 * 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 25
            note = new SongNote(21.92f + 2 * 14.6f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.14f + 2 * 14.6f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.36f + 2 * 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.58f + 2 * 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(22.8f + 2 * 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.02f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.13f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.24f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.46f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.57f + 2 * 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 26
            note = new SongNote(23.68f + 2 * 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.79f + 2 * 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(23.9f + 2 * 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.01f + 2 * 14.6f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.12f + 2 * 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.23f + 2 * 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.34f + 2*14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.45f + 2 * 14.6f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.56f + 2 * 14.6f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.78f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(24.89f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.1f + 2 * 14.6f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.43f + 2 * 14.7f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 27
            note = new SongNote(25.54f + 2 * 14.7f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.76f + 2 * 14.7f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(25.98f + 2 * 14.7f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.2f + 2 * 14.7f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.42f + 2 * 14.7f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.64f + 2 * 14.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.75f + 2 * 14.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(26.86f + 2 * 14.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.08f + 2 * 14.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.19f + 2 * 14.7f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            //bar 28
            note = new SongNote(27.3f + 2 * 14.7f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.41f + 2 * 14.7f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.52f + 2 * 14.7f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.63f + 2 * 14.7f, 1, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.74f + 2 * 14.7f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.85f + 2 * 14.7f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(27.96f + 2 * 14.7f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.07f + 2 * 14.7f, 3, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.18f + 2 * 14.7f, 2, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.4f + 2 * 14.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.51f + 2 * 14.7f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.62f + 29.4f, 4, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);

            note = new SongNote(28.62f + 29.4f, 0, Content, camera, GraphicsDevice, light);
            songBlocks.Add(note);
        }
        #endregion Song Maps

        void CalculateScore()
        {
            float temp1 = totalNotes - missedNotes;
            float temp2 = temp1 / totalNotes;
            float temp3 = temp2 * 100;
            score = (int)temp3;

            if (score > highscore1)
            {
                highscore3 = highscore2;
                highscore2 = highscore1;
                highscore1 = score;
            }

            else if (score > highscore2)
            {
                highscore3 = highscore2;
                highscore2 = score;
            }

            else if (score > highscore3)
                highscore3 = score;
        }
    }
}
