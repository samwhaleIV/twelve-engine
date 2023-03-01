using System;
using System.Threading.Tasks;

namespace Elves.Battle.Scripting {
    public abstract partial class BattleScript {

        public async Task Speech(string speech1) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2,string speech3) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech3,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech3,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech4,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech3,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech4,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech5,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,string speech6) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech3,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech4,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech5,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech6,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,string speech7) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech3,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech4,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech5,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech6,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech7,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,string speech7,string speech8) {
            _sequencer.ShowSpeech(speech1,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech2,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech3,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech4,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech5,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech6,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech7,ActorSprite);
            await Continue();
            _sequencer.ShowSpeech(speech8,ActorSprite);
            await Continue();
            _sequencer.HideSpeech(ActorSprite);
        }

        public async Task Tag(string tag) {
            _sequencer.SetTag(tag);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2,string tag3) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.SetTag(tag3);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.SetTag(tag3);
            await Continue();
            _sequencer.SetTag(tag4);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.SetTag(tag3);
            await Continue();
            _sequencer.SetTag(tag4);
            await Continue();
            _sequencer.SetTag(tag5);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,string tag6) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.SetTag(tag3);
            await Continue();
            _sequencer.SetTag(tag4);
            await Continue();
            _sequencer.SetTag(tag5);
            await Continue();
            _sequencer.SetTag(tag6);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,string tag7) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.SetTag(tag3);
            await Continue();
            _sequencer.SetTag(tag4);
            await Continue();
            _sequencer.SetTag(tag5);
            await Continue();
            _sequencer.SetTag(tag6);
            await Continue();
            _sequencer.SetTag(tag7);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,string tag7,string tag8) {
            _sequencer.SetTag(tag1);
            await Continue();
            _sequencer.SetTag(tag2);
            await Continue();
            _sequencer.SetTag(tag3);
            await Continue();
            _sequencer.SetTag(tag4);
            await Continue();
            _sequencer.SetTag(tag5);
            await Continue();
            _sequencer.SetTag(tag6);
            await Continue();
            _sequencer.SetTag(tag7);
            await Continue();
            _sequencer.SetTag(tag8);
            await Continue();
            _sequencer.HideTag();
        }

        public async Task<int> Button(string option1) {
            return await _sequencer.GetButton(false,new(option1));
        }

        public async Task<int> Button(string option1,string option2) {
            return await _sequencer.GetButton(false,new(option1,option2));
        }

        public async Task<int> Button(string option1,string option2,string option3) {
            return await _sequencer.GetButton(false,new(option1,option2,option3));
        }

        public async Task<int> Button(string option1,string option2,string option3,string option4) {
            return await _sequencer.GetButton(false,new(option1,option2,option3,option4));
        }

        public async Task Button(ButtonTable<Action> buttonTable) {
            var result = await _sequencer.GetButton(false,buttonTable.Options);
            buttonTable.Values[result].Invoke();
        }

        public async Task Button<TParameter>(TParameter parameter,ButtonTable<Action<TParameter>> buttonTable) {
            var result = await _sequencer.GetButton(false,buttonTable.Options);
            buttonTable.Values[result].Invoke(parameter);
        }

        public async Task<TResult> Button<TResult>(ButtonTable<TResult> buttonTable) {
            var result = await _sequencer.GetButton(false,buttonTable.Options);
            return buttonTable.Values[result];
        }

        public async Task Button(ButtonTable<Func<Task>> buttonTable) {
            int index = await _sequencer.GetButton(false,buttonTable.Options);
            await buttonTable.Values[index].Invoke();
        }

        public async Task<TResult> Button<TResult>(ButtonTable<Func<Task<TResult>>> buttonTable) {
            int index = await _sequencer.GetButton(false,buttonTable.Options);
            var result = await buttonTable.Values[index].Invoke();
            return result;
        }

        public async Task<TResult> Button<TParameter,TResult>(TParameter parameter,ButtonTable<Func<TParameter,Task<TResult>>> buttonTable) {
            int index = await _sequencer.GetButton(false,buttonTable.Options);
            var result = await buttonTable.Values[index].Invoke(parameter);
            return result;
        }
    }
}
