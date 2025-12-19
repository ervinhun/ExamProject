import { notificationsAtom, removeNotificationAtom, type Notification } from "@core/atoms/error";
import { useAtom, useSetAtom } from "jotai";
import { useEffect } from "react";

export default function ErrorPopUp() {
    const [notifications] = useAtom(notificationsAtom);
    const removeNotification = useSetAtom(removeNotificationAtom);

    useEffect(() => {
        // Auto-dismiss notifications after 5 seconds
        if (notifications.length > 0) {
            const timers = notifications.map(notification => 
                setTimeout(() => {
                    removeNotification(notification.id);
                }, 5000)
            );

            return () => {
                timers.forEach(timer => clearTimeout(timer));
            };
        }
    }, [notifications, removeNotification]);

    const getAlertClass = (type: Notification['type']) => {
        switch (type) {
            case 'error': return 'alert-error';
            case 'success': return 'alert-success';
            case 'warning': return 'alert-warning';
            case 'info': return 'alert-info';
            default: return 'alert-info';
        }
    };

    const getIcon = (type: Notification['type']) => {
        switch (type) {
            case 'error':
                return (
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                );
            case 'success':
                return (
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                );
            case 'warning':
                return (
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                );
            case 'info':
                return (
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                );
        }
    };

    if (notifications.length === 0) return null;

    return (
        <div className="toast toast-bottom toast-end z-50">
            {notifications.map((notification) => (
                <div 
                    key={notification.id} 
                    className={`alert ${getAlertClass(notification.type)} shadow-lg animate-slide-in-right`}
                >
                    {getIcon(notification.type)}
                    <span className="text-sm font-medium">{notification.message}</span>
                    <button 
                        onClick={() => removeNotification(notification.id)}
                        className="btn btn-ghost btn-xs btn-circle"
                    >
                        ✕
                    </button>
                </div>
            ))}
        </div>
    );
}