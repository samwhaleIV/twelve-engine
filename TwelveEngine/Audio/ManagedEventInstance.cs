using FMOD;
using FMOD.Studio;

namespace TwelveEngine.Audio {
    public sealed class ManagedEventInstance {

        private readonly EventInstance instance;
        private readonly EventWrapper eventWrapper;

        internal ManagedEventInstance(EventWrapper eventWrapper) {
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

        public ManagedEventInstance Play(bool autoRelease = true) {
            ValidateInstance();
            instance.start();
            if(autoRelease) {
                instance.release();
            }
            return this;
        }

        public event Action OnStopped, OnTimelineBeat;

        private RESULT EventCallback(EVENT_CALLBACK_TYPE type,IntPtr _event,IntPtr parameters) {
            if(type.HasFlag(EVENT_CALLBACK_TYPE.STARTED)) {
                Logger.WriteLine($"Started sound instance '{eventWrapper.Name}'.",LoggerLabel.Audio);
            }
            if(type.HasFlag(EVENT_CALLBACK_TYPE.STOPPED)) {
                Logger.WriteLine($"Sound instance stopped '{eventWrapper.Name}'.",LoggerLabel.Audio);
                OnStopped?.Invoke();
            }
            if(type.HasFlag(EVENT_CALLBACK_TYPE.TIMELINE_BEAT)) {
                OnTimelineBeat?.Invoke();
            }
            if(type.HasFlag(EVENT_CALLBACK_TYPE.START_FAILED)) {
                Logger.WriteLine($"Failure starting sound instance '{eventWrapper.Name}'.",LoggerLabel.Audio);
            }
            return RESULT.OK;
        }

        public ManagedEventInstance FadeOut() {
            ValidateInstance();
            instance.stop(STOP_MODE.ALLOWFADEOUT);
            return this;
        }

        public ManagedEventInstance Stop() {
            ValidateInstance();
            instance.stop(STOP_MODE.IMMEDIATE);
            return this;
        }

        public ManagedEventInstance Release() {
            ValidateInstance();
            instance.release();
            return this;
        }

        public ManagedEventInstance SetPaused(bool paused) {
            ValidateInstance();
            instance.setPaused(paused);
            return this;
        }

        public ManagedEventInstance SetParameter(PARAMETER_ID ID,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(ID,value,ignoreSeekSpeed);
            return this;
        }

        public ManagedEventInstance SetParameter(PARAMETER_DESCRIPTION description,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(description.id,value,ignoreSeekSpeed);
            return this;
        }

        public ManagedEventInstance SetParameter(ParameterDescription description,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(description.ID,value,ignoreSeekSpeed);
            return this;
        }

        public ManagedEventInstance SetParameter(string name,float value,bool ignoreSeekSpeed = false) {
            ValidateInstance();
            instance.setParameterByID(eventWrapper.Parameters[name].ID,value,ignoreSeekSpeed);
            return this;
        }

        public ManagedEventInstance SetPosition(int position) {
            ValidateInstance();
            instance.setTimelinePosition(position);
            return this;
        }

        public ManagedEventInstance SetVolume(float volume) {
            ValidateInstance();
            instance.setVolume(volume);
            return this;
        }
    }
}
