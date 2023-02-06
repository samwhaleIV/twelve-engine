using FMOD;
using FMOD.Studio;

namespace TwelveEngine.Audio {
    public sealed class EventInstanceController {

        private readonly EventInstance instance;
        private readonly EventWrapper eventWrapper;

        internal EventInstanceController(EventWrapper eventWrapper) {
            this.eventWrapper = eventWrapper;
            eventWrapper.Description.createInstance(out EventInstance instance);
            instance.setCallback(EventCallback,EVENT_CALLBACK_TYPE.ALL);
            this.instance = instance;
        }

        private void ValidateInstance() {
            if(instance.isValid()) {
                return;
            }
            throw new InvalidOperationException("Cannot control audio event instance. The event has already been invalidated.");
        }

        public EventInstanceController Play(bool autoRelease = true) {
            ValidateInstance();
            instance.start();
            if(autoRelease) {
                instance.release();
            }
            return this;
        }

        public event Action OnStopped, OnTimelineBeat;

        private RESULT EventCallback(EVENT_CALLBACK_TYPE type,IntPtr _event,IntPtr parameters) {
            if(type.HasFlag(EVENT_CALLBACK_TYPE.STOPPED)) {
                OnStopped?.Invoke();
            }
            if(type.HasFlag(EVENT_CALLBACK_TYPE.TIMELINE_BEAT)) {
                OnTimelineBeat?.Invoke();
            }
            return RESULT.OK;
        }

        public EventInstanceController FadeOut() {
            ValidateInstance();
            instance.stop(STOP_MODE.ALLOWFADEOUT);
            return this;
        }

        public EventInstanceController Stop() {
            ValidateInstance();
            instance.stop(STOP_MODE.IMMEDIATE);
            return this;
        }

        public EventInstanceController Release() {
            ValidateInstance();
            instance.release();
            return this;
        }

        public EventInstanceController SetPaused(bool paused) {
            ValidateInstance();
            instance.setPaused(paused);
            return this;
        }

        public EventInstanceController SetParameter(PARAMETER_ID ID,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(ID,value,ignoreSeekSpeed);
            return this;
        }

        public EventInstanceController SetParameter(PARAMETER_DESCRIPTION description,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(description.id,value,ignoreSeekSpeed);
            return this;
        }

        public EventInstanceController SetParameter(ParameterDescription description,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(description.ID,value,ignoreSeekSpeed);
            return this;
        }

        public EventInstanceController SetParameter(string name,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(eventWrapper.Parameters[name].ID,value,ignoreSeekSpeed);
            return this;
        }

        public EventInstanceController SetPosition(int position) {
            ValidateInstance();
            instance.setTimelinePosition(position);
            return this;
        }

        public EventInstanceController SetVolume(float volume) {
            ValidateInstance();
            instance.setVolume(volume);
            return this;
        }
    }
}
