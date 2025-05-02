<template>
  <div class="external-streaming-directions">
    <p>
      Streaming from external decks and mixer, or similar all-in-one hardware like a Numark
      Mixstream+, requires that you have a way to get audio from the output of your device into your
      computer. This could be either a USB mixer or an audio interface of some kind. I personally
      use a Focusrite 2i4, with the two audio inputs serving as my L/R channels.
    </p>
    <p>
      If you don't have a way to get audio from your devices to your computer currently, you might
      ask around in the Discord server with your setup info and look for recommendations.
    </p>
    <p>
      Assuming you have a way to get audio coming out of your setup and into your computer, you can
      download <a href="https://danielnoethen.de/butt/">BUTT</a>. It is hands down the best, most
      simple software for this.
    </p>
    <p>
      After downloading and installing BUTT, open it and click the Settings button. After this,
      under Server Settings, click the Add button. In the window that appears, match the following
      setup:
    </p>
    <img
      alt="BUTT settings"
      src="@/assets/stream-setup-img/butt-server-settings.png" />
    <p>You will need to get the password from @phlank in the discord server.</p>
    <p>
      Hit Add on that window, and then under Stream Infos, hit Add. This will open up another
      window. Configure it as follows, and then click the Add button:
    </p>
    <img
      alt="BUTT stream info"
      src="@/assets/stream-setup-img/butt-stream-infos.png" />
    <p>
      Next, in the settings window, select the Audio tab. Here, under Primary Audio Device, select
      your audio input. In mine, I have my Focusrite 2i4's analog 1&2 inputs selected. Also set the
      streaming codec here.
    </p>
    <img
      alt="Shows the fields to set for the audio tab for BUTT"
      src="@/assets/stream-setup-img/butt-audio-device.png" />
    <Message severity="warn">
      You <i>MUST</i> use MP3 for the encoding in order for all browsers to be able to listen to the
      stream. 256kbps is the minimum bitrate we will use for quality reasons.
    </Message>
    <p>
      Finally, hit the play button on the main BUTT window. Start playing, and you should see audio
      show up in the meter. You want this to be higher, but you don't want it to be at 0 (0 = all
      the way to the right, in the red), otherwise you'll clip. I try to get my peaks down to a
      couple of pixels below 0.
    </p>
    <p>
      BUTT should now show that you are successfully connected. Navigate to the
      <a :href="streamStatus">stream status page</a> and ensure that the stream shows up in the
      debug view:
    </p>
    <img
      alt="Icecast debug page"
      src="@/assets/stream-setup-img/broadcasting-debug-test.png" />
    <p>
      Take a listen to the stream on
      <a href="https://lowpressurezone.com:8443/test">https://lowpressurezone.com:8443/test</a>.
    </p>
    <p>
      If everything is sounding good, go ahead and stop the stream with the square button next to
      the play button in BUTT. After that, go back to settings, click EDIT on the Server settings,
      and set the mount to
      <kbd>live</kbd>.
    </p>
    <img
      alt="Set the icecast mountpoint to 'live'"
      src="@/assets/stream-setup-img/butt-live.png" />
    <p>
      That's it! The next time you connect to the server for a stream, your audio will be accessible
      via the play button on the website :)
    </p>
  </div>
</template>

<script lang="ts" setup>
import { Message } from 'primevue'

const streamStatus = import.meta.env.VITE_STREAM_STATUS_URL
</script>

<style lang="scss">
.external-streaming-directions {
  overflow-x: auto;

  img {
    max-width: 100%;
  }
}
</style>
