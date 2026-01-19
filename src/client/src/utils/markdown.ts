import { marked } from 'marked'
import { type Config, default as DOMPurify } from 'dompurify'
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import { getPublicSlotHours, getSlotForTime } from '@/utils/scheduleUtils.ts'
import { TZDate } from '@date-fns/tz'
import ianaTimezones from '@/constants/ianaTimezones.ts'

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
    'hr'
  ]
}

export const scheduleToRedditMarkdown = (schedule: ScheduleResponse) => {
  const rows = getRowsForRedditMarkdownSchedule(schedule)
  let titleRow = '| Pacific Time | Eastern Time | UK Time | Performer |'
  let separatorRow = '|---|---|---|---|'
  const rowsMarkdown: string[] = []
  const shouldUseDetails = rows.some((row) => row.subtitle !== '')
  if (shouldUseDetails) {
    titleRow += ' Details |'
    separatorRow += '---|'
  }

  rows.forEach((row) => {
    const pacific = new TZDate(row.start, ianaTimezones.Pacific).toLocaleTimeString()
    const eastern = new TZDate(row.start, ianaTimezones.Eastern).toLocaleTimeString()
    const uk = new TZDate(row.start, ianaTimezones.UK).toLocaleTimeString()
    let rowMarkdown = `| ${pacific} | ${eastern} | ${uk} | ${row.title} |`
    if (shouldUseDetails) rowMarkdown += ` ${row.subtitle} |`
    rowsMarkdown.push(rowMarkdown)
  })

  return titleRow + '\n' + separatorRow + '\n' + rowsMarkdown.join('\n')
}

const getRowsForRedditMarkdownSchedule = (schedule: ScheduleResponse) => {
  const rows: { start: Date; title: string; subtitle: string }[] = []
  const hours = getPublicSlotHours(schedule)
  hours.forEach((hour) => {
    const slot = getSlotForTime(schedule, hour)
    if (slot === undefined) {
      rows.push({ start: hour, title: '', subtitle: '' })
      return
    }
    // TimeslotResponse check
    if ('performer' in slot) {
      rows.push({
        start: hour,
        title: slot.performer.name,
        subtitle: slot.subtitle || ''
      })
    } else {
      rows.push({
        start: hour,
        title: `${slot.performerOne.name} vs. ${slot.performerTwo.name}`,
        subtitle: `${slot.roundOne} - ${slot.roundTwo} - ${slot.roundThree}`
      })
    }
  })

  return rows
}
