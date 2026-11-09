import { useEffect, useState } from 'react'
import {
  getClasses,
  createClass,
  createHold,
  getMyHolds,
  confirmHold,
  cancelHold,
} from './api'
import './App.css'

function ClassCard({ c, studentId, myHold, onChange }) {
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

  const secondsLeft = myHold
    ? Math.max(0, Math.round((new Date(myHold.expiresAt) - Date.now()) / 1000))
    : 0

  const run = async (fn) => {
    setMsg(null)
    setWorking(true)
    try {
      await fn()
      onChange()
    } catch (err) {
      setMsg({ type: 'err', text: err.message })
    } finally {
      setWorking(false)
    }
  }

  const handleApply = () => {
    if (!/^\d{17}$/.test(studentId)) {
      setMsg({ type: 'err', text: 'Enter a valid 17-digit Student ID in the header first.' })
      return
    }
    run(() => createHold(c.id, studentId))
  }

  const handleConfirm = () => run(() => confirmHold(myHold.holdId, studentId))
  const handleCancel = () => run(() => cancelHold(myHold.holdId, studentId))

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

      {myHold ? (
        <div className="app-status">
          <div className="status-line">
            <span className="status-label">👤 Your application status:</span>
            <span className={`status-badge ${myHold.status.toLowerCase()}`}>
              {myHold.status}
            </span>
          </div>

          {isHeld && (
            <>
              <p className="expiry">Expires in {secondsLeft}s — confirm to secure your seat.</p>
              <div className="actions">
                <button
                  className="btn confirm"
                  type="button"
                  disabled={working}
                  onClick={handleConfirm}
                >
                  {working ? 'Working…' : '✓ Confirm Seat'}
                </button>
                <button
                  className="btn-ghost"
                  type="button"
                  disabled={working}
                  onClick={handleCancel}
                >
                  Cancel
                </button>
              </div>
            </>
          )}

          {isConfirmed && (
            <p className="expiry confirmed-note">🎉 Seat confirmed — you’re enrolled.</p>
          )}
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
}

function App() {
  const [classes, setClasses] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const [name, setName] = useState('')
  const [totalSeats, setTotalSeats] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [formError, setFormError] = useState('')

  const [studentId, setStudentId] = useState(
    () => localStorage.getItem('studentId') || '',
  )
  const [holdsByClass, setHoldsByClass] = useState({})

  const handleStudentId = (value) => {
    setStudentId(value)
    localStorage.setItem('studentId', value)
  }

  const loadClasses = () => {
    setLoading(true)
    getClasses()
      .then((data) => {
        setClasses(Array.isArray(data) ? data : [])
        setError('')
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }

  const loadMyHolds = () => {
    if (!/^\d{17}$/.test(studentId)) {
      setHoldsByClass({})
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
        setHoldsByClass(map)
      })
      .catch(() => setHoldsByClass({}))
  }

  const refresh = () => {
    loadClasses()
    loadMyHolds()
  }

  useEffect(() => {
    loadClasses()
  }, [])

  useEffect(() => {
    loadMyHolds()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [studentId])

  const handleSubmit = async (e) => {
    e.preventDefault()
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
          <input
            className="student-input"
            type="text"
            inputMode="numeric"
            maxLength={17}
            placeholder="Student ID (17 digits)"
            value={studentId}
            onChange={(e) => handleStudentId(e.target.value.replace(/\D/g, ''))}
          />
          <span className="header-count">{classes.length} classes</span>
        </div>
      </header>

      <main className="container">
        <section className="panel">
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
        </section>

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
                onChange={refresh}
              />
            ))}
          </div>
        )}
      </main>
    </>
  )
}

export default App
