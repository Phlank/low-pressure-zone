export default (ms: number, controller?: AbortController) =>
  new Promise<void>((resolve) => {
    const signal = controller?.signal
    if (signal?.aborted) {
      resolve()
      return
    }

    const onAbort = () => {
      clearTimeout(timeout)
      resolve()
    }

    const timeout = setTimeout(() => {
      signal?.removeEventListener('abort', onAbort)
      resolve()
    }, ms)
    signal?.addEventListener('abort', onAbort, { once: true })
  })
