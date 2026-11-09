import { useEffect, useState } from 'react'
import { getClasses } from './api'

function App() {
  const [status, setStatus] = useState('Connecting...')
  const [classes, setClasses] = useState([])
  const [error, setError] = useState('')

  useEffect(() => {
    getClasses()
      .then((data) => {
        setClasses(Array.isArray(data) ? data : [])
        setStatus('Connected to backend')
      })
      .catch((err) => {
        setStatus('Failed to connect')
        setError(err.message)
      })
  }, [])

  return (
    <main style={{ fontFamily: 'sans-serif', padding: '2rem', maxWidth: 720, margin: '0 auto' }}>
      <h1>School Admission System</h1>
      <p>
        Backend status: <strong>{status}</strong>
      </p>
      {error && <p style={{ color: 'crimson' }}>{error}</p>}
      <h2>Classes ({classes.length})</h2>
      <pre style={{ background: '#f4f4f4', padding: '1rem', overflowX: 'auto' }}>
        {JSON.stringify(classes, null, 2)}
      </pre>
    </main>
  )
}

export default App
