import { useEffect, useState } from 'react'
import { getClasses, createClass } from './api'
import './App.css'

function ClassCard({ c }) {
  const confirmedPct = c.totalSeats ? (c.confirmedSeats / c.totalSeats) * 100 : 0
  const heldPct = c.totalSeats ? (c.heldSeats / c.totalSeats) * 100 : 0
  const isFull = c.availableSeats <= 0

  return (
    <div className="card">
      <div className="card-top">
        <h3>{c.name}</h3>
        <span className="id">#{c.id}</span>
      </div>

      <span className={`badge ${isFull ? 'full' : 'open'}`}>
        {isFull ? 'Full' : 'Open'}
      </span>

      <div className="bar-wrap">
        <div className="bar-label">
          <span>Seats filled</span>
          <span>
            {c.confirmedSeats + c.heldSeats} / {c.totalSeats}
          </span>
        </div>
        <div className="bar">
          <div className="confirmed" style={{ width: `${confirmedPct}%` }} />
          <div className="held" style={{ width: `${heldPct}%` }} />
        </div>
      </div>

      <div className="stats">
        <div className="stat total">
          <span className="num">{c.totalSeats}</span>
          <span className="lbl">Total</span>
        </div>
        <div className="stat available">
          <span className="num">{c.availableSeats}</span>
          <span className="lbl">Available</span>
        </div>
        <div className="stat held">
          <span className="num">{c.heldSeats}</span>
          <span className="lbl">Held</span>
        </div>
        <div className="stat confirmed">
          <span className="num">{c.confirmedSeats}</span>
          <span className="lbl">Confirmed</span>
        </div>
      </div>
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

  useEffect(() => {
    loadClasses()
  }, [])

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
              <ClassCard c={c} key={c.id} />
            ))}
          </div>
        )}
      </main>
    </>
  )
}

export default App
