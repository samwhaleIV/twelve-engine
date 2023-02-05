using FMOD.Studio;

namespace TwelveEngine.Audio {
    public readonly struct EventInstanceController {

        private readonly EventInstance instance;
        internal EventInstanceController(EventInstance instance) => this.instance = instance;

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
