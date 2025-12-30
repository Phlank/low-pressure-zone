import { marked } from 'marked'
import { type Config, default as DOMPurify } from 'dompurify'

export const parseMarkdownAsync = async (markdown: string): Promise<string> => {
  const parsed = await marked.parse(markdown)
  return DOMPurify.sanitize(parsed, domPurifyConfig)
}

const domPurifyConfig: Config = {
  USE_PROFILES: { html: true },
  ALLOWED_TAGS: [
    'br',
    'p',
    'h1',
    'h2',
    'h3',
    'h4',
    'h5',
    'h6',
    'div',
    'span',
    'div',
    'table',
    'tr',
    'td',
    'th',
    'tbody',
    'thead',
    'pre',
    'code',
    'strong',
    'em',
    'blockquote',
    'ul',
    'ol',
    'li',
    'a',
    'img',
    'hr',
    'div'
  ]
}
