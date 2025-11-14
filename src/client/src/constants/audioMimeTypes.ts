// Pulled from https://developer.mozilla.org/en-US/docs/Web/Media/Guides/Formats/Containers

const flac = 'audio/flac'
const m4a = 'audio/mp4'
const mp3 = 'audio/mpeg'
const ogg: string[] = ['audio/ogg', 'audio/x-ogg']
const wav: string[] = ['audio/wav', 'audio/wave', 'audio/vnd.wave', 'audio/x-wave', 'audio/x-pn-wave']

export const audioMimeTypes: string[] = [mp3, ...wav, flac, m4a, ...ogg]
