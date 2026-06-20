import { useState, useRef, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { ArrowLeft, Send, PawPrint } from 'lucide-react'
import api from '../services/api'
import { useSignalR } from '../hooks/useSignalR'
import { useAuthStore } from '../store/authStore'
import '../styles/chat.css'

export default function ChatPage() {
  const { appointmentId } = useParams<{ appointmentId: string }>()
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const [text, setText] = useState('')
  const bottomRef = useRef<HTMLDivElement>(null)
  const inputRef = useRef<HTMLInputElement>(null)

  const { data: history } = useQuery({
    queryKey: ['messages', appointmentId],
    queryFn: () => api.get(`/api/appointment/${appointmentId}/messages`).then((r) => r.data.result),
    enabled: !!appointmentId,
  })

  const { messages, setMessages, sendMessage, connected } = useSignalR(appointmentId ?? null)

  useEffect(() => { if (history) setMessages(history) }, [history])
  useEffect(() => { bottomRef.current?.scrollIntoView({ behavior: 'smooth' }) }, [messages])

  const handleSend = () => {
    if (!text.trim()) return
    sendMessage(text.trim())
    setText('')
    inputRef.current?.focus()
  }

  return (
    <div className="chat-wrap">

      {/* Header */}
      <div className="chat-header">
        <button className="chat-back-btn" onClick={() => navigate(-1)} aria-label="Volver">
          <ArrowLeft style={{ width: 17, height: 17 }} />
        </button>
        <div className="chat-header-icon">
          <PawPrint style={{ width: 16, height: 16, color: '#fff' }} />
        </div>
        <div className="chat-header-info">
          <p className="chat-header-title">Chat de cita</p>
          <p className="chat-header-sub">#{appointmentId?.slice(0, 8)}</p>
        </div>
        <div className={`chat-status${connected ? ' online' : ' offline'}`}>
          <span className="chat-status-dot" />
          {connected ? 'En línea' : 'Conectando...'}
        </div>
      </div>

      {/* Messages */}
      <div className="chat-messages">
        {messages.length === 0 && (
          <div className="chat-empty">
            <div className="chat-empty-icon">
              <PawPrint style={{ width: 22, height: 22, color: '#94a3b8' }} />
            </div>
            <p className="chat-empty-text">Inicia la conversación</p>
          </div>
        )}
        {messages.map((msg) => {
          const isMe = msg.senderId === user?.id
          return (
            <div key={msg.id} className={`chat-msg-row${isMe ? ' mine' : ' theirs'}`}>
              {!isMe && (
                <div className="chat-avatar">
                  {msg.senderName?.[0]?.toUpperCase() ?? '?'}
                </div>
              )}
              <div className="chat-bubble-wrap">
                {!isMe && <p className="chat-sender-name">{msg.senderName}</p>}
                <div className={`chat-bubble${isMe ? ' mine' : ' theirs'}`}>
                  <p style={{ margin: 0 }}>{msg.content}</p>
                  <p className={`chat-time${isMe ? ' mine' : ' theirs'}`}>
                    {new Date(msg.sentAt).toLocaleTimeString('es-CO', { hour: '2-digit', minute: '2-digit' })}
                  </p>
                </div>
              </div>
            </div>
          )
        })}
        <div ref={bottomRef} />
      </div>

      {/* Input */}
      <div className="chat-input-bar">
        <input
          ref={inputRef}
          className="chat-input"
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && !e.shiftKey && handleSend()}
          placeholder="Escribe un mensaje..."
        />
        <button className="chat-send-btn" onClick={handleSend} disabled={!connected || !text.trim()}>
          <Send style={{ width: 16, height: 16, color: '#fff' }} />
        </button>
      </div>
    </div>
  )
}
