using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Settings {

    public sealed class SettingsPage<TBackButton>:BookPage<SpriteElement> where TBackButton:SpriteElement,ISettingsBackButton {

        private readonly Vector2 MusicBarOrigin = new(9,52),
            SfxBarOrigin = new(9,88),
            MusicUpOrigin = new(57,39),
            MusicDownOrigin = new(50,40),
            SfxUpOrigin = new(43,75),
            SfxDownOrigin = new(36,74);

        public event Action OnUpdate, OnBack;

        private readonly TBackButton backButton;

        private readonly VolumeBar musicVolumeBar, sfxVolumeBar;
        private readonly VolumeAdjustmentButton musicDown, musicUp, sfxDown, sfxUp;
        private readonly List<VolumeAdjustmentButton> volumeButtons = new();

        private readonly List<SpriteElement> embeddedElements = new();

        public SettingsPage(TBackButton backButton) {
            this.backButton = backButton;
            settingsPhone = new() {
                Scale = 1,
                PositionMode = CoordinateMode.Absolute,
                SizeMode = CoordinateMode.Absolute,
                Offset = Vector2.Zero,
                TextureSource = TotalArea.ToRectangle(),
                Texture = Program.Textures.SettingsPhone,
                Depth = Constants.Depth.MiddleClose
            };

            musicDown = new(VolumeAdjustment.Down);
            musicUp = new(VolumeAdjustment.Up);

            sfxDown = new(VolumeAdjustment.Down);
            sfxUp = new(VolumeAdjustment.Up);

            musicDown.OnActivated += AdjustMusicVolume;
            sfxDown.OnActivated += AdjustSfxVolume;

            musicUp.OnActivated += AdjustMusicVolume;
            sfxUp.OnActivated += AdjustSfxVolume;

            musicVolumeBar = new();
            sfxVolumeBar = new();

            volumeButtons.Add(sfxUp);
            volumeButtons.Add(sfxDown);
            volumeButtons.Add(musicUp);
            volumeButtons.Add(musicDown);

            foreach(var volumeButton in volumeButtons) {
                volumeButton.Scale = 1;
                volumeButton.Depth = Constants.Depth.MiddleCloser;
                embeddedElements.Add(volumeButton);
            }

            embeddedElements.Add(musicVolumeBar);
            embeddedElements.Add(sfxVolumeBar);
        }

        private void AdjustMusicVolume(VolumeAdjustment adjustment) {
            //todo
        }

        private void AdjustSfxVolume(VolumeAdjustment adjustment) {
            //todo
        }

        private readonly FloatRectangle TotalArea = new(0,0,131,256), PhoneArea = new(5,0,72,120);

        private readonly SpriteElement settingsPhone;

        public override bool Back() {
            if(OnBack is null) {
                return false;
            }
            OnBack.Invoke();
            return true;
        }

        public IEnumerable<SpriteElement> GetElements() {
            yield return settingsPhone;
            foreach(var element in embeddedElements) {
                yield return element;
            }
        }

        public override void Close() {
            backButton.OnBackAction -= BackButton_OnBackAction;
            settingsPhone.KeyFrame(Now,TransitionDuration);

            settingsPhone.Scale = 1;
            var newPos = settingsPhone.Position;
            newPos.Y = Viewport.Bottom;
            settingsPhone.Position = newPos;

            foreach(var element in embeddedElements) {
                element.KeyFrame(Now,TransitionDuration);
            }
            UpdateEmbeddedElements(new Vector2(0,newPos.Y));
        }

        private void SetupFocusTree() {
            musicUp.FocusSet = new() {
                Left = musicDown,
                Down = sfxUp
            };
            musicDown.FocusSet = new() {
                Left = backButton,
                Right = musicUp,
                Down = sfxDown
            };
            sfxUp.FocusSet = new() {
                Up = musicUp,
                Left = sfxDown
            };
            sfxDown.FocusSet = new() {
                Left = backButton,
                Right = sfxUp,
                Up = musicDown
            };
            backButton.FocusSet = new() {
                Right = musicUp,
                IndeterminateRight = true
            };
        }

        public override BookElement Open() {
            var bb = backButton;
                bb.Scale = 1;
                bb.PositionModeX = CoordinateMode.Absolute;
                bb.PositionModeY = CoordinateMode.Relative;
                bb.SizeMode = CoordinateMode.Absolute;
                bb.Offset = new(-0.5f,-0.5f);
                bb.Flags = ElementFlags.UpdateAndInteract;
                bb.OnBackAction += BackButton_OnBackAction;
            Update();

            settingsPhone.Scale = 1;
            var newPos = settingsPhone.Position;
            newPos.Y = Viewport.Bottom;
            settingsPhone.Position = newPos;

            UpdateEmbeddedElements(new Vector2(0,newPos.Y));

            foreach(var element in embeddedElements) {
                element.Scale = 1;
                element.KeyFrame(Now,TransitionDuration);
            }

            foreach(var button in volumeButtons) {
                button.Flags = ElementFlags.UpdateAndInteract;
            }

            settingsPhone.KeyFrame(Now,TransitionDuration);

            SetupFocusTree();
            return musicDown;
        }

        private void BackButton_OnBackAction() {
            OnBack?.Invoke();
        }

        public float Scale { get; set; } = 0.98f;

        private Vector2 GetEmbeddedElementOrigin(Vector2 position,Vector2 originOffset) {
            return _phoneOrigin + originOffset + position * _pixelSize;
        }

        private void UpdateEmbeddedElements(Vector2 offset) {
            Vector2 musicBarOrigin = GetEmbeddedElementOrigin(MusicBarOrigin,offset);
            Vector2 sfxBarOrigin = GetEmbeddedElementOrigin(SfxBarOrigin,offset);

            musicVolumeBar.SizeByPixels(_pixelSize);
            sfxVolumeBar.Size = musicVolumeBar.Size;

            musicVolumeBar.SetOrigin(musicBarOrigin,_pixelSize);
            sfxVolumeBar.SetOrigin(sfxBarOrigin,_pixelSize);

            Vector2 volumeButtonSize = musicDown.SourceSize * _pixelSize;

            foreach(var volumeButton in volumeButtons) {
                volumeButton.Size = volumeButtonSize;
            }

            Vector2 buttonOriginOffset = volumeButtonSize * 0.5f;

            musicUp.Position = GetEmbeddedElementOrigin(MusicUpOrigin,offset) + buttonOriginOffset;
            musicDown.Position = GetEmbeddedElementOrigin(MusicDownOrigin,offset) + buttonOriginOffset;

            sfxDown.Position = GetEmbeddedElementOrigin(SfxDownOrigin,offset) + buttonOriginOffset;
            sfxUp.Position = GetEmbeddedElementOrigin(SfxUpOrigin,offset) + buttonOriginOffset;
        }

        private Vector2 _phoneOrigin, _innerSize, _outerSize;
        private float _pixelSize;

        private void UpdateOrigin() {
            float height = Viewport.Height * Scale;

            _innerSize = new(height * PhoneArea.AspectRatio,height);
            _outerSize = _innerSize * (TotalArea.Size / PhoneArea.Size);

            _pixelSize = _innerSize.Y / PhoneArea.Height;
            _phoneOrigin = Viewport.Center - _innerSize * 0.5f;
        }

        public override void Update() {
            UpdateOrigin();

            float settingsX = Viewport.Center.X - _innerSize.X * 0.5f;
            settingsX -= PhoneArea.X * _pixelSize;
            float settingsY = Viewport.Center.Y - _innerSize.Y * 0.5f;

            Vector2 settingsPosition = new(settingsX,settingsY);

            settingsPhone.PositionMode = CoordinateMode.Absolute;
            settingsPhone.Size = _outerSize;
            settingsPhone.Position = settingsPosition;

            backButton.SizeByPixels(_pixelSize);
            float backButtonX = Viewport.Center.X - _innerSize.X * 0.5f - backButton.Size.X * 0.5f - _pixelSize * 2;
            backButton.Position = new Vector2(backButtonX,0.5f);

            UpdateEmbeddedElements(Vector2.Zero);

            OnUpdate?.Invoke();
        }
    }
}
