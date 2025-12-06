export default function formatDate(dateString: string) {
    const date = new Date(dateString)
    return date.toLocaleDateString(undefined, {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    })
}
