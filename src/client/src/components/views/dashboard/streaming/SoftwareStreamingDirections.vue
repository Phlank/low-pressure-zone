<template>
  <div>
    <h2>Streaming from DJ software</h2>
    <p>
      Most DJing software has streaming capabilities built in. For the purpose of these
      instructions, I will show the process on the open-source, cross-platform DJing software
      <a href="https://mixxx.org/">Mixxx</a>.
    </p>
    <h3>Stream settings</h3>
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
      match what is given in the below image.
      <b>You will need to ask @phlank in discord for the password.</b>
    </p>
    <img
      alt="How to locate the Live Broadcasting section, and the first set of items available"
      src="@/assets/stream-setup-img/mixxx-broadcasting-test-1.png" />
    <Message severity="warn">
      You <i>MUST</i> use MP3 for the encoding in order for all browsers to be able to listen to the
      stream. 256kbps is the minimum bitrate we will use for quality reasons.
    </Message>
    <p>Next, scroll down and set up the stream info.</p>
    <img
      alt="Stream info setup"
      src="@/assets/stream-setup-img/mixxx-broadcasting-test-2.png" />
    <p>
      Finally, click the Apply button at the bottom right and close the Preferences window. Now,
      click on the following to begin streaming to the "test" location:
    </p>
    <img
      alt="Directions for starting the broadcast stream"
      src="@/assets/stream-setup-img/mixxx-broadcasting-test-3.png" />
    <p>
      After clicking on Enable Live Broadcasting, navigate to the server's
      <a :href="streamDebug">stream debug page</a>. You should see your stream appear like so:
    </p>
    <img
      alt="Broadcasting debug test"
      src="@/assets/stream-setup-img/broadcasting-debug-test.png" />
    <p>
      If your connection was unsuccessful and someone else is using the /test mount point, you can
      change the Mount in your Server connection settings to <kbd>test2</kbd>.
    </p>
    <p>
      After seeing your appropriate stream info appear on the debug page, navigate to the stream's
      URL to listen and ensure things sound okay. The URL will be
      <kbd>https://lowpressurezone.com:8443/{mount}</kbd>, where <kbd>{mount}</kbd> is either
      <kbd>test</kbd> or <kbd>test2</kbd>.
    </p>
    <p>
      Now that you have streaming configured, change your Mount to <kbd>live</kbd>, and the next
      time you stream, it will be accessible through the play button on the website :)
    </p>
  </div>
</template>

<script lang="ts" setup>
import { Message } from 'primevue'

const streamDebug = import.meta.env.VITE_STREAM_DEBUG
</script>
