<template>
  <div class="external-streaming-directions">
    <h4>Introduction</h4>
    <p>
      Streaming from external decks and mixer, or similar all-in-one hardware like a Numark
      Mixstream+, requires that you have a way to get audio from the output of your device into your
      computer. This could be either a USB mixer or an audio interface of some kind. I personally
      use a Focusrite Scarlett 2i4, with the two audio inputs serving as my L/R channels.
    </p>
    <p>
      If you don't have a way to get audio from your devices to your computer currently, you might
      ask around in the Discord server with your setup info and look for recommendations. If you are
      using a proper mixer and just need RCA lines sending audio into your computer, the Behringer
      UCA222 is a very affordable option.
    </p>
    <p>
      Assuming you have a way to get audio coming out of your setup and into your computer, you can
      download <a href="https://danielnoethen.de/butt/">Butt</a>. It is hands down the best, most
      simple software for this.
    </p>
    <h4>Server settings</h4>
    <p>
      After downloading and installing Butt, open it and click the Settings button. After this,
      under Server Settings, click the Add button. In the window that appears, match the following
      setup:
    </p>
    <img
      alt="Butt settings"
      src="@/assets/stream-setup-img/butt-server-settings.png" />
    <p>
      The password for the test stream is
      <kbd>{{ connectionInfoStore.testInfo()?.password }}</kbd
      >.
    </p>
    <h4>Stream info</h4>
    <p>
      Under Stream Infos, hit Add. This will open up another window. Configure it as follows, and
      then click the Add button:
    </p>
    <img
      alt="Butt stream info"
      src="@/assets/stream-setup-img/butt-stream-infos.png" />
    <h4>Audio settings</h4>
    <p>
      Next, in the settings window, select the Audio tab. Here, under Primary Audio Device, select
      your audio input. In mine, I have my Focusrite 2i4's analog 1&2 inputs selected. Also set the
      streaming codec here to MP3, and set the bitrate to either 256 or 320 kbps.
    </p>
    <img
      alt="Shows the fields to set for the audio tab for Butt"
      src="@/assets/stream-setup-img/butt-audio-device.png" />
    <Message severity="warn">
      You <i>MUST</i> use MP3 for the encoding in order for all browsers to be able to listen to the
      stream. 256kbps is the minimum bitrate we will use for quality reasons.
    </Message>
    <h4>Sound check</h4>
    <p>
      Start playing audio. On Butt, you should see in the main window the word "idle" at the top and
      below that, volume meters showing left and right audio levels. When audio is playing, you
      should see the meter levels change. Stop sending audio, and the meters should show no audio.
      If the meters show audio when you are not playing, you may have the computer microphone set as
      the audio source.
    </p>
    <img
      alt="Audio meter in Butt"
      src="@/assets/stream-setup-img/butt-sound-check.png" />
    <p>
      You want this to be higher when you're playing music, but you don't want it to be at 0 (0 =
      all the way to the right, in the red), otherwise you'll clip. I try to get my peaks down to a
      couple of pixels below 0. A little bit of clipping won't be too noticeable though, and the
      chat can always inform you of sound issues.
    </p>
    <h4>Test stream</h4>
    <p>
      Finally, hit the play button on the main Butt window. You should see text appear in the white
      section of the main window indicating that you are connected and that your stream metadata has
      been updated.
    </p>
    <img
      alt="Butt log update on successful stream connection"
      src="@/assets/stream-setup-img/butt-log-update.png" />
    <p>
      View your stream metadata at the
      <a href="https://lowpressurezone.com:8443">test stream info page.</a>
    </p>
    <p>
      Take a listen on your phone to the sound you're sending on the
      <a href="https://lowpressurezone.com:8443/test">audio page.</a>
    </p>
    <img
      alt="Icecast debug page"
      src="@/assets/stream-setup-img/broadcasting-debug-test.png" />
    <p>
      If everything is sounding good, go ahead and stop the stream with the square button next to
      the play button in Butt.
    </p>
    <h4>Live stream settings</h4>
    <p>
      After that, go back to the Butt settings. Click Edit on the Server settings and update the
      settings as follows:
    </p>
    <LiveConnectionInformation />
    <h4>Set Stream Title</h4>
    <p>Enter the text you want to appear on the website's play button and click Save.</p>
    <BroadcastingForm
      v-if="connectionInfoStore.liveInfo()"
      :info="connectionInfoStore.liveInfo()!" />
    <p>The above setting is also visible on the Streaming Info tab at the top of this page.</p>
    <h4>Enjoy streaming!</h4>
    <p>
      That's it! The next time you connect to the server for a stream, your audio will be accessible
      via the play button on the website.
    </p>
  </div>
</template>

<script lang="ts" setup>
import { Message } from 'primevue'
import { onMounted } from 'vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import LiveConnectionInformation from '@/components/views/dashboard/streaming/LiveConnectionInformation.vue'
import BroadcastingForm from '@/components/form/requestForms/BroadcastingForm.vue'

const connectionInfoStore = useConnectionInfoStore()

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
})
</script>

<style lang="scss">
.external-streaming-directions {
  overflow-x: auto;
  text-align: center;

  img {
    max-width: 100%;
  }

  p {
    text-align: left;
  }
}
</style>
