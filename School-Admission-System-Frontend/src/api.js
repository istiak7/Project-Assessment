const BASE = '/api'

export async function apiFetch(path, options = {}) {
  const res = await fetch(`${BASE}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {}),
    },
  })

  const text = await res.text()
  const data = text ? JSON.parse(text) : null

  if (!res.ok) {
    throw new Error(data?.message || `Request failed (${res.status})`)
  }

  return data
}

export const getClasses = () => apiFetch('/classes')

export const createClass = (name, totalSeats) =>
  apiFetch('/classes', {
    method: 'POST',
    body: JSON.stringify({ name, totalSeats }),
  })
