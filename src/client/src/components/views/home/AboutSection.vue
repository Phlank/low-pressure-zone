<template>
  <div class="about-section">
    <p>
      Low Pressure Zone is an internet radio venture by members of the
      <a
        class="nowrap"
        href="https://reddit.com/r/realdubstep"
        >/r/realdubstep</a
      >
      subreddit community. Just a bunch of music enthusiasts chatting and streaming (mostly deep
      dubstep) for fun.
    </p>

    <div :class="moreClass">
      <p>
        <b>
          To chat and participate, join the Discord server by clicking the Chat button at the top of
          the page. If you want to DJ with us, join the Discord server and make a post in the
          #i-want-to-stream-please channel.
        </b>
      </p>
      <p>
        Background: around ~2013 or so some users took it upon themselves to set up weekly streaming
        events for community members to participate in (subreddit get-togethers, basically). This
        was called C9 (or Clouwdnine) and was run through a simple website. People would show up in
        the chat, one person would send audio and people would listen directly from the server.
        <a
          href="https://www.reddit.com/r/realdubstep/comments/3ntv0t/clouwdnine_tonight_from_4pm_5am_with_itto_walla/"
          >Example thread for one of these events.</a
        >
      </p>
      <p>
        Eventually this gained more traction and people wanted to do more than just the weekly
        shows, so they set up a soundclash event. Two DJs/producers would take turns streaming audio
        over an hour and a half, spinning for three rounds, 15 minutes each per round. After each
        round, there would be a vote in the chat and by the end of it you had a "winner" of that
        soundclash. This caught the attention of FatKidOnFire, which eventually led to
        <a
          href="https://www.reddit.com/r/realdubstep/comments/37ucco/c9_x_fkof_soundclash_recordings/">
          a soundclash event between C9 and FKOF.
        </a>
      </p>

      <p>
        We'd like to hold similar events eventually, but for now we're focusing on hosting regular
        radio days as we get back into the swing of things.
      </p>
    </div>

    <div style="display: flex; justify-content: center">
      <Button
        :label="`Read ${isShowingMore ? 'Less' : 'More'}`"
        class="about-section__show-more"
        outlined
        severity="secondary"
        @click="isShowingMore = !isShowingMore" />
    </div>
  </div>
</template>
<script lang="ts" setup>
import { Button } from 'primevue'
import { computed, ref } from 'vue'
import { useResizeObserver } from '@vueuse/core'

const isShowingMore = ref(false)
const moreHeight = ref('0px')
useResizeObserver(document.body, () => {
  const newHeight =
    (document.getElementsByClassName('about-section__more')[0]?.scrollHeight ?? 0) + 'px'
  if (newHeight !== moreHeight.value) {
    moreHeight.value = newHeight
  }
})

const moreClass = computed(
  () => `about-section__more about-section__more--${isShowingMore.value ? 'visible' : 'hidden'}`
)
</script>

<style lang="scss">
//noinspection CssInvalidFunction
.about-section {
  &__more {
    overflow: hidden;
    transition: max-height 0.15s ease-in-out;

    &--hidden {
      max-height: 0;
    }

    &--visible {
      max-height: v-bind(moreHeight);
    }

    :first-child {
      margin-top: 0;
    }

    :last-child {
      margin-bottom: 0;
    }
  }

  &__show-more {
    margin-top: 1rem;
  }
}
</style>
