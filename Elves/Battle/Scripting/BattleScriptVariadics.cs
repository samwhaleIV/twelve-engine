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
            buttonTable.Values[await _sequencer.GetButton(false,buttonTable.Options)].Invoke();
        }

        public async Task Button<TParameter>(TParameter parameter,ButtonTable<Action<TParameter>> buttonTable) {
            buttonTable.Values[await _sequencer.GetButton(false,buttonTable.Options)].Invoke(parameter);
        }

        public async Task<TResult> Button<TResult>(ButtonTable<TResult> buttonTable) {
            return buttonTable.Values[await _sequencer.GetButton(false,buttonTable.Options)];
        }

        public async Task Button(ButtonTable<Func<Task>> buttonTable) {
            await buttonTable.Values[await _sequencer.GetButton(false,buttonTable.Options)].Invoke();
        }

        public async Task<TResult> Button<TResult>(ButtonTable<Func<Task<TResult>>> buttonTable) {
            return await buttonTable.Values[await _sequencer.GetButton(false,buttonTable.Options)].Invoke();
        }

        public async Task<TResult> Button<TParameter,TResult>(TParameter parameter,ButtonTable<Func<TParameter,Task<TResult>>> buttonTable) {
            return await buttonTable.Values[await _sequencer.GetButton(false,buttonTable.Options)].Invoke(parameter);
        }

        public async Task<TResult> Button<TResult>(string option1,Func<Task<TResult>> action1) {
            var result = await Button<Func<Task<TResult>>>(new ButtonTable<Func<Task<TResult>>>(option1,action1));
            return await result.Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<Task<TResult>> action1,string option2,Func<Task<TResult>> action2) {
            return await (await Button<Func<Task<TResult>>>(new ButtonTable<Func<Task<TResult>>>(option1,action1,option2,action2))).Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<Task<TResult>> action1,string option2,Func<Task<TResult>> action2,string option3,Func<Task<TResult>> action3) {
            return await (await Button<Func<Task<TResult>>>(new ButtonTable<Func<Task<TResult>>>(option1,action1,option2,action2,option3,action3))).Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<Task<TResult>> action1,string option2,Func<Task<TResult>> action2,string option3,Func<Task<TResult>> action3,string option4,Func<Task<TResult>> action4) {
            return await (await Button<Func<Task<TResult>>>(new ButtonTable<Func<Task<TResult>>>(option1,action1,option2,action2,option3,action3,option4,action4))).Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<TResult> action1) {
            return (await Button(new ButtonTable<Func<TResult>>(option1,action1))).Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<TResult> action1,string option2,Func<TResult> action2) {
            return (await Button(new ButtonTable<Func<TResult>>(option1,action1,option2,action2))).Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<TResult> action1,string option2,Func<TResult> action2,string option3,Func<TResult> action3) {
            return (await Button(new ButtonTable<Func<TResult>>(option1,action1,option2,action2,option3,action3))).Invoke();
        }

        public async Task<TResult> Button<TResult>(string option1,Func<TResult> action1,string option2,Func<TResult> action2,string option3,Func<TResult> action3,string option4,Func<TResult> action4) {
            return (await Button(new ButtonTable<Func<TResult>>(option1,action1,option2,action2,option3,action3,option4,action4))).Invoke();
        }

        public async Task<TResult> Button<TParameter,TResult>(TParameter parameter,string option1,Func<TParameter,Task<TResult>> action1) {
            return await (await Button(new ButtonTable<Func<TParameter,Task<TResult>>>(option1,action1))).Invoke(parameter);
        }

        public async Task<TResult> Button<TParameter,TResult>(TParameter parameter,string option1,Func<TParameter,Task<TResult>> action1,string option2,Func<TParameter,Task<TResult>> action2) {
            return await (await Button(new ButtonTable<Func<TParameter,Task<TResult>>>(option1,action1,option2,action2))).Invoke(parameter);
        }

        public async Task<TResult> Button<TParameter,TResult>(TParameter parameter,string option1,Func<TParameter,Task<TResult>> action1,string option2,Func<TParameter,Task<TResult>> action2,string option3,Func<TParameter,Task<TResult>> action3) {
            return await (await Button(new ButtonTable<Func<TParameter,Task<TResult>>>(option1,action1,option2,action2,option3,action3))).Invoke(parameter);
        }

        public async Task<TResult> Button<TParameter,TResult>(TParameter parameter,string option1,Func<TParameter,Task<TResult>> action1,string option2,Func<TParameter,Task<TResult>> action2,string option3,Func<TParameter,Task<TResult>> action3,string option4,Func<TParameter,Task<TResult>> action4) {
            return await (await Button(new ButtonTable<Func<TParameter,Task<TResult>>>(option1,action1,option2,action2,option3,action3,option4,action4))).Invoke(parameter);
        }

        public async Task Button(string option1,Func<Task> action1) {
            await (await Button<Func<Task>>(new ButtonTable<Func<Task>>(option1,action1))).Invoke();
        }

        public async Task Button(string option1,Func<Task> action1,string option2,Func<Task> action2) {
            await (await Button<Func<Task>>(new ButtonTable<Func<Task>>(option1,action1,option2,action2))).Invoke();
        }

        public async Task Button(string option1,Func<Task> action1,string option2,Func<Task> action2,string option3,Func<Task> action3) {
            await (await Button<Func<Task>>(new ButtonTable<Func<Task>>(option1,action1,option2,action2,option3,action3))).Invoke();
        }

        public async Task Button(string option1,Func<Task> action1,string option2,Func<Task> action2,string option3,Func<Task> action3,string option4,Func<Task> action4) {
            await (await Button<Func<Task>>(new ButtonTable<Func<Task>>(option1,action1,option2,action2,option3,action3,option4,action4))).Invoke();
        }
    }
}
