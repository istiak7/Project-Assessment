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
    const message =
      data?.message ||
      data?.Message ||
      data?.Errors?.[0]?.ErrorMessage ||
      `Request failed (${res.status})`
    throw new Error(message)
  }

  return data
}

export const getClasses = () => apiFetch('/classes')

export const createClass = (name, totalSeats) =>
  apiFetch('/classes', {
    method: 'POST',
    body: JSON.stringify({ name, totalSeats }),
  })

export const createHold = (classId, studentId) =>
  apiFetch(`/classes/${classId}/holds`, {
    method: 'POST',
    headers: { 'X-Student-Id': studentId },
  })

export const getMyHolds = (studentId) =>
  apiFetch('/me/holds', { headers: { 'X-Student-Id': studentId } })

export const confirmHold = (holdId, studentId) =>
  apiFetch(`/holds/${holdId}/confirm`, {
    method: 'POST',
    headers: { 'X-Student-Id': studentId },
  })

export const cancelHold = (holdId, studentId) =>
  apiFetch(`/holds/${holdId}`, {
    method: 'DELETE',
    headers: { 'X-Student-Id': studentId },
  })
