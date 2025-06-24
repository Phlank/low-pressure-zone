<template>
  <div class="software-streaming-directions">
    <h4>Introduction</h4>
    <p>
      Many DJ software applications have streaming capability built in. For those that don't, this
      guide won't be very useful, but that doesn't mean they can't be used. Skip to the end if you
      are a Serato user or use another software without streaming built-in. For the purpose of these
      instructions, I will show the process on the open-source, cross-platform DJing software
      <a href="https://mixxx.org/">Mixxx.</a>
    </p>
    <h4>Stream settings</h4>
    <p>
      In Mixxx, first open the Preferences menu by clicking on Options in the menu bar and then
      Preferences.
    </p>
    <img
      alt="Alternately, press Ctrl+P"
      src="@/assets/stream-setup-img/mixxx-preferences.png" />
    <p>
      From here, find the Live Broadcasting section on the left-side navigation of the Preferences
      menu. In here, you will set up the connection and stream info. Set the first sections up to
      match what is given in the below image. The password is
      {{ connectionInfoStore.testInfo()?.password }}
    </p>
    <img
      alt="How to locate the Live Broadcasting section, and the first set of items available"
      src="@/assets/stream-setup-img/mixxx-broadcasting-test-1.png" />
    <Message severity="warn">
      You <i>MUST</i> use MP3 for the encoding in order for all browsers to be able to listen to the
      stream. 256kbps is the minimum bitrate we will use for quality reasons.
    </Message>
    <h4>Stream metadata</h4>
    <p>
      Scroll down and set up the stream info. In whatever software you are using, the track title is
      what will appear on the play button on the website.
    </p>
    <img
      alt="Stream info setup"
      src="@/assets/stream-setup-img/mixxx-broadcasting-test-2.png" />
    <p>Finally, click the Apply button at the bottom right and close the Preferences window.</p>
    <h4>Test stream</h4>
    <p>
      Start broadcasting. In Mixxx, click the On Air button at the top right of the main window to
      begin. Once the button turns green to indicate a good connection, start playing a song.
    </p>
    <img
      alt="Directions for starting the broadcast stream"
      src="@/assets/stream-setup-img/mixxx-broadcasting-test-3.png" />
    <p>
      Navigate to the
      <a href="https://lowpressurezone.com:8443/">test stream status page.</a> You should see your
      stream appear like below. You may need to send audio to see the Now Playing field appear.
    </p>
    <img
      alt="Broadcasting debug test"
      src="@/assets/stream-setup-img/broadcasting-debug-test.png" />
    <p>
      On your phone, navigate to the
      <a href="https://lowpressurezone.com:8443/test">test audio page.</a> Listen for a moment and
      make sure that the audio sounds good. Disconnect when you are done.
    </p>
    <h4>Live stream settings</h4>
    <p>
      Now that you have streaming configured, open the preferences for streaming again and update
      the information to match the Live Stream configuration below. The next time you stream, it
      will be accessible through the play button on the website.
    </p>
    <img
      alt="Mixxx live configuration"
      src="@/assets/stream-setup-img/mixxx-live-config.png" />
    <p>The password for the live stream is the same as the test stream.</p>
    <h4>Software without streaming</h4>
    <p>
      Several people on the server use Serato. However, those users typically use some type of
      virtual audio cable software and route the application audio into a program called Butt.
      Instructions for connecting to the stream with Butt are on the Decks and Mixer tab at the top
      of this page. A recommended option for the virtual audio cable software is VB-CABLE, which
      works on both Windows and Mac.
    </p>
  </div>
</template>

<script lang="ts" setup>
import { Message } from 'primevue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { onMounted } from 'vue'

const connectionInfoStore = useConnectionInfoStore()

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
})
</script>

<style lang="scss">
.software-streaming-directions {
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
