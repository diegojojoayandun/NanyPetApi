import * as signalR from '@microsoft/signalr'
import { useEffect, useRef, useState } from 'react'

export interface ChatMessage {
  id: string
  appointmentId: string
  senderId: string
  senderName: string
  content: string
  sentAt: string
  isRead: boolean
}

export function useSignalR(appointmentId: string | null) {
  const connectionRef = useRef<signalR.HubConnection | null>(null)
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [connected, setConnected] = useState(false)

  useEffect(() => {
    if (!appointmentId) return

    const token = localStorage.getItem('token')
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/chat', { accessTokenFactory: () => token ?? '' })
      .withAutomaticReconnect()
      .build()

    connection.on('ReceiveMessage', (msg: ChatMessage) => {
      setMessages((prev) => [...prev, msg])
    })

    connection.start().then(() => {
      setConnected(true)
      connection.invoke('JoinAppointmentRoom', appointmentId)
    })

    connectionRef.current = connection

    return () => {
      connection.invoke('LeaveAppointmentRoom', appointmentId).finally(() => connection.stop())
    }
  }, [appointmentId])

  const sendMessage = (content: string) => {
    if (connectionRef.current && connected && appointmentId) {
      connectionRef.current.invoke('SendMessage', appointmentId, content)
    }
  }

  return { messages, setMessages, sendMessage, connected }
}
