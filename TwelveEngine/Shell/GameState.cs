﻿using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Serial;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class GameState:ISerializable {

        public GameManager Game { get; private set; } = null;

        public event Action OnLoad, OnUnload;

        public event Action<DebugWriter> OnWriteDebug;
        public event Action<SerialFrame> OnExport, OnImport;
        public event Action<GameTime> OnUpdate, OnRender, OnPreRender;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public bool IsUpdating { get; private set; } = false;
        public bool IsRendering { get; private set; } = false;
        public bool IsPreRendering { get; private set; } = false;

        private readonly SpriteBatchSettings _spriteBatchSettings = new SpriteBatchSettings();
        internal SpriteBatchSettings SpriteBatchSettings => _spriteBatchSettings;

        protected SmartSpriteBatch SpriteBatch => Game.SpriteBatch;

        internal void Load(GameManager game) {
            IsLoading = true;
            Game = game;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;
        }

        internal void Unload() {
            OnUnload?.Invoke();
            Game = null;
            IsLoaded = false;
        }

        internal void WriteDebug(DebugWriter writer) => OnWriteDebug?.Invoke(writer);

        internal void Update(GameTime gameTime) {
            IsUpdating = true;
            OnUpdate?.Invoke(gameTime);
            IsUpdating = false;
        }

        internal void Render(GameTime gameTime) {
            IsRendering = true;
            OnRender?.Invoke(gameTime);
            IsRendering = false;
        }

        internal void PreRender(GameTime gameTime) {
            IsPreRendering = true;
            OnPreRender?.Invoke(gameTime);
            IsPreRendering = false;
        }

        public void Export(SerialFrame frame) => OnExport?.Invoke(frame);
        public void Import(SerialFrame frame) => OnImport?.Invoke(frame);
    }
}
