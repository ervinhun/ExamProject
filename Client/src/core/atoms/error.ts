import { atom } from "jotai";

export type NotificationType = 'error' | 'success' | 'warning' | 'info';

export interface Notification {
    message: string;
    type: NotificationType;
    id: number;
}

export const notificationsAtom = atom<Notification[]>([]);
notificationsAtom.debugLabel = "Notifications";

// Helper to add notifications
export const addNotificationAtom = atom(
    null,
    (get, set, notification: Omit<Notification, 'id'>) => {
        const notifications = get(notificationsAtom);
        const newNotification = {
            ...notification,
            id: Date.now()
        };
        set(notificationsAtom, [...notifications, newNotification]);
    }
);

// Helper to remove notifications
export const removeNotificationAtom = atom(
    null,
    (get, set, id: number) => {
        const notifications = get(notificationsAtom);
        set(notificationsAtom, notifications.filter(n => n.id !== id));
    }
);

// Backward compatibility
export const errorAtom = atom<string | null>(null);
errorAtom.debugLabel = "Error Message";

