// Pulled from https://developer.mozilla.org/en-US/docs/Web/Media/Guides/Formats/Containers

const flac = 'audio/flac'
const m4a: string[] = ['audio/mp4', 'audio/m4a', 'audio/x-mp4', 'audio/x-m4a']
const mp3 = 'audio/mpeg'
const ogg: string[] = ['audio/ogg', 'audio/x-ogg']
const wav: string[] = [
  'audio/wav',
  'audio/wave',
  'audio/vnd.wave',
  'audio/x-wav',
  'audio/x-wave',
  'audio/x-pn-wav',
  'audio/x-pn-wave'
]

export const audioMimeTypes: string[] = [mp3, ...wav, flac, ...m4a, ...ogg]
