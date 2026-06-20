import { create } from 'zustand'

interface Notification {
  id: string
  title: string
  body: string
  type: number
  relatedEntityId?: string
  isRead: boolean
  createdAt: string
}

interface NotifState {
  notifications: Notification[]
  unreadCount: number
  setNotifications: (notifs: Notification[]) => void
  markAllRead: () => void
}

export const useNotifStore = create<NotifState>((set) => ({
  notifications: [],
  unreadCount: 0,
  setNotifications: (notifs) =>
    set({ notifications: notifs, unreadCount: notifs.filter((n) => !n.isRead).length }),
  markAllRead: () =>
    set((s) => ({ notifications: s.notifications.map((n) => ({ ...n, isRead: true })), unreadCount: 0 })),
}))
