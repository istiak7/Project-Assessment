import { memo, useCallback, useEffect, useState } from 'react'
import {
  getClasses,
  createClass,
  createHold,
  getMyHolds,
  confirmHold,
  cancelHold,
} from './api'
import './App.css'

function HoldModal({ hold, studentId, onClose, onChange }) {
  const [status, setStatus] = useState(hold.status || 'HELD')
  const [working, setWorking] = useState(false)
  const [msg, setMsg] = useState(null)
  const [, setTick] = useState(0)

  useEffect(() => {
    if (status !== 'HELD') return
    const id = setInterval(() => {
      if (new Date(hold.expiresAt) - Date.now() <= 0) {
        clearInterval(id)
        setStatus('EXPIRED')
        onChange()
      } else {
        setTick((t) => t + 1)
      }
    }, 1000)
    return () => clearInterval(id)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [status, hold.expiresAt])

  const secondsLeft = Math.max(
    0,
    Math.round((new Date(hold.expiresAt) - Date.now()) / 1000),
  )
  const mm = String(Math.floor(secondsLeft / 60)).padStart(2, '0')
  const ss = String(secondsLeft % 60).padStart(2, '0')
  const low = secondsLeft <= 10
  const utcTime = new Date(hold.expiresAt).toLocaleTimeString('en-US', {
    timeZone: 'UTC',
    hour12: true,
  })

  const handleConfirm = async () => {
    setMsg(null)
    setWorking(true)
    try {
      await confirmHold(hold.holdId, studentId)
      setStatus('CONFIRMED')
      onChange()
    } catch (err) {
      setMsg({ type: 'err', text: err.message })
    } finally {
      setWorking(false)
    }
  }

  const handleCancel = async () => {
    setMsg(null)
    setWorking(true)
    try {
      await cancelHold(hold.holdId, studentId)
      onChange()
      onClose()
    } catch (err) {
      setMsg({ type: 'err', text: err.message })
      setWorking(false)
    }
  }

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal wide" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <div className="modal-head-left">
            <span className="modal-icon">🕐</span>
            <div>
              <h3 className="modal-title">Application &amp; Admission Details</h3>
              <span className="modal-holdid">Hold ID: h_{hold.holdId}</span>
            </div>
          </div>
          <button className="modal-close" type="button" onClick={onClose}>
            ×
          </button>
        </div>

        {status === 'HELD' && (
          <>
            <div className="status-grid">
              <span className="status-side">Status:</span>
              <div className={`expiry-box ${low ? 'low' : ''}`}>
                <div className="expiry-top">
                  <span className="expiry-title">🕐 Time left to expiry (HOLD EXPIRY)</span>
                  <span className="expiry-count">
                    {mm}:{ss}
                  </span>
                </div>
                <p className="expiry-note">
                  If the admission fee is not paid within this time, the seat will
                  be released and made available to everyone again.
                </p>
              </div>
            </div>

            <div className="detail-box">
              <div className="detail-row">
                <span>🎓 Class &amp; Shift</span>
                <strong>{hold.className}</strong>
              </div>
              <div className="detail-row">
                <span>👤 Student Birth Registration</span>
                <strong>{studentId}</strong>
              </div>
              <div className="detail-row">
                <span># Class ID</span>
                <strong>c_{hold.classId}</strong>
              </div>
              <div className="detail-row">
                <span>📅 Expiry time (UTC)</span>
                <strong>{utcTime}</strong>
              </div>
            </div>

            {msg && <p className={`card-msg ${msg.type}`}>{msg.text}</p>}

            <div className="modal-footer">
              <button
                className="btn-danger-outline"
                type="button"
                disabled={working}
                onClick={handleCancel}
              >
                🗑 Cancel Hold
              </button>
              <button
                className="btn-ghost"
                type="button"
                disabled={working}
                onClick={onClose}
              >
                Close
              </button>
              <button
                className="btn confirm"
                type="button"
                disabled={working}
                onClick={handleConfirm}
              >
                {working ? 'Processing…' : '💳 Confirm & Pay'}
              </button>
            </div>
          </>
        )}

        {status === 'CONFIRMED' && (
          <div className="modal-result">
            <div className="big-icon">🎉</div>
            <p>
              Payment successful — your seat is <strong>confirmed</strong>.
            </p>
            <button className="btn confirm" type="button" onClick={onClose}>
              Done
            </button>
          </div>
        )}

        {status === 'EXPIRED' && (
          <div className="modal-result">
            <div className="big-icon">⏰</div>
            <p>
              This hold has <strong>expired</strong>. Please apply again.
            </p>
            <button className="btn-ghost" type="button" onClick={onClose}>
              Close
            </button>
          </div>
        )}
      </div>
    </div>
  )
}

const ClassCard = memo(function ClassCard({ c, studentId, myHold, onOpenHold, onChange }) {
  const confirmedPct = c.totalSeats ? (c.confirmedSeats / c.totalSeats) * 100 : 0
  const heldPct = c.totalSeats ? (c.heldSeats / c.totalSeats) * 100 : 0
  const isFull = c.availableSeats <= 0
  const isDay = /day/i.test(c.name)

  const [working, setWorking] = useState(false)
  const [msg, setMsg] = useState(null)
  const [, setTick] = useState(0)

  const isHeld = myHold?.status === 'HELD'
  const isConfirmed = myHold?.status === 'CONFIRMED'

  useEffect(() => {
    if (!isHeld) return
    const id = setInterval(() => {
      if (new Date(myHold.expiresAt) - Date.now() <= 0) {
        clearInterval(id)
        onChange()
      } else {
        setTick((t) => t + 1)
      }
    }, 1000)
    return () => clearInterval(id)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isHeld, myHold?.expiresAt])

  const heldSecondsLeft = isHeld
    ? Math.max(0, Math.round((new Date(myHold.expiresAt) - Date.now()) / 1000))
    : 0

  const handleApply = async () => {
    setMsg(null)
    if (!/^\d{17}$/.test(studentId)) {
      setMsg({ type: 'err', text: 'Please log in with your Student ID first.' })
      return
    }
    setWorking(true)
    try {
      const hold = await createHold(c.id, studentId)
      onChange()
      onOpenHold({
        holdId: hold.holdId,
        classId: c.id,
        className: c.name,
        expiresAt: hold.expiresAt,
        status: 'HELD',
      })
    } catch (err) {
      setMsg({ type: 'err', text: err.message })
    } finally {
      setWorking(false)
    }
  }

  const openExisting = () =>
    onOpenHold({
      holdId: myHold.holdId,
      classId: myHold.classId,
      className: myHold.className,
      expiresAt: myHold.expiresAt,
      status: 'HELD',
    })

  return (
    <div className="card">
      <div className="card-head">
        <span className={`shift ${isDay ? 'day' : 'morning'}`}>
          {isDay ? 'Day Shift' : 'Morning Shift'}
        </span>
        <span className="id">ID: c_{c.id}</span>
        <span className={`avail-pill ${isFull ? 'full' : ''}`}>
          {isFull ? 'Full' : `${c.availableSeats} available`}
        </span>
      </div>

      <h3 className="title">{c.name}</h3>

      <div className="bar">
        <div className="confirmed" style={{ width: `${confirmedPct}%` }} />
        <div className="held" style={{ width: `${heldPct}%` }} />
      </div>

      <div className="inline-row">
        <span>
          Confirmed: <strong className="c-green">{c.confirmedSeats}</strong>
        </span>
        <span>
          Held: <strong className="c-amber">{c.heldSeats}</strong>
        </span>
        <span>
          Total: <strong>{c.totalSeats}</strong>
        </span>
      </div>

      <div className="stats">
        <div className="stat total">
          <span className="lbl">Total Seats</span>
          <span className="num">{c.totalSeats}</span>
        </div>
        <div className="stat confirmed">
          <span className="lbl">Confirmed</span>
          <span className="num">{c.confirmedSeats}</span>
        </div>
        <div className="stat held">
          <span className="lbl">Held</span>
          <span className="num">{c.heldSeats}</span>
        </div>
        <div className="stat available">
          <span className="lbl">Available</span>
          <span className="num">{c.availableSeats}</span>
        </div>
      </div>

      {isHeld ? (
        <div className="app-status">
          <div className="status-line">
            <span className="status-label">👤 Your application status:</span>
            <span className="status-badge held">HELD</span>
          </div>
          <p className="expiry">Expires in {heldSecondsLeft}s — pay to secure your seat.</p>
          <button className="btn confirm" type="button" onClick={openExisting}>
            Confirm & Pay
          </button>
        </div>
      ) : isConfirmed ? (
        <div className="app-status">
          <div className="status-line">
            <span className="status-label">👤 Your application status:</span>
            <span className="status-badge confirmed">CONFIRMED</span>
          </div>
          <p className="expiry confirmed-note">🎉 Seat confirmed — you’re enrolled.</p>
        </div>
      ) : (
        <button
          className="btn apply"
          type="button"
          disabled={isFull || working}
          onClick={handleApply}
        >
          {isFull ? 'No Seats Available' : working ? 'Holding…' : '◎ Apply & Hold Seat'}
        </button>
      )}

      {msg && <p className={`card-msg ${msg.type}`}>{msg.text}</p>}
    </div>
  )
})

function App() {
  const [classes, setClasses] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const [name, setName] = useState('')
  const [totalSeats, setTotalSeats] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [formError, setFormError] = useState('')
  const [isAdmin, setIsAdmin] = useState(false)
  const [adminPasswordInput, setAdminPasswordInput] = useState('')
  const [adminError, setAdminError] = useState('')

  const [studentId, setStudentId] = useState(
    () => localStorage.getItem('studentId') || '',
  )
  const [draft, setDraft] = useState(
    () => localStorage.getItem('studentId') || '',
  )
  const [holdsByClass, setHoldsByClass] = useState({})
  const [modalHold, setModalHold] = useState(null)

  const loggedIn = /^\d{17}$/.test(studentId)

  const handleLogin = (e) => {
    e.preventDefault()
    if (!/^\d{17}$/.test(draft)) return
    setStudentId(draft)
    localStorage.setItem('studentId', draft)
  }

  const handleReset = () => {
    localStorage.removeItem('studentId')
    setStudentId('')
    setDraft('')
    setHoldsByClass({})
    setModalHold(null)
  }

  const handleAdminUnlock = (e) => {
    e.preventDefault()
    if (adminPasswordInput === import.meta.env.VITE_ADMIN_SECRET_KEY) {
      localStorage.removeItem('studentId')
      setStudentId('')
      setDraft('')
      setHoldsByClass({})
      setModalHold(null)
      setIsAdmin(true)
      setAdminPasswordInput('')
      setAdminError('')
    } else {
      setAdminError('Incorrect admin password.')
    }
  }

  const handleAdminExit = () => {
    setIsAdmin(false)
    setAdminError('')
  }

  const loadClasses = useCallback((initial = false) => {
    if (initial) setLoading(true)
    return getClasses()
      .then((data) => {
        const nextClasses = Array.isArray(data) ? data : []
        setClasses((current) =>
          JSON.stringify(current) === JSON.stringify(nextClasses) ? current : nextClasses,
        )
        setError((current) => (current === '' ? current : ''))
      })
      .catch((err) => setError((current) => (current === err.message ? current : err.message)))
      .finally(() => {
        if (initial) setLoading(false)
      })
  }, [])

  const loadMyHolds = useCallback(() => {
    if (!/^\d{17}$/.test(studentId)) {
      setHoldsByClass((current) => (Object.keys(current).length === 0 ? current : {}))
      return
    }
    getMyHolds(studentId)
      .then((list) => {
        const map = {}
        for (const h of Array.isArray(list) ? list : []) {
          if ((h.status === 'HELD' || h.status === 'CONFIRMED') && !map[h.classId]) {
            map[h.classId] = h
          }
        }
        setHoldsByClass((current) =>
          JSON.stringify(current) === JSON.stringify(map) ? current : map,
        )
      })
      .catch(() =>
        setHoldsByClass((current) => (Object.keys(current).length === 0 ? current : {})),
      )
  }, [studentId])

  const refresh = useCallback(() => {
    loadClasses(false)
    loadMyHolds()
  }, [loadClasses, loadMyHolds])

  useEffect(() => {
    loadClasses(true)
  }, [loadClasses])

  useEffect(() => {
    loadMyHolds()
  }, [loadMyHolds])

  useEffect(() => {
    const interval = setInterval(refresh, 5000)
    return () => clearInterval(interval)
  }, [refresh])
  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!isAdmin) return
    setFormError('')
    setSubmitting(true)
    try {
      await createClass(name.trim(), Number(totalSeats))
      setName('')
      setTotalSeats('')
      loadClasses()
    } catch (err) {
      setFormError(err.message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <>
      <header className="app-header">
        <div className="bar">
          <div className="brand">
            <span className="logo">🎓</span>
            <span>
              Admissions
              <small>Class & seat management</small>
            </span>
          </div>
          <div className="admin-toggle">
            {isAdmin ? (
              <>
                <span className="admin-active">🔒 Admin Mode Active</span>
                <button className="btn-reset" type="button" onClick={handleAdminExit}>
                  Exit Admin Mode
                </button>
              </>
            ) : (
              <form className="admin-form" onSubmit={handleAdminUnlock}>
                <input
                  className="admin-input"
                  type="password"
                  aria-label="Admin password"
                  placeholder="Admin password"
                  value={adminPasswordInput}
                  onChange={(e) => {
                    setAdminPasswordInput(e.target.value)
                    setAdminError('')
                  }}
                />
                <button className="btn-login" type="submit">
                  Unlock Admin Mode
                </button>
                {adminError && <span className="admin-error" role="alert">{adminError}</span>}
              </form>
            )}
          </div>
          {!isAdmin && <div className="auth">
            {loggedIn ? (
              <div className="auth-in">
                <span className="auth-chip">
                  <span className="auth-avatar">👤</span>
                  <span className="auth-meta">
                    <small>Signed in as</small>
                    <span className="auth-id">{studentId}</span>
                  </span>
                </span>
                <button className="btn-reset" type="button" onClick={handleReset}>
                  Reset
                </button>
              </div>
            ) : (
              <form className="auth-form" onSubmit={handleLogin}>
                <div className="auth-input-wrap">
                  <span className="auth-ic">🎫</span>
                  <input
                    className="auth-input"
                    type="text"
                    inputMode="numeric"
                    maxLength={17}
                    placeholder="Enter 17-digit Student ID"
                    value={draft}
                    onChange={(e) => setDraft(e.target.value.replace(/\D/g, ''))}
                  />
                  {draft.length > 0 && (
                    <span className={`auth-counter ${draft.length === 17 ? 'ok' : ''}`}>
                      {draft.length}/17
                    </span>
                  )}
                </div>
                <button className="btn-login" type="submit" disabled={draft.length !== 17}>
                  Log in
                </button>
              </form>
            )}
          </div>}
        </div>
      </header>

      <main className="container">
        {isAdmin && <section className="panel">
          <h2>Create a class</h2>
          <p className="subtitle">Add a new class and set how many seats it offers.</p>
          <form className="form-grid" onSubmit={handleSubmit}>
            <div className="field">
              <label htmlFor="name">Class name</label>
              <input
                id="name"
                type="text"
                placeholder="e.g. Class 6 - Morning Shift"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>
            <div className="field">
              <label htmlFor="seats">Total seats</label>
              <input
                id="seats"
                type="number"
                min="1"
                placeholder="e.g. 120"
                value={totalSeats}
                onChange={(e) => setTotalSeats(e.target.value)}
                required
              />
            </div>
            <button className="btn" type="submit" disabled={submitting}>
              {submitting ? 'Adding…' : 'Add class'}
            </button>
          </form>
          {formError && <p className="msg error">{formError}</p>}
        </section>}

        <div className="section-head">
          <h2>All classes</h2>
          {!loading && !error && <span>{classes.length} total</span>}
        </div>

        {error && <p className="msg error">{error}</p>}

        {loading ? (
          <div className="grid">
            {Array.from({ length: 3 }).map((_, i) => (
              <div className="skeleton" key={i} />
            ))}
          </div>
        ) : classes.length === 0 && !error ? (
          <div className="state">No classes yet — create your first one above.</div>
        ) : (
          <div className="grid">
            {classes.map((c) => (
              <ClassCard
                c={c}
                key={c.id}
                studentId={studentId}
                myHold={holdsByClass[c.id]}
                onOpenHold={setModalHold}
                onChange={refresh}
              />
            ))}
          </div>
        )}
      </main>

      {modalHold && (
        <HoldModal
          hold={modalHold}
          studentId={studentId}
          onClose={() => setModalHold(null)}
          onChange={refresh}
        />
      )}
    </>
  )
}

export default App
